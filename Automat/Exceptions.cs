namespace Serpen.Uni {
    public class Exception : System.ApplicationException {
        public Exception(string message) : base(message) { }
    }
}

namespace Serpen.Uni.Automat {

    [System.Serializable]
    public class NotInAlphabetException : Serpen.Uni.Exception {
        public NotInAlphabetException(char c) : base($"{c} not in Alphabet") => Char = c;
        public char Char;

    }

    [System.Serializable]
    public class StateNotFoundException : Serpen.Uni.Exception {
        public StateNotFoundException(uint q) : base($"{q} not in States") => State = q;
        public uint State { get; }
    }


    [System.Serializable]
    public class DeterministicException : Serpen.Uni.Exception {
        public DeterministicException(string message) : base(message) { }
    }

    


}