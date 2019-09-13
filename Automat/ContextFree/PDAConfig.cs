namespace Serpen.Uni.Automat.ContextFree {
    public class PDAConfig : IConfig {
        public uint q { get; }
        public string word { get; }
        public char[] Stack { get; }
        public PDAConfig Origin { get; }
        public string OriginPath() {
            var pcfg = this;
            var sw = new System.Text.StringBuilder();
            sw.Append($"[{pcfg.ToString()}]");
            while (pcfg.Origin != null) {
                pcfg = pcfg.Origin;
                sw.Append($"<-[{pcfg.ToString()}]");
            }
            return sw.ToString();
        }

        public PDAConfig(uint q, string w, char[] stack, PDAConfig origin) {
            this.q = q;
            this.word = w;
            this.Stack = stack;
            this.Origin = origin;
        }

        public override bool Equals(object obj) {
            if (obj is PDAConfig pobj) {
                return pobj.GetHashCode() == this.GetHashCode();
            }
            return false;
        }

        public override int GetHashCode() => ToString().GetHashCode();

        public override string ToString() => $"({q},'{word}','{string.Join("", Stack)}'";
    }
}