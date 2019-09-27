using Serpen.Uni.Automat.Finite;
using Serpen.Uni.Automat.ContextFree;
using Serpen.Uni.Automat.Turing;
using System.Linq;

namespace Serpen.Uni.Automat {
    public static class Tests {

        public static IAutomat[][] CastToEveryPossibility(IAutomat[] As) {
            var Automates = new System.Collections.Generic.List<IAutomat[]>(); 

            foreach (IAutomat A in As) {
                var list = new System.Collections.Generic.List<IAutomat>();
                if (A is DFA D) {
                    NFA NfromD = (NFA)D;
                    NFAe NEfromD = (NFAe)D;
                    NFAe NEfromN = (NFAe)NfromD;
                    StatePDA QPDAfromNe = (StatePDA)NEfromD;
                    TuringMachineSingleBand TMfromD = (TuringMachineSingleBand)D;

                    list.Add(D);
                    list.Add(NfromD);
                    list.Add(NfromD);
                    list.Add(NEfromD);
                    try {
                        DPDA DPDAFromD = (DPDA)D;
                        list.Add(DPDAFromD);
                    } catch {}
                    list.Add(QPDAfromNe);
                    list.Add(TMfromD);
                } else if (A is NFA N) {
                    DFA DfromN = Converter.Nea2TeilmengenDea(N);
                    NFAe NEfromD = (NFAe)DfromN;
                    NFAe NEfromN = (NFAe)N;
                    StatePDA QPDAfromNe = (StatePDA)NEfromD;

                    list.Add(N);
                    list.Add(DfromN);
                    list.Add(NEfromD);
                    list.Add(NEfromN);
                    list.Add(QPDAfromNe);
                } else if (A is NFAe Ne) {
                    DFA DfromNe = Converter.Nea2TeilmengenDea(Ne);
                    // NFA NfromD = (NFAe)DfromNe;
                    StatePDA QPDAfromNe = (StatePDA)Ne;

                    list.Add(Ne);
                    list.Add(DfromNe);
                    list.Add(QPDAfromNe);
                } else if (A is StatePDA QPDA) {
                    list.Add(QPDA);
                    
                    try {
                        var SPDAfromQPDA = (StackPDA)QPDA;
                        list.Add(SPDAfromQPDA);
                    } catch {}
                } else if (A is StackPDA SPDA) {
                    var QPDAfromSPDA = (StackPDA)SPDA;

                    list.Add(SPDA);
                    list.Add(QPDAfromSPDA);
                }
                Automates.Add(list.ToArray());
            }

            return Automates.ToArray();
        }

        [System.Obsolete()]
        public static bool CastingEquality() { //IAutomat[][] automats
            foreach (DFA D in KnownAutomat.GetDFAModels()) {
                NFA NfromD = (NFA)D;
                NFAe NEfromD = (NFAe)D;
                NFAe NEfromN = (NFAe)NfromD;

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
            var retAut = new System.Collections.Generic.List<IAutomat>();
            for (int i = 0; i < count; i++)
            {
                retAut.Add(DFA.GenerateRandom());
                retAut.Add(NFA.GenerateRandom());
                retAut.Add(NFAe.GenerateRandom());
                retAut.Add(StatePDA.GenerateRandom());
                retAut.Add(StackPDA.GenerateRandom());
                retAut.Add(DPDA.GenerateRandom());
                retAut.Add(TuringMachineSingleBand.GenerateRandom());
                retAut.Add(TuringMachineSingleBand1659.GenerateRandom());
                // retAut.Add(TuringMachineMultiTrack.GenerateRandom());
            }
            return retAut.ToArray();
        }

        public static void PurgeEquality(IAutomat[] automats, int words = 100) {
            foreach (IAutomat a in automats) {
                if (a is NFA nfa) {
                    var nfa_removed = nfa.PurgeStates();
                    string[] rwords = nfa.GetRandomWords(words); 
                    for (int i = 0; i < rwords.Length; i++)
                    {
                        if (!nfa.AcceptWord(rwords[i]) == nfa_removed.AcceptWord(rwords[i]))
                            throw new Uni.Exception($"Automats {nfa} not equal {nfa_removed} by word");
                    }
                    if (!nfa.Equals(nfa_removed)) {
                        throw new Uni.Exception($"Automats {nfa} not equal {nfa_removed}");
                    }
                }
            }
        }

        public static void ExportAllAutomatBitmaps() => ExportAllAutomatBitmaps(KnownAutomat.GetAllAutomats());
        public static void ExportAllAutomatBitmaps(IAutomat[] As) {
            {
                var path = $@"{System.Environment.GetEnvironmentVariable("TEMP")}\automat";
                System.IO.Directory.CreateDirectory(path);
                foreach (var a in As)
                {
                    var bmp = Visualization.DrawAutomat(a);
                    bmp.Save($@"{path}\{a.Name}.png");
                }
            }
            return;
        }

#region  "Operations"
        public static FABase[] GenerateJoins() {
            var finiteAutomats = KnownAutomat.GetAllFiniteAutomats();
            var ret = new System.Collections.Generic.List<FABase>(finiteAutomats.Length*finiteAutomats.Length);

            foreach (var d1 in finiteAutomats)
            {
                foreach (var d2 in finiteAutomats)
                {
                    if (d1.GetType()==d2.GetType())
                        if (Utils.SameAlphabet(d1,d2))
                            ret.Add(d1.Join(d2));
                }
            }
            return ret.ToArray();
        }

        public static FABase[] GenerateDiffs() {
            var finiteAutomats = (from fa in KnownAutomat.GetAllFiniteAutomats() where fa is DFA da select (DFA)fa);
            var ret = new System.Collections.Generic.List<FABase>(finiteAutomats.Count()*finiteAutomats.Count());

            foreach (var d1 in finiteAutomats)
                foreach (var d2 in finiteAutomats)
                    if (d1.GetType()==d2.GetType())
                        if (Utils.SameAlphabet(d1,d2))
                            ret.Add(DFA.Diff(d1,d2));
            
            return ret.ToArray();
        }

        public static FABase[] GenerateUnions() {
            var finiteAutomats = (from fa in KnownAutomat.GetAllFiniteAutomats() where fa is DFA da select (DFA)fa);
            var ret = new System.Collections.Generic.List<FABase>(finiteAutomats.Count()*finiteAutomats.Count());

            foreach (var d1 in finiteAutomats)
                foreach (var d2 in finiteAutomats)
                    if (d1.GetType()==d2.GetType())
                        if (Utils.SameAlphabet(d1,d2)) {
                            ret.Add(d1.UnionNEA(d2));
                            ret.Add(DFA.UnionProduct(d1,d2));
                        }
            
            return ret.ToArray();
        }

        
        public static IAutomat[] GenerateIntersects() {
            var finiteAutomats = (from fa in KnownAutomat.GetAllFiniteAutomats() where fa is DFA da select (DFA)fa);
            var ret = new System.Collections.Generic.List<IAutomat>(finiteAutomats.Count()*finiteAutomats.Count());

            foreach (var d1 in finiteAutomats)
                foreach (var d2 in finiteAutomats)
                    if (d1.GetType()==d2.GetType())
                        if (Utils.SameAlphabet(d1,d2)) {
                            ret.Add(DFA.Intersect(d1,d2));
                        }
            
            return ret.ToArray();
        }
        
        public static IAutomat[] GenerateComplements() {
            var finiteAutomats = KnownAutomat.GetAllFiniteAutomats();
            var ret = new System.Collections.Generic.List<IAutomat>(finiteAutomats.Length);

            foreach (var fa1 in finiteAutomats)
                ret.Add(fa1.Complement());
            
            return ret.ToArray();
        }

        public static IAutomat[] GenerateReverses() {
            var finiteAutomats = KnownAutomat.GetAllFiniteAutomats();
            var ret = new System.Collections.Generic.List<IAutomat>(finiteAutomats.Length);

            foreach (var fa1 in finiteAutomats)
                ret.Add(fa1.Reverse());
            
            return ret.ToArray();
        }
        
        public static IAutomat[] GenerateCconcats() {
            var finiteAutomats = KnownAutomat.GetAllFiniteAutomats();
            var ret = new System.Collections.Generic.List<IAutomat>(finiteAutomats.Length*finiteAutomats.Length);

            foreach (var fa1 in finiteAutomats)
            foreach (var fa2 in finiteAutomats)
                ret.Add(fa1.Concat(fa2));
            
            return ret.ToArray();
        }

#endregion
        

        public static bool TestEqualWithWords(IAutomat A1, IAutomat A2, int count, int maxlen) {
            var rnd = Utils.RND;

            string[] words = A1.GetRandomWords(count);
            foreach (string w in words)
            {
                var erg1 = A1.AcceptWord(w);
                var erg2 = A2.AcceptWord(w);

                if (erg1 != erg2) {
                    Utils.DebugMessage($"word '{w}' divides Automates", A1);
                    return false;
                } else
                    Utils.DebugMessage($"word '{w}' passes", A1);

            }
            Utils.DebugMessage($"{count} words passed", A1);
            return true;
        }
        
    } //end class
} //end ns