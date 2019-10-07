namespace Serpen.Uni.Automat.ContextFree {
    public class PDAConfig : IConfig {
        public uint State { get; }
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
            this.State = q;
            this.word = w;
            this.Stack = stack;
            this.Origin = origin;
        }

        public override string ToString() => $"({State},'{word}','{string.Join("", Stack)}')";
    }
}