using System.Linq;

namespace Serpen.Uni.Automat {

    public interface IAutomat
    {
        uint StartState {get;}
        string[] States {get;}
        uint StatesCount {get;}
        char[] Alphabet { get; }
        string Name { get;}
        uint[] AcceptedStates { get; }

        string GetRandomWord();
        bool AcceptWord(string w);
        System.Tuple<int, int, string>[] VisualizationLines();
    }

    public abstract class AutomatBase<TKey, TVal> : IAutomat {
        public TransformBase<TKey, TVal> Transform { get; protected set;}
        public string[] States {get;}
        public uint StatesCount => (uint)States.Length;
        public uint StartState {get;} = 0;
        public char[] Alphabet { get; }
        public string Name { get; protected set;}
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
            
            checkConstrains();
        }

        public AutomatBase(string[] states, char[] alphabet, uint startState, string name) {
            this.States = states;

            var sortAlp = alphabet.ToList();
            sortAlp.Sort();
            this.Alphabet = sortAlp.ToArray();

            this.StartState = startState;
            this.Name = name;
            
            checkConstrains();
        }
        public virtual void checkConstrains() {
            if (StartState > StatesCount)
                throw new Automat.StateNotFoundException(StartState);
        }


        public abstract bool AcceptWord(string w);

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