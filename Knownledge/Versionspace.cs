using System;
using System.Collections.Generic;

namespace Serpen.Uni.Knownledge {

    [AlgorithmSource("FUH1656_A5.13_P184")]
    class Versionspace {
        public List<Hypothese> S;
        public List<Hypothese> G;

        Hypothese LastS { get => S[S.Count - 1]; }

        public Versionspace() {
            S = new List<Hypothese>() { new Hypothese(Hypothese.Null, Hypothese.Null, Hypothese.Null, Hypothese.Null, Hypothese.Null) };
            G = new List<Hypothese>() { new Hypothese(Hypothese.Any, Hypothese.Any, Hypothese.Any, Hypothese.Any, Hypothese.Any) };
        }

        public void Invoke(Sample[] samples) {
            foreach (var sample in samples) {
                Utils.DebugMessage("S: " + string.Join(",", S), Utils.eDebugLogLevel.Normal);
                Utils.DebugMessage("G: " + string.Join(",", G), Utils.eDebugLogLevel.Normal);
                if (sample.Result) {
                    var newG = new List<Hypothese>(G);
                    foreach (var g in G) {
                        if (!g.Match(sample))
                            newG.Remove(g);
                    }
                    G = newG;

                    var newS = new List<Hypothese>(S);
                    foreach (var h in S) {
                        if (!h.Match(sample)) {
                            newS.Remove(h);

                            newS.AddRange(h.minCommon(sample));
                        }
                    }

                    S = newS;
                    newS = new List<Hypothese>(S);

                    foreach (var s in S) {
                        foreach (var s2 in S) {
                            if (s != s2 && s2 > s)
                                newS.Remove(s2);
                        }
                    }
                    S = newS;
                } else { // -sample
                    var newS = new List<Hypothese>(S);
                    foreach (var s in G) {
                        if (s.Match(sample))
                            newS.Remove(s);
                    }
                    S = newS;

                    var newG = new List<Hypothese>(G);
                    foreach (var h in G) {
                        if (h.Match(sample)) {
                            newG.Remove(h);
                            newG.AddRange(h.minSpecialized(sample, LastS));
                        }
                    }

                    G = newG;
                    newG = new List<Hypothese>(G);

                    foreach (var g in G) {
                        foreach (var g2 in G) {
                            if (g != g2 && g2 < g) {
                                newG.Remove(g2);
                            }
                        }
                    }
                    G = newG;
                }
            }
        }
    }

    static class VSSample {
        public static Sample[] Sportsendungen = {
            new Sample(true, "Fußball", "Mannschaft", "draußen", "national","Samstag"),
            new Sample(true, "Hockey", "Mannschaft", "draußen", "national","Samstag"),
            new Sample(false, "Bodenturnen", "Einzel", "drinnen", "Welt","Samstag"),
            new Sample(true, "Handball", "Mannschaft", "drinnen", "national","Samstag"),
            new Sample(true, "Zehnkampf", "Einzel", "draußen", "Welt","Sonntag")
        };
    }

    class Hypothese {
        private readonly string[] array;

        public const string Null = "0";
        public const string Any = "*";

        public Hypothese(params string[] array) {
            this.array = array;
        }

        public string this[int index] {
            get {
                return array[index];
            }
            private set {
                array[index] = value;
            }
        }

        public static bool operator >(Hypothese x, Hypothese y) { // allgemeiner
            bool covers = true;
            if (x.array.Length != y.array.Length) throw new System.ApplicationException("Hypothese Params differ");
            for (int i = 0; i < x.array.Length; i++) {
                if (x.array[i] != y.array[i] && x.array[i] != Hypothese.Any)
                    return false;
            }
            return covers;
        }
        public static bool operator <(Hypothese x, Hypothese y) { // spezieller
            bool covers = false;
            if (x.array.Length != y.array.Length) throw new System.ApplicationException("Hypothese Params differ");
            for (int i = 0; i < x.array.Length; i++) {
                if (x.array[i] != y.array[i] && x.array[i] != Hypothese.Any)
                    covers = true;
                else if (x[i] != y[i])
                    return false;
            }
            return covers;
        }

        public bool Match(Sample s) {
            bool match = true;
            if (s.array.Length != array.Length) throw new System.ApplicationException("Hypothese Params differ");
            for (int i = 0; i < array.Length; i++) {
                if (array[i] != s.array[i] && array[i] != Hypothese.Any)
                    return false;
            }
            return match;
        }

        public override string ToString() {
            var sb = new System.Text.StringBuilder("{<");
            for (int i = 0; i < array.Length; i++) {
                if (array[i] == Hypothese.Null) // unneccessary since string
                    sb.Append("0");
                else if (array[i] == Hypothese.Any)
                    sb.Append("*");
                else
                    sb.Append(array[i]);

                if (i < array.Length - 1)
                    sb.Append(",");
            }
            sb.Append("}>");
            return sb.ToString();
        }

        internal IEnumerable<Hypothese> minCommon(Sample s) {
            var ret = new List<Hypothese>() { this };

            for (int i = 0; i < array.Length; i++) {
                if (this[i] == Hypothese.Null) {
                    foreach (var newH in ret) {
                        newH.array[i] = s[i];
                    }
                } else if (this[i] != s[i] & this[i] != Hypothese.Any) {
                    foreach (var newH in ret) {
                        newH.array[i] = "*";
                    }
                }

            }
            return ret;
        }

        internal IEnumerable<Hypothese> minSpecialized(Sample s, Hypothese LastS) { //TODO: LastS[]
            var ret = new List<Hypothese>();

            for (int i = 0; i < LastS.array.Length; i++) {
                if (this[i] != LastS[i] && LastS[i] != s[i]) {
                    var newH = new Hypothese((string[])array.Clone());
                    newH[i] = LastS[i];
                    ret.Add(newH);
                }
            }


            return ret;
        }
    }

    class Sample : Hypothese {
        public Sample(bool result, params string[] array) : base(array) {
            Result = result;
        }

        public bool Result { get; }
    }

}