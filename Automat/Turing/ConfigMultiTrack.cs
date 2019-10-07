namespace Serpen.Uni.Automat.Turing {

    internal class TuringConfigMultiTrack : TuringConfigBase<char[]> {

        public TuringConfigMultiTrack(char blankSymbol, string[] band, int bandPos) : base(blankSymbol) {
            base.band = band;
            position = new int[] { bandPos };
            state = new uint[1];
        }

        public string[] Band {
            get => band;
            set => band = value;
        }

        public int Position {
            get => position[0];
            private set => position[0] = value;
        }

        public uint State {
            get => state[0];
            internal set => state[0] = value;
        }

        public override char[] CurSymbol {
            get {
                if (Position >= Band[0].Length)
                    for (int i = 0; i < Band.Length; i++)
                        Band[i] = Band[i].Insert(Position, BlankSymbol.ToString());

                char[] ret = new char[Band.Length];
                for (int i = 0; i < Band.Length; i++)
                    ret[i] = Band[i][Position];
                return ret;
            }
        }

        public override void ReplaceChar(char[] newChar, TMDirection dir) {
            string[] s = new string[Band.GetLength(0)];
            for (int i = 0; i < Band.GetLength(0); i++) {
                s[i] = Band[i].Remove(Position, 1);
                s[i] = s[i].Insert(Position, newChar[i].ToString());
            }

            if (dir == TMDirection.Left) {
                Position--;
                if (Position == -1) {
                    for (int i = 0; i < Band.GetLength(0); i++) {
                        s[i] = s[i].Insert(0, BlankSymbol.ToString());
                        Position = 0;
                    }
                }
            } else if (dir == TMDirection.Right) {
                Position++;
                if (Position > s[0].Length)
                    for (int i = 0; i < Band.GetLength(0); i++) {
                        s[i] = s[i].Insert(Position, BlankSymbol.ToString());
                    }
            }
            Band = s;
        }
    }
}