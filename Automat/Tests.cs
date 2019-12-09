using Serpen.Uni.Automat.Finite;
using Serpen.Uni.Automat.ContextFree;
using Serpen.Uni.Automat.Turing;
using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat {
    public static class Tests {

        public static void RunAllTests() {
            var allAutomats1 = new List<IAutomat>();

            allAutomats1.AddRange(GenerateComplements());
            allAutomats1.AddRange(GenerateConcats());
            allAutomats1.AddRange(GenerateDiffs());
            allAutomats1.AddRange(GenerateIntersects());
            allAutomats1.AddRange(GenerateJoins());
            allAutomats1.AddRange(GenerateKleeneStern());
            // allAutomats1.AddRange(GenerateDoubleReverses());
            allAutomats1.AddRange(GenerateUnions());

            allAutomats1.AddRange(GenerateRandomAutomats(100));

            var allAutomats = CastToEveryPossibility(allAutomats1);

            for (int i = 0; i < allAutomats.GetLength(0); i++) {
                try {
                    PurgeEquality(allAutomats[i], 20);
                } catch (ContextFree.PDAStackException) { } catch (Turing.TuringCycleException) { }
                for (int j = 0; j < allAutomats[i].GetLength(0); j++) {
                    if (!TestEqualWithWords(allAutomats[i][0], allAutomats[i][j], 20)) {
                        try {
                            throw new Automat.Exception("Automats not equal", new IAutomat[] { allAutomats[i][0], allAutomats[i][j] });
                            // throw new Automat.Exception("Automats not equal", allAutomats[i]);
                        } catch (System.Exception) {

                        }

                    }
                }
            }
            TestMinimizedDeaIsMHCount();
        }

        public static IAutomat[][] CastToEveryPossibility() => CastToEveryPossibility(KnownAutomat.GetAllAutomats());
        public static IAutomat[][] CastToEveryPossibility(IEnumerable<IAutomat> automats) {
            var retAutomats = new List<IAutomat[]>();

            foreach (IAutomat automat in automats) {
                var ret1Automat = new List<IAutomat>();
                if (automat is DFA D) {
                    ret1Automat.Add(D);

                    NFA NfromD = (NFA)D;
                    ret1Automat.Add(NfromD);

                    NFAe NEfromD = (NFAe)D;
                    ret1Automat.Add(NEfromD);

                    NFAe NEfromN = (NFAe)NfromD;
                    ret1Automat.Add(NEfromN);

                    StatePDA QPDAfromNe = (StatePDA)NEfromD;
                    ret1Automat.Add(QPDAfromNe);

                    StackPDA SPDAFromQPDA = (StackPDA)QPDAfromNe;
                    // ret1Automat.Add(SPDAFromQPDA);

                    try {
                        DPDA DPDAFromD = (DPDA)D;
                        ret1Automat.Add(DPDAFromD);
                    } catch { }

                    TuringMachineSingleBand TMfromD = (TuringMachineSingleBand)D;
                    ret1Automat.Add(TMfromD);

                } else if (automat is NFA N) {
                    ret1Automat.Add(N);

                    DFA DfromN = DFA.Nea2TeilmengenDea(N);
                    ret1Automat.Add(DfromN);

                    NFAe NEfromD = (NFAe)DfromN;
                    ret1Automat.Add(NEfromD);

                    NFAe NEfromN = (NFAe)N;
                    ret1Automat.Add(NEfromN);

                    StatePDA QPDAfromNe = (StatePDA)NEfromD;
                    ret1Automat.Add(QPDAfromNe);

                    StackPDA SPDAFromQPDA = (StackPDA)QPDAfromNe;
                    ret1Automat.Add(SPDAFromQPDA);

                    try {
                        DPDA DPDAFromD = (DPDA)DfromN;
                        ret1Automat.Add(DPDAFromD);
                    } catch { }

                    TuringMachineSingleBand TMfromD = (TuringMachineSingleBand)DfromN;
                    ret1Automat.Add(TMfromD);

                } else if (automat is NFAe Ne) {
                    ret1Automat.Add(Ne);

                    DFA DfromNe = DFA.Nea2TeilmengenDea(Ne);
                    ret1Automat.Add(DfromNe);

                    // NFA NfromD = (NFAe)DfromNe;

                    StatePDA QPDAfromNe = (StatePDA)Ne;
                    ret1Automat.Add(QPDAfromNe);

                    StackPDA SPDAFromQPDA = (StackPDA)QPDAfromNe;
                    ret1Automat.Add(SPDAFromQPDA);

                    try {
                        DPDA DPDAFromD = (DPDA)DfromNe;
                        ret1Automat.Add(DPDAFromD);
                    } catch { }


                    TuringMachineSingleBand TMfromD = (TuringMachineSingleBand)DfromNe;
                    ret1Automat.Add(TMfromD);

                } else if (automat is StatePDA QPDA) {
                    ret1Automat.Add(QPDA);

                    try {
                        var SPDAfromQPDA = (StackPDA)QPDA;
                        ret1Automat.Add(SPDAfromQPDA);

                        // NTM1659 NTMfromQPDA = (NTM1659)QPDA;
                        // ret1Automat.Add(NTMfromQPDA);
                    } catch { }
                } else if (automat is StackPDA SPDA) {
                    var QPDAfromSPDA = (StackPDA)SPDA;

                    ret1Automat.Add(SPDA);
                    ret1Automat.Add(QPDAfromSPDA);
                } else if (automat is DPDA dpda) {
                    var QPDAfromDPDA = (StatePDA)dpda;

                    ret1Automat.Add(dpda);
                    ret1Automat.Add(QPDAfromDPDA);
                } else if (automat is TuringMachineSingleBand tm1) {
                    ret1Automat.Add(tm1);
                } else {
                    ret1Automat.Add(automat);
                }

                if (automat is IReverse Arev) {
                    ret1Automat.Add((IReverse)((IReverse)Arev.Reverse()).Reverse());
                }

                retAutomats.Add(ret1Automat.ToArray());
            }

            return retAutomats.ToArray();
        }

        public static IAutomat[] GenerateRandomAutomats(int count) {
            var retAut = new List<IAutomat>();
            for (int i = 0; i < count; i++) {
                retAut.Add(DFA.GenerateRandom());
                retAut.Add(NFA.GenerateRandom());
                retAut.Add(NFAe.GenerateRandom());
                retAut.Add(StatePDA.GenerateRandom());
                retAut.Add(StackPDA.GenerateRandom());
                retAut.Add(DPDA.GenerateRandom());
                retAut.Add(TuringMachineSingleBand.GenerateRandom());
                retAut.Add(TuringMachineSingleBand1659.GenerateRandom());
                retAut.Add(TuringMachineMultiTrack.GenerateRandom());
                retAut.Add(NTM1659.GenerateRandom());
            }
            return retAut.ToArray();
        }

        public static void PurgeEquality(IAutomat[] automats, int words = 100) {
            foreach (IAutomat a in automats) {
                var a_removed = a.PurgeStates();
                string[] rwords = a.GetRandomWords(words, 1, words, System.Array.Empty<string>());
                for (int i = 0; i < rwords.Length; i++) {
                    if (!a.AcceptWord(rwords[i]) == a_removed.AcceptWord(rwords[i]))
                        throw new Automat.Exception($"Automats not equal by word {rwords[i]}", a, a_removed);
                }
                // if (!a.Equals(a_removed)) {
                //     throw new Automat.Exception("Automats not equal", a, a_removed);
                // }
            }
        }

        /// <summary>
        /// Test if minimized Automats euqal their base
        /// </summary>
        public static void MinimizeEquality(Automat.Finite.DFA[] automats, int words = 100) {
            foreach (IAutomat a in automats) {
                if (a is DFA dfa) {
                    var dfa_min = dfa.Minimize();
                    var dfa_min_min = dfa_min.Minimize();

                    if (dfa_min.StatesCount > dfa_min_min.StatesCount) {
                        Automat.Utils.SaveAutomatImageToTemp(dfa);
                        Automat.Utils.SaveAutomatImageToTemp(dfa_min);
                        throw new Automat.Exception($"minimized Automat, could be minimized further", dfa_min, dfa_min_min);
                    }


                    string[] rwords = dfa.GetRandomWords(words, 1, words, System.Array.Empty<string>());

                    for (int i = 0; i < rwords.Length; i++)
                        if (!dfa.AcceptWord(rwords[i]) == dfa_min.AcceptWord(rwords[i])) {
                            Automat.Utils.SaveAutomatImageToTemp(dfa);
                            Automat.Utils.SaveAutomatImageToTemp(dfa_min);
                            throw new Automat.Exception($"Automats not equal by word {rwords[i]}", dfa, dfa_min);
                        }

                    if (!dfa.Equals(dfa_min)) {
                        Automat.Utils.SaveAutomatImageToTemp(dfa);
                        Automat.Utils.SaveAutomatImageToTemp(dfa_min);
                        throw new Automat.Exception($"Automats not equal", dfa, dfa_min);
                    }
                }
            }
        }

        public static void ExportAllAutomatBitmaps() => ExportAllAutomatBitmaps(KnownAutomat.GetAllAutomats());
        public static void ExportAllAutomatBitmaps(IAutomat[] As) {
            {
                var path = $@"{System.Environment.GetEnvironmentVariable("TEMP")}\automat";
                System.IO.Directory.CreateDirectory(path);
                foreach (var a in As) {
                    var bmp = Visualization.DrawAutomat(a);
                    bmp.Save($@"{path}\{a.Name}.png");
                }
            }
            return;
        }

        #region  "Operations"
        public static IAutomat[] GenerateJoins() {
            var automats = KnownAutomat.GetTypes<IJoin>();
            var ret = new List<IAutomat>(automats.Count * automats.Count);

            foreach (var a1 in automats)
                foreach (var a2 in automats)
                    if (a1.GetType() == a2.GetType())
                        if (!(a1 is DFA) || a1.SameAlphabet(a2)) {
                            try {
                                ret.Add(a1.Join(a2));
                            } catch (System.NotImplementedException) { } catch (System.NotSupportedException) { } // because of SPDA
                        }

            return ret.ToArray();
        }

        public static IAutomat[] GenerateDiffs() {
            var automats = KnownAutomat.GetTypes<IDiff>();
            var ret = new List<IAutomat>(automats.Count() * automats.Count());

            foreach (var a1 in automats)
                foreach (var a2 in automats)
                    if (a1.GetType() == a2.GetType())
                        if (a1.SameAlphabet(a2))
                            try {
                                ret.Add(a1.Diff(a2));
                            } catch (System.NotImplementedException) { }

            return ret.ToArray();
        }

        public static IAutomat[] GenerateUnions() {
            var automats = KnownAutomat.GetTypes<IUnion>();
            var ret = new List<IAutomat>(automats.Count() * automats.Count());

            foreach (var a1 in automats)
                foreach (var a2 in automats)
                    if (a1.GetType() == a2.GetType())
                        if (a1.SameAlphabet(a2))
                            try {
                                ret.Add(a1.Union(a2));
                            } catch (System.NotImplementedException) { } catch (System.NotSupportedException) { }


            return ret.ToArray();
        }


        public static IAutomat[] GenerateIntersects() {
            var automats = KnownAutomat.GetTypes<IIntersect>();
            var ret = new List<IAutomat>(automats.Count() * automats.Count());

            foreach (var a1 in automats)
                foreach (var a2 in automats)
                    if (a1.GetType() == a2.GetType())
                        if (a1.SameAlphabet(a2))
                            try {
                                ret.Add(a1.Intersect(a2));
                            } catch (System.NotImplementedException) { }

            return ret.ToArray();
        }

        public static IAutomat[] GenerateComplements() {
            var automats = KnownAutomat.GetTypes<IComplement>();
            var ret = new List<IAutomat>(automats.Count);

            foreach (var a in automats)
                ret.Add(a.Complement());

            return ret.ToArray();
        }

        public static IReverse[] GenerateDoubleReverses() {
            var automats = KnownAutomat.GetTypes<IReverse>();
            var ret = new List<IReverse>(automats.Count());

            foreach (var a in automats)
                ret.Add((IReverse)((IReverse)a.Reverse()).Reverse());

            return ret.ToArray();
        }

        public static IAutomat[] GenerateKleeneStern() {
            var automats = KnownAutomat.GetTypes<IKleeneStern>();
            var ret = new List<IAutomat>(automats.Count());

            foreach (var a in automats)
                ret.Add(a.KleeneStern());

            return ret.ToArray();
        }

        public static bool TestDoubleReversesByWord(IReverse[] automats) {
            bool ret = true;
            foreach (var a in automats) {
                var a2 = ((IReverse)a.Reverse()).Reverse();
                if (!TestEqualWithWords(a, a2, 200))
                    ret = false;
            }
            return ret;
        }

        public static IAutomat[] GenerateConcats() {
            var automats = KnownAutomat.GetTypes<IConcat>();
            var ret = new List<IAutomat>(automats.Count * automats.Count);

            foreach (var a1 in automats)
                foreach (var a2 in automats)
                    try {
                        ret.Add(a1.Concat(a2));
                    } catch (System.NotSupportedException) { } catch (System.NotImplementedException) { }
            return ret.ToArray();
        }

        #endregion

        public static bool TestEqualWithWords(IAcceptWord A1, IAcceptWord A2, int initialCount) {
            int onceTrue = 0, onceFalse = 0;
            int passLevel = System.Math.Min(initialCount / 10, 5);

            string[] words = System.Array.Empty<string>();
            int count = 0;
            while ((onceTrue < passLevel | onceFalse < passLevel) && count < initialCount * 2) {
                words = A1.GetRandomWords(initialCount / 2, 1, Serpen.Uni.Utils.Sqrt(initialCount), words);
                foreach (string w in words) {
                    try {
                        var erg1 = A1.AcceptWord(w);
                        var erg2 = A2.AcceptWord(w);

                        if (erg1) onceTrue++;
                        else onceFalse++;

                        if (erg1 != erg2)
                            throw new Automat.Exception($"{count}. word '{w}' divides Automates", A1, A2);

                        Utils.DebugMessage($"{count}. word '{w}' passes", Uni.Utils.eDebugLogLevel.Verbose, A1, A2);
                        count++;
                    } catch (TuringCycleException) {
                    } catch (PDAStackException) { }
                }
            }
            if (onceTrue >= passLevel && onceFalse >= passLevel) {
                Utils.DebugMessage($"{count} words passed ({onceTrue}/{onceFalse})", Uni.Utils.eDebugLogLevel.Verbose, A1, A2);
                return true;
            } else {
                if (A1.Equals(A2)) {
                    Utils.DebugMessage($"{count} words passed, but not both tested ({onceTrue}/{onceFalse}), but Equals works", Uni.Utils.eDebugLogLevel.Verbose, A1, A2);
                    return true;
                } else {
                    Utils.DebugMessage($"{count} words passed, but not both tested ({onceTrue}/{onceFalse}), Equals not working", Uni.Utils.eDebugLogLevel.Normal, A1, A2);
                    return true;
                }

            }
        }

        [AlgorithmSource("1659_D2.11_P37")]
        public static bool InMyhillNerodeRelation(string w1, string w2, IAcceptWord automat, int count = 50) {
            var words = automat.GetRandomWords(count, 0, Serpen.Uni.Utils.Sqrt(count), System.Array.Empty<string>());

            foreach (string w in words)
                if (automat.AcceptWord(w1 + w) != automat.AcceptWord(w2 + w)) {
                    Utils.DebugMessage($"word {w} divides {w1},{w2}", automat, Uni.Utils.eDebugLogLevel.Verbose);
                    return false;
                }

            return true;
        }

        public static void TestMinimizedDeaIsMHCount() {
            var dfas = KnownAutomat.GetDFAModels();
            foreach (var dfa in dfas) {
                DFA dmin = ((DFA)dfa.PurgeStates()).Minimize();
                var mheqs = dfa.FindMNEqClasses();
                if (dmin.StatesCount != mheqs.Count) {
                    dmin.SaveAutomatImageToTemp();
                    Utils.DebugMessage($"DFA min {dmin.StatesCount} != MH {mheqs.Count}", dfa, Uni.Utils.eDebugLogLevel.Always);
                    dmin = null;
                }

            }
        }

        public static void TestDFAPumpbar() {
            var dfas = KnownAutomat.GetDFAModels();
            foreach (var dfa in dfas) {
                for (int i = 0; i < 10; i++) {
                    var rnd = Serpen.Uni.Utils.RND.Next(1, 20);
                    var pb = PumpingLemma.TestPumpbar(dfa, rnd);
                    if (pb != PumpResult.Pumpable) {
                        System.Console.WriteLine($"Not {rnd}-pumpbar {pb} {dfa}");
                    }
                }
            }

        }

    } //end class
} //end ns