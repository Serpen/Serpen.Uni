using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat.ContextFree {
    public struct PDATransformKey : ITransformKey {
        public uint q { get; }
        public char? ci { get; }

        char[] ITransformKey.c {
            get {
                if (ci.HasValue)
                    return new char[] { ci.Value };
                else
                    return new char[] { };
            }
        }
        public char? cw { get; }

        public PDATransformKey(uint q, char? ci, char? cw) {
            this.q = q;
            this.ci = ci;
            this.cw = cw;
        }

        public override bool Equals(object obj) {
            if (obj is PDATransformKey pk) {
                return pk.q == this.q && pk.ci == this.ci && pk.cw == this.cw;
            }
            return false;
        }

        public override int GetHashCode() => this.ToString().GetHashCode();

        public override string ToString()
            => $"({q}, {(ci.HasValue ? ci.Value : Uni.Utils.EPSILON)}, {(cw.HasValue ? cw.Value : Uni.Utils.EPSILON)})";
    }

    public struct PDATransformValue : ITransformValue {
        public PDATransformValue(string cw2, uint qnext) {
            this.cw2 = cw2;
            this.qNext = qnext;
        }

        public string cw2 { get; }
        public uint qNext { get; }

        public override string ToString() => $"({(!string.IsNullOrEmpty(cw2) ? cw2 : Uni.Utils.EPSILON.ToString())}, {qNext})";
    }

    
    public class PDATransform : TransformBase<PDATransformKey, PDATransformValue[]> {
        public void Add(uint q, char? ci, char? cw, string cw2, uint qNext)
            => Add(new PDATransformKey(q, ci, cw), new PDATransformValue[] { new PDATransformValue(cw2, qNext) });


        /// <summary>
        /// Adds Tuple + Appends if already exists
        /// </summary>
        public void AddM(uint q, char? ci, char? cw, string cw2, uint qNext) {
            ContextFree.PDATransformValue[] pvalout;
            ContextFree.PDATransformValue pval = new PDATransformValue(cw2, qNext);
            var pkey = new ContextFree.PDATransformKey(q, ci, cw);

            if (TryGetValue(pkey, out pvalout))
                this[pkey] = pvalout.Append(pval).ToArray();
            else
                base.Add(pkey, new PDATransformValue[] { pval });
        }
        public bool TryGetValue(ref PDATransformKey[] initcfg, out PDATransformValue[] qnext) {
            var retVals = new List<PDATransformValue>();
            var retKeys = new List<PDATransformKey>();

            for (int i = 0; i < initcfg.Length; i++) {
                var workSuccessor = new PDATransformKey(initcfg[i].q, initcfg[i].ci, initcfg[i].cw);
                if (base.TryGetValue(workSuccessor, out qnext)) {
                    Uni.Utils.DebugMessage($"full match {workSuccessor}", null, Uni.Utils.eDebugLogLevel.Verbose);
                    retVals.AddRange(qnext);
                    for (int j = 0; j < qnext.Length; j++)
                        retKeys.Add(workSuccessor);
                    // continue;
                }

                workSuccessor = new PDATransformKey(initcfg[i].q, initcfg[i].ci, null);
                if (base.TryGetValue(workSuccessor, out qnext)) {
                    Uni.Utils.DebugMessage($"ignore work char {workSuccessor}", null, Uni.Utils.eDebugLogLevel.Verbose);
                    retVals.AddRange(qnext);
                    for (int j = 0; j < qnext.Length; j++)
                        retKeys.Add(workSuccessor);
                    // continue;
                }

                workSuccessor = new PDATransformKey(initcfg[i].q, null, initcfg[i].cw);
                if (base.TryGetValue(workSuccessor, out qnext)) {
                    Uni.Utils.DebugMessage($"ignore input char {workSuccessor}", null, Uni.Utils.eDebugLogLevel.Verbose);
                    retVals.AddRange(qnext);
                    for (int j = 0; j < qnext.Length; j++)
                        retKeys.Add(workSuccessor);
                    // continue;
                }

                workSuccessor = new PDATransformKey(initcfg[i].q, null, null);
                if (base.TryGetValue(workSuccessor, out qnext)) {
                    Uni.Utils.DebugMessage($"ignore input,work char {workSuccessor}", null, Uni.Utils.eDebugLogLevel.Verbose);
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
            foreach (var item in this) {
                sw.Append($"({item.Key.ToString()})=>");
                foreach (var v in item.Value)
                    sw.Append($"({v.ToString()}); ");
                // sw.Append("); ");
            }
            return sw.ToString();
        }
    }
}