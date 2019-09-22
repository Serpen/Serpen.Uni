using System;
using Serpen.Uni.Automat;
using Serpen.Uni.Automat.Finite;
using Serpen.Uni.Automat.ContextFree;

#pragma warning disable CS0162

namespace Serpen.Uni {
    class Program {
        static void Main(string[] args) {

            var eqAutomats = Tests.CastToEveryPossibility(KnownAutomat.GetAllAutomats());
            foreach (var a in eqAutomats) {
                if (a.Length > 0) {
                    foreach (string w in a[0].GetRandomWords(20)) {
                        // string w = a[0].GetRandomWord();
                        bool result0 = a[0].AcceptWord(w);
                        foreach (var a2 in a) {
                            if (a2.AcceptWord(w) != result0) {
                                Tests.ExportAllAutomatBitmaps(a);
                                System.Console.WriteLine($"{a2.Name} not equal {a[0].Name}");
                                a2.AcceptWord(w);
                            }
                        }
                    }

                }
            }
            return;

            // var tm1 = KnownAutomat.TM1_EFAK_B87_wcw;
            var tm2 = KnownAutomat.TMk_EFAK_B87_wcw;
            // Visualization.DrawAutomat(tm1).Save(@"c:\temp\tm1.png");
            // Visualization.DrawAutomat(tm2).Save(@"c:\temp\tm2.png");
            Utils.AcceptWordConsoleLine(tm2, "0c0");
            Utils.AcceptWordConsoleLine(tm2, "1c1");
            Utils.AcceptWordConsoleLine(tm2, "00c00");
            Utils.AcceptWordConsoleLine(tm2, "01c01");
            var rnd = Utils.RND;
            for (int i = 0; i < 3000; i++) {
                string w = tm2.GetRandomWord();
                if (tm2.AcceptWord(w))
                    Utils.AcceptWordConsoleLine(tm2, w);
                // System.Console.WriteLine($"{tm.Name}({w})={tm.AcceptWord(w)}");
            }

            // Tests.ExportAllAutomatBitmaps();
            return;
            // {
            //     PDA qpda = KnownAutomat.PDA_1659_A33_K1;
            // var spda = (StatePDA)qpda;
            // var sspda = (StackPDA)qpda;

            // }
            // System.Console.WriteLine(Tests.CastingEquality());
            // return;
            // {
            //     var nea1 = KnownAutomat.NEAe_Simple;
            //     try {
            //         Visualization.DrawAutomat(nea1).Save(@"D:\temp\nea1.png");
            //     } catch (Exception e) {
            //         System.Console.WriteLine(e);
            //     }
            //     Utils.AcceptWordConsoleLine(nea1, "0011");
            //     var rg1 = (ReGrammer)nea1;
            //     //Utils.AcceptWordConsoleLine(rg1, "0011");
            //     var crg1 = ((CFGrammer)rg1).toChomskyNF(eSourceMode.K1659);
            //     Utils.AcceptWordConsoleLine(crg1, "0011");
            //     return;
            // }
            // foreach (var cfg in KnownAutomat.GetCFGs())
            // {
            //     var ccfg = cfg.toChomskyNF();
            //     // Utils.AcceptWordConsoleLine(cfg, "babb");
            //     Utils.AcceptWordConsoleLine(ccfg, "babb");

            // }

            // {
            //     var cfg = KnownAutomat.CFG_1659_P_G6_RG;
            //     var ccfg = cfg.toChomskyNF();
            //     System.Console.WriteLine(ccfg.Rules.ToString());
            //     Utils.AcceptWordConsoleLine(ccfg,"a");
            //     return;
            // }

            // {
            //     var ia = new int[] {1 , 11, 5, 27, 9, 15, 8, 26, 22};
            //     Func<int, int, bool> f = delegate(int i1, int i2) {
            //         return (i1 / 10 == i2 / 10);
            //     };

            //     var eq = Utils.EqualityClasses(ia, f);
            //     foreach (var e in eq) {
            //         System.Console.WriteLine($"{e.Key}={string.Join(',', e.Value)}");
            //     }

            //     return;
            // }

            for (int i = 0; i < 20; i++) {
                // var rcfg = (CFGrammer)CFGrammer.GenerateRandom();
                // var rccfg = rcfg.toChomskyNF();
                // Utils.AcceptWordConsoleLine(rccfg, rccfg.GetRandomWord());
                var nr = (NFA)NFA.GenerateRandom();

                var txtwr = System.IO.File.CreateText($@"{System.Environment.GetEnvironmentVariable("TEMP")}\automat\rnd\{nr.Name}-z.txt");
                txtwr.Write(nr.ToString().Replace(";", Environment.NewLine));
                txtwr.Close();

                // if (nr.Alphabet.Length == 2 && drmin.StatesCount>1 && drmin.StatesCount < nr.StatesCount/2) {
                Visualization.DrawAutomat(nr).Save($@"{System.Environment.GetEnvironmentVariable("TEMP")}\automat\rnd\{nr.Name}.png");
                var nrmin1 = nr;  //.RemoveUnreachable();
                Visualization.DrawAutomat(nrmin1).Save($@"{System.Environment.GetEnvironmentVariable("TEMP")}\automat\rnd\{nr.Name}-entf.png");
                var drmin = Converter.Nea2TeilmengenDea(nr);
                drmin = drmin.MinimizeTF();
                Visualization.DrawAutomat(drmin).Save($@"{System.Environment.GetEnvironmentVariable("TEMP")}\automat\rnd\{nr.Name}-min.png");
                // }

            }
            return;
            //return;

            // return;
            // return;
            // {
            // var cfg = KnownAutomat.CFG_1659_G1;
            // var qpda4 = (StatePDA)(cfg);
            // var spda5 = (StackPDA)(qpda4);
            // Visualization.DrawAutomat(qpda4).Save(@"c:\temp\Automat\qpda4.png");
            // Visualization.DrawAutomat(spda5).Save(@"c:\temp\Automat\spda5.png");
            // var spda4 = (StackPDA)(cfg);
            // var qpda5 = (StatePDA)spda4;
            // Visualization.DrawAutomat(spda4).Save(@"c:\temp\Automat\spda4.png");
            // Visualization.DrawAutomat(qpda5).Save(@"c:\temp\Automat\qpda5.png");
            // var w = "abb";

            // System.Console.WriteLine($"{nameof(spda4)} accept {w}: {spda4.AcceptWord(w)}");
            // System.Console.WriteLine($"{nameof(qpda5)} accept {w}: {qpda5.AcceptWord(w)}");
            // System.Console.WriteLine($"{nameof(qpda4)} accept {w}: {qpda4.AcceptWord(w)}");
            // System.Console.WriteLine($"{nameof(spda5)} accept {w}: {spda5.AcceptWord(w)}");
            // return;

            // }
            // var oma = new Serpen.Uni.Graph.TreeNode<string>("Oma", null);
            // oma.AddChild("erwin");
            // var mama = oma.AddChild("mama");
            // mama.AddChild("marco");
            // var t = new Serpen.Uni.Graph.Tree<string>(oma);

            // System.Console.WriteLine("TiefenDurchLauf");
            // foreach (var c in t.TiefenDurchLauf())
            //     System.Console.WriteLine(c.ToString());

            // System.Console.WriteLine("\nBreitenDurchLauf:");
            // foreach (var c in t.BreitenDurchLauf())
            //     System.Console.WriteLine(c.ToString());


            // return;
            // {
            // var PDA2 = KnownAutomat.SPDA_EAFK_B612_T52_ArithmExp;
            // //var spda = (StatePDA)PDA2;
            // var w9 = "(a1)*b";
            // System.Console.WriteLine($"dPDA accept '{w9}': {PDA2.AcceptWord(w9)}");
            // //System.Console.WriteLine($"spda accept '{w9}': {spda.AcceptWord(w9)}");

            // }
            return;

            {
                var qpda = KnownAutomat.QPDA_simple_10;
                var spda = (StatePDA)qpda;
                var qpda2 = (StatePDA)spda;
                var w8 = "1010";
                System.Console.WriteLine($"{nameof(qpda)} accept '{w8}': {qpda.AcceptWord(w8)}");
                System.Console.WriteLine($"{nameof(spda)} accept '{w8}': {spda.AcceptWord(w8)}");
                System.Console.WriteLine($"{nameof(qpda2)} accept '{w8}': {qpda2.AcceptWord(w8)}");
            }

            return;

            // var D9 = (DFA)KnownAutomat.DEA_BinFreq(2,4);
            // var dPDA = KnownAutomat.DPDA_EAFK_A611_wcwr_Palindrom;
            // var PDA = KnownAutomat.PDA_EAFK_A62_wwr_Palindrom;

            // string[] words = new string[] {"01c10", "101c101", "0c0","01c01","", "01c10","0c1","c","0c1","c01","c0110","1001c0110"};
            // foreach (string w in words) {
            //     // System.Console.WriteLine($"DEA  accept '{w}': {D9.AcceptWord(w)}");
            //     System.Console.WriteLine($"dPDA accept '{w}': {dPDA.AcceptWord(w)}");
            //     var w2 = w.Replace("c", "");
            //     System.Console.WriteLine($"PDA  accept '{w2}': {PDA.AcceptWord(w2)}");
            // }

            // return;

            // op();
            // Concat();
            {
                var D = Automat.KnownAutomat.DEA_ContainsOnes(4);
                // D = DeaModel.Empty;
                var Dm = D.MinimizeTF();
                var N = (NFA)D;
                var Ne = (NFAe)(N);
                var D2 = Converter.Nea2TeilmengenDea(N);

                // var Ne = Automat.KnownAutomat.NEAe_1659_T_22_N3;;
                // var D = Converter.Nea2TeilmengenDea(Ne);
                // //var N = Converter.Nea2NeaE(Ne);


                string[] words;
                // words = new string[]{""};

                if (D.Alphabet[0] == '0')
                    words = new string[] { "0", "00", "101", "11010010", "" };
                else
                    words = new string[] { "a", "aa", "bab", "aababbab", "" };

                // Console.WriteLine($"RegExp {Converter.DEA2RegExp(D)}");
                // Console.WriteLine();
                // Console.WriteLine($"RegExp {Converter.DEA2RegExp(Dm)}");

                Console.WriteLine("Automat D |{0}| minimized to Automat Dm |{1}|", D.States.Length, Dm.States.Length);
                Console.WriteLine("Automat D |{0}| minimized to Automat D2 |{1}|", D.States.Length, D2.States.Length);

                Console.WriteLine("Automat D '{0}' equals Automat Dm '{1}': {2}", D, Dm, D.Equals(Dm));
                Console.WriteLine("Automat D '{0}' equals Automat D2 '{1}': {2}", D, D2, D.Equals(D2));

                foreach (string word in words) {
                    Console.WriteLine();
                    Console.WriteLine($"DEA  Accept Word '{word}': {D.AcceptWord(word)}");
                    Console.WriteLine($"DEAm Accept Word '{word}': {Dm.AcceptWord(word)}");
                    Console.WriteLine($"NEA  Accept Word '{word}': {N.AcceptWord(word)}");
                    Console.WriteLine($"NEAe Accept Word '{word}': {Ne.AcceptWord(word)}");
                    Console.WriteLine($"DEA2 Accept Word '{word}': {D2.AcceptWord(word)}");
                }
                // Console.WriteLine(String.Join(',',Ne.EpsilonHuelle(new uint[] {0,1})));
            }
        }
    }
}
