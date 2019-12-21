using System.Linq;

namespace Serpen.Uni.Automat.Turing {

    [System.Serializable]
    public abstract class TuringMachineBase<TKey, TVal> : AutomatBase<TKey, TVal> where TKey : ITransformKey { //where TConfig : ITuringConfig {

        public const char BLANK = '_';

        protected const int MAX_TURING_RUNS = 10000;
        public char[] BandAlphabet { get; }
        public char BlankSymbol { get; }

        public TuringMachineBase(string name, uint stateCount, char[] inputAlphabet, char[] bandAlphabet, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(name, stateCount, inputAlphabet, startState, acceptedStates) {
            BandAlphabet = bandAlphabet;
            BlankSymbol = blankSymbol;
        }
        public TuringMachineBase(string name, string[] states, char[] inputAlphabet, char[] bandAlphabet, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(name, states, inputAlphabet, startState, acceptedStates) {
            BandAlphabet = bandAlphabet;
            BlankSymbol = blankSymbol;
        }

        protected override void CheckConstraints() {
            base.CheckConstraints();
            if (base.Alphabet.Intersect(BandAlphabet).Count() != base.Alphabet.Length)
                throw new Uni.Automat.AlphabetException("Inputalphabet not in Bandalphabet", this);
            if (!BandAlphabet.Contains(BlankSymbol))
                throw new Automat.AlphabetException(BlankSymbol, this);
        }



        public abstract string GetBandOutput(string w);


    }

    public enum TMDirection { Left, Right, Halt }

}