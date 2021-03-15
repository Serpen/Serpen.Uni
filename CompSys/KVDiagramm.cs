namespace Serpen.Uni.CompSys {
    public class KVDiagramm {

        public readonly bool[,] Array;

        public KVDiagramm(WerteTabelle wt) {
            switch (wt.VarCount) {
                case 1:
                    Array = new bool[1, 2];
                    Array[0, 0] = wt.Array[1, 1];
                    Array[0, 1] = wt.Array[0, 1];
                    break;
                case 2:
                    Array = new bool[2, 2];
                    Array[0, 0] = wt.Array[0, 2];
                    Array[0, 1] = wt.Array[1, 2];
                    Array[1, 0] = wt.Array[2, 2];
                    Array[1, 1] = wt.Array[3, 2];
                    break;
                case 3:
                    Array = new bool[2, 4];
                    Array[0, 0] = wt.Array[0, 3];
                    Array[0, 1] = wt.Array[4, 3];
                    Array[0, 2] = wt.Array[6, 3];
                    Array[0, 3] = wt.Array[2, 3];
                    Array[1, 0] = wt.Array[1, 3];
                    Array[1, 1] = wt.Array[5, 3];
                    Array[1, 2] = wt.Array[7, 3];
                    Array[1, 3] = wt.Array[3, 3];
                    break;
                case 4:
                    Array = new bool[4, 4];
                    Array[0, 0] = wt.Array[0, 4];
                    Array[0, 1] = wt.Array[1, 4];
                    Array[0, 2] = wt.Array[3, 4];
                    Array[0, 3] = wt.Array[2, 4];
                    Array[1, 0] = wt.Array[4, 4];
                    Array[1, 1] = wt.Array[5, 4];
                    Array[1, 2] = wt.Array[7, 4];
                    Array[1, 3] = wt.Array[6, 4];
                    Array[2, 0] = wt.Array[12, 4];
                    Array[2, 1] = wt.Array[13, 4];
                    Array[2, 2] = wt.Array[15, 4];
                    Array[2, 3] = wt.Array[14, 4];
                    Array[3, 0] = wt.Array[8, 4];
                    Array[3, 1] = wt.Array[9, 4];
                    Array[3, 2] = wt.Array[11, 4];
                    Array[3, 3] = wt.Array[10, 4];
                    break;
                default:
                    throw new System.NotImplementedException();
            } //end switch
        } //end constructor

        public KVDiagramm(Schaltfunktion sf) {
            switch (sf.VarCount) {
                case 2:
                    Array = new bool[2, 2];
                    Array[0, 0] = sf.Invoke(false, false);
                    Array[0, 1] = sf.Invoke(false, true);
                    Array[1, 0] = sf.Invoke(true, false);
                    Array[1, 1] = sf.Invoke(true, true);
                    break;
                case 3:
                    Array = new bool[2, 4];
                    Array[0, 0] = sf.Invoke(false, false, false);
                    Array[0, 1] = sf.Invoke(true, false, false);
                    Array[0, 2] = sf.Invoke(true, true, false);
                    Array[0, 3] = sf.Invoke(false, true, false);
                    Array[1, 0] = sf.Invoke(false, false, true);
                    Array[1, 1] = sf.Invoke(true, false, true);
                    Array[1, 2] = sf.Invoke(true, true, true);
                    Array[1, 3] = sf.Invoke(false, true, true);
                    break;
                case 4:
                    Array = new bool[4, 4];
                    Array[0, 0] = sf.Invoke(0);
                    Array[0, 1] = sf.Invoke(1);
                    Array[0, 2] = sf.Invoke(3);
                    Array[0, 3] = sf.Invoke(2);
                    
                    Array[1, 0] = sf.Invoke(4);
                    Array[1, 1] = sf.Invoke(5);
                    Array[1, 2] = sf.Invoke(7);
                    Array[1, 3] = sf.Invoke(6);
                    
                    Array[2, 0] = sf.Invoke(12);
                    Array[2, 1] = sf.Invoke(13);
                    Array[2, 2] = sf.Invoke(15);
                    Array[2, 3] = sf.Invoke(14);
                    
                    Array[3, 0] = sf.Invoke(8);
                    Array[3, 1] = sf.Invoke(9);
                    Array[3, 2] = sf.Invoke(11);
                    Array[3, 3] = sf.Invoke(10);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }

        public System.Tuple<System.Tuple<int, int>, System.Tuple<int, int>> FindGroups() {
            var ret = new System.Collections.Generic.List<System.Tuple<System.Tuple<int, int>, System.Tuple<int, int>>>();
            for (int i = 0; i < Array.GetLength(0); i++) {
                int r = 0;
                for (r = 1; r < Array.GetLength(1); r++) {
                    if (!Array[i, r]) {
                        r--;
                        break;
                    }
                }
            }
            throw new System.NotImplementedException();
        }

        public override string ToString() {
            var sb = new System.Text.StringBuilder();

            for (int r = 0; r < Array.GetLength(0); r++) {
                for (int c = 0; c < Array.GetLength(1); c++)
                    sb.Append(Array[r, c] ? 1 : 0);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}