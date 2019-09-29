using Serpen.Uni.Automat;

#pragma warning disable CS0162

namespace Serpen.Uni {
    class Program {
        static void Main(string[] args) {

            // Utils.SaveAutomatImageToTemp(KnownAutomat.TM_EFAK_A812_copy);
            // Utils.SaveAutomatImageToTemp(KnownAutomat.TM_EFAK_A813);
            // System.Console.WriteLine(KnownAutomat.TM_EFAK_A813.GetBandOutput("0001000"));
            // return;
            // var all = KnownAutomat.GetAllAutomats();
            
            // return;
            // // var fa1 = KnownAutomat.DEA_Contains01;
            // // Utils.DrawAutomatToTemp(fa1);
            // // var fa1r = fa1.Reverse();
            // // Utils.DrawAutomatToTemp(fa1r);
            // // var fa1rr = fa1r.Reverse();
            // // Utils.DrawAutomatToTemp(fa1rr);

            // // Tests.TestEqualWithWords(fa1, fa1rr, 100);
            // var m1 = KnownAutomat.QPDA_1659_A33_K1_anbn;
            // Utils.AcceptWordConsoleLine(m1, "aabb");
            // var m2 = KnownAutomat.MPDA_1659_A33_K1_anbn;
            // Utils.AcceptWordConsoleLine(m2, "aabb");
            // // Utils.SaveAutomatImageToTemp(m1);

            // return;
            

            // var ws = m1.GetRandomWords(100, 20);
            // for (int i = 0; i < ws.Length; i++)
            // {
            //     var a = m1.AcceptWord(ws[i]);
            //     if (a)
            //         System.Console.WriteLine(ws[i]);
                
            // }

            // return;

            var pdas = KnownAutomat.GetAllContextFreeAutomats();
            Tests.TestDoubleReversesByWord(pdas);

            return; 

            var pda = KnownAutomat.DEA_1659_M1_A22_0even;
            var pda2 = KnownAutomat.DEA_1659_M1_A22_0even.Reverse();
            Utils.SaveAutomatImageToTemp(pda);
            Utils.SaveAutomatImageToTemp(pda2);

            Tests.TestEqualWithWords(pda, pda2, 100);
            Tests.TestEqualWithWords(pda, pda2.Reverse(), 100);

            return;
            
            var upda = pda.Concat(pda2);
            var upda2 = pda2.Concat(pda);
            Utils.SaveAutomatImageToTemp(upda);
            Utils.SaveAutomatImageToTemp(upda2);

            upda2.AcceptWord("");

            Tests.TestEqualWithWords(KnownAutomat.DEA_1659_A213_M1_a, KnownAutomat.DEA_1659_A213_M1_b, 100);

            if (!Tests.TestEqualWithWords(upda, upda2, 100))
                throw new Uni.Exception("Not Equal");

            return;
            // var rndaut = Tests.GenerateRandomAutomats(10);
            var rndaut = Tests.GenerateRandomAutomats(10);
            Tests.ExportAllAutomatBitmaps(rndaut);
            Tests.PurgeEquality(rndaut);

            // return;

            var eqAutomats = Tests.CastToEveryPossibility(KnownAutomat.GetAllAutomats());
            foreach (var a in eqAutomats) {
                if (a.Length > 0) {
                    foreach (string w in a[0].GetRandomWords(20,20)) {
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
