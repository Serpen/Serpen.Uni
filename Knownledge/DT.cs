using System.Collections.Generic;
using System.Linq;


namespace Serpen.Uni.Knownledge {

    // TODO: doppelte Att auf gleicher Baumebene
    public class DT {
        private readonly Sample[] samples;
        private readonly string[] attNames;

        private readonly IDictionary<string, string[]> AttAuspraegungen;

        public DT(Sample[] samples, string[] AttNames) {
            this.samples = samples;
            attNames = AttNames;

            AttAuspraegungen = new Dictionary<string, string[]>(AttNames.Length);
            foreach (var a in attNames)
                AttAuspraegungen.Add(a, samples.Select(x => x[attNameToNum(a)]).Distinct().ToArray());
        }

        public dynamic Invoke(dynamic def) => Invoke(samples, attNames, null);

        [AlgorithmSource("1696_A5.6_P166", "KI_A18.5_P803")]
        dynamic Invoke(IReadOnlyCollection<DT.Sample> examples, IEnumerable<string> attributes, dynamic def) {
            Utils.DebugMessage("E: " + string.Join(",", examples) + ", A: " + string.Join(",", attributes), Utils.eDebugLogLevel.Normal);
            if (examples.Count() == 0) {
                return def;
            } else if (examples.All(x => x.Result == true)) {
                return true;
            } else if (examples.All(x => x.Result == false))
                return false;
            else if (attributes.Count() == 0)
                throw new System.ApplicationException("gleiche Beispiele mit unterschiedlicher Klassifikation");
            else {
                var an = ChooseAttribut(attributes, examples);
                var T = new Serpen.Uni.Graph.TreeNode<string>(an);
                foreach (var w in AttAuspraegungen[an]) {
                    var Ei = examples.Where(e => e[(int)attNames.ArrayIndex(an)] == w).ToList();
                    var Ai = attributes.Where(x => x != an).ToList();
                    // TODO: don't allow same nodes on same level
                    var Ti = Invoke(Ei, Ai, MajorityVal(examples));
                    if (Ti.GetType() != typeof(bool))
                        T.AddChild(Ti);
                }
                return T;
            }
        }

        string ChooseAttribut(IEnumerable<string> Attributes, IReadOnlyCollection<Sample> Examples) {
            var max = new System.Tuple<string, float>(string.Empty, 0f);
            foreach (var at in Attributes) {
                // var y = gains[attNames.ArrayIndex(i)];
                var gr = gainRation(Examples.ToArray(), at);
                // var x = SplitNodes(E, i);
                if (gr > max.Item2)
                    max = new System.Tuple<string, float>(at, gr);
            }
            Utils.DebugMessage($"best Attr {max.Item1} {max.Item2:N2} with {Examples.Count} samples and {Attributes.Count()} attributes", Utils.eDebugLogLevel.Normal);
            return max.Item1;
        }

        int attNameToNum(string attname)
            => (int)attNames.ArrayIndex(attname);

        [AlgorithmSource("FUH1656_5.3.5")]
        public float gain(Sample[] samples, string att) {
            var nodes = SplitNodes(samples, attNameToNum(att));
            var pos = samples.Where(x => x.Result).Count();
            var ie = Informationsgehalt(pos, samples.Length);
            var z = 0f;
            var swdebug = new System.Text.StringBuilder(att + " " + ie.ToString("0.00") + "-[");
            foreach (var item in nodes) {
                int count = item.Sum(x => x.Value.Count);
                int ipos = item.SelectMany(x => x.Value).Where(x => x.Result == true).Count();
                int ineg = item.SelectMany(x => x.Value).Where(x => x.Result == false).Count();
                z += (float)count / samples.Length * Informationsgehalt(ipos, count);
                swdebug.AppendFormat("+{0}/{1}*H({2}/{0}; {3}/{0})", count, samples.Length, ipos, ineg);
            }
            swdebug.Append("] = " + (ie - z).ToString("0.00"));
            Utils.DebugMessage(swdebug.ToString(), Utils.eDebugLogLevel.Verbose);
            return ie - z;
        }

        [AlgorithmSource("FUH1656_5.3.5")]
        public float splitInfo(Sample[] samples, string att) {
            var distinctValRatio = samples
                            .Select(x => x[attNameToNum(att)])
                            .GroupBy(x => x)
                            .Select(x => (float)x.Count() / samples.Count());
            var ret = Informationsgehalt(distinctValRatio.ToArray());
            Utils.DebugMessage("H(" + string.Join(";", distinctValRatio) + ") = " + ret, Utils.eDebugLogLevel.Verbose);
            return ret;
        }

        [AlgorithmSource("FUH1656_5.3.5")]
        public float gainRation(Sample[] samples, string att) {
            var ret = gain(samples, att) / splitInfo(samples, att);
            Utils.DebugMessage($"{att}, S#{samples.Length}: " + ret, Utils.eDebugLogLevel.Verbose);
            return ret;
        }

