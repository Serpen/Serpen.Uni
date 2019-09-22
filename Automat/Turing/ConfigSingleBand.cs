namespace Serpen.Uni.Automat.Turing {
    public sealed class TuringConfigSingleBand : TuringConfigBase<char> {

        public TuringConfigSingleBand(char blankSymbol, string band, int bandPos) : base(blankSymbol) {
            base.band = new string[] { band };
            position = new int[] { bandPos };
            state = new uint[1];
        }

        public string Band {
            get => band[0];
            internal set {
                band[0] = value;
            }
        }

        public uint State {
            get {
                return state[0];
            }
            internal set {
                state[0] = value;
            }
        }

        public int Position {
            get {
                return position[0];
            }
            private set {
                position[0] = value;
            }
        }

        public override char CurSymbol {
            get {
                if (position[0] >= Band.Length)
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

        public override string ToString() {
            string str = string.Join('|', Band);
            str = str.Insert(Position, $"<{State}>");
            return str;
        }
    }
}