namespace Serpen.Uni.CompSys {

    public class Schaltfunktion : System.Object {

        public delegate bool Schaltfunction(params bool[] a);
        readonly Schaltfunction SF;
        
        public Schaltfunktion(int vars, Schaltfunction sf) {
            SF = sf;
            VarCount = vars;
        }

        public readonly int VarCount;

        public static  explicit operator WerteTabelle(Schaltfunktion sf) {
                var ret = new bool[Serpen.Uni.Utils.Pow2(sf.VarCount), sf.VarCount+1];

            for (int x = 0; x < ret.GetLength(0); x++) {
                bool[] args = new bool[ret.GetLength(1)-1];
                for (byte y = 0; y < ret.GetLength(1)-1; y++) {
                    ret[x, ret.GetLength(1)-2 - y] = y.HasBitSet(x);
                    args[y] = ret[x, ret.GetLength(1)-2 - y];
                }
                ret[x, ret.GetLength(1)-1] = sf.Invoke(args);
            }
            return new WerteTabelle(ret);
        }

        public bool Invoke(params bool[] b) => SF.Invoke(b);

        public static Schaltfunktion SF_1608_S22 = new Schaltfunktion(3, a => (a[0] & (a[1] | a[2]) | a[1] & a[2]));
        public static Schaltfunktion DNF_T23 = new Schaltfunktion(3, a => (!a[0] & !a[1] & !a[2]) |
                                                                          ( a[0] &  a[1] & !a[2]) | 
                                                                          (!a[0] & !a[1] &  a[2]) | 
                                                                          (a[0] &  !a[1] &  a[2]));
        public static Schaltfunktion KNF_T23 = new Schaltfunktion(3, a => (!a[0] |  a[1] |  a[2]) &
                                                                          ( a[0] | !a[1] |  a[2]) & 
                                                                          ( a[0] | !a[1] | !a[2]) & 
                                                                          (!a[0] | !a[1] | !a[2]));
        public static Schaltfunktion DNF_B21 = new Schaltfunktion(3, a => (!a[0] & !a[1] & !a[2]) |
                                                                          ( a[0] &  a[1] & !a[2]) | 
                                                                          (!a[0] & !a[1] &  a[2]) | 
                                                                          (!a[0] &  a[1] &  a[2]));
        public static Schaltfunktion KNF_B22 = new Schaltfunktion(3, a => (!a[0] | a[1]  | a[2]) 
                                                                        & (a[0]  | !a[1] | a[2]) 
                                                                        & (a[0]  | a[1]  | !a[2]) 
                                                                        & (a[0]  | !a[1] | !a[2]));
        public static Schaltfunktion SF_212 = new Schaltfunktion(3, a => ( a[0] |  a[1]) & 
                                                                         !(a[0] & !a[2]) |
                                                                         ( a[1] &  a[2]));
    }

}