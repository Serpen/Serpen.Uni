namespace Serpen.Uni {
    public class Exception : System.ApplicationException {
        public Exception(string message) : base(message) { }
    }
}

namespace Serpen.Uni.Automat {

    [System.Serializable]
    public class Exception : Serpen.Uni.Exception {
        public Exception(string message, params IAutomat[] automats) : base(message) {
            Automats = automats;
            foreach (var automat in automats) {
                try {
                    Utils.ExportToTemp(automat);
                }
                catch (System.Runtime.Serialization.SerializationException sex) {
                    Serpen.Uni.Utils.DebugMessage(sex.Message, Uni.Utils.eDebugLogLevel.Always);
                } catch { }
            }
        }

        public readonly IAutomat[] Automats;
    }

    [System.Serializable]
    public class AlphabetException : Exception {
        public AlphabetException(char c, params IAutomat[] automats) : base($"{c} not in Alphabet", automats) => Char = c;
        public AlphabetException(string msg, params IAutomat[] automats) : base(msg, automats) { }
        public readonly char Char;
    }

    [System.Serializable]
    public class StateException : Exception {
        public StateException(uint q, params IAutomat[] automats) : base($"{q} not in States", automats) => State = q;
        public StateException(uint q, string message, params IAutomat[] automats) : base($"{q} {message}", automats) => State = q;
        public readonly uint State;
    }


    [System.Serializable]
    public class DeterministicException : Exception {
        public DeterministicException(string message, params IAutomat[] automats) : base(message, automats) { }
    }

    [System.Serializable]
    public class GrammerException : Serpen.Uni.Exception {
        public GrammerException(string message) : base(message) { }
    }
}

namespace Serpen.Uni.Automat.Turing {
    [System.Serializable]
    public sealed class TuringCycleException : Exception {
        public TuringCycleException(string msg, IAutomat automat) : base(msg, automat) { }
    }
}

namespace Serpen.Uni.Automat.ContextFree {
    [System.Serializable]
    public sealed class PDAStackException : Exception {
        public PDAStackException(string msg, PDA automat) : base(msg, automat) { }
    }
}