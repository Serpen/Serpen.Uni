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

        public override string ToString() => $"{Index} {string.Join(',', Row)} {CountOnes} {Processed}";

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

        internal static string[] WesentlichePrimimplikanten(QuineMcCluskeyRow[] qmcRows, int indiciesCount) {

            // var Indicies = new List<int>();
            // foreach (var item in minTerms)
            //     Indicies.AddRange(from i in item.Index.Split(',') select System.Convert.ToInt32(i));
            // Indicies = Indicies.Distinct().ToList();

            var matchTable = new Dictionary<int, int>();

            bool[,] table = new bool[qmcRows.Length, indiciesCount];
            int mtIndex = 0;
            for (int r = 0; r < qmcRows.Length; r++) {
                IEnumerable<int> inds = (from i in qmcRows[r].Index.Split(',') select System.Convert.ToInt32(i));
                foreach (var ind in inds) {
                    int tindex;
                    if (!matchTable.TryGetValue(ind, out tindex)) {
                        tindex = mtIndex;
                        matchTable.Add(ind, mtIndex++);
                    }
                    table[r, tindex] = true;
                }
            }

            var processedIndicies = new List<int>();

            //find Kernimplikanten
            var Kernimplikanten = new List<int>();
            for (int c = 0; c < table.GetLength(1); c++) {
                int pindex = -1;
                for (int r = 0; r < table.GetLength(0); r++) {
                    if (table[r, c] && !processedIndicies.Contains(c))
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
                        IEnumerable<int> inds = (from i in qmcRows[pindex].Index.Split(',') select System.Convert.ToInt32(i));
                        foreach (var ind in inds)
                            processedIndicies.Add(matchTable[ind]);

                    }
            } //next c

            Utils.DebugMessage($"found Kernimplikanten: {string.Join(',', Kernimplikanten)}", Utils.eDebugLogLevel.Normal);

            for (int c = 0; c < table.GetLength(1); c++) {
                if (!processedIndicies.Contains(c))
                    for (int r = 0; r < table.GetLength(0); r++) {
                        if (table[r, c]) {
                            // yield return r;
                            Kernimplikanten.Add(r);
                            // unnessesarry?! : 
                            IEnumerable<int> inds = (from i in qmcRows[r].Index.Split(',') select System.Convert.ToInt32(i));
                            foreach (var ind in inds)
                                processedIndicies.Add(matchTable[ind]);
                        }
                    }
            }

            Utils.DebugMessage($"found implikanten: {string.Join(',', Kernimplikanten)}", Utils.eDebugLogLevel.Normal);
            
            var kistr = new string[Kernimplikanten.Count];
            for (int i = 0; i < Kernimplikanten.Count; i++)
                kistr[i] = qmcRows[i].Index;
            
            return kistr;
            // return Kernimplikanten;
        }
        
        
    }

}