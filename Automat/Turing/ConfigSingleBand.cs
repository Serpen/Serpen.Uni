namespace Serpen.Uni.Automat.Turing {
    internal class TuringConfigSingleBand : TuringConfigBase<char> {

        public TuringConfigSingleBand(char blankSymbol, string band, int bandPos) : base(blankSymbol) {
            base.band = new string[] { band };
            position = new int[] { bandPos };
            base.state = new uint[1];
        }

        public string Band {
            get => band[0];
            internal set => band[0] = value;
        }

        public uint State {
            get => base.state[0];
            internal set => base.state[0] = value;
        }

        public int Position {
            get => position[0];
            private set => position[0] = value;
        }

        public override char CurSymbol {
            get {
                if (Position >= Band.Length)
                    Band = Band.Insert(Position, BlankSymbol.ToString());
                return Band[Position];

            }
        }

        public override void ReplaceChar(char newChar, TMDirection dir) {
            string s = Band.Remove(Position, 1);
            s = s.Insert(Position, newChar.ToString());
            if (dir == TMDirection.Left) {
                Position--;
                if (Position == -1) {
                    s = s.Insert(0, BlankSymbol.ToString());
                    Position = 0;
                }
            } else if (dir == TMDirection.Right) {
                Position++;
                if (Position > s.Length)
                    s = s.Insert(Position, BlankSymbol.ToString());
            }
            Band = s;
        }
    }
}