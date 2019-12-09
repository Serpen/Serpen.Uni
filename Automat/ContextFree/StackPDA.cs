using System.Linq;

namespace Serpen.Uni.Automat.ContextFree {

    [System.Serializable]
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
        [System.Obsolete()]
        public StackPDA(string name, uint StatesCount, char[] InputAlphabet, char[] Workalphabet, PDATransform Transform, uint StartState, char? Startsymbol)
        : base(name, StatesCount, InputAlphabet, Workalphabet, Transform, StartState, Startsymbol, new uint[] { }) {
        }

        [System.Obsolete()]
        public StackPDA(string name, string[] states, char[] InputAlphabet, char[] Workalphabet, PDATransform Transform, uint StartState, char? Startsymbol)
        : base(name, states, InputAlphabet, Workalphabet, Transform, StartState, Startsymbol, new uint[] { }) {
        }

        public StackPDA(string name, uint StatesCount, char[] InputAlphabet, char[] Workalphabet, PDATransform Transform, uint StartState)
        : base(name, StatesCount, InputAlphabet, Workalphabet, Transform, StartState, new uint[] { }) {
        }

        public StackPDA(string name, string[] states, char[] InputAlphabet, char[] Workalphabet, PDATransform Transform, uint StartState)
        : base(name, states, InputAlphabet, Workalphabet, Transform, StartState, new uint[] { }) {
        }

        public static readonly StackPDA Empty = new StackPDA("SPDA_Empty", 1, new char[] { }, new char[] { }, new PDATransform(), 0, START);

        [AlgorithmSource("1659_L33")]
        public static explicit operator StackPDA(StatePDA pda) {

            // find new unused Stack Startsymbol for workalphabet
            char extra_symbol = Utils.NextFreeCapitalLetter(pda.Alphabet.Concat(pda.WorkAlphabet).ToArray(), EXTRASYMBOLS[0], EXTRASYMBOLS);
            var spdaWorkAlphabet = new System.Collections.Generic.List<char>(pda.WorkAlphabet) {
                extra_symbol
            };


            // start Stackpda with new pushed Stackstartsymbol
            var newt = new PDATransform();

            byte inc = 1;
            if (pda.StartSymbol != null) {
                inc = 2;
                newt.Add(0, null, null, extra_symbol.ToString(), 1);
                newt.Add(1, null, null, pda.StartSymbol.ToString(), pda.StartState + inc);
            } else
                newt.Add(0, null, null, extra_symbol.ToString(), pda.StartState + 1);


            uint qPump = pda.StatesCount + inc; // ;

            // inc all States cause of new first § state
            foreach (var t in pda.Transforms) {
                for (int j = 0; j < t.Value.Length; j++) {
                    newt.AddM(t.Key.q + inc, t.Key.ci, t.Key.cw, t.Value[j].cw2, t.Value[j].qNext + inc);
                }
            }

            // all accepted states goes to qPump
            for (int i = 0; i < pda.AcceptedStates.Length; i++)
                newt.AddM(pda.AcceptedStates[i] + inc, null, null, null, qPump);

            // qPump consumes whole alphabet, to empty stack    
            foreach (char cw in spdaWorkAlphabet)
                newt.Add(qPump, null, cw, null, qPump);

            return new StackPDA($"SPDA_({pda.Name})", pda.StatesCount + inc + 1, pda.Alphabet, spdaWorkAlphabet.ToArray(), newt, 0, null);
        }

        public override bool AcceptWord(string w) {
            CheckWordInAlphabet(w);

            int runCount = 0;
            //construct start config
            PDAConfig[] pcfgs;
            if (StartSymbol.HasValue)
                pcfgs = new PDAConfig[] { new PDAConfig(StartState, w, new char[] { StartSymbol.Value }, null) };
            else
                pcfgs = new PDAConfig[] { new PDAConfig(StartState, w, new char[] { }, null) };


            //while any pcfg exists
            while (pcfgs.Length > 0) { //&& (pcfg.Where((a) => a.Stack.Length>0).Any())
                foreach (var p in pcfgs)
                    if ((p.Stack.Count() == 0 || (p.Stack[p.Stack.Count() - 1] == this.StartSymbol && p.Stack.Count() == 1)) && p.word.Length == 0)
                        return true;

                pcfgs = GoChar(pcfgs);

                runCount++;

                if (pcfgs.Length > MAX_RUNS_OR_STACK || runCount > MAX_RUNS_OR_STACK) {
                    throw new PDAStackException($"{runCount}: Stack >= {pcfgs.Length}, abort", this);
                }
            }

            return false;
        }