        [AlgorithmSource("FUH1656_5.3.5")]
        public float Informationsgehalt(params float[] v) {
            double ret = 0;
            for (int i = 0; i < v.Length; i++) {
                if (v[i] != 0)
                    ret -= v[i] * System.Math.Log2(v[i]);
                else
                    ret -= 0;
            }
            return (float)ret;
        }

        public float Informationsgehalt(int pos, int total)
            => Informationsgehalt((float)pos/total, (float)(total-pos)/total);

        dynamic MajorityVal(IEnumerable<Sample> E) {
            return false;
            throw new System.NotImplementedException();
        }

        public IEnumerable<IGrouping<string, KeyValuePair<System.Tuple<string, bool>, List<Sample>>>> SplitNodes(IList<Sample> samples, int intcat) {
            var parts = new Dictionary<System.Tuple<string, bool>, List<Sample>>();
            foreach (var s in samples)
            {
                var t = new System.Tuple<string, bool>(s[intcat], s.Result);
                if (parts.ContainsKey(t))
                    parts[t].Add(s);
                else
                    parts.Add(t, new List<Sample>() { s });
            }
            Utils.DebugMessage($"{attNames[intcat]} splits {samples.Count} samples into {parts.Count} distinct {parts.GroupBy(x=>x.Key.Item1).Count()}", Utils.eDebugLogLevel.Verbose);
            return parts.GroupBy(x=>x.Key.Item1);
        }

        public class Sample {
            private readonly string[] Attributes;
            public bool Result { get; }
            public string Name { get; }

            public Sample(string name, bool result, params string[] atts) {
                Result = result;
                this.Attributes = atts;
                this.Name = name;
            }
            public string this[int num] { get => Attributes[num]; }

            public override bool Equals(object obj) {
                if (obj is Sample objsample)
                    return objsample.Name == this.Name;
                return false;
            }

            public override int GetHashCode() => Name.GetHashCode();

            public override string ToString() => Name;

            public static Sample[] EA {
                get => new Sample[] {
                    new Sample("01",false,"Handwerker","groß","gering","gut"),
                    new Sample("02",false,"Handwerker","gering","gering","neutral"),
                    new Sample("03",true,"Handwerker","mittel","mittel","gut"),
                    new Sample("04",false,"Handwerker","mittel","mittel","schlecht"),
                    new Sample("05",true,"Beratungsnetz","mittel","hoch","neutral"),
                    new Sample("06",false,"Beratungsnetz","gering","mittel","neutral"),
                    new Sample("07",true,"Beratungsnetz","groß","mittel","schlecht"),
                    new Sample("08",true,"Beratungsnetz","mittel","gering","gut"),
                    new Sample("09",false,"Online-Shop","groß","hoch","schlecht"),
                    new Sample("10",false,"Online-Shop","mittel","mittel","schlecht"),
                    new Sample("11",true,"Online-Shop","mittel","gering","gut"),
                    new Sample("12",true,"Online-Shop","groß","hoch","gut")
                };
            }
            public static string[] EA_Names = { "Bereich", "Aufwand", "Attraktivität", "Bauchgefühl" };

            public static string[] A53_Kino_Names = { "Attraktivität", "Preis", "Loge", "Wetter", "Warten", "Besetzung", "Kategorie", "Land", "Reservierung", "Gruppe" };
            public static Sample[] A53_Kino {
                get => new Sample[] {
                    new Sample("01",true,"hoch","€€","ja","schlecht","ja","top","AC","int.","ja","Freunde"),
                    new Sample("02",true,"mittel","€","ja","mittel","nein","mittel","KO","int.","nein","Paar"),
                    new Sample("03",false,"mittel","€","nein","mittel","ja","mittel","DR","int.","nein","Freunde"),
                    new Sample("04",false,"gering","€","ja","mittel","ja","mittel","SF","int.","nein","allein"),
                    new Sample("05",true,"mittel","€","ja","mittel","nein","mittel","DR","int.","nein","Paar"),
                    new Sample("06",true,"hoch","€€","ja","schön","nein","top","SF","int.","ja","Freunde"),
                    new Sample("07",true,"mittel","€","ja","schlecht","nein","mittel","KO","nat.","nein","Freunde"),
                    new Sample("08",true,"mittel","€","nein","schlecht","ja","mittel","AC","int.","nein","Freunde"),
                    new Sample("09",false,"gering","€","ja","schön","nein","mittel","KO","nat.","nein","Freunde"),
                    new Sample("10",false,"mittel","€","ja","schön","nein","mittel","KO","int.","nein","Paar"),
                    new Sample("11",true,"hoch","€","ja","mittel","ja","top","DR","int.","nein","Paar"),
                    new Sample("12",false,"mittel","€","nein","schlecht","ja","mittel","AC","nat.","nein","allein"),
                    new Sample("13",false,"hoch","€€","ja","mittel","ja","mittel","SF","int.","nein","allein"),
                    new Sample("14",false,"mittel","€","ja","schön","ja","top","DR","int.","ja","Freunde"),
                    new Sample("15",true,"mittel","€","ja","schlecht","nein","mittel","AC","int.","nein","Paar")
                };
            }
        }
    }
}