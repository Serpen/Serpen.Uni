using System.Linq;
namespace Serpen.Uni.Automat.Finite
{

    [System.Serializable]
    public class NFAeTransform : TransformBase<EATuple, uint[]>
    {
        internal void Add(uint q1, char? w, params uint[] q2) => Add(new EATuple(q1, w), q2);

        internal void AddM(uint q1, char? c, uint qNext) {
            var eat = new EATuple(q1, c);
            if (TryGetValue(eat, out uint[] qBefore))
                this[eat] = qBefore.Append(qNext).ToArray();
            else
                this.Add(eat.q, eat.c, qNext);
        }

        public uint[] this[uint q, char c]
        {
            get => base[new EATuple(q, c)];
            set => base[new EATuple(q, c)] = value;
        }

        internal bool ContainsKey(uint q, char c) => ContainsKey(new EATuple(q, c));

        public override string ToString()
        {
            var sw = new System.Text.StringBuilder();
            foreach (var item in this)
                sw.Append($"({item.Key.q},{item.Key.c})=>{string.Join(',', item.Value)}; ");
            return sw.ToString();
        }
    }
}