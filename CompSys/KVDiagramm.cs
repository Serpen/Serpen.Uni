namespace Serpen.Uni.CompSys {
    public class KVDiagramm {

        public readonly bool[,] Array;

        public KVDiagramm(WerteTabelle wt) {
            switch (wt.VarCount) {
                case 1:
                    Array = new bool[1, 2]; 
                    Array[0,0] = wt.Array[1,1];
                    Array[0,1] = wt.Array[0,1];
                    break;
                case 2:
                    Array = new bool[2, 2];
                    Array[0,0] = wt.Array[3,2];
                    Array[0,1] = wt.Array[1,2];
                    Array[1,0] = wt.Array[2,2];
                    Array[1,1] = wt.Array[0,2];
                    break;
                case 3:
                    Array = new bool[2, 4];
                    Array[0,0] = wt.Array[7,3];
                    Array[0,1] = wt.Array[5,3];
                    Array[0,2] = wt.Array[4,3];
                    Array[0,3] = wt.Array[6,3];
                    Array[1,0] = wt.Array[3,3];
                    Array[1,1] = wt.Array[1,3];
                    Array[1,2] = wt.Array[0,3];
                    Array[1,3] = wt.Array[2,3];
                    break;
                case 4:
                    Array = new bool[4, 4];
                    Array[0,0] = wt.Array[15,4];
                    Array[0,1] = wt.Array[13,4];
                    Array[0,2] = wt.Array[12,4];
                    Array[0,3] = wt.Array[14,4];
                    Array[1,0] = wt.Array[11,4];
                    Array[1,1] = wt.Array[09,4];
                    Array[1,2] = wt.Array[08,4];
                    Array[1,3] = wt.Array[10,4];
                    Array[2,0] = wt.Array[03,4];
                    Array[2,1] = wt.Array[01,4];
                    Array[2,2] = wt.Array[00,4];
                    Array[2,3] = wt.Array[02,4];
                    Array[3,0] = wt.Array[07,4];
                    Array[3,1] = wt.Array[05,4];
                    Array[3,2] = wt.Array[04,4];
                    Array[3,3] = wt.Array[06,4];
                    break;
                default:
                    throw new System.NotImplementedException();
            } //end switch
        } //end constructor

        public override string ToString() {
            var sb = new System.Text.StringBuilder();

            for (int r = 0; r < Array.GetLength(0); r++) {
                for (int c = 0; c < Array.GetLength(1); c++)
                    sb.Append(Array[r,c] ? 1 : 0);
                sb.AppendLine();
            }
            
            return sb.ToString();
        } 
    }
}