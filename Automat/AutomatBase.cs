using System.Linq;

namespace Serpen.Uni.Automat {

    public interface IAcceptWord
    {
        string Name { get; }
        bool AcceptWord(string w);

        //protected void CheckConstraints();
    }

    public interface IAutomat : IAcceptWord {
        uint StartState { get; }
        string[] States { get; }
        uint StatesCount { get; }
        char[] Alphabet { get; }
        uint[] AcceptedStates { get; }
        bool IsAcceptedState(uint q);

        string GetRandomWord();
        string[] GetRandomWords(int count);
        IAutomat PurgeStates();

        System.Tuple<int, int, string>[] VisualizationLines();
    }

    public abstract class AutomatBase<TKey, TVal> : IAutomat where TKey : struct, ITransformKey {
        public TransformBase<TKey, TVal> Transform { get; protected set; }
        public string[] States { get; }
        public uint StatesCount => (uint)States.Length;
        public uint StartState { get; } = 0;
        public char[] Alphabet { get; }
        public string Name { get; protected set; }
        public uint[] AcceptedStates { get; protected set; }

        public AutomatBase(uint stateCount, char[] alphabet, uint startState, string name, uint[] acceptedStates) {
            this.States = new string[stateCount];

            var sortAlp = alphabet.ToList();
            sortAlp.Sort();
            this.Alphabet = sortAlp.ToArray();

            this.StartState = startState;
            this.Name = name;
            
            AcceptedStates = acceptedStates;
            
            for (int i = 0; i < stateCount; i++)
                States[i] = i.ToString();

        }

        public AutomatBase(string[] states, char[] alphabet, uint startState, string name, uint[] acceptedStates)
            : this ((uint)states.Length, alphabet, startState, name, acceptedStates) {
            States = states;
        }
        protected virtual void CheckConstraints() {
            if (StartState > StatesCount)
                throw new Automat.StateException(StartState);
        }

        private bool[] reachableStates() {
            bool[] fromStartReachable = new bool[StatesCount];
            fromStartReachable[StartState] = true;
            bool foundnew = true;
            while (foundnew) {
                foundnew = false;
                foreach (var t in (from tr in Transform where fromStartReachable[tr.Key.q] select tr)) {
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
                    } else
                        throw new System.NotImplementedException();
                }
            }
            return fromStartReachable;
        }

        protected (uint[], string[], uint[]) removedStateTranslateTables() {
            bool[] fromStartReachable = reachableStates();

            uint[] translate = new uint[fromStartReachable.Count((b) => b)];
            for (uint i = 0; i < translate.Length; i++) {
                uint j = i;
                if (i > 0)
                    j = System.Math.Max(i, translate[i - 1] + 1);
                while (!fromStartReachable[j])
                    j++;
                translate[i] = j;
            }

            if (Utils.ArrayIndex(translate, StartState) > this.StatesCount)
                throw new Uni.Automat.StateException(StartState, "removed with too high start state");

            string[] names = new string[translate.Length];
            for (int i = 0; i < translate.Length; i++)
                names[i] = translate[i].ToString();

            var astates = new System.Collections.Generic.List<uint>();
            foreach (var accept in AcceptedStates)
                if (translate.Contains(accept))
                    astates.Add(Utils.ArrayIndex(translate, accept));


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

        public abstract System.Tuple<int, int, string>[] VisualizationLines();

        public abstract override string ToString();

        public string GetRandomWord() {
            var rnd = Utils.RND;
            var wLen = rnd.Next(0, 10);
            string w = "";
            for (int k = 0; k < wLen; k++)
                w = w.Insert(k, Alphabet[rnd.Next(0, Alphabet.Length)].ToString());

            return w;
        }

        public string[] GetRandomWords(int count) {
            var words = new System.Collections.Specialized.StringCollection();
            string[] wordArray = new string[count];

            var rnd = Utils.RND;

            while (words.Count < count) {
                string w = "";
                var wLen = rnd.Next(0, System.Math.Max(10, count));
                for (int k = 0; k < wLen; k++)
                    w = w.Insert(k, Alphabet[rnd.Next(0, Alphabet.Length)].ToString());

                if (!words.Contains(w))
                    words.Add(w);
            }
            words.CopyTo(wordArray, 0);
            return wordArray;

        }
    }
}

namespace Serpen.Uni.Automat.Finite {
    public interface INFA : IAutomat {
        uint[] GoChar(uint[] q, char w);
    }
    public interface IDFA : IAutomat {
        uint[] GoChar(uint q, char w);
    }
}