namespace Serpen.Uni.Automat.Turing {
    public interface ITuringConfig : IConfig { }
    public abstract class TuringConfigBase<TQ, TBand, TPos, TChar> : ITuringConfig {

        protected TuringConfigBase(char blankSymbol) {
            BlankSymbol = blankSymbol;
        }
        protected char BlankSymbol;
        public TQ q;
        public TBand Band;
        public TPos Position;

        public abstract TChar CurSymbol { get; }
        public abstract void ReplaceChar(TChar newChar, TMDirection dir);

        public override bool Equals(object obj) {
            if (obj is TuringConfigBase<TQ, TBand, TPos, TChar> tcfg) {
                return tcfg.q.Equals(this.q) && tcfg.Band.Equals(this.Band) && tcfg.Position.Equals(this.Position);
            }
            return false;
        }

        public override int GetHashCode() => this.ToString().GetHashCode();

        public override string ToString() => $"({q}, {Band}, {Position})";

    }

    public sealed class TuringConfigSingleBand : TuringConfigBase<uint, string, int, char> {

        public TuringConfigSingleBand(char blankSymbol, string band, int bandPos) : base(blankSymbol) {
            Band = band;
            Position = bandPos;
        }

        public override char CurSymbol {
            get {
                if (Position >= Band.Length)
                    Band = Band.Insert(Position, BlankSymbol.ToString());
                return base.Band[base.Position];

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