using System.Linq;

namespace Serpen.Uni.Automat.Finite {
    public class DFATransform : TransformBase<EATuple, uint[]> {
    
        public void Add(uint q1, char w, uint q2)
            => Add(new EATuple(q1, w), new uint[] {q2});
        
        internal void AddBinTuple(uint q, uint q0, uint q1) {
            this.Add(q,'0', q0);
            this.Add(q,'1', q1);
        }

        public bool TryAdd(uint q, char c, uint qNext) {
            return TryAdd(new EATuple(q, c), new uint[] {qNext});
        }

        internal bool ContainsKey(uint q, char c) 
            => ContainsKey(new EATuple(q, c));

        internal bool TryGetValue(uint q, char c, out uint qNext)
        {
            uint[] qNextArray;
            if (TryGetValue(new EATuple(q, c), out qNextArray)) {
                qNext=qNextArray[0];
                return true;
            } else {
                qNext = 0;
                return false;
            }
        }

        public uint this[uint q, char c] {
            get {
                return base[new EATuple(q,c)][0];
            }
        }

        public override string ToString() {
            var sw = new System.Text.StringBuilder();
            foreach (var item in this)
                sw.Append($"({item.Key.q},{item.Key.c.Value})=>{item.Value[0]}; ");
            return sw.ToString();
        }

    }
}