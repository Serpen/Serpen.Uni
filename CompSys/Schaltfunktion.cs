namespace Serpen.Uni.CompSys {

    public class Schaltfunktion {

        public delegate bool Schaltfunction(params bool[] a);
        readonly Schaltfunction SF;

        public Schaltfunktion(int vars, Schaltfunction sf) {
            SF = sf;
            VarCount = vars;
        }

        public readonly int VarCount;

        public static explicit operator WerteTabelle(Schaltfunktion sf) {
            var ret = new bool[Serpen.Uni.Utils.Pow2(sf.VarCount), sf.VarCount + 1];

            for (byte x = 0; x < ret.GetLength(0); x++) {
                bool[] args = new bool[ret.GetLength(1) - 1];
                for (byte y = 0; y < ret.GetLength(1) - 1; y++) {
                    args[y] = Utils.HasBitSet(x, y);
                    ret[x, ret.GetLength(1) - 2 - y] = args[y];
                }
                ret[x, ret.GetLength(1) - 1] = sf.Invoke(args);
            }
            return new WerteTabelle(ret);
        }

        public bool Invoke(params bool[] b) => SF.Invoke(b);
        public bool Invoke(int num) {
            bool[] b = new bool[VarCount];
            for (int i = 0; i < b.Length; i++) {
                if ((num & Utils.Pow2(i)) > 0) b[i] = true;
            }
            return SF.Invoke(b);
        }
    }

}