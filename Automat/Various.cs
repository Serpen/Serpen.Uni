namespace Serpen.Uni.Automat {

    public interface IUnion : IAcceptWord, IConcat, IJoin { IAcceptWord Union(IUnion A); }
    public interface IIntersect : IAutomat { IAutomat Intersect(IIntersect A); }
    public interface IComplement : IAutomat { IAutomat Complement(); }
    public interface IDiff : IAutomat { IAutomat Diff(IDiff A); }
    public interface IConcat : IAcceptWord { IAcceptWord Concat(IConcat A); }
    public interface IJoin : IAcceptWord { IAcceptWord Join(IJoin A); }
    public interface IReverse : IAcceptWord { IAcceptWord Reverse(); }
    public interface IKleeneStern : IAutomat { IAutomat KleeneStern(); }
    public interface IHomomorphismChar : IAutomat { IAutomat HomomorphismChar(System.Collections.Generic.Dictionary<char, char> Translate); }

    enum JoinConcatUnionKind {Join, Concat, Union}
    
    public interface IAlleAbgeschlossenheitseigenschaften : IUnion, IIntersect, IComplement, IDiff, IConcat, IJoin, IReverse, IKleeneStern, IHomomorphismChar { }

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