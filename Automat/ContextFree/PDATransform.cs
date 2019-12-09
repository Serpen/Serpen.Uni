using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat.ContextFree {
    
    [System.Serializable]
    public struct PDATransformKey : ITransformKey {
        public uint q { get; }
        public char? ci { get; }

        char[] ITransformKey.c {
            get {
                if (ci.HasValue)
                    return new char[] { ci.Value };
                else
                    return System.Array.Empty<char>();
            }
        }
        public char? cw { get; }

        public PDATransformKey(uint q, char? ci, char? cw) {
            this.q = q;
            this.ci = ci;
            this.cw = cw;
        }

        public override bool Equals(object obj)
            => (obj is PDATransformKey pk) &&
            pk.q == this.q && pk.ci == this.ci && pk.cw == this.cw;

        public override int GetHashCode() => this.ToString().GetHashCode();

        public override string ToString()
            => $"({q}, {(ci ?? Utils.EPSILON)}, {(cw ?? Utils.EPSILON)})";
    }

    [System.Serializable]
    public struct PDATransformValue : ITransformValue {
        public PDATransformValue(string cw2, uint qnext) {
            this.cw2 = cw2;
            this.qNext = qnext;
        }

        public string cw2 { get; }
        public uint qNext { get; }

        public override string ToString() => $"({(!string.IsNullOrEmpty(cw2) ? cw2 : Utils.EPSILON.ToString())}, {qNext})";
    }

    [System.Serializable]
    public class PDATransform : TransformBase<PDATransformKey, PDATransformValue[]> {
        public void Add(uint q, char? ci, char? cw, string cw2, uint qNext)
            => Add(new PDATransformKey(q, ci, cw), new PDATransformValue[] { new PDATransformValue(cw2, qNext) });
        public void Add(uint q, char? ci, char? cw, char cw2, uint qNext)
            => Add(new PDATransformKey(q, ci, cw), new PDATransformValue[] { new PDATransformValue(cw2.ToString(), qNext) });

        /// <summary>
        /// Adds Tuple + Appends if already exists
        /// </summary>
        public void AddM(uint q, char? ci, char? cw, string cw2, uint qNext) {
            ContextFree.PDATransformValue pval = new PDATransformValue(cw2, qNext);
            var pkey = new ContextFree.PDATransformKey(q, ci, cw);

            if (TryGetValue(pkey, out PDATransformValue[] pvalout))
                this[pkey] = pvalout.Append(pval).ToArray();
            else
                base.Add(pkey, new PDATransformValue[] { pval });
        }
        public void AddM(uint q, char? ci, char? cw, char cw2, uint qNext) 
            => AddM(q,ci,cw,cw2.ToString(), qNext);

        public bool TryGetValue(ref PDATransformKey[] initcfg, out PDATransformValue[] qnext) {
            var retVals = new List<PDATransformValue>();
            var retKeys = new List<PDATransformKey>();

            for (int i = 0; i < initcfg.Length; i++) {
                var workSuccessor = new PDATransformKey(initcfg[i].q, initcfg[i].ci, initcfg[i].cw);
                if (base.TryGetValue(workSuccessor, out qnext)) {
                    Utils.DebugMessage($"full match {workSuccessor}", null, Uni.Utils.eDebugLogLevel.Verbose);
                    retVals.AddRange(qnext);
                    for (int j = 0; j < qnext.Length; j++)
                        retKeys.Add(workSuccessor);
                    // continue;
                }

                workSuccessor = new PDATransformKey(initcfg[i].q, initcfg[i].ci, null);
                if (base.TryGetValue(workSuccessor, out qnext)) {
                    Utils.DebugMessage($"ignore work char {workSuccessor}", null, Uni.Utils.eDebugLogLevel.Verbose);
                    retVals.AddRange(qnext);
                    for (int j = 0; j < qnext.Length; j++)
                        retKeys.Add(workSuccessor);
                    // continue;
                }

                workSuccessor = new PDATransformKey(initcfg[i].q, null, initcfg[i].cw);
                if (base.TryGetValue(workSuccessor, out qnext)) {
                    Utils.DebugMessage($"ignore input char {workSuccessor}", null, Uni.Utils.eDebugLogLevel.Verbose);
                    retVals.AddRange(qnext);
                    for (int j = 0; j < qnext.Length; j++)
                        retKeys.Add(workSuccessor);
                    // continue;
                }

                workSuccessor = new PDATransformKey(initcfg[i].q, null, null);
                if (base.TryGetValue(workSuccessor, out qnext)) {
                    Utils.DebugMessage($"ignore input,work char {workSuccessor}", null, Uni.Utils.eDebugLogLevel.Verbose);
                    retVals.AddRange(qnext);
                    for (int j = 0; j < qnext.Length; j++)
                        retKeys.Add(workSuccessor);
                    // continue;
                }
            } //next i

            if (retKeys.Count > 0) {
                initcfg = retKeys.ToArray();
                qnext = retVals.ToArray();
                return true;
            } else {
                qnext = retVals.ToArray();
                return false;
            }

        }

        public override string ToString() {
            var sw = new System.Text.StringBuilder();
            foreach (var item in this.OrderBy(a => a.Key.ToString())) {
                sw.Append($"({item.Key.ToString()})=>");
                foreach (var v in item.Value.OrderBy(a => a.ToString()))
                    sw.Append($"({v.ToString()}); ");
                // sw.Append("); ");
            }
            return sw.ToString();
        }
    }
}