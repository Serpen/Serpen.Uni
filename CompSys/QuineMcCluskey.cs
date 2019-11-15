using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.CompSys {
    struct QuineMcCluskeyRow : System.IComparable<QuineMcCluskeyRow> {
        const int IRRELEVANT = -1;
        sbyte[] Row;

        public QuineMcCluskeyRow(bool[,] Array, int row, string index) {
            Row = new sbyte[Array.GetLength(1) - 1];
            for (int j = 0; j < Row.Length; j++)
                Row[j] = (sbyte)(Array[row, j] ? 1 : 0);

            Processed = false;
            Index = index;
        }

        public readonly int CountOnes => Row.Count(b => b == 1);
        public bool Processed;
        public string Index;

        /// <summary>
        /// Checks if other row differs only in one position
        /// </summary>
        /// <returns>from both generated combined row</returns>
        public QuineMcCluskeyRow? OneDifferent(ref QuineMcCluskeyRow other) {
            const int INVALID_ROW = -1;

            int differentRow = INVALID_ROW; //number of differnt entry or invalid

            // check if only one row is differnt
            for (int i = 0; i < Row.Length; i++)
                if (Row[i] != other.Row[i]) {
                    if (differentRow == INVALID_ROW && Row[i] != IRRELEVANT && other.Row[i] != IRRELEVANT)
                        differentRow = i;
                    else { //more than one different -> reset and exit
                        differentRow = INVALID_ROW;
                        break; //or return null;
                    }
                }

            if (differentRow != INVALID_ROW) {
                Utils.DebugMessage($"only {differentRow}. row different in {this} and {other}", Utils.eDebugLogLevel.Verbose);
                var newRow = new QuineMcCluskeyRow() {
                    Index = $"{this.Index},{other.Index}",
                    Row = new sbyte[this.Row.Length],
                };
                this.Row.CopyTo(newRow.Row, 0);
                newRow.Row[differentRow] = IRRELEVANT;

                Processed = true;
                other.Processed = true;

                return newRow;
            } else {
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

        // sort CountOnes, Index.Lenght, Index
        public int CompareTo(QuineMcCluskeyRow other) =>
            4 * this.CountOnes.CompareTo(other.CountOnes) +
            2 * this.Index.Length.CompareTo(other.Index.Length) +
            1 * this.Index.CompareTo(other.Index);

        [AlgorithmSource("~1608 2.2.3")]
        public static string QuineMcCluskeyAlg(WerteTabelle wt, AlgSourceMode algSourceMode = AlgSourceMode.K1608) {
            var minTerms = new List<QuineMcCluskeyRow>();

            // generate minterm table
            for (int i = 0; i < wt.Array.GetLength(0); i++)
                if (wt.Array[i, wt.Array.GetLength(1) - 1])
                    minTerms.Add(new QuineMcCluskeyRow(wt.Array, i, i.ToString()));

            int initialMintermCount = minTerms.Count;
            minTerms.Sort();
            var minTermArray = minTerms.ToArray();
            while (QuineMcCluskeyRow.Step2(ref minTermArray)) ;

            return QuineMcCluskeyRow.WesentlichePrimimplikanten(minTermArray, algSourceMode);
        }

        [AlgorithmSource("~1608 2.2.3")]
        internal static bool Step2(ref QuineMcCluskeyRow[] qmcRows) {
            var newTerms = new HashSet<QuineMcCluskeyRow>();

            // iterate each row x each follow row
            for (int i = 0; i < qmcRows.Length - 1; i++) {
                for (int j = i + 1; j < qmcRows.Length; j++) {
                    if (qmcRows[j].CountOnes == qmcRows[i].CountOnes)
                        continue; // same count not relevant
                    else if (qmcRows[j].CountOnes > qmcRows[i].CountOnes + 1)
                        break; // sorted! -> outofbounds
                    else {// # == #+1
                        var newTerm = qmcRows[i].OneDifferent(ref qmcRows[j]);
                        if (newTerm.HasValue)
                            newTerms.Add(newTerm.Value);
                    }
                }
            }

            bool foundnew = newTerms.Any();

            // add unprocessed rules, to keep them, different from org alg, less performant
            foreach (var item in qmcRows)
                if (!item.Processed)
                    newTerms.Add(item);

            qmcRows = newTerms.OrderBy(a => a.CountOnes).ToArray();

            return foundnew;
        }

        [AlgorithmSource("~1608 2.2.3", "~wiki:Verfahren_nach_Quine_und_McCluskey")]
        internal static string WesentlichePrimimplikanten(QuineMcCluskeyRow[] qmcRows, AlgSourceMode algSourceMode) {
            //generated index columns sorted
            var colIndexs = qmcRows
                .SelectMany(r => r.Index.Split(','))
                .Distinct()
                .Select(a => System.Convert.ToInt32(a))
                .OrderBy(a => a);

            var table = new int[qmcRows.Length + 1, colIndexs.Count() + 1]; //data with border desc +1

            var matchColumIndex = new Dictionary<int, int>();
            int mcIndexInc = 1;
            foreach (var ind in colIndexs)
                matchColumIndex.Add(ind, mcIndexInc++);

            // generate primeimplicanttable
            for (int r = 0; r < qmcRows.Length; r++)
                foreach (var ind in (from i in qmcRows[r].Index.Split(',')
                                     select matchColumIndex[System.Convert.ToInt32(i)]))
                    table[r + 1, ind] = 1;

            // border description
            table[0, 0] = -1;
            foreach (var item in matchColumIndex)
                table[0, item.Value] = item.Key;
            for (int i = 1; i < table.GetLength(0); i++)
                table[i, 0] = i - 1;

            Utils.DebugMessage("Primimplikanttable:\n" + Utils.FormatArray(table), Utils.eDebugLogLevel.Normal);

            var processedIndicies = new List<int>();

            if (algSourceMode == AlgSourceMode.K1608) {
                const int ROW_NOT_UNIQUE = -1;
                var Kernimplikanten = new List<int>(); // rows

                // find Kernimplikanten, column has only one unique true row
                for (int c = 1; c < table.GetLength(1); c++) {
                    int pindex = ROW_NOT_UNIQUE;
                    for (int r = 1; r < table.GetLength(0); r++) {
                        if (table[r, c] == 1 && !processedIndicies.Contains(c))
                            if (pindex == ROW_NOT_UNIQUE)
                                pindex = r;
                            else {
                                pindex = ROW_NOT_UNIQUE;
                                break;
                            }
                    } // next r

                    if (pindex != ROW_NOT_UNIQUE)
                        if (!Kernimplikanten.Contains(pindex)) {
                            Kernimplikanten.Add(pindex);

                            processedIndicies.AddRange(
                                // matching indexes of qmcRows Indexes
                                from ind in qmcRows[pindex - 1].Index.Split(',')
                                select matchColumIndex[System.Convert.ToInt32(ind)]
                            );
                        }
                } // next c

                Utils.DebugMessage($"found Kernimplikanten: {string.Join(',', Kernimplikanten)}", Utils.eDebugLogLevel.Verbose);

                // add remaining unmatched rows
                for (int c = 1; c < table.GetLength(1); c++)
                    if (!processedIndicies.Contains(c))
                        for (int r = 1; r < table.GetLength(0); r++)
                            if (table[r, c] == 1)
                                Kernimplikanten.Add(r);

                Utils.DebugMessage($"found implikanten: {string.Join(',', Kernimplikanten)}", Utils.eDebugLogLevel.Verbose);

                var sb = new System.Text.StringBuilder();
                int i;
                for (i = 0; i < Kernimplikanten.Count - 1; i++)
                    sb.Append(qmcRowToString(qmcRows[Kernimplikanten[i] - 1]) + " | ");
                sb.Append(qmcRowToString(qmcRows[Kernimplikanten[i] - 1]));

                return sb.ToString();
            } else if (algSourceMode == AlgSourceMode.Wiki) {
                Dominanz(ref table);

                var sb = new System.Text.StringBuilder();
                int i;
                for (i = 0; i < table.GetLength(0) - 2; i++)
                    sb.Append(qmcRowToString(qmcRows[table[i + 1, 0]]) + " | ");
                sb.Append(qmcRowToString(qmcRows[table[i + 1, 0]]));

                return sb.ToString();
            } else // invalid alg source
                throw new System.NotSupportedException();
        }

        private static string qmcRowToString(QuineMcCluskeyRow qmcRow) {
            var sb = new System.Text.StringBuilder();

            int rowLen = qmcRow.Row.Length;

            for (int c = 0; c < rowLen; c++) {
                if (sb.Length > 0 && qmcRow.Row[c] != -1)
                    sb.Append(" & ");

                if (qmcRow.Row[c] == 1)
                    sb.Append($"x{rowLen - c}");
                else if (qmcRow.Row[c] == 0)
                    sb.Append($"-x{rowLen - c}");
                // else {} // -1 = IRRELEVANT = -
            } //next c
            return sb.ToString();
        }

        const int NO_DOMINANT = -1;

        /// <summary>
        /// Find Unique implicants by col, row dominance
        /// </summary>
        /// <param name="table"></param>
        [AlgorithmSource("wikipedia")]
        internal static void Dominanz(ref int[,] table) {
            int removeColumn = NO_DOMINANT;
            int removeRow = NO_DOMINANT;

            // inital first col- then row-dominance
            removeColumn = SpaltenDominanz(table);
            if (removeColumn == NO_DOMINANT)
                removeRow = ZeilenDominanz(table);

            while (removeColumn != NO_DOMINANT || removeRow != NO_DOMINANT) {
                if (removeColumn != NO_DOMINANT) {
                    table = table.RemoveArrayCol(removeColumn);
                    Utils.DebugMessage($"remove Col {removeColumn}: \n{Utils.FormatArray(table)}", Utils.eDebugLogLevel.Normal);
                } else { // if (removeRow != NO_DOMINANT) {
                    table = table.RemoveArrayRow(removeRow);
                    Utils.DebugMessage($"remove Row {removeRow}: \n{Utils.FormatArray(table)}", Utils.eDebugLogLevel.Normal);
                }

                removeColumn = SpaltenDominanz(table);
                if (removeColumn == -1)
                    removeRow = ZeilenDominanz(table);
            }
        }

        /// <summary>
        /// Find dominating subset col and return their upset col for removal 
        /// </summary>
        static int SpaltenDominanz(int[,] table) {
            for (int c1 = 1; c1 < table.GetLength(1); c1++) {
                for (int c2 = c1 + 1; c2 < table.GetLength(1); c2++) {
                    bool isSubset = true;
                    for (int r = 1; r < table.GetLength(0); r++)
                        if (table[r, c1] != 1 && table[r, c2] == 1)
                            isSubset = false;

                    if (isSubset)
                        return c1;
                } // next c2
            } // next c1
            return NO_DOMINANT;
        }

        /// <summary>
        /// Find dominating subset row and return it for removal
        /// </summary>
        static int ZeilenDominanz(int[,] table) {
            for (int r1 = 1; r1 < table.GetLength(0); r1++) {
                for (int r2 = r1 + 1; r2 < table.GetLength(0); r2++) {
                    bool isSubset = true;
                    for (int c = 1; c < table.GetLength(1); c++)
                        if (table[r1, c] != 1 && table[r2, c] == 1)
                            isSubset = false;

                    if (isSubset)
                        return r2;
                } // next r2
            } // next r1
            return NO_DOMINANT;
        }
    }

}