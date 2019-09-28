using System.Linq;

namespace Serpen.Uni.Automat.Turing {

    public abstract class TuringMachineBase<TKey, TVal> : AutomatBase<TKey, TVal> where TKey : struct, ITransformKey { //where TConfig : ITuringConfig {

        public const char BLANK = '_';
        public char[] BandAlphabet;
        public char BlankSymbol;

        public TuringMachineBase(string name, uint stateCount, char[] inputAlphabet, char[] bandAlphabet, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(stateCount, inputAlphabet, startState, name, acceptedStates) {
            BandAlphabet = bandAlphabet;
            BlankSymbol = blankSymbol;
        }
        public TuringMachineBase(string name, string[] states, char[] inputAlphabet, char[] bandAlphabet, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(states, inputAlphabet, startState, name, acceptedStates) {
            BandAlphabet = bandAlphabet;
            BlankSymbol = blankSymbol;
        }

        protected override void CheckConstraints() {
            base.CheckConstraints();
            if (base.Alphabet.Intersect(BandAlphabet).Count() != base.Alphabet.Length)
                throw new Uni.Exception("Inputalphabet not in Bandalphabet");
            if (!BandAlphabet.Contains(BlankSymbol))
                throw new Automat.AlphabetException(BlankSymbol);
        }

        protected const int MAX_TURING_RUNS = 10000;
        
        public abstract string GetBandOutput(string w);


    }
    
    public enum TMDirection { Left, Right, Halt }

}