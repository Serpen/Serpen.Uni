using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat.ContextFree {
    public struct PDATransformKey : ITransformKey {
        public uint q {get;}
        public char? ci {get;}

        char[] ITransformKey.c {
            get {
                if (ci.HasValue)
                    return new char[] {ci.Value};
                else
                    return new char[] {};
            }
        }
        public char? cw {get;}
        
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
            => $"({q}, {(ci.HasValue ? ci.Value : Utils.EPSILON)}, {(cw.HasValue ? cw.Value : Utils.EPSILON)})";
    }

    public struct PDATransformValue : ITransformValue {
        public PDATransformValue(string cw2, uint qnext) {
            this.cw2 = cw2;
            this.qNext = qnext;
        }

        public string cw2 {get;}
        public uint qNext {get;}

        public override string ToString() => $"({(!string.IsNullOrEmpty(cw2) ? cw2 : Utils.EPSILON.ToString())}, {qNext})";
    }

    public class DPDATransform : TransformBase<PDATransformKey, PDATransformValue> {
        public void Add(uint q, char? ci, char? cw, string cw2, uint qNext) 
            => base.Add(new PDATransformKey(q, ci, cw), new PDATransformValue(cw2, qNext));
    
        public bool TryGetValue(ref PDATransformKey initcfg, out PDATransformValue qnext) {
            bool result;
            var worksuccessor = new PDATransformKey(initcfg.q,initcfg.ci,initcfg.cw);

            result = base.TryGetValue(worksuccessor, out qnext);
            if (result) {
                initcfg = worksuccessor;
                return true;
            }

            worksuccessor = new PDATransformKey(initcfg.q, initcfg.ci, null);
            result = base.TryGetValue(worksuccessor, out qnext);
            if (result) {
                initcfg = worksuccessor;
                return true;
            }
            
            worksuccessor = new PDATransformKey(initcfg.q, null, initcfg.cw);
            result = base.TryGetValue(worksuccessor, out qnext);
            if (result) {
                initcfg = worksuccessor;
                return true;
            }
        
            return false;

        }

        public override string ToString() {
            var sw = new System.Text.StringBuilder();
            foreach (var item in this) {
                sw.Append($"({item.Key.ToString()})=>");
                sw.Append($"({item.Value.ToString()}); ");
                sw.Append("); ");
            }
            return sw.ToString();
        }
    }

    public class PDATransform : TransformBase<PDATransformKey, PDATransformValue[]> {
        public void Add(uint q, char? ci, char? cw, string cw2, uint qNext) 
            => Add(new PDATransformKey(q, ci, cw), new PDATransformValue[] {new PDATransformValue(cw2, qNext)});
    
    
        /// <summary>
        /// Adds Tuple + Appends if already exists
        /// </summary>
        public void AddM(uint q, char? ci, char? cw, string cw2, uint qNext) {
            ContextFree.PDATransformValue[] pvalRef;
            ContextFree.PDATransformValue pval = new PDATransformValue(cw2, qNext);
            var pkey = new ContextFree.PDATransformKey(q, ci, cw);
            
            if (TryGetValue(pkey, out pvalRef))
                this[pkey] = pvalRef.Append(pval).ToArray();
            else
                base.Add(pkey, new PDATransformValue[] {pval});
        } 
        public bool TryGetValue(ref PDATransformKey[] initcfg, out PDATransformValue[] qnext) {
            bool result;
            var retVals = new List<PDATransformValue>();
            var retKeys = new List<PDATransformKey>();

            for (int i = 0; i < initcfg.Length; i++)
            {
                var workSuccessor = new PDATransformKey(initcfg[i].q,initcfg[i].ci,initcfg[i].cw);

                result = base.TryGetValue(workSuccessor, out qnext);
                if (result) {
                    retVals.AddRange(qnext);
                    for (int j = 0; j < qnext.Length; j++)
                        retKeys.Add(workSuccessor);
                    // continue;
                }

                workSuccessor = new PDATransformKey(initcfg[i].q, initcfg[i].ci, null);
                result = base.TryGetValue(workSuccessor, out qnext);
                if (result) {
                    retVals.AddRange(qnext);
                    for (int j = 0; j < qnext.Length; j++)
                        retKeys.Add(workSuccessor);
                    // continue;
                }
                
                workSuccessor = new PDATransformKey(initcfg[i].q, null, initcfg[i].cw);
                result = base.TryGetValue(workSuccessor, out qnext);
                if (result) {
                    retVals.AddRange(qnext);
                    for (int j = 0; j < qnext.Length; j++)
                        retKeys.Add(workSuccessor);
                    // continue;
                }
                
                workSuccessor = new PDATransformKey(initcfg[i].q, null, null);
                result = base.TryGetValue(workSuccessor, out qnext);
                if (result) {
                    retVals.AddRange(qnext);
                    for (int j = 0; j < qnext.Length; j++)
                        retKeys.Add(workSuccessor);
                    // continue;
                }
            } //next i

            if (retKeys.Count>0) {
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