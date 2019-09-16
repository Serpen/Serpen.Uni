using System.Linq;

namespace Serpen.Uni.Automat.ContextFree {
    public class StatePDA : PDA {

        public StatePDA(string name, uint StatesCount, char[] InputAlphabet, char[] Workalphabet, PDATransform Transform, uint StartState, char Startstacksymbol, uint[] AcceptedStates)
         : base(name, StatesCount, InputAlphabet, Workalphabet, Transform, StartState, Startstacksymbol, AcceptedStates) {
        }

        
        [System.ComponentModel.Description("EAFK_A64")]
        public static explicit operator StatePDA(StackPDA pda) {
            var newt = new PDATransform();

            char extra_symbol = Utils.NextFreeCapitalLetter(pda.Alphabet.Concat(pda.WorkAlphabet).ToArray(), EXTRASYMBOLS[0], EXTRASYMBOLS);

            newt.Add(0, null, null, extra_symbol.ToString(), 1);
            for (int i = 0; i < pda.Transform.Count; i++)
            {
                var t = pda.Transform.ElementAt(i);
                for (int j = 0; j < t.Value.Length; j++)
                {
                    newt.AddM(t.Key.q+1, t.Key.ci, t.Key.cw, t.Value[j].cw2, t.Value[j].qNext+1);
                    newt.AddM(t.Key.q+1, null, extra_symbol, null, pda.StatesCount+1);
                }
            }

            return new StatePDA($"QPDA_({pda.Name})", pda.StatesCount+2, pda.Alphabet, pda.WorkAlphabet.Append(extra_symbol).ToArray(), newt, 0, pda.StartStackSymbol, new uint[] {pda.StatesCount+1});
        }

        
        public static explicit operator StatePDA(Finite.NFAe N) {
            var newTrans = new PDATransform();

            foreach (var t in (Finite.NFAeTransform)N.Transform) {
                var newVals = new PDATransformValue[t.Value.Length];
                for (int i=0; i<newVals.Length; i++)
                    newVals[i] = new PDATransformValue(null, t.Value[i]);
                newTrans.Add(new PDATransformKey(t.Key.q, t.Key.c, null), newVals);
            }
            return new StatePDA($"QPDA_({N.Name})", N.StatesCount, N.Alphabet, new char[] {}, newTrans, N.StartState, (char)0, N.AcceptedStates);
        }

        [System.ComponentModel.Description("1659_L3.1_P76")]
        public static explicit operator StatePDA(ContextFree.CFGrammer cfg) {
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


            t.Add(qSim, null, ContextFree.PDA.START, null, q);
            sn.Add(q, "accept");

            var WorkAlphabet = new System.Collections.Generic.List<char>();
            WorkAlphabet.AddRange(cfg.Terminals);
            WorkAlphabet.AddRange(cfg.Variables);
            WorkAlphabet.Add(ContextFree.PDA.START);

            var qpda = new ContextFree.StatePDA($"QPDA_({cfg.Name})", q+1, cfg.Terminals, WorkAlphabet.ToArray(), t, 0, ContextFree.PDA.START, new uint[] {q});
            for (uint i = 0; i < qpda.StatesCount; i++)
            {
                qpda.States[i] = sn[i];
            }
            return qpda;
        }

        public override bool AcceptWord(string w) {
            //construct start config
            int runCount = 0;
            PDAConfig[] pcfgs;
            if (StartStackSymbol !=  0)
                pcfgs = new PDAConfig[] {new PDAConfig(StartState, w, new char[] {StartStackSymbol},null)};
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
                    if (p.word.Length == 0)
                        if (IsAcceptedState(p.q))
                            return true;
                        
                } //next p
            }

            return false;
        }
    }
}