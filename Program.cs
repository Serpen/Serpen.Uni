using System;
using Serpen.Uni.Automat;
using Serpen.Uni.Automat.Finite;
using Serpen.Uni.Automat.ContextFree;

#pragma warning disable CS0162

namespace Serpen.Uni {
    class Program {
        static void Main(string[] args) {

            var rndaut = KnownAutomat.GetAllAutomats();
            // Tests.ExportAllAutomatBitmaps(rndaut);
            Tests.PurgeEquality(rndaut);

            return;

            var eqAutomats = Tests.CastToEveryPossibility(KnownAutomat.GetAllAutomats());
            foreach (var a in eqAutomats) {
                if (a.Length > 0) {
                    foreach (string w in a[0].GetRandomWords(20)) {
                        bool result0 = a[0].AcceptWord(w);
                        foreach (var a2 in a) {
                            if (a2.AcceptWord(w) != result0) {
                                // Tests.ExportAllAutomatBitmaps(a);
                                throw new Uni.Exception($"{a2.Name} not equal {a[0].Name}");
                            }
                        }
                    }

                }
            }
            return;
            
        }
    }
}
