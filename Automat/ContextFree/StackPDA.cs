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
        : base(name, StatesCount, InputAlphabet, Workalphabet, Transform, StartState, Startsymbol, new uint[] {}) {
            
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

            for (int i = 0; i < pda.Transform.Count; i++)
            {
                var t = pda.Transform.ElementAt(i);
                for (int j = 0; j < t.Value.Length; j++)
                {
                    //inc all States cause of new first § state
                    newt.AddM(t.Key.q+1, t.Key.ci, t.Key.cw, t.Value[j].cw2, t.Value[j].qNext+1);

                    //accepted state goes to qPump
                    if (pda.IsAcceptedState(t.Key.q)) {
                        newt.Add(t.Key.q+1, null, null, null, qPump);
                    }
                }
            }

            

            foreach (char c in pda.Alphabet)
            {
                if (!spdaWorkAlphabet.Contains(c))
                    spdaWorkAlphabet.Add(c);
                newt.Add(qPump,null, c, null, qPump);
            }
            newt.Add(qPump, null, extra_symbol, null, qPump);

            return new StackPDA($"SPDA_({pda.Name})", pda.StatesCount+1, pda.Alphabet, spdaWorkAlphabet.ToArray(), newt, 0, extra_symbol);
        }


        public override bool AcceptWord(string w) {
            int runCount = 0;
            //construct start config
            PDAConfig[] pcfgs;
            if (StartStackSymbol != 0)
                pcfgs = new PDAConfig[] {new PDAConfig(StartState, w, new char[] {StartStackSymbol}, null)};
            else
                pcfgs = new PDAConfig[] {new PDAConfig(StartState, w, new char[] {}, null)};


            //while any pcfg exists
            while (pcfgs.Length > 0) { //&& (pcfg.Where((a) => a.Stack.Length>0).Any())
                pcfgs = GoChar(pcfgs);

                runCount++;

                if (pcfgs.Length > 10000000 || runCount > 10000) {
                    System.Console.WriteLine($"{runCount}: Stack >= {pcfgs.Length}, abort");
                    return false;
                }
                //check if a cfg has word and stack cleared and ends in accepted states
                foreach (var p in pcfgs) {
                    if (p.Stack.Count() == 0 && p.word.Length == 0) {
                        return true;
                    }
                }
            }

            return false;
        }

        [System.ComponentModel.Description("1659_L3.1_P76")]
        public static explicit operator StackPDA(ContextFree.CFGrammer cfg) {
            var t = new ContextFree.PDATransform();
            var sn = new System.Collections.Generic.Dictionary<uint,string>();
            
            const uint qSim = 1;
            uint q = qSim+1;

            // t.Add(0,null, null, ContextFree.PDA.START.ToString(), 1);
            // sn.Add(0, "0");
            t.Add(0, null, null, cfg.StartSymbol.ToString(), qSim);
            sn.Add(0, "start");

            foreach (char c in cfg.Terminals)
            {
                t.Add(qSim, c, c, null, qSim);
            }
            t.Add(qSim, null, ContextFree.PDA.START, null, qSim);
            sn.Add(qSim, "sim");

            foreach (var r in cfg.Rules)
            {
                foreach (string body in r.Value) {
                    if (body.Length>2) {
                        t.AddM(qSim, null, r.Key, body.Substring(body.Length-1,1), q);
                        sn.TryAdd(q, $"{q}; {r.Key}=>{body}");  
                        for (int i=body.Length-2; i > 0 ; i--)
                        {
                            t.AddM(q, null, null, body.Substring(i,1), ++q);
                            sn.TryAdd(q, $"{q}; {r.Key}=>{body}");
                        }
                        t.AddM(q, null, null, body.Substring(0,1), qSim);
                        q++;
                    } else if (body.Length == 2) {
                        t.AddM(qSim, null, r.Key, body.Substring(1,1), q);
                        t.AddM(q, null, null, body.Substring(0,1), qSim);
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
            for (uint i = 0; i < spda.StatesCount; i++)
            {
                spda.States[i] = sn[i];
            }
            return spda;
        }
    }
}