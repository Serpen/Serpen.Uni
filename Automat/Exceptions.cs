namespace Serpen.Uni {
    public class Exception : System.ApplicationException {
        public Exception(string message) : base(message) { }
    }
}

namespace Serpen.Uni.Automat {

    [System.Serializable]
    public class AlphabetException : Serpen.Uni.Exception {
        public AlphabetException(char c) : base($"{c} not in Alphabet") => Char = c;
        public char Char;
        public char[] Alphabet;

    }

    [System.Serializable]
    public class StateException : Serpen.Uni.Exception {
        public StateException(uint q) : base($"{q} not in States") => State = q;
        public uint State { get; }
    }


    [System.Serializable]
    public class DeterministicException : Serpen.Uni.Exception {
        public DeterministicException(string message) : base(message) { }
    }

    [System.Serializable]
    public sealed class TuringCycleException : Serpen.Uni.Exception {
        public TuringCycleException(string msg) : base(msg) { }
    }


}