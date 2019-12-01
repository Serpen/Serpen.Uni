using Serpen.Uni.Automat.Finite;
using Serpen.Uni.Automat.ContextFree;
using Serpen.Uni.Automat.Turing;
using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat {
    public static class Tests {

        public static void RunAllTests() {
            var allAutomats1 = new List<IAutomat>();
            allAutomats1.AddRange(GenerateRandomAutomats(100));

            allAutomats1.AddRange(GenerateComplements());
            allAutomats1.AddRange(GenerateComplements());
            allAutomats1.AddRange(GenerateConcats());
            allAutomats1.AddRange(GenerateDiffs());
            allAutomats1.AddRange(GenerateIntersects());
            allAutomats1.AddRange(GenerateJoins());
            allAutomats1.AddRange(GenerateKleeneStern());

            var revs = GenerateReverses();
            TestDoubleReversesByWord(revs);
            allAutomats1.AddRange(revs);

            allAutomats1.AddRange(GenerateUnions());

            var allAutomats = CastToEveryPossibility(allAutomats1);
            
            for (int i = 0; i < allAutomats.GetLength(0); i++) {
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

                    DFA DfromN = Converter.Nea2TeilmengenDea(N);
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

                    DFA DfromNe = Converter.Nea2TeilmengenDea(Ne);
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
                } else if (automat is TuringMachineSingleBand tm1) {
                    ret1Automat.Add(tm1);
                } else {
                    ret1Automat.Add(automat);
                }

                retAutomats.Add(ret1Automat.ToArray());
            }

            return retAutomats.ToArray();
        }

        [System.Obsolete()]
        public static bool CastingEquality() { //IAutomat[][] automats
            foreach (DFA D in KnownAutomat.GetDFAModels()) {
                NFA NfromD = (NFA)D;
                NFAe NEfromD = (NFAe)D;
                // NFAe NEfromN = (NFAe)NfromD;

                if (!D.Equals(D))
                    return false;
                if (!D.Equals(NfromD))
                    return false;
                if (!D.Equals(NEfromD))
                    return false;

                if (!NfromD.Equals(D))
                    return false;
                if (!NfromD.Equals(NfromD))
                    return false;
                if (!NfromD.Equals(NEfromD))
                    return false;

                if (!NEfromD.Equals(D))
                    return false;
                if (!NEfromD.Equals(NfromD))
                    return false;
                if (!NEfromD.Equals(NEfromD))
                    return false;
            }

            foreach (var N in KnownAutomat.GetNFAModels()) {
                DFA DfromN = Converter.Nea2TeilmengenDea(N);
                NFAe NEfromD = (NFAe)DfromN;
                NFAe NEfromN = (NFAe)N;

                if (!DfromN.Equals(DfromN))
                    return false;
                if (!DfromN.Equals(N))
                    return false;
                if (!DfromN.Equals(NEfromD))
                    return false;

                if (!N.Equals(DfromN))
                    return false;
                if (!N.Equals(N))
                    return false;
                if (!N.Equals(NEfromD))
                    return false;

                if (!NEfromD.Equals(DfromN))
                    return false;
                if (!NEfromD.Equals(N))
                    return false;
                if (!NEfromD.Equals(NEfromD))
                    return false;
            }

            foreach (var Ne in KnownAutomat.GetNFAeModels()) {
                DFA DfromNe = Converter.Nea2TeilmengenDea(Ne);
                NFA NFromD = (NFA)DfromNe;

                if (!DfromNe.Equals(DfromNe))
                    return false;
                if (!DfromNe.Equals(NFromD))
                    return false;
                if (!DfromNe.Equals(Ne))
                    return false;

                if (!NFromD.Equals(DfromNe))
                    return false;
                if (!NFromD.Equals(NFromD))
                    return false;
                if (!NFromD.Equals(Ne))
                    return false;

                if (!Ne.Equals(DfromNe))
                    return false;
                if (!Ne.Equals(NFromD))
                    return false;
                if (!Ne.Equals(Ne))
                    return false;
            }

            return true;
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
                if (a is NFA nfa) {
                    var nfa_removed = nfa.PurgeStates();
                    string[] rwords = nfa.GetRandomWords(words, 1, words, System.Array.Empty<string>());
                    for (int i = 0; i < rwords.Length; i++) {
                        if (!nfa.AcceptWord(rwords[i]) == nfa_removed.AcceptWord(rwords[i]))
                            throw new Automat.Exception($"Automats not equal by word {rwords[i]}", nfa, nfa_removed);
                    }
                    if (!nfa.Equals(nfa_removed)) {
                        throw new Automat.Exception("Automats not equal", nfa, nfa_removed);
                    }
                }
            }
        }

        /// <summary>
        /// Test if minimized Automats euqal their base
        /// </summary>
        public static void MinimizeEquality(Automat.Finite.DFA[] automats, int words = 100) {
            foreach (IAutomat a in automats) {
                if (a is DFA dfa) {
                    var dfa_min = dfa.MinimizeTF();
                    var dfa_min_min = dfa_min.MinimizeTF();

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
                        if (!(a1 is DFA) || a1.SameAlphabet(a2))
                            ret.Add(a1.Join(a2));

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
                            } catch (System.NotImplementedException) { }


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

        public static IReverse[] GenerateReverses() {
            var automats = KnownAutomat.GetTypes<IReverse>();
            var ret = new List<IReverse>(automats.Count());

            foreach (var a in automats)
                ret.Add((IReverse)a.Reverse());

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

        public static bool TestEqualWithWords(IAutomat A1, IAutomat A2, int initialCount) {
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
                        if (!erg1) onceFalse++;
                        if (erg1 != erg2) {
                            Utils.DebugMessage($"{count}. word '{w}' divides Automates", Uni.Utils.eDebugLogLevel.Always, A1, A2);
                            return false;
                        }
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

    } //end class
} //end ns