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
    public sealed class TuringConfigMultiTracks : TuringConfigBase<uint, string[], int, char[]> {
        public TuringConfigMultiTracks(char blankSymbol, string[] band, int bandPos) : base(blankSymbol) {
            Band = band;
            Position = bandPos;
        }

        public override char[] CurSymbol {
            get {
                if (Position >= Band.Length) {
                    for (int i = 0; i < Band.GetLength(0); i++) {
                        Band[i] = Band[i].Insert(Position, BlankSymbol.ToString());
                    }
                }
                char[] ret = new char[Band.GetLength(0)];
                for (int i = 0; i < ret.Length; i++)
                    ret[i] = Band[i][Position];

                return ret;
            }
        }

        public override void ReplaceChar(char[] newChar, TMDirection dir) {
            string[] s = new string[Band.GetLength(0)];
            for (int i = 0; i < Band.GetLength(0); i++) {
                s[i] = Band[i].Remove(Position, 1);
                s[i] = s[i].Insert(Position, newChar.ToString());
            }

            if (dir == TMDirection.Left) {
                Position--;
                if (Position == -1) {
                    for (int i = 0; i < Band.GetLength(0); i++) {
                        s[i] = s[i].Insert(0, BlankSymbol.ToString());
                        Position = 0;
                    }
                } else if (dir == TMDirection.Right) {
                    Position++;
                    if (Position > s.Length)
                        for (int i = 0; i < Band.GetLength(0); i++) {
                            s[i] = s[i].Insert(Position, BlankSymbol.ToString());
                        }
                    Band = s;
                }
            }
        }
            // public sealed class TuringConfigMultiBands : TuringConfigBase<uint, string[], int[], char> {
            //     TuringConfigMultiBands(char blankSymbol) : base(blankSymbol) { }
            // }
            // public sealed class TuringConfigStateStorage : TuringConfigBase<System.Tuple<uint, char>, string, int, char> {
            //     TuringConfigStateStorage(char blankSymbol) : base(blankSymbol) { }

            //     public override char CurSymbol {
            //         get {
            //             if (Position >= Band.Length)
            //                 Band = Band.Insert(Position, BlankSymbol.ToString());
            //             return base.Band[base.Position];
            //         }
            //     }

            //     public override void ReplaceChar(char newChar, TMDirection dir) {
            //         string s = Band.Remove(Position, 1);
            //         s = s.Insert(Position, newChar.ToString());
            //         if (dir == TMDirection.Left) {
            //             Position--;
            //             if (Position == -1) {
            //                 s = s.Insert(0, BlankSymbol.ToString());
            //                 Position = 0;
            //             }
            //         } else if (dir == TMDirection.Right) {
            //             Position++;
            //             if (Position > s.Length)
            //                 s = s.Insert(Position, BlankSymbol.ToString());
            //         }
            //         Band = s;
            //     }
            // }

    }
}