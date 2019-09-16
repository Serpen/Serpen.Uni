using System.Linq;

namespace Serpen.Uni.Automat.Turing {
    public abstract class TuringMachineBase : AutomatBase<TuringTransformSingleBand.TuringKey, TuringTransformSingleBand.TuringVal> { //where TConfig : ITuringConfig {

        public const char BLANK = ' ';

        public char[] BandAlphabet;
        public char BlankSymbol;

        public TuringMachineBase(string name, uint stateCount, char[] inputAlphabet, char[] bandAlphabet, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(stateCount, inputAlphabet, startState, name) {
            BandAlphabet = bandAlphabet;
            BlankSymbol = blankSymbol;
            AcceptedStates = acceptedStates;
            checkConstraints();
        }
        public TuringMachineBase(string name, string[] states, char[] inputAlphabet, char[] bandAlphabet, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(states, inputAlphabet, startState, name) {
            BandAlphabet = bandAlphabet;
            BlankSymbol = blankSymbol;
            AcceptedStates = acceptedStates;
            checkConstraints();
        }

        public void checkConstraints() {
            if (base.Alphabet.Intersect(BandAlphabet).Count() != base.Alphabet.Length)
                throw new Uni.Exception("Inputalphabet not in Bandalphabet");
            if (!BandAlphabet.Contains(BlankSymbol))
                throw new Automat.NotInAlphabetException(BlankSymbol);
        }

        protected const int MAX_TURING_RUNS = 10000;
        
        // public abstract string GetBandOutput(string w);


    }
    
    public sealed class TuringCycleException : Serpen.Uni.Exception {
        public TuringCycleException(string msg) : base(msg) { }
    }

    public enum TMDirection { Left, Right, Halt }

}