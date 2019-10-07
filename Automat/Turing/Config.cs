namespace Serpen.Uni.Automat.Turing {
    public abstract class TuringConfigBase<TChar> : IConfig {

        protected TuringConfigBase(char blankSymbol) {
            BlankSymbol = blankSymbol;
        }
        protected char BlankSymbol;
        public uint[] state {get; protected set;}
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

        public override string ToString() => $"({state}, {band}, {position})";

    }
}