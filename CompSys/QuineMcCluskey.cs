using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.CompSys {
    struct QuineMcCluskeyRow {
        const int IRRELEVANT = -1;
        sbyte[] Row;

        public QuineMcCluskeyRow(bool[,] Array, int row, string index) {
            Row = new sbyte[Array.GetLength(1) - 1];
            for (int j = 0; j < Row.Length; j++)
                Row[j] = (sbyte)(Array[row, j] ? 1 : 0);

            Processed = false;
            Index = index;
        }

        public int CountOnes => Row.Count(b => b == 1);
        public bool Processed;
        public string Index;

        public QuineMcCluskeyRow? OneDifferent(ref QuineMcCluskeyRow other) {
            const int NO_ROW = -1;
            int differentRow = NO_ROW;

            for (int i = 0; i < Row.Length; i++)
                if (Row[i] != other.Row[i]) {
                    if (differentRow == NO_ROW && Row[i] != IRRELEVANT && other.Row[i] != IRRELEVANT)
                        differentRow = i;
                    else {
                        differentRow = NO_ROW;
                        break;
                    }
                }

            if (differentRow != NO_ROW) {

                var clone = new QuineMcCluskeyRow();

                clone.Index = $"{this.Index},{other.Index}";
                clone.Row = new sbyte[this.Row.Length];
                this.Row.CopyTo(clone.Row, 0);
                clone.Row[differentRow] = IRRELEVANT;
                // other.Row[differentRow] = IRRELEVANT;

                Processed = true;
                other.Processed = true;

                Utils.DebugMessage($"compare ({this}) with ({other}) == true", Utils.eDebugLogLevel.Verbose);
                return clone;
            } else {
                Utils.DebugMessage($"compare ({this}) with ({other}) == false", Utils.eDebugLogLevel.Verbose);
                return null;
            }
        }

        public override int GetHashCode() {
            int hc = Row.Length;
            for (int i = 0; i < Row.Length; i++)
                hc = hc * 128 + Row[i];

            return hc;
        }

        public override bool Equals(object obj) => obj is QuineMcCluskeyRow && obj.GetHashCode() == this.GetHashCode();

        public override string ToString() => $"{Index.PadLeft(5)} {string.Join(',', Row)} {CountOnes} {Processed}";

        
        [AlgorithmSource("~1608 2.2.3")]
        public static string[] QuineMcCluskey(WerteTabelle wt) {
            QuineMcCluskeyRow[] minTerms;

            int minTermCount = 0;
            for (int i = 0; i < wt.Array.GetLength(0); i++)
                if (wt.Array[i, wt.Array.GetLength(1) - 1]) minTermCount++;

            minTerms = new QuineMcCluskeyRow[minTermCount];

            int j = 0;
            for (int i = 0; i < minTerms.Length; i++) {
                while (!wt.Array[j, wt.Array.GetLength(1) - 1])
                    j++;
                minTerms[i] = new QuineMcCluskeyRow(wt.Array, j, j.ToString());
                j++;
            }

            minTerms = minTerms.OrderBy(a => a.CountOnes).ToArray();

            while (QuineMcCluskeyRow.Step2(ref minTerms)) ;

            return QuineMcCluskeyRow.WesentlichePrimimplikanten(minTerms, minTermCount);
        }

        [AlgorithmSource("~1608 2.2.3")]
        internal static bool Step2(ref QuineMcCluskeyRow[] qmcRows) {
            var newTerms = new System.Collections.Generic.HashSet<QuineMcCluskeyRow>();

            for (int i = 0; i < qmcRows.Length - 1; i++) {
                for (int j = i + 1; j < qmcRows.Length; j++) {
                    if (qmcRows[j].CountOnes == qmcRows[i].CountOnes)
                        continue;
                    else if (qmcRows[j].CountOnes > qmcRows[i].CountOnes + 1)
                        break;
                    else {// # == #+1
                        var newTerm = qmcRows[i].OneDifferent(ref qmcRows[j]);
                        if (newTerm.HasValue)
                            newTerms.Add(newTerm.Value);
                    }
                }
            }

            bool foundnew = newTerms.Any();

            foreach (var item in qmcRows)
                if (!item.Processed)
                    newTerms.Add(item);

            qmcRows = newTerms.OrderBy(a => a.CountOnes).ToArray();

            return foundnew;
        }

        [AlgorithmSource("~1608 2.2.3", "~wiki:Verfahren_nach_Quine_und_McCluskey")]
        internal static string[] WesentlichePrimimplikanten(QuineMcCluskeyRow[] qmcRows, int indiciesCount) {
            
            var matchTable = new Dictionary<int, int>();
            int mtIndex = 1;

            var table = new int[qmcRows.Length+1, indiciesCount+1];

            //generate Primeimplicanttable
            for (int r = 0; r < qmcRows.Length; r++) {
                var inds = (from i in qmcRows[r].Index.Split(',') select System.Convert.ToInt32(i));
                foreach (var ind in inds) {
                    int tindex;
                    if (!matchTable.TryGetValue(ind, out tindex)) {
                        tindex = mtIndex;
                        matchTable.Add(ind, mtIndex++);
                    }
                    table[r+1, tindex] = 1;
                }
            }

            // border description
            table[0,0]=-1;
            foreach (var item in matchTable)
                table[0, item.Value] = item.Key;
            for (int i = 1; i < table.GetLength(0); i++)
                table[i,0] = i-1;

            Utils.DebugMessage("Primimplikanttable:\n" + Utils.FormatArray(table), Utils.eDebugLogLevel.Normal);

            var processedIndicies = new List<int>();



            //find Kernimplikanten
            var Kernimplikanten = new List<int>();
            for (int c = 1; c < table.GetLength(1); c++) {
                int pindex = -1;
                for (int r = 1; r < table.GetLength(0); r++) {
                    if (table[r, c] == 1 && !processedIndicies.Contains(c))
                        if (pindex == -1)
                            pindex = r;
                        else {
                            pindex = -1;
                            break;
                        }
                }
                if (pindex != -1)
                    if (!Kernimplikanten.Contains(pindex)) {
                        // yield return pindex;
                        Kernimplikanten.Add(pindex);
                        IEnumerable<int> inds = (from i in qmcRows[pindex-1].Index.Split(',') select System.Convert.ToInt32(i));
                        foreach (var ind in inds)
                            processedIndicies.Add(matchTable[ind]);

                    }
            } //next c

            Utils.DebugMessage($"found Kernimplikanten: {string.Join(',', Kernimplikanten)}", Utils.eDebugLogLevel.Normal);

            for (int c = 1; c < table.GetLength(1); c++)
                if (!processedIndicies.Contains(c))
                    for (int r = 1; r < table.GetLength(0); r++)
                        if (table[r, c]==1)
                            Kernimplikanten.Add(r);

            // Dominanz(ref table);

            Utils.DebugMessage($"found implikanten: {string.Join(',', Kernimplikanten)}", Utils.eDebugLogLevel.Normal);

            var kistr = new string[Kernimplikanten.Count];
            for (int i = 0; i < Kernimplikanten.Count; i++)
                kistr[i] = qmcRows[Kernimplikanten[i]-1].Index;

            return kistr;
            // return Kernimplikanten;
        }

        [AlgorithmSource("wikipedia")]
        internal static void Dominanz(ref int[,] table) {
            Utils.DebugMessage('\n' + Utils.FormatArray(table), Utils.eDebugLogLevel.Normal);

            int removeColumn = -1;
            int removeRow = -1;

            removeColumn = SpaltenDominanz(table);
            if (removeColumn == -1)
                removeRow = ZeilenDominanz(table);

            while (removeColumn != -1 || removeRow != -1) {
                if (removeColumn != -1) {
                    table = table.RemoveArrayCol(removeColumn);
                    Utils.DebugMessage($"remove Col {removeColumn}: \n{Utils.FormatArray(table)}", Utils.eDebugLogLevel.Normal);
                } else if (removeRow != -1) {
                    table = table.RemoveArrayRow(removeRow);
                    Utils.DebugMessage($"remove Row {removeRow}: \n{Utils.FormatArray(table)}", Utils.eDebugLogLevel.Normal);
                }

                removeColumn = SpaltenDominanz(table);
                if (removeColumn == -1)
                    removeRow = ZeilenDominanz(table);

            }
        }

        static int SpaltenDominanz(int[,] table) {
            for (int c = 1; c < table.GetLength(1); c++) {
                for (int c2 = c + 1; c2 < table.GetLength(1); c2++) {
                    bool isTeilmenge = true;
                    for (int r = 1; r < table.GetLength(0); r++) {
                        if (table[r, c]!=1 && table[r, c2]==1)
                            isTeilmenge = false;
                    }
                    if (isTeilmenge)
                        return c;
                }
            }
            return -1;
        }

        static int ZeilenDominanz(int[,] table) {
            for (int r1 = 1; r1 < table.GetLength(0); r1++) {
                for (int r2 = r1 + 1; r2 < table.GetLength(0); r2++) {
                    bool isTeilmenge = true;
                    for (int c = 1; c < table.GetLength(1); c++) {
                        if (table[r1, c]!=1 && table[r2, c]==1)
                            isTeilmenge = false;
                    }
                    if (isTeilmenge)
                        return r2;
                }
            }
            return -1;
        }
    }

}