using Serpen.Uni.Automat;

#pragma warning disable CS0162

namespace Serpen.Uni {
    static class Program {
        static void Main() {

            var cas = Tests.CastToEveryPossibility(KnownAutomat.GetAllAutomats());
            foreach (var ca in cas) {
                for (int i = 1; i < ca.Length; i++)
                    Tests.TestEqualWithWords(ca[i], ca[0], 20);
            }
            return;
            var nea = KnownAutomat.NEA_EAFK_A211_webay;
            Utils.SaveAutomatImageToTemp(nea);
            var qpda = (Automat.ContextFree.StatePDA)nea;
            Utils.SaveAutomatImageToTemp(qpda);

            Utils.TestEqualWithWord(nea, qpda, "web");
            Utils.TestEqualWithWord(nea, qpda, "ebay");
            Utils.TestEqualWithWord(nea, qpda, "wbewaebayaweb");

            Tests.TestEqualWithWords(nea,qpda, 100);
            

            Utils.AcceptWordConsoleLine(KnownAutomat.TM_1659_A414_M1, "00");
            return;
            // KnownAutomat.NEAe_1659_A220.Join(KnownAutomat.NEAe_1659_A27_N1);

            // return;
            // var fo = new System.IO.StreamWriter(System.Environment.ExpandEnvironmentVariables("%temp%\\automat\\automats.txt"));
            // foreach (var automat2 in KnownAutomat.GetAllAutomats())
            // {
            //     var automat = automat2.PurgeStates();
            //     PumpResult pumpbar = PumpResult.Unknown;
            //     try {
            //         pumpbar = PumpingLemma.TestPumpbar(automat, 6, 400);
            //     } catch (TuringCycleException) {}
            //     if (pumpbar == PumpResult.NotPumpable) {
            //         Utils.SaveAutomatImageToTemp(automat);
            //         fo.WriteLine(automat.ToString());
            //         fo.Flush();
            //     }
            //     System.Console.WriteLine($"{automat.Name.PadRight(40)}: {pumpbar}");
            // }
            // fo.Close();


            // Tests.GenerateComplements();
            // Tests.GenerateConcats();
            // Tests.GenerateDiffs();
            // Tests.GenerateIntersects();
            // Tests.GenerateJoins();
            // Tests.GenerateReverses();
            // Tests.GenerateUnions();
            // Tests.GenerateKleeneStern();
            // return;

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
            var pda2 = (Automat.Finite.FABase)KnownAutomat.DEA_1659_M1_A22_0even.Reverse();
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
                    foreach (string w in a[0].GetRandomWords(20, 1, 20)) {
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
