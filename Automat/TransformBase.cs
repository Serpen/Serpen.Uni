namespace Serpen.Uni.Automat {

    [System.Serializable]
    public class TransformBase<TKey, TVal> : Graph.EdgeBase<TKey, TVal> where TKey : ITransformKey { }

    public interface ITransformKey {
        uint q { get; }
        char[] c { get; }
    }

    public interface ITransformValue  {
        uint qNext { get; }
    }
}