        public static StackPDA GenerateRandom() {
            const byte MAX_STATES = 20;
            const byte MAX_CHAR = 7;

            var rnd = Uni.Utils.RND;

            var t = new PDATransform();
            int stateCount = rnd.Next(1, MAX_STATES);

            char[] inputAlphabet = new char[rnd.Next(1, MAX_CHAR)];
            for (int i = 0; i < inputAlphabet.Length; i++)
                inputAlphabet[i] = (char)rnd.Next('a', 'z');
            inputAlphabet = inputAlphabet.Distinct().ToArray();

            char[] workAlphabet = new char[rnd.Next(1, MAX_CHAR)];
            for (int i = 1; i < workAlphabet.Length; i++)
                workAlphabet[i] = (char)rnd.Next('a', 'z');
            workAlphabet[0] = START;
            workAlphabet = workAlphabet.Distinct().ToArray();

            for (uint i = 0; i < stateCount; i++) {
                int transformsRnd = rnd.Next(0, inputAlphabet.Length);
                for (int j = 0; j < transformsRnd; j++)
                    t.AddM(i, inputAlphabet.RndElement(), workAlphabet.RndElement(), workAlphabet.RndElement().ToString(), (uint)rnd.Next(0, stateCount));

            }

            var ret = new StackPDA("SPDA_Random", (uint)stateCount, inputAlphabet, workAlphabet, t, (uint)rnd.Next(0, stateCount), START);
            ret.Name = $"SPDA_Random_{ret.GetHashCode()}";
            return ret;
        }

        public override IAutomat PurgeStates() {
            (uint[] translate, string[] names, _) = base.RemovedStateTranslateTables();

            var newT = new PDATransform();
            foreach (var t2 in Transforms)
                if (translate.Contains(t2.Key.q))
                    foreach (var v in t2.Value)
                        if (translate.Contains(v.qNext))
                            newT.AddM(translate.ArrayIndex(t2.Key.q), t2.Key.ci, t2.Key.cw, v.cw2, translate.ArrayIndex(v.qNext));

            return new StackPDA($"{Name}_purged", names, Alphabet, WorkAlphabet, newT, translate.ArrayIndex(StartState), StartSymbol);
        }

        [AlgorithmSource("1659_L3.1_P76")]
        public static explicit operator StackPDA(ContextFree.CFGrammer cfg) {
            var t = new ContextFree.PDATransform();
            var names = new System.Collections.Generic.Dictionary<uint, string>();

            const uint qSim = 1;
            uint q = qSim + 1;

            // t.Add(0,null, null, ContextFree.PDA.START.ToString(), 1);
            // sn.Add(0, "0");
            t.Add(0, null, null, cfg.StartSymbol.ToString(), qSim);
            names.Add(0, "start");

            foreach (char c in cfg.Terminals)
                t.Add(qSim, c, c, null, qSim);

            t.Add(qSim, null, ContextFree.PDA.START, null, qSim);
            names.Add(qSim, "sim");

            foreach (var r in cfg.Rules) {
                foreach (string body in r.Value) {
                    if (body.Length > 2) {
                        t.AddM(qSim, null, r.Key, body.Substring(body.Length - 1, 1), q);
                        names.TryAdd(q, $"{q}; {r.Key}=>{body}");
                        for (int i = body.Length - 2; i > 0; i--) {
                            t.AddM(q, null, null, body.Substring(i, 1), ++q);
                            names.TryAdd(q, $"{q}; {r.Key}=>{body}");
                        }
                        t.AddM(q, null, null, body.Substring(0, 1), qSim);
                        q++;
                    } else if (body.Length == 2) {
                        t.AddM(qSim, null, r.Key, body.Substring(1, 1), q);
                        t.AddM(q, null, null, body.Substring(0, 1), qSim);
                        names.TryAdd(q, $"{q}; {r.Key}=>{body}");
                        q++;

                    } else
                        t.AddM(qSim, null, r.Key, body, qSim);

                }
            }

            char[] WorkAlphabet = cfg.Terminals.Union(cfg.Variables).ToArray(); //.Append(START)

            return new ContextFree.StackPDA($"SPDA_({cfg.Name})", names.Values.ToArray(), cfg.Terminals, WorkAlphabet, t, 0, START);
        }

        public override string ToString() => $"{Name} SPDA(|{States.Length}|={string.Join(";", States)}), {{{string.Join(',', Alphabet)}}},{{{string.Join(',', WorkAlphabet)}}}, {{{Transforms.ToString()}}}, {StartState}, {StartSymbol})".Trim();

    }
}