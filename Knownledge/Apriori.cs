using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Knownledge {

    [Serpen.Uni.AlgorithmSource("FUH1656_6.4")]
    class Apriori {

        internal readonly bool[,] DB;
        public readonly float MinSupport;
        public readonly float MinConf;

        public readonly int RowCount;
        public readonly int TransactionCount;


        public Apriori(bool[,] db, float minsupport, float minconf) {
            DB = db;
            MinSupport = minsupport;
            MinConf = minconf;

            RowCount = db.GetLength(0);
            TransactionCount = db.GetLength(1);

            Transactions = new List<int>[TransactionCount];
            for (int t = 0; t < TransactionCount; t++) {
                var itms = new List<int>();
                for (int i = 0; i < RowCount; i++)
                    if (db[i, t])
                        itms.Add(i);

                Transactions[t] = itms;
            }

            support = new float[RowCount];
            for (int i = 0; i < RowCount; i++) {
                int count = 0;
                for (int t = 0; t < TransactionCount; t++)
                    if (db[i, t])
                        count++;
                support[i] = (float)count / RowCount;
            }
        }

        internal readonly List<int>[] Transactions;
        readonly float[] support;

        string FormatListOut(List<List<int>> l) {
            var sb = new System.Text.StringBuilder();
            foreach (var item in l) {
                var lst = new List<string>();
                sb.Append("{");
                foreach (var subitem in item) {
                    int num = (char)(65 + subitem);
                    if (num >= 65 + 8) num++;
                    lst.Add(((char)num).ToString());
                }
                sb.Append(string.Join(",", lst) + "}, ");
            }
            if (sb.Length > 2)
                sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        public List<List<int>> AprioriAlg() {
            var L = new Dictionary<int, List<List<int>>>();
            L.Add(1, Frequent1Sets());

            Serpen.Uni.Utils.DebugMessage("L1 " + FormatListOut(L[1]), Serpen.Uni.Utils.eDebugLogLevel.Normal);

            int k = 2;
            while (L.ContainsKey(k - 1) && L[k - 1].Any()) {
                var Ck = AprioriGen(L[k - 1], k - 1);

                var ccountList = new Dictionary<List<int>, int>();

                for (int t = 0; t < TransactionCount; t++) {
                    var Ct = from x in Ck
                             where Serpen.Uni.Utils.ContainsSubset(Transactions[t], x)
                             select x;

                    foreach (var candidate in Ct)
                        if (!ccountList.ContainsKey(candidate))
                            ccountList.Add(candidate, 1);
                        else
                            ccountList[candidate]++;
                }

                L[k] = new List<List<int>>();
                foreach (var c in Ck)
                    if (ccountList.ContainsKey(c) && ccountList[c] >= (float)TransactionCount * MinSupport)
                        L[k].Add(c);


                Serpen.Uni.Utils.DebugMessage("Lk " + FormatListOut(L[k]), Serpen.Uni.Utils.eDebugLogLevel.Normal);

                k++;
            }

            var ret = (from x in L.Values.SelectMany(y => y) select x).ToList();
            Serpen.Uni.Utils.DebugMessage("Ret " + FormatListOut(ret), Serpen.Uni.Utils.eDebugLogLevel.Normal);
            return ret;
        }

        List<List<int>> Frequent1Sets() {
            var L1 = new List<List<int>>();
            for (int p = 0; p < RowCount; p++) {
                int sum = 0;
                for (int t = 0; t < TransactionCount; t++)
                    if (DB[p, t]) sum++;

                if ((float)sum / TransactionCount >= MinSupport)
                    L1.Add(new List<int>() { p });
            }
            return L1;
        }

        [AlgorithmSource("1696WBS_A6.2_P206")]
        public List<List<int>> AprioriGen(List<List<int>> L_km1, int k) {
            Serpen.Uni.Utils.DebugMessage("in " + FormatListOut(L_km1), Serpen.Uni.Utils.eDebugLogLevel.Normal);

            List<List<int>> Ck = new List<List<int>>();

            foreach (var p in L_km1) {
                foreach (var q in L_km1) {
                    if (p != q) {
                        var e1 = p.Take(k - 1); // first k-2 matching elements
                        if (e1.SequenceEqual(q.Take(k - 1))) {
                            var p1 = p[k - 1];
                            var q1 = q[k - 1];
                            if (p1 < q1)
                                Ck.Add(new List<int>(e1) { p1, q1 });
                        }
                    }
                }
            }

            // Teilmengencheck
            var ret = new List<List<int>>(Ck);
            foreach (var c in Ck) {
                var parts = Utils.GetPowerSet(c).Where(x => x.Count() == c.Count - 1).ToArray();
                foreach (var s in parts) {
                    bool exist = false;
                    foreach (var lx in L_km1) {
                        if (lx.ContainsSubset(s)) {
                            exist = true;
                        }
                    }
                    if (!exist)
                        ret.Remove(c);
                }
            }

            Serpen.Uni.Utils.DebugMessage("out " + FormatListOut(ret), Serpen.Uni.Utils.eDebugLogLevel.Normal);

            return ret;
        }

        public float Support(IEnumerable<int> list) {
            int sum = 0;
            foreach (var t in Transactions) {
                if (Serpen.Uni.Utils.ContainsSubset(t, list)) {
                    sum++;
                }
            }
            return (float)sum / TransactionCount;
        }

        public float Confidence(IEnumerable<int> list1, IEnumerable<int> list2)
            => Support(list1.Concat(list2).ToList()) / Support(list1);

        public void AssocRules(List<List<int>> list) {
            foreach (var l in list) {
                var perm = Utils.GetPermutations(l, l.Count);
                var avoidDoubles = new List<string>();

                foreach (var p in perm) {
                    if (p.Count() > 1) {
                        var array = p.ToArray();
                        for (int splitpos = 1; splitpos < p.Count(); splitpos++) {
                            var left = p.Take(splitpos);
                            var right = p.Skip(splitpos);

                            string text = string.Join(",", left.OrderBy(s => s)) + "->" + string.Join(",", right.OrderBy(s => s));
                            if (!avoidDoubles.Contains(text)) {
                                avoidDoubles.Add(text);
                                float conf = Confidence(left, right);
                                float supp = Support(p);
                                if (conf >= MinConf)
                                    System.Console.WriteLine(text.PadRight(10) + " s:" + supp + " c:" + conf);
                            }

                        }
                    }
                }

            }
        }

        public static bool[,] Drogeriemarkt {
            get { //GetLenght(0)=Produkte, 1=Transationen
                return new bool[,] {
                {true, false, false, false, true, false, true, false, true, false}, // Seife A
                {true, true, true,true, false, true, false, true, true, true}, // Shampoo B
                {false, true, true,true,false,true,false,true,true,false}, // Haarspülung C
                {true,false,false,true,false,true,true,false,true,true},
                {true,false,true,false,true,false,true,false,false,false},
                {false,false,true,false,true,false,false,false,false,false},
                {false,true,false,true,false,false,false,true,false,false},
                {false,true,false,false,false,false,false,false,false,false},
                {false,false,true,true,true,true,true,true,false,false},
                {false,false,false,false,false,true,false,true,false,false},
                {false,true,false,true,false,true,false,true,false,true} // Kosmetikartikel L
            };
            }
        }
    }
}