using System;
using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Knownledge.Versionspace {

    [AlgorithmSource("FUH1656_A5.13_P184")]
    public class Versionspace {
        IReadOnlyCollection<Hypothese> S;
        IReadOnlyCollection<Hypothese> G;

        public Versionspace() {

        }

        string[][] AttAusPraeg;

        public IEnumerable<Hypothese> Invoke(Sample[] samples) {
            int AttCount = samples.First().array.Length;
            String[] Sstr = new string[AttCount];
            String[] Gstr = new string[AttCount];
            AttAusPraeg = new string[AttCount][];
            for (int i = 0; i < AttCount; i++) {
                Sstr[i] = Hypothese.Null;
                Gstr[i] = Hypothese.Any;
            }
            G = new List<Hypothese>() { new Hypothese(Gstr) };
            S = new List<Hypothese>() { new Hypothese(Sstr) };

            for (int i = 0; i < AttCount; i++)
                AttAusPraeg[i] = samples.Select(s => s.array[i]).Distinct().ToArray();

            Utils.DebugMessage("S: " + string.Join(",", S), Utils.eDebugLogLevel.Normal);
            Utils.DebugMessage("G: " + string.Join(",", G), Utils.eDebugLogLevel.Normal);
            foreach (var sample in samples) {

                Utils.DebugMessage("enter " + sample, Utils.eDebugLogLevel.Normal);

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
                    S = newS.Distinct().ToList();
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
                            newG.AddRange(h.minSpecialized(sample, S.ToArray(), AttAusPraeg));
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
                    G = newG.Distinct().ToList();
                }
                Utils.DebugMessage("S: " + string.Join(",", S), Utils.eDebugLogLevel.Normal);
                Utils.DebugMessage("G: " + string.Join(",", G), Utils.eDebugLogLevel.Normal);

                if (S.Count == 0 || G.Count == 0) {
                    Utils.DebugMessage("VS collapsed", Utils.eDebugLogLevel.Always);
                    return System.Array.Empty<Hypothese>();
                }

            }

            return S.Union(G).Distinct();
        }

        public static void runTest() {
            var vs = new Versionspace();

            foreach (var s in new Sample[][] { Sample.Sportsendungen, Sample.JapanCars, Sample.SportWeather, Sample.JapanCarsCollapse }) {
                var erg = vs.Invoke(s).OrderBy(x => x.ToString());
                for (int i = 0; i < s.Length * s.Length; i++) {
                    var r = s.Randomize();
                    System.Console.WriteLine();
                    var erg2 = vs.Invoke(r.ToArray()).OrderBy(x => x.ToString());
                    if (!erg.All(x => erg2.Contains(x)) || !erg2.All(x => erg.Contains(x)))
                        throw new System.ApplicationException($"{s} != {r}");
                }
            }
        }
    }

    public class Hypothese : ICloneable {
        internal readonly string[] array;

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
                // if (x.array[i] != y.array[i] && x.array[i] != Hypothese.Any && y.array[i] != Hypothese.Any)
                if (x.array[i] != y.array[i] && y.array[i] == Hypothese.Any)
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

        public override bool Equals(object obj) {
            if (obj is Hypothese objh)
                return objh.ToString() == this.ToString();
            return false;
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
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

        internal IEnumerable<Hypothese> minCommon(Hypothese s) {
            var ret = new List<Hypothese>() { (Hypothese)this.Clone() };

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
            Utils.DebugMessage($"minCommon({this}, {s}) = {String.Join(",", ret)}", Utils.eDebugLogLevel.Normal);
            return ret;
        }

        internal IEnumerable<Hypothese> minSpecialized(Hypothese s, Hypothese[] S, string[][] AttAuspraeg) { //TODO: LastS[]
            var ret = new List<Hypothese>();
            foreach (var LastS in S) {
                for (int i = 0; i < LastS.array.Length; i++) {
                    if (this[i] != LastS[i] && LastS[i] != s[i]) {
                        if (!LastS.array.All(x => x == Hypothese.Null)) {
                            var newH = (Hypothese)this.Clone();
                            newH[i] = LastS[i];
                            ret.Add(newH);
                        } else { // if LastS is 0 is opposite attributes instead
                            for (int j = 0; j < AttAuspraeg[i].Length; j++) {
                                if (AttAuspraeg[i][j] != s[i]) {
                                    var newH = (Hypothese)this.Clone();
                                    newH[i] = AttAuspraeg[i][j];
                                    ret.Add(newH);
                                }
                            }
                        }
                    }
                }
            }
            Utils.DebugMessage($"({this}, {s}) = {String.Join(",", ret)}", Utils.eDebugLogLevel.Normal);
            
            return ret;
        }

        public object Clone() {
            return new Hypothese((string[])this.array.Clone());
        }
    }

    public class Sample : Hypothese {
        public Sample(bool result, params string[] array) : base(array) {
            Result = result;
        }

        public bool Result { get; }

        public override string ToString() {
            return (Result ? "+" : "-") + base.ToString();
        }

        //////////////////////////// Samples ////////////////////////////
        public static Sample[] Sportsendungen = {
            new Sample(true, "Fußball", "Mannschaft", "draußen", "national","Samstag"),
            new Sample(true, "Hockey", "Mannschaft", "draußen", "national","Samstag"),
            new Sample(false, "Bodenturnen", "Einzel", "drinnen", "Welt","Samstag"),
            new Sample(true, "Handball", "Mannschaft", "drinnen", "national","Samstag"),
            new Sample(false, "Zehnkampf", "Einzel", "draußen", "Welt","Sonntag")
        };

        public static Sample[] EA45_2_1_collapse = {
            new Sample(false,"Handwerker","groß","gering","gut"),
            new Sample(false,"Handwerker","gering","gering","neutral"),
            new Sample(true,"Handwerker","mittel","mittel","gut"),
            new Sample(false,"Handwerker","mittel","mittel","schlecht"),
            new Sample(true,"Beratungsnetz","mittel","hoch","neutral"),
            new Sample(false,"Beratungsnetz","gering","mittel","neutral"),
            new Sample(true,"Beratungsnetz","groß","mittel","schlecht"),
            new Sample(true,"Beratungsnetz","mittel","gering","gut"),
            new Sample(false,"Online-Shop","groß","hoch","schlecht"),
            new Sample(false,"Online-Shop","mittel","mittel","schlecht"),
            new Sample(true,"Online-Shop","mittel","gering","gut"),
            new Sample(true,"Online-Shop","groß","hoch","gut")
        };

        // public static Sample[] EA45_2_2 = {
        //     new Sample(true, "Feldsalat", "Pute"),
        //     new Sample(false, "Feldsalat", "Schwein"),
        //     new Sample(true, "Chicoree", "Huhn"),
        //     new Sample(false, "Tomate", "Huhn"),
        //     new Sample(false, "Paprika", "Pute")
        // };

        // http://www2.cs.uregina.ca/~dbd/cs831/notes/ml/vspace/vs_prob1.html
        public static Sample[] JapanCars = {
            new Sample(true, "Japan","Honda","Blue","1980","Economy"),
            new Sample(false, "Japan","Toyota","Green","1970","Sports"),
            new Sample(true, "Japan","Toyota","Blue","1990","Economy"),
            new Sample(false, "USA","Chrysler","Red","1980","Economy"),
            new Sample(true, "Japan","Honda","White","1980","Economy"),
            new Sample(true, "Japan","Toyota","Green","1980","Economy")
        };

        public static Sample[] JapanCarsCollapse = JapanCars.Append(new Sample(false, "Japan", "Honda", "Red", "1990", "Economy")).ToArray();

        public static Sample[] SportWeather = {
            new Sample(true, "Sunny","Warm","Normal","Strong","Warm","Same"),
            new Sample(true, "Sunny","Warm","High","Strong","Warm","Same"),
            new Sample(false,"Rainy","Cold","High","Strong","Warm","Change"),
            new Sample(true, "Sunny","Warm","High","Strong","Cool","Change")
        };
    }

}