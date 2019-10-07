namespace Serpen.Uni.Automat.Turing {
    public abstract class TuringConfigBase<TChar> : IConfig {

        protected TuringConfigBase(char blankSymbol) {
            BlankSymbol = blankSymbol;
        }
        protected char BlankSymbol;
        protected uint[] state;
        uint[] IConfig.State => state;
        protected string[] band;
        protected int[] position;

        public abstract TChar CurSymbol { get; }
        public abstract void ReplaceChar(TChar newChar, TMDirection dir);

        public override bool Equals(object obj) {
            if (obj is TuringConfigBase<TChar> tcfg) {
                return tcfg.state.Equals(this.state) && tcfg.band.Equals(this.band) && tcfg.position.Equals(this.position);
            }
            return false;
        }

        public override int GetHashCode() => this.ToString().GetHashCode();

        public override string ToString() {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < band.Length; i++) {
                string str = band[i];
                str = str.Insert(position[0], $"<{state[0]}>");
                sb.Append(str + '|');
            }
            return sb.ToString();
        }
    }
}