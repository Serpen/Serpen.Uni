using System.Linq;

namespace Serpen.Uni.Automat {

    public interface IAutomat {
        uint StartState { get; }
        string[] States { get; }
        uint StatesCount { get; }
        char[] Alphabet { get; }
        string Name { get; }
        uint[] AcceptedStates { get; }
        bool IsAcceptedState(uint q);

        string GetRandomWord();
        string[] GetRandomWords(int count);
        bool AcceptWord(string w);
        IAutomat RemoveUnreachable();

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

        public AutomatBase(uint stateCount, char[] alphabet, uint startState, string name) {
            this.States = new string[stateCount];

            var sortAlp = alphabet.ToList();
            sortAlp.Sort();
            this.Alphabet = sortAlp.ToArray();

            this.StartState = startState;
            this.Name = name;

            for (int i = 0; i < stateCount; i++)
                States[i] = i.ToString();

        }

        public AutomatBase(string[] states, char[] alphabet, uint startState, string name) {
            this.States = states;

            var sortAlp = alphabet.ToList();
            sortAlp.Sort();
            this.Alphabet = sortAlp.ToArray();

            this.StartState = startState;
            this.Name = name;

        }
        protected virtual void CheckConstraints() {
            if (StartState > StatesCount)
                throw new Automat.StateException(StartState);
        }

        protected bool[] reachableStates() {
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


        public abstract bool AcceptWord(string w);

        public bool IsAcceptedState(uint q) {
            return AcceptedStates.Contains(q);
        }

        public abstract IAutomat RemoveUnreachable();

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