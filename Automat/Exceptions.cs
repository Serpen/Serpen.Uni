namespace Serpen.Uni {
    public class Exception : System.ApplicationException {
        public Exception(string message) : base(message) { }
    }
}

namespace Serpen.Uni.Automat {

    [System.Serializable]
    public class AlphabetException : Serpen.Uni.Exception {
        public AlphabetException(char c) : base($"{c} not in Alphabet") => Char = c;
        public AlphabetException(string msg) : base(msg) { }
        public readonly char Char;
    }

    [System.Serializable]
    public class StateException : Serpen.Uni.Exception {
        public StateException(uint q) : base($"{q} not in States") => State = q;
        public StateException(uint q, string message) : base($"{q} {message}") => State = q;
        public readonly uint State;
    }


    [System.Serializable]
    public class DeterministicException : Serpen.Uni.Exception {
        public DeterministicException(string message) : base(message) { }
    }

    [System.Serializable]
    public class GrammerException : Serpen.Uni.Exception {
        public GrammerException(string message) : base(message) { }
    }

    [System.Serializable]
    public sealed class TuringCycleException : Serpen.Uni.Exception {
        public TuringCycleException(string msg, IAutomat automat) : base(msg) { }
    }

    [System.Serializable]
    public sealed class PDAStackException : Serpen.Uni.Exception {
        public PDAStackException(string msg, ContextFree.PDA automat) : base(msg) { }
    }


}