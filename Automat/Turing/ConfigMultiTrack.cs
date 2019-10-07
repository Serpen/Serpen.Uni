namespace Serpen.Uni.Automat.Turing {
    internal class TuringConfigMultiTrack : TuringConfigBase<char[]> {

        public TuringConfigMultiTrack(char blankSymbol, string[] bands, int bandPos) : base(blankSymbol) {
            base.band = bands;
            Position = bandPos;
        }

        public string[] Bands {
            get => band;
            private set => band = value;
        }

        public int Position {
            get => position[0];
            private set => position[0] = value;
        }

        public override char[] CurSymbol {
            get {
                if (Position >= Bands[0].Length)
                    for (int i = 0; i < Bands.Length; i++)
                        Bands[i] = Bands[i].Insert(Position, BlankSymbol.ToString());

                char[] ret = new char[Bands.Length];
                for (int i = 0; i < Bands.Length; i++)
                    ret[i] = Bands[i][Position];
                return ret;
            }
        }
    }
}