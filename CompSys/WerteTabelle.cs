// using System.Linq;

namespace Serpen.Uni.CompSys {

    public class WerteTabelle {
        public readonly int VarCount;

        public readonly bool[,] Array;
        public WerteTabelle(bool[,] array) {
            Array = array;
            VarCount = array.GetLength(1) - 1;
        }

        [System.Obsolete()]
        public Schaltfunktion toDNF() {
            Schaltfunktion.Schaltfunction sf = (a) => false;

            for (int i = 0; i < Array.GetLength(0); i++)
                if (Array[i, VarCount]) {
                    Schaltfunktion.Schaltfunction mt = (a) => true;

                    for (int j = 0; j < VarCount; j++) {
                        if (Array[i, j])
                            mt = (a) => mt(a) & a[j];
                        else
                            mt = (a) => mt(a) & !a[j];
                        mt = delegate (bool[] b) { return mt(b); };
                    }
                    sf = (a) => sf(a) | mt(a);
                }

            return new Schaltfunktion(VarCount, sf);
        }

        public string[] QuineMcCluskey() {
            QuineMcCluskeyRow[] minTerms;

            int minTermCount = 0;
            for (int i = 0; i < Array.GetLength(0); i++)
                if (Array[i, Array.GetLength(1) - 1]) minTermCount++;

            minTerms = new QuineMcCluskeyRow[minTermCount];

            int j = 0;
            for (int i = 0; i < minTerms.Length; i++) {
                while (!Array[j, Array.GetLength(1) - 1])
                    j++;
                minTerms[i] = new QuineMcCluskeyRow(Array, j, j.ToString());
                j++;
            }

            while (QuineMcCluskeyRow.Step2(ref minTerms));

            return QuineMcCluskeyRow.WesentlichePrimimplikanten(minTerms, minTermCount);
        }

        public override string ToString() {
            var sb = new System.Text.StringBuilder();
            for (byte y = 0; y < Array.GetLength(1) - 1; y++)
                sb.Append($" {y} |");
            sb.AppendLine(" f ");
            sb.AppendLine(new string('-', sb.Length - 1));

            for (int x = 0; x < Array.GetLength(0); x++) {
                for (byte y = 0; y < Array.GetLength(1) - 1; y++)
                    sb.Append($" {(Array[x, y] ? 1 : 0)} |");

                sb.AppendLine($" {(Array[x, Array.GetLength(1) - 1] ? 1 : 0)}");
            }
            return sb.ToString();
        } //end function ToString()

        public static WerteTabelle T25 => new WerteTabelle(new bool[,] {
                    {false, false, false, false, true},
                    {false, false, false, true, false},
                    {false, false, true, false, true},
                    {false, false, true, true, false},
                    {false, true, false, false, true},
                    {false, true, false, true, true},
                    {false, true, true, false, true},
                    {false, true, true, true, true},
                    {true, false, false, false, false},
                    {true, false, false, true, false},
                    {true, false, true, false, true},
                    {true, false, true, true, true},
                    {true, true, false, false, false},
                    {true, true, false, true, false},
                    {true, true, true, false, false},
                    {true, true, true, true, false},
                });
    }
}