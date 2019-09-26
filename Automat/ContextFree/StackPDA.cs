using System.Linq;

namespace Serpen.Uni.Automat.ContextFree {
    public class StackPDA : PDA {

        /// <summary>
        /// Create PDA which accepts by empty stack
        /// </summary>
        /// <param name="StatesCount">How many states</param>
        /// <param name="InputAlphabet">Input Alphabet for Word processing</param>
        /// <param name="Workalphabet">Work Alphabet for Stack</param>
        /// <param name="Transform"></param>
        /// <param name="StartState">State from which to start</param>
        /// <param name="Startsymbol">Inital Stack Population</param>
        public StackPDA(string name, uint StatesCount, char[] InputAlphabet, char[] Workalphabet, PDATransform Transform, uint StartState, char Startsymbol)
        : base(name, StatesCount, InputAlphabet, Workalphabet, Transform, StartState, Startsymbol, new uint[] { }) {

            //Make all States acceptable for 
            base.AcceptedStates = new uint[StatesCount];
            for (uint i = 0; i < StatesCount; i++)
                base.AcceptedStates[i] = i;

        }

        public StackPDA(string name, string[] states, char[] InputAlphabet, char[] Workalphabet, PDATransform Transform, uint StartState, char Startsymbol)
        : base(name, states, InputAlphabet, Workalphabet, Transform, StartState, Startsymbol, new uint[] { }) {

            //Make all States acceptable for 
            base.AcceptedStates = new uint[StatesCount];
            for (uint i = 0; i < StatesCount; i++)
                base.AcceptedStates[i] = i;

        }

        [System.ComponentModel.Description("1659_L33")]
        public static explicit operator StackPDA(StatePDA pda) {
            var newt = new PDATransform();

            char extra_symbol = Utils.NextFreeCapitalLetter(pda.Alphabet.Concat(pda.WorkAlphabet).ToArray(), EXTRASYMBOLS[0], EXTRASYMBOLS);
            var spdaWorkAlphabet = new System.Collections.Generic.List<char>(pda.WorkAlphabet);
            spdaWorkAlphabet.Add(extra_symbol);

            newt.Add(0, null, null, pda.StartStackSymbol.ToString(), 1);

            uint qPump = pda.StatesCount;

            for (int i = 0; i < pda.Transform.Count; i++) {
                var t = pda.Transform.ElementAt(i);
                for (int j = 0; j < t.Value.Length; j++) {
                    //inc all States cause of new first § state
                    newt.AddM(t.Key.q + 1, t.Key.ci, t.Key.cw, t.Value[j].cw2, t.Value[j].qNext + 1);

                    //accepted state goes to qPump
                    if (pda.IsAcceptedState(t.Key.q)) {
                        newt.Add(t.Key.q + 1, null, null, null, qPump);
                    }
                }
            }

            foreach (char c in pda.Alphabet) {
                if (!spdaWorkAlphabet.Contains(c))
                    spdaWorkAlphabet.Add(c);
                newt.Add(qPump, null, c, null, qPump);
            }
            newt.Add(qPump, null, extra_symbol, null, qPump);

            return new StackPDA($"SPDA_({pda.Name})", pda.StatesCount + 1, pda.Alphabet, spdaWorkAlphabet.ToArray(), newt, 0, extra_symbol);
        }

        public override bool AcceptWord(string w) {
            int runCount = 0;
            //construct start config
            PDAConfig[] pcfgs;
            if (StartStackSymbol != 0)
                pcfgs = new PDAConfig[] { new PDAConfig(StartState, w, new char[] { StartStackSymbol }, null) };
            else
                pcfgs = new PDAConfig[] { new PDAConfig(StartState, w, new char[] { }, null) };


            //while any pcfg exists
            while (pcfgs.Length > 0) { //&& (pcfg.Where((a) => a.Stack.Length>0).Any())
                foreach (var p in pcfgs) {
                    if ((p.Stack.Count() == 0 || (p.Stack[p.Stack.Count() - 1] == this.StartStackSymbol && p.Stack.Count() == 1)) && p.word.Length == 0) {
                        return true;
                    }
                }
                pcfgs = GoChar(pcfgs);

                runCount++;

                if (pcfgs.Length > MAX_RUNS_OR_STACK || runCount > MAX_RUNS_OR_STACK) {
                    Utils.DebugMessage($"{runCount}: Stack >= {pcfgs.Length}, abort", this);
                    return false;
                }
                //check if a cfg has word and stack cleared and ends in accepted states

            }

            return false;
        }

        public static StackPDA GenerateRandom() {
            const byte MAX_STATES = 20;
            const byte MAX_CHAR = 7;

            var rnd = Utils.RND;

            var t = new PDATransform();
            int stateCount = rnd.Next(1, MAX_STATES);

            char[] inputAlphabet = new char[rnd.Next(1, MAX_CHAR)];
            for (int i=0; i < inputAlphabet.Length; i++)
                inputAlphabet[i] = (char)rnd.Next('a', 'z');
            inputAlphabet = inputAlphabet.Distinct().ToArray();

            char[] workAlphabet = new char[rnd.Next(1, MAX_CHAR)];
            for (int i=1; i < workAlphabet.Length; i++)
                workAlphabet[i] = (char)rnd.Next('a', 'z');
            workAlphabet[0] = START;
            workAlphabet = workAlphabet.Distinct().ToArray();

            for (uint i = 0; i < stateCount; i++) {
                int transformsRnd = rnd.Next(0, inputAlphabet.Length);
                for (int j = 0; j < transformsRnd; j++) {
                    t.AddM(i, Utils.GrAE(inputAlphabet), Utils.GrAE(workAlphabet), Utils.GrAE(workAlphabet).ToString(), (uint)rnd.Next(0, stateCount));
                }
            }

            var ret = new StackPDA("SPDA_Random", (uint)stateCount, inputAlphabet, workAlphabet, t, (uint)rnd.Next(0,stateCount), START);
            ret.Name = $"SPDA_Random_{ret.GetHashCode()}";
            return ret;
        }

        public override IAutomat RemoveUnreachable() {
            var newT = new PDATransform();

            bool[] fromStartReachable = new bool[StatesCount];
            fromStartReachable[StartState] = true;
            bool foundnew = true;
            while (foundnew) {
                foundnew = false;
                foreach (var t in (from tr in Transform where fromStartReachable[tr.Key.q] select tr)) {
                    foreach (var v in (from vr in t.Value where !fromStartReachable[vr.qNext] select vr)) {
                        fromStartReachable[v.qNext] = true;
                        foundnew = true;
                    }
                }
            }

            uint[] translate = new uint[(from fsr in fromStartReachable where fsr select fsr).Count()];
            for (uint i=0; i < translate.Length; i++) {
                uint j=i;
                if (i>0)
                    j = System.Math.Max(i, translate[i-1]+1);
                while (!fromStartReachable[j])
                    j++;
                translate[i] = j;
            }

            string[] names = new string[translate.Length];
            for (int i = 0; i < translate.Length; i++)
                names[i] = translate[i].ToString();

            if (Utils.ArrayIndex(translate,StartState) > 100) {
                Utils.DebugMessage("removed with high start state", this);
            }
                
            foreach (var t2 in Transform)
                if (translate.Contains(t2.Key.q))
                    foreach (var v in t2.Value)
                        if (translate.Contains(v.qNext))
                            newT.AddM(Utils.ArrayIndex(translate,t2.Key.q), t2.Key.ci, t2.Key.cw, v.cw2, Utils.ArrayIndex(translate,v));
            
            var astates = new System.Collections.Generic.List<uint>();
            foreach (var accept in AcceptedStates)
                if (translate.Contains(accept))
                    astates.Add(Utils.ArrayIndex(translate,accept));

            return new StackPDA($"{Name}_remove", names, Alphabet, WorkAlphabet, newT, Utils.ArrayIndex(translate,StartState), StartStackSymbol);
        
        }

        [System.ComponentModel.Description("1659_L3.1_P76")]
        public static explicit operator StackPDA(ContextFree.CFGrammer cfg) {
            var t = new ContextFree.PDATransform();
            var sn = new System.Collections.Generic.Dictionary<uint, string>();

            const uint qSim = 1;
            uint q = qSim + 1;

            // t.Add(0,null, null, ContextFree.PDA.START.ToString(), 1);
            // sn.Add(0, "0");
            t.Add(0, null, null, cfg.StartSymbol.ToString(), qSim);
            sn.Add(0, "start");

            foreach (char c in cfg.Terminals) {
                t.Add(qSim, c, c, null, qSim);
            }
            t.Add(qSim, null, ContextFree.PDA.START, null, qSim);
            sn.Add(qSim, "sim");

            foreach (var r in cfg.Rules) {
                foreach (string body in r.Value) {
                    if (body.Length > 2) {
                        t.AddM(qSim, null, r.Key, body.Substring(body.Length - 1, 1), q);
                        sn.TryAdd(q, $"{q}; {r.Key}=>{body}");
                        for (int i = body.Length - 2; i > 0; i--) {
                            t.AddM(q, null, null, body.Substring(i, 1), ++q);
                            sn.TryAdd(q, $"{q}; {r.Key}=>{body}");
                        }
                        t.AddM(q, null, null, body.Substring(0, 1), qSim);
                        q++;
                    } else if (body.Length == 2) {
                        t.AddM(qSim, null, r.Key, body.Substring(1, 1), q);
                        t.AddM(q, null, null, body.Substring(0, 1), qSim);
                        sn.TryAdd(q, $"{q}; {r.Key}=>{body}");
                        q++;

                    } else {
                        t.AddM(qSim, null, r.Key, body, qSim);
                    }
                }
            }

            var WorkAlphabet = new System.Collections.Generic.List<char>();
            WorkAlphabet.AddRange(cfg.Terminals);
            WorkAlphabet.AddRange(cfg.Variables);
            // WorkAlphabet.Add(ContextFree.PDA.START);

            var spda = new ContextFree.StackPDA($"SPDA_({cfg.Name})", q, cfg.Terminals, WorkAlphabet.ToArray(), t, 0, ContextFree.PDA.START);
            for (uint i = 0; i < spda.StatesCount; i++) {
                spda.States[i] = sn[i];
            }
            return spda;
        }
    }
}