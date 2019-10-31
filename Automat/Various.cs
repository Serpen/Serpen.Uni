namespace Serpen.Uni.Automat {

    public interface IUnion : IAutomat { IAutomat Union(IUnion A); }
    public interface IIntersect : IAutomat { IAutomat Intersect(IIntersect A); }
    public interface IComplement : IAutomat { IAutomat Complement(); }
    public interface IDiff : IAutomat { IAutomat Diff(IDiff A); }
    public interface IConcat : IAutomat { IAutomat Concat(IConcat A); }
    public interface IJoin : IAutomat { IAutomat Join(IJoin A); }
    public interface IReverse : IAutomat { IAutomat Reverse(); }
    public interface IKleeneStern : IAutomat { IAutomat KleeneStern(); }
    public interface IHomomorphismChar : IAutomat { IAutomat HomomorphismChar(System.Collections.Generic.Dictionary<char, char> Translate); }

    public interface IAlleAbgeschlossenheitseigenschaften : IUnion, IIntersect, IComplement, IDiff, IConcat, IJoin, IReverse, IKleeneStern, IHomomorphismChar { }

    public enum SourceMode { K1659, EAFK, Wiki }

    public enum RuleConstaint { None, Left, Right }
    public interface IConfig {
        uint State {get;}
    }

    [System.AttributeUsage(System.AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class AcceptedWordSamples : System.Attribute {
        public string[] Words { get; }
        public AcceptedWordSamples(params string[] words) => Words = words;
    }

}