using Serpen.Uni.Automat;
using Serpen.Uni.CompSys;
using Serpen.Uni.Automat.Finite;

#pragma warning disable CS0162

namespace Serpen.Uni {
    static class Program {
        static void Main() {
            Automat.Tests.CastToEveryPossibility();
            KnownAutomat.DEA_1659_A213_M1_a.MinimizeTF();

            // var es = new NFAe("Empty2", 1, new char[] { }, new NFAeTransform(), 0, new uint[] { });

        }

    }
}
