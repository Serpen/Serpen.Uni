namespace Serpen.Uni.Automat.Turing {
    internal class TuringConfigMultiBand : TuringConfigBase<char[]> {

        public TuringConfigMultiBand(char blankSymbol, string[] bands, int[] bandPos) : base(blankSymbol) {
            base.band = bands;
            position = new int[bands.Length];
            Position = bandPos;
        }

        public string[] Bands {
            get => band;
            private set => band = value;
        }

        public int[] Position {
            get => position;
            private set => position = value;
        }

        public override char[] CurSymbol {
            get {
                char[] ret = new char[Bands.Length];
                for (int i = 0; i < Bands.Length; i++) {
                    if (Position[i] >= Bands[i].Length)
                        Bands[i] = Bands[i].Insert(Position[i], BlankSymbol.ToString());
                    ret[i] = Bands[i][Position[i]];
                }
                return ret;
            }
        }
    }
}