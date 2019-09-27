namespace Serpen.Uni.Automat {
    public interface IAbgeschlossenheitseigenschaften<This, T1> {
        This Union(This A);
        This Intersect(This A);
        This Complement();
        This Diff(This A);
        T1 Concat(This A);
        This Join(This A);
        T1 Reverse();
        T1 KleeneStern();
        This HomomorphismChar(System.Collections.Generic.Dictionary<char, char> Translate);
    }

    public enum SourceMode {K1659, EAFK}

    public enum RuleConstaint {None, Left, Right}
    public interface IConfig {}

}

namespace Serpen.Uni.Automat.ExtensionMethods
{
    public static class ExtensionMethods {
        
    }
}