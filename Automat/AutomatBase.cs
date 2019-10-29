using System.Linq;
using Serpen.Uni;

namespace Serpen.Uni.Automat {

    public interface IAcceptWord {
        string Name { get; }
        bool AcceptWord(string w);

        string[] GetRandomWords(int count, int minLen, int maxLen);
        char[] Alphabet { get; }

        // System.Func<string,bool> SimplifiedAcceptFunction {get; internal set;}
        //protected void CheckConstraints();
    }

    public interface IAutomat : IAcceptWord {
        uint StartState { get; }
        string[] States { get; }
        uint StatesCount { get; }
        uint[] AcceptedStates { get; }
        bool IsAcceptedState(uint q);

        IAutomat PurgeStates();

        bool SameAlphabet(IAutomat A2);

        VisualizationTuple[] VisualizationLines();
    }

    public abstract class AutomatBase<TKey, TVal> : IAutomat where TKey : ITransformKey {
        public TransformBase<TKey, TVal> Transforms { get; protected set; }
        public string[] States { get; }
        public uint StatesCount => (uint)States.Length;
        public uint StartState { get; } = 0;
        public char[] Alphabet { get; }
        public string Name { get; protected set; }
        public uint[] AcceptedStates { get; protected set; }

        internal System.Func<string, bool> SimplyfiedAcceptFunction { get; set; }

        public AutomatBase(uint stateCount, char[] alphabet, uint startState, string name, uint[] acceptedStates) {

            var sortAlp = alphabet.ToList();
            sortAlp.Sort();
            this.Alphabet = sortAlp.ToArray();

            this.StartState = startState;
            this.Name = name;

            AcceptedStates = acceptedStates;

            this.States = new string[stateCount];
            for (int i = 0; i < stateCount; i++)
                States[i] = i.ToString();

        }

        public AutomatBase(string[] states, char[] alphabet, uint startState, string name, uint[] acceptedStates)
            : this((uint)states.Length, alphabet, startState, name, acceptedStates) {
            States = states;
        }
        protected virtual void CheckConstraints() {
            if (StatesCount == 0)
                throw new Automat.StateException(StatesCount, "automat must have at least a startstate");
            if (StartState >= StatesCount)
                throw new Automat.StateException(StartState);
            foreach (uint acc in AcceptedStates) {
                if (acc >= StatesCount)
                    throw new Automat.StateException(acc);
            }
        }

        public bool SameAlphabet(IAutomat A2) {
            if (this.Alphabet.Length != A2.Alphabet.Length) return false;
            for (int i = 0; i < this.Alphabet.Length; i++)
                if (this.Alphabet[i] != A2.Alphabet[i])
                    return false;
            return true;
        }

        public bool[] ReachableStates() {
            bool[] fromStartReachable = new bool[StatesCount];
            fromStartReachable[StartState] = true;
            bool foundnew = true;
            while (foundnew) {
                foundnew = false;
                foreach (var t in (from tr in Transforms where fromStartReachable[tr.Key.q] select tr)) {
                    if (t.Value is uint qnext) {
                        if (!fromStartReachable[qnext])
                            fromStartReachable[qnext] = foundnew = true;
                    } else if (t.Value is uint[] qnexts) {
                        foreach (var qn in qnexts)
                            if (!fromStartReachable[qn])
                                fromStartReachable[qn] = foundnew = true;
                    } else if (t.Value is ITransformValue tt) {
                        if (!fromStartReachable[tt.qNext])
                            fromStartReachable[tt.qNext] = foundnew = true;
                    } else if (t.Value is System.Array tarray) { //why is [is ITransformValue[]] not working? 
                        foreach (var tt1 in tarray)
                            if (tt1 is ITransformValue tt1v)
                                if (!fromStartReachable[tt1v.qNext])
                                    fromStartReachable[tt1v.qNext] = foundnew = true;
                    } else
                        throw new System.NotImplementedException();
                }
            }
            return fromStartReachable;
        }

        protected (uint[], string[], uint[]) removedStateTranslateTables() {
            bool[] fromStartReachable = ReachableStates();

            uint[] translate = new uint[fromStartReachable.Count((b) => b)];
            for (uint i = 0; i < translate.Length; i++) {
                uint j = i;
                if (i > 0)
                    j = System.Math.Max(i, translate[i - 1] + 1);
                while (!fromStartReachable[j])
                    j++;
                translate[i] = j;
            }

            if (translate.ArrayIndex(StartState) > this.StatesCount)
                throw new Uni.Automat.StateException(StartState, "removed with too high start state");

            string[] names = new string[translate.Length];
            for (int i = 0; i < translate.Length; i++)
                names[i] = translate[i].ToString();

            var astates = new System.Collections.Generic.List<uint>(AcceptedStates.Length);
            foreach (var accept in AcceptedStates)
                if (translate.Contains(accept))
                    astates.Add(translate.ArrayIndex(accept));


            return (translate, names, astates.ToArray());
        }


        public abstract bool AcceptWord(string w);

        protected void CheckWordInAlphabet(string w) {
            for (int i = 0; i < w.Length; i++)
                if (!Alphabet.Contains(w[i]))
                    throw new AlphabetException(w[i]);
        }

        public bool IsAcceptedState(uint q) {
            return AcceptedStates.Contains(q);
        }

        public abstract IAutomat PurgeStates();

        public abstract VisualizationTuple[] VisualizationLines();

        public abstract override string ToString();

        public string[] GetRandomWords(int count, int minLen, int maxLen) {
            var words = new System.Collections.Generic.List<string>();
            var rnd = Uni.Utils.RND;

            int i = 0;

            //count shouldn't be higher than words available with maxLen
            count = System.Math.Min(count, Alphabet.Length * maxLen * maxLen);

            while (words.Count < count) {
                string w = "";
                var wLen = rnd.Next(minLen, maxLen);
                for (int k = 0; k < wLen; k++)
                    w = w.Insert(k, Alphabet[rnd.Next(0, Alphabet.Length)].ToString());

                if (!words.Contains(w))
                    words.Add(w);

                if (i > count * 10) {
                    Uni.Utils.DebugMessage($"Unable to get enough random words {i} tries>{words.Count}>{count}", this, Uni.Utils.eDebugLogLevel.Verbose);
                    break;
                }
                i++;

            }
            
            words.Sort(new StringLengthComparer());
            return words.ToArray();
        }

        class StringLengthComparer : System.Collections.Generic.IComparer<string> {
            public int Compare(string s1, string s2) {

                if (s1.Length < s2.Length) return -1;
                else if (s1.Length > s2.Length) return 1;
                else
                    return s1.CompareTo(s2);
            }
        }


    }
}

namespace Serpen.Uni.Automat.Finite {
    public interface INFA : IAutomat, IAlleAbgeschlossenheitseigenschaften {
        uint[] GoChar(uint[] q, char w);
    }
}