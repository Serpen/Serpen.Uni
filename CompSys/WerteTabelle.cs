
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
                Array[i, Array.GetLength(1)-1] = array[i];
            VarCount = vars;
        
        }
        public WerteTabelle(int vars, params int[] array) {
            Array = Utils.GetPowerSet(vars, 1);
            for (int i = 0; i < array.GetLength(0); i++)
                Array[array[i], Array.GetLength(1)-1] = true;
            VarCount = vars;
        }

        #endregion

        public bool Invoke(params bool[] b) {
            int number = 0;
            for (int i = b.Length - 1; i >= 0 ; i--)
                number += b[i] ? Utils.Pow2(i) : 0;
            return Array[number, Array.GetLength(1)-1];
        }

        public override string ToString() {
            var sb = new System.Text.StringBuilder();
            
            int countlen = (int)System.Math.Log10(Array.GetLength(0))+2;

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

        public static WerteTabelle T25_2 => new WerteTabelle(4, new bool[] {
            true, false, true, false, true, true, true, true,
            false, false, true, true, false, false, false, false});
        
        public static WerteTabelle wiki_de_qmcc => new WerteTabelle(4,
            0, 1, 4, 5, 6, 7, 8, 9, 11, 15
        );
        
    }
}