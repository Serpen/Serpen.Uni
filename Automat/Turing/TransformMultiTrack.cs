using System.Linq;

namespace Serpen.Uni.Automat.Turing {
public class TuringTransformMultiTrack : TransformBase<TuringTransformMultiTrack.TuringKey, TuringTransformMultiTrack.TuringVal> {
        internal string[] StateTracks { get; }
        public TuringTransformMultiTrack(string[] stateTracks) {
            StateTracks = stateTracks;
        }
        
        public void AddByStateStoreAndTracks(string q, string c1, string qNext, string c2, TMDirection dir) {
            Add(new TuringTransformMultiTrack.TuringKey(Utils.ArrayIndex(StateTracks, q), 
                    c1.Replace(",", "").ToCharArray().Reverse().ToArray()),
                new TuringTransformMultiTrack.TuringVal(Utils.ArrayIndex(StateTracks, qNext), 
                    c2.Replace(",", "").ToCharArray().Reverse().ToArray(), dir));

        }
        
        public struct TuringKey {
            public TuringKey(uint q, char[] c) {
                this.q = q;
                this.c = c;
            }
            public uint q;
            public char[] c;

            public override bool Equals(object obj) {
                if (obj is TuringKey tk) {
                    return tk.q == this.q && new string(tk.c) == new string(c);
                }
                return false;
            }

            public override int GetHashCode() => this.ToString().GetHashCode();

            public override string ToString() => $"({q}, {string.Join("", c)})";
        }

        public struct TuringVal {
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

            public override string ToString() => $"({qNext}, {c2}, {(Direction)})";
        }
    }
}