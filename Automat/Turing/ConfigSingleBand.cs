namespace Serpen.Uni.Automat.Turing {
    internal class TuringConfigSingleBand : TuringConfigBase<char> {

        public TuringConfigSingleBand(char blankSymbol, string band, int bandPos) : base(blankSymbol) {
            Band = band;
            Position = bandPos;
        }

        public string Band {
            get => band[0];
            set => band[0] = value;
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

        public void ReplaceChar(char newChar, TMDirection dir) {
            base.ReplaceChar(new char[] {newChar}, dir);
        }
    }
}