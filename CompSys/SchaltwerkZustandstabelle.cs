using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.CompSys {
    public class Schaltwerkzustandstabelle {

        public class StatePair {
            public StatePair(int first, int second) {
                if (first > second) {
                    First = second;
                    Second = first;
                } else {
                    First = first;
                    Second = second;
                }
            }

            public readonly int First;
            public readonly int Second;

            internal bool Canceled = false;

            public override string ToString() => $"{(Canceled ? "-" : "")}{First},{Second}{(Canceled ? "-" : "")}";

            public override bool Equals(object obj) =>
                (obj is StatePair objsp) && objsp.First == this.First && objsp.Second == this.Second;

            public override int GetHashCode() => ToString().GetHashCode();


        }

        [AlgorithmSource("K1608_3.5.2")]
        public static object Minimize(int[,] table) {
            var col = new Dictionary<StatePair, List<StatePair>>();

            var stupidRefCollection = new HashSet<StatePair>();

            // init phase
            for (int i = 0; i < table.GetLength(0) - 1; i++) {
                for (int j = i + 1; j < table.GetLength(0); j++) {
                    if (table[i, 3] == table[j, 3]) {

                        var pair = new StatePair(table[i, 0], table[j, 0]);
                        if (stupidRefCollection.TryGetValue(pair, out StatePair outPair))
                            pair = outPair;
                        else
                            stupidRefCollection.Add(pair);

                        if (!col.ContainsKey(pair))
                            col.Add(pair, new List<StatePair>());

                        var nextpair = new StatePair(table[i, 1], table[j, 1]);
                        if (stupidRefCollection.TryGetValue(nextpair, out outPair))
                            nextpair = outPair;
                        else
                            stupidRefCollection.Add(nextpair);

                        if (!col[pair].Contains(nextpair) && !pair.Equals(nextpair))
                            col[pair].Add(nextpair);

                        nextpair = new StatePair(table[i, 2], table[j, 2]);
                        if (stupidRefCollection.TryGetValue(nextpair, out outPair))
                            nextpair = outPair;
                        else
                            stupidRefCollection.Add(nextpair);

                        if (!col[pair].Contains(nextpair) && !pair.Equals(nextpair))
                            col[pair].Add(nextpair);
                    } // end if
                } // next j
            } // next i

            // needed all as references to work that way, so (1,2) could be same as (1,2)
            bool hasChanges;
            int run = 0;
            do {
                Utils.DebugMessage($"run {++run}", Utils.eDebugLogLevel.Normal);
                hasChanges = false;
                foreach (var item in col) {
                    foreach (var nextItem in item.Value) {
                        if (!col.ContainsKey(nextItem) || nextItem.Canceled) {
                            if (!nextItem.Canceled) hasChanges = true;
                            if (!item.Key.Canceled) hasChanges = true;
                            nextItem.Canceled = item.Key.Canceled = true;

                        }

                    }
                }
            } while (hasChanges);

            var sb = new System.Text.StringBuilder();

            foreach (var item in col.Where(a => !a.Key.Canceled)) {
                sb.Append(item.Key.ToString() + '=');
                foreach (var nextitem in item.Value.Where(a => !a.Canceled)) {
                    sb.Append(nextitem);
                }
                sb.Append("; ");
            }

            return sb.ToString();
        }

        public static int[,] K1608_ZT_T33 = {
            {1,4,3,0},
            {2,6,8,0},
            {3,5,4,1},
            {4,1,5,0},
            {5,3,1,1},
            {6,6,2,1},
            {7,2,8,0},
            {8,3,7,1}
        };
    }
}