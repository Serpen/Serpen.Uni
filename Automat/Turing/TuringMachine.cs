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

        }
        public TuringMachineBase(string name, string[] states, char[] inputAlphabet, char[] bandAlphabet, uint startState, char blankSymbol, uint[] acceptedStates)
            : base(states, inputAlphabet, startState, name) {
            BandAlphabet = bandAlphabet;
            BlankSymbol = blankSymbol;
            AcceptedStates = acceptedStates;
        }

        protected const int MAX_TURING_RUNS = 10000;
        
        // public abstract string GetBandOutput(string w);


    }
    
    public sealed class TuringCycleException : Serpen.Uni.Exception {
        public TuringCycleException(string msg) : base(msg) { }
    }

    public enum TMDirection { Left, Right, Halt }

}