
namespace Serpen.Uni.CompSys {

    public class WerteTabelle {
        public readonly int VarCount;

        public readonly bool[,] Array;

        #region Constructors

        public WerteTabelle(bool[,] array) {
            Array = array;
            VarCount = array.GetLength(1) - 1;
        }

        public WerteTabelle(int vars, params bool[] array) {
            if (Utils.Pow2(vars) != array.Length)
                throw new System.ArgumentOutOfRangeException();

            Array = Utils.GetPowerSet(vars, 1);
            for (int i = 0; i < Array.GetLength(0); i++)
                Array[i, Array.GetLength(1) - 1] = array[i];
            VarCount = vars;

        }
        public WerteTabelle(int vars, params int[] array) {
            Array = Utils.GetPowerSet(vars, 1);
            for (int i = 0; i < array.GetLength(0); i++)
                Array[array[i], Array.GetLength(1) - 1] = true;
            VarCount = vars;
        }

        #endregion

        public bool Invoke(params bool[] b) {
            int number = 0;
            for (int i = b.Length - 1; i >= 0; i--)
                number += b[i] ? Utils.Pow2(i) : 0;
            return Array[number, Array.GetLength(1) - 1];
        }

        public string toKDNF() {
            var sb = new System.Text.StringBuilder();

            for (int r = 0; r < Array.GetLength(0); r++) {
                if (Array[r, Array.GetLength(1)-1]) {
                    int c;
                    for (c = 0; c < Array.GetLength(1) - 2; c++) {
                        sb.Append((Array[r, c] ? "x" : "-x") + $"{Array.GetLength(1)-1-c} & ");
                    }
                    sb.Append((Array[r, c] ? "x" : "-x") + $"{Array.GetLength(1)-1-c} | ");
                }
            }
            sb = sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }
        public string toKKNF() {
            var sb = new System.Text.StringBuilder();

            for (int r = 0; r < Array.GetLength(0); r++) {
                if (!Array[r, Array.GetLength(1)-1]) {
                    int c;
                    sb.Append('(');
                    for (c = 0; c < Array.GetLength(1) - 2; c++) {
                        sb.Append((Array[r, c] ? "-x" : "x") + $"{Array.GetLength(1)-1-c} | ");
                    }
                    sb.Append((Array[r, c] ? "-x" : "x") + $"{Array.GetLength(1)-1-c}) & ");
                }
            }
            sb = sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        public override string ToString() {
            var sb = new System.Text.StringBuilder();

            int countlen = (int)System.Math.Log10(Array.GetLength(0)) + 2;

            // write head
            sb.Append(new string('#', countlen));
            for (byte y = 0; y < Array.GetLength(1) - 1; y++)
                sb.Append($" {y} |");
            sb.AppendLine(" f ");
            sb.AppendLine(new string('-', sb.Length - 1));

            // write values
            for (int x = 0; x < Array.GetLength(0); x++) {
                sb.Append($"#{x}".PadRight(countlen));
                for (byte y = 0; y < Array.GetLength(1) - 1; y++)
                    sb.Append($" {(Array[x, y] ? 1 : 0)} |");

                sb.AppendLine($" {(Array[x, Array.GetLength(1) - 1] ? 1 : 0)}");
            }
            return sb.ToString();
        } //end function

        public static WerteTabelle VT_1608_T25 => new WerteTabelle(4, new bool[] {
            true, false, true, false, true, true, true, true,
            false, false, true, true, false, false, false, false});

        public static WerteTabelle VT_wikide_qmcc => new WerteTabelle(4,
            0, 1, 4, 5, 6, 7, 8, 9, 11, 15
        );

    }
}