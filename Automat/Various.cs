namespace Serpen.Uni.Automat {

    public interface IUnion : IAutomat { IAutomat Union(IAutomat A); }
    public interface IIntersect : IAutomat { IAutomat Intersect(IAutomat A); }
    public interface IComplement : IAutomat { IAutomat Complement(); }
    public interface IDiff : IAutomat { IAutomat Diff(IAutomat A); }
    public interface IConcat : IAutomat { IAutomat Concat(IAutomat A); }
    public interface IJoin : IAutomat { IAutomat Join(IAutomat A); }
    public interface IReverse : IAutomat { IAutomat Reverse(); }
    public interface IKleeneStern : IAutomat { IAutomat KleeneStern(); }
    public interface IHomomorphismChar : IAutomat { IAutomat HomomorphismChar(System.Collections.Generic.Dictionary<char, char> Translate); }

    interface IAlleAbgeschlossenheitseigenschaften : IUnion, IIntersect, IComplement, IDiff, IConcat, IJoin, IReverse, IKleeneStern, IHomomorphismChar { }

    public enum SourceMode { K1659, EAFK }

    public enum RuleConstaint { None, Left, Right }
    public interface IConfig { }

    [System.AttributeUsage(System.AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    sealed class AcceptedWordSamples : System.Attribute {
        public string[] Source { get; }
        public AcceptedWordSamples(params string[] words) => this.Source = words;
    }

}

namespace Serpen.Uni.Automat.ExtensionMethods {
    public static class ExtensionMethods {

    }
}