namespace Serpen.Uni.Automat.Turing {
    public abstract class TuringConfigBase<TChar> : IConfig {

        protected TuringConfigBase(char blankSymbol) => BlankSymbol = blankSymbol;
        
        protected char BlankSymbol;
        public uint State {get; set;}
        protected string[] band;
        protected int[] position;

        public abstract TChar CurSymbol { get; }

        public void ReplaceChar(char[] newChar, TMDirection dir) {
            string[] s = new string[band.GetLength(0)];
            for (int i = 0; i < band.GetLength(0); i++) {
                s[i] = band[i].Remove(position[0], 1);
                s[i] = s[i].Insert(position[0], newChar[i].ToString());
            }

            if (dir == TMDirection.Left) {
                position[0]--;
                if (position[0] == -1) {
                    for (int i = 0; i < band.GetLength(0); i++) {
                        s[i] = s[i].Insert(0, BlankSymbol.ToString());
                        position[0] = 0;
                    }
                }
            } else if (dir == TMDirection.Right) {
                position[0]++;
                if (position[0] > s[0].Length)
                    for (int i = 0; i < band.GetLength(0); i++) {
                        s[i] = s[i].Insert(position[0], BlankSymbol.ToString());
                    }
            }
            band = s;
        }

        public override bool Equals(object obj) {
            if (obj is TuringConfigBase<TChar> tcfg) {
                return tcfg.State.Equals(this.State) 
                    && tcfg.band.Equals(this.band) 
                    && tcfg.position.Equals(this.position);
            }
            return false;
        }

        public override int GetHashCode() => this.ToString().GetHashCode();

        public override string ToString() {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < band.Length; i++) {
                string str = band[i];
                str = str.Insert(position[0], $"<{State}>");
                sb.Append(str + '|');
            }
            return sb.ToString();
        }
    }
}