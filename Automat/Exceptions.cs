namespace Serpen.Uni
{
	public class Exception : System.ApplicationException
	{
		public Exception(string message) : base(message) { }
	}
}

namespace Serpen.Uni.Automat
{

	[System.Serializable]
	public class AlphabetException : Exception
	{
		public AlphabetException(char c) : base($"{c} not in Alphabet") => Char = c;
		public AlphabetException(string msg) : base(msg) { }
		public readonly char Char;
	}

	[System.Serializable]
	public class StateException : Exception
	{
		public StateException(uint q) : base($"{q} not in States") => State = q;
		public StateException(uint q, string message) : base($"{q} {message}") => State = q;
		public readonly uint State;
	}


	[System.Serializable]
	public class DeterministicException : Exception
	{
		public DeterministicException(string message) : base(message) { }
	}

	[System.Serializable]
	public class GrammerException : Exception
	{
		public GrammerException(string message) : base(message) { }
	}
}

namespace Serpen.Uni.Automat.Turing
{
	[System.Serializable]
	public sealed class TuringCycleException : Exception
	{
		public TuringCycleException(string msg, IAutomat automat) : base(msg) { }
	}
}

namespace Serpen.Uni.Automat.ContextFree
{
	[System.Serializable]
	public sealed class PDAStackException : Exception
	{
		public PDAStackException(string msg, PDA automat) : base(msg) { }
	}
}