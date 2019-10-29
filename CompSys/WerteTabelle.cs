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
                            mt = delegate (bool[] b) {return mt(b);};
                    }
                    sf = (a) => sf(a) | mt(a);
                }

            return new Schaltfunktion(VarCount, sf);
        }

        public Schaltfunktion McCluskey() {
            return null;
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
    }
}