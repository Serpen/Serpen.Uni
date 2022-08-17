using System.Collections.Generic;
using System.Linq;

class Apriori {

    internal readonly bool[,] DB;
    public readonly float MinSupport;
    public readonly float MinConf;

    public Apriori(bool[,] db, float minsupport, float minconf) {
        DB = db;
        MinSupport = minsupport;
        MinConf = minconf;

        Transactions = new List<int>[db.GetLength(1)];
        for (int t = 0; t < db.GetLength(1); t++) {
            var itms = new List<int>();
            for (int i = 0; i < db.GetLength(0); i++) {
                if (db[i, t])
                    itms.Add(i);
            }
            Transactions[t] = itms;
        }

        support = new float[db.GetLength(0)];
        for (int i = 0; i < db.GetLength(0); i++) {
            int count = 0;
            for (int t = 0; t < db.GetLength(1); t++) {
                if (db[i, t])
                    count++;
            }
            support[i] = (float)count / db.GetLength(0);
        }
    }

    internal readonly List<int>[] Transactions;
    readonly float[] support;

    void FormatListOut(string cap, List<List<int>> l) {
        var sb = new System.Text.StringBuilder(cap + ": ");
        foreach (var item in l) {
            sb.Append("{");
            foreach (var subitem in item) {
                int num = (char)(65 + subitem);
                if (num >= 65 + 8) num++;
                sb.Append((char)num + ", ");
            }
            sb.Append("}, ");
        }
        Serpen.Uni.Utils.DebugMessage(sb.ToString(), Serpen.Uni.Utils.eDebugLogLevel.Always, 2);
    }
    void FormatListOut(string cap, List<int> l) {
        var newl = new List<List<int>>();
        foreach (var item in l)
            newl.Add(new List<int>() { item });

        FormatListOut(cap, newl);
    }


    public List<List<int>> AprioriAlg() {
        var L = new Dictionary<int, List<List<int>>>();
        L.Add(1, Frequent1Sets(DB, MinSupport));

        FormatListOut("L1", L[1]);

        int k = 2;
        while (L.ContainsKey(k - 1) && L[k - 1].Any()) {
            var Ck = AprioriGen(L[k - 1], k - 1);

            var ccountList = new Dictionary<List<int>, int>();

            for (int t = 0; t < DB.GetLength(1); t++) {
                var Ct = new List<List<int>>();
                foreach (var c in Ck) {
                    if (Serpen.Uni.Utils.ContainsSubset(Transactions[t], c)) {
                        Ct.Add(c);
                    }
                }

                foreach (var candidate in Ct) {
                    if (!ccountList.ContainsKey(candidate)) {
                        ccountList.Add(candidate, 1);
                    } else {
                        ccountList[candidate]++;
                    }
                }

            }
            L[k] = new List<List<int>>();
            foreach (var c in Ck) {
                if (ccountList.ContainsKey(c) && ccountList[c] >= (float)DB.GetLength(1) * MinSupport) {
                    L[k].Add(c);
                }
            }
            FormatListOut("Lk", L[k]);

            k++;
        }
        var ret = new List<List<int>>();
        foreach (var item in L.Values)
            ret.AddRange(item); // ?

        FormatListOut("Ret", ret);
        return ret;
    }

    static List<List<int>> Frequent1Sets(bool[,] DB, float min) {
        var L1 = new List<List<int>>();
        for (int p = 0; p < DB.GetLength(0); p++) {
            int sum = 0;
            for (int t = 0; t < DB.GetLength(1); t++)
                if (DB[p, t]) sum++;

            if ((float)sum / DB.GetLength(1) >= min)
                L1.Add(new List<int>() { p });
        }
        return L1;
    }

    Dictionary<int, int> DistinctItems(List<List<int>> DB) {
        var distincts = new Dictionary<int, int>();

        foreach (var item in DB)
            foreach (var item2 in item)
                if (!distincts.ContainsKey(item2))
                    distincts.Add(item2, 1);
                else
                    distincts[item2] += 1;

        return distincts;
    }

    public List<List<int>> AprioriGen(List<List<int>> L, int k) {
        FormatListOut("in ", L);

        List<List<int>> Ck = new List<List<int>>();
        foreach (var p in L) {
            foreach (var q in L) {
                if (p != q) {
                    bool itemsEq = true;
                    for (int i = 0; i <= k - 2; i++)
                        if (p[i] != q[i]) itemsEq = false;

                    if (itemsEq) {
                        if (p[k - 1] < q[k - 1]) {
                            List<int> next = new List<int>();
                            for (int i = 0; i <= k - 2; i++)
                                next.Add(p[i]);

                            next.Add(p[k - 1]);
                            next.Add(q[k - 1]);

                            Ck.Add(next);
                        }
                    }
                }
            }
        }

        Ck = Teilmengencheck(Ck);

        FormatListOut("out: ", Ck);

        return Ck;
    }

    private List<List<int>> Teilmengencheck(List<List<int>> L) {
        return L;
        throw new System.NotImplementedException();
    }

    [Serpen.Uni.AlgorithmSource("FUH1656_6.4")]
    public float Support(List<int> list) {
        int sum = 0;
        foreach (var t in Transactions) {
            if (Serpen.Uni.Utils.ContainsSubset(t, list)) {
                sum++;
            }
        }
        return (float)sum / DB.GetLength(1);
    }

    [Serpen.Uni.AlgorithmSource("FUH1656_6.4")]
    public float Confidence(List<int> list1, List<int> list2) {
        return Support(list1.Concat(list2).ToList()) / Support(list1);
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



