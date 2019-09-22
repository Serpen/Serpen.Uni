using Serpen.Uni.Automat.Finite;
using Serpen.Uni.Automat.ContextFree;
using Serpen.Uni.Automat.Turing;

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

        public static bool CastingEquality() {
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

        public static bool Joins() {
            
            FABase[] As = KnownAutomat.GetAllFiniteAutomats();

            for (int i = 0; i < As.Length; i++)
            {
                for (int j = 0; j < As.Length; j++)
                {
                    var d1 = As[i];
                    var d2 = As[j];
                    
                    if (d1.GetType()==d2.GetType())
                        if (Utils.SameAlphabet(d1,d2))
                            d1.Join(d2);
                }
            }
            
            return true;
        }

        public static bool TestEqualAutomatWords(IAutomat A1, IAutomat A2, int count, int maxlen) {
            var rnd = Utils.RND;

            var sw = new System.Text.StringBuilder(maxlen);
            for (int c = 0; c < count; c++)
            {
                int l = rnd.Next(maxlen);
                sw.Clear();

                for (int i = 0; i < l; i++)
                {
                    sw.Append(A1.Alphabet[rnd.Next(A1.Alphabet.Length)]);
                }

                var erg1 = A1.AcceptWord(sw.ToString());
                var erg2 = A2.AcceptWord(sw.ToString());

                if (erg1 != erg2) {
                    System.Console.WriteLine($"word {c} {sw.ToString()} divides Automates");
                    //return false;
                } else
                    System.Console.WriteLine($"word {c} {sw.ToString()} passes");

            }
            System.Console.WriteLine($"{count} words passed, last {sw.ToString()}");
            return true;
        }
        
    } //end class
} //end ns