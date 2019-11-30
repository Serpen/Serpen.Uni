using System.Linq;

namespace Serpen.Uni.Automat.Turing {
    
    [System.Serializable]
    public class TuringTransformMultiTrack : TransformBase<TuringTransformMultiTrack.TuringKey, TuringTransformMultiTrack.TuringVal> {
        internal string[] StateTracks { get; }
        public TuringTransformMultiTrack(string[] stateTracks) {
            StateTracks = stateTracks;
        }

        public override string ToString() {
            var sw = new System.Text.StringBuilder();
            foreach (var item in this) {
                sw.Append($"({item.Key.ToString()})=>");
                sw.Append($"({item.Value.ToString()}); ");
                // sw.Append("); ");
            }
            return sw.ToString();
        }

        public void AddByStateStoreAndTracks(string q, string c1, string qNext, string c2, TMDirection dir) {
            Add(new TuringTransformMultiTrack.TuringKey(StateTracks.ArrayIndex(q),
                    c1.Replace(",", string.Empty).ToCharArray().Reverse().ToArray()),
                new TuringTransformMultiTrack.TuringVal(StateTracks.ArrayIndex(qNext),
                    c2.Replace(",", string.Empty).ToCharArray().Reverse().ToArray(), dir));

        }

        [System.Serializable]
        public struct TuringKey : ITransformKey {
            public TuringKey(uint q, char[] c) {
                this.q = q;
                this.c = c;
            }
            public uint q { get; }
            public char[] c { get; }

            public override bool Equals(object obj) {
                if (obj is TuringKey tk) {
                    return tk.q == this.q && new string(tk.c) == new string(c);
                }
                return false;
            }

            public override int GetHashCode() => this.ToString().GetHashCode();

            public override string ToString() => $"({q}, {string.Join("", c)})";
        }

        [System.Serializable]
        public struct TuringVal : ITransformValue {
            public TuringVal(uint qNext, char[] c, TMDirection dir) {
                this.qNext = qNext;
                this.c2 = c;
                this.Direction = dir;
            }
            public uint qNext { get; }
            public char[] c2 { get; }
            public TMDirection Direction { get; }

            public override bool Equals(object obj) {
                if (obj is TuringVal tv) {
                    return tv.qNext == this.qNext && tv.c2 == this.c2 && tv.Direction == this.Direction;
                }
                return false;
            }

            public override int GetHashCode() => this.ToString().GetHashCode();

            public override string ToString() => $"({qNext}, ({string.Join(',', c2)}), {(Direction)})";
        }
    }
}