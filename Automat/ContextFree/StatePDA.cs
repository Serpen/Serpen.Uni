using System.Linq;

namespace Serpen.Uni.Automat.ContextFree {
    [System.Serializable()]
    public class StatePDA : PDA, IKleeneStern {

        public StatePDA(string name, uint StatesCount, char[] InputAlphabet, char[] Workalphabet, PDATransform Transform, uint StartState, char? Startstacksymbol, params uint[] AcceptedStates)
         : base(name, StatesCount, InputAlphabet, Workalphabet, Transform, StartState, Startstacksymbol, AcceptedStates) {
        }

        public StatePDA(string name, string[] names, char[] InputAlphabet, char[] Workalphabet, PDATransform Transform, uint StartState, char? Startstacksymbol, params uint[] AcceptedStates)
         : base(name, names, InputAlphabet, Workalphabet, Transform, StartState, Startstacksymbol, AcceptedStates) {
        }
        public StatePDA(string name, uint StatesCount, char[] InputAlphabet, char[] Workalphabet, PDATransform Transform, uint StartState, params uint[] AcceptedStates)
         : base(name, StatesCount, InputAlphabet, Workalphabet, Transform, StartState, null, AcceptedStates) {
        }

        public StatePDA(string name, string[] names, char[] InputAlphabet, char[] Workalphabet, PDATransform Transform, uint StartState, params uint[] AcceptedStates)
         : base(name, names, InputAlphabet, Workalphabet, Transform, StartState, null, AcceptedStates) {
        }

        [AlgorithmSource("EAFK_A64")]
        public static explicit operator StatePDA(StackPDA pda) {
            var newt = new PDATransform();

            char extra_symbol = Utils.NextFreeCapitalLetter(pda.Alphabet.Concat(pda.WorkAlphabet).ToArray(), EXTRASYMBOLS[0], EXTRASYMBOLS);

            newt.Add(0, null, null, extra_symbol, 1);
            for (int i = 0; i < pda.Transforms.Count; i++) {
                var t = pda.Transforms.ElementAt(i);
                for (int j = 0; j < t.Value.Length; j++) {
                    newt.AddM(t.Key.q + 1, t.Key.ci, t.Key.cw, t.Value[j].cw2, t.Value[j].qNext + 1);
                    newt.AddM(t.Key.q + 1, null, extra_symbol, null, pda.StatesCount + 1);
                }
            }

            return new StatePDA($"QPDA_({pda.Name})", pda.StatesCount + 2, pda.Alphabet, pda.WorkAlphabet.Append(extra_symbol).ToArray(), newt, 0, pda.StartSymbol, new uint[] { pda.StatesCount + 1 });
        }

        [AlgorithmComplexity("n²")]
        public static explicit operator StatePDA(Finite.FABase FA) {
            var pdat = new PDATransform();

            foreach (var t in FA.Transforms) {
                var newVals = new PDATransformValue[t.Value.Length];
                for (int i = 0; i < newVals.Length; i++)
                    newVals[i] = new PDATransformValue(null, t.Value[i]);
                pdat.Add(new PDATransformKey(t.Key.q, t.Key.c, null), newVals);
            }
            return new StatePDA($"QPDA_({FA.Name})", FA.StatesCount, FA.Alphabet, new char[] { }, pdat, FA.StartState, FA.AcceptedStates);
        }

        [AlgorithmSource("1659_L3.1_P76")]
        [AlgorithmComplexity("O(n²)")]
        public static explicit operator StatePDA(CFGrammer cfg) {
            var t = new PDATransform();
            var names = new System.Collections.Generic.Dictionary<uint, string>();

            const uint qSim = 2;
            uint q = qSim + 1;

            t.Add(0, null, null, START, 1);
            names.Add(0, "0");

            t.Add(1, null, null, cfg.StartSymbol, qSim);
            names.Add(1, "start");

            foreach (char c in cfg.Terminals)
                t.Add(qSim, c, c, null, qSim);

            names.Add(qSim, "sim");

            foreach (var r in cfg.Rules) {
                foreach (string body in r.Value) {
                    if (body.Length >= 2) {
                        t.AddM(qSim, null, r.Key, body[^1], q);
                        names.TryAdd(q, $"{q}; {r.Key}=>{body}");
                        for (int i = body.Length - 2; i > 0; i--) {
                            t.AddM(q, null, null, body[i], q+1);
                            q++;
                            names.TryAdd(q, $"{q}; {r.Key}=>{body}");
                        }
                        t.AddM(q, null, null, body[0], qSim);
                        q++;
                    } else {
                        t.AddM(qSim, null, r.Key, body, qSim);
                    }
                }
            }

            t.Add(qSim, null, START, null, q);
            names.Add(q, "accept");

            char[] WorkAlphabet = cfg.Terminals.Union(cfg.Variables).Append(START).ToArray();

            return new StatePDA($"QPDA_({cfg.Name})", names.Values.ToArray(), cfg.Terminals, WorkAlphabet, t, 0, q);
        }

        public override bool AcceptWord(string w) {
            CheckWordInAlphabet(w);

            //construct start config
            int runCount = 0;
            PDAConfig[] pcfgs;
            if (StartSymbol.HasValue)
                pcfgs = new PDAConfig[] { new PDAConfig(StartState, w, new char[] { StartSymbol.Value }, null) };
            else
                pcfgs = new PDAConfig[] { new PDAConfig(StartState, w, new char[] { }, null) };

            //while any pcfg exists
            while (pcfgs.Length > 0) { //&& (pcfg.Where((a) => a.Stack.Length>0).Any())
                Utils.DebugMessage(string.Join(',', (from a in pcfgs select a.ToString())), this, Uni.Utils.eDebugLogLevel.Verbose);
                foreach (var p in pcfgs)
                    if (p.word.Length == 0)
                        if (IsAcceptedState(p.State))
                            return true;

                pcfgs = GoChar(pcfgs);

                runCount++;
                if (pcfgs.Length > MAX_RUNS_OR_STACK || runCount > MAX_RUNS_OR_STACK)
                    throw new PDAStackException($"Stack >= {pcfgs.Length}, {runCount}. run abort", this);
            }

            return false;
        }

        public static StatePDA GenerateRandom() {
            const byte MAX_STATES = 20;
            const byte MAX_CHAR = 7;

            var rnd = Uni.Utils.RND;

            var t = new PDATransform();
            int stateCount = rnd.Next(1, MAX_STATES);

            char[] inputAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR);
            char[] workAlphabet = RandomGenerator.RandomAlphabet(1, MAX_CHAR, new char[] { START });
            uint[] accState = RandomGenerator.RandomAcceptedStates(1, stateCount / 3, stateCount);

            for (uint i = 0; i < stateCount; i++) {
                int transformsRnd = rnd.Next(0, inputAlphabet.Length);
                for (int j = 0; j < transformsRnd; j++)
                    t.AddM(i, inputAlphabet.RndElement(), workAlphabet.RndElement(), workAlphabet.RndElement(), (uint)rnd.Next(0, stateCount));
            }

            var ret = new StatePDA("QPDA_Random", (uint)stateCount, inputAlphabet, workAlphabet, t, (uint)rnd.Next(0, stateCount), START, accState);
            ret.Name = $"QPDA_Random_{ret.GetHashCode()}";
            return ret;
        }


        public override IAutomat PurgeStates() {
            (uint[] translate, string[] names, uint[] aStates) = base.RemovedStateTranslateTables();

            var newT = new PDATransform();
            foreach (var t2 in Transforms)
                if (translate.Contains(t2.Key.q))
                    foreach (var v in t2.Value)
                        if (translate.Contains(v.qNext))
                            newT.AddM(translate.ArrayIndex(t2.Key.q), t2.Key.ci, t2.Key.cw, v.cw2, translate.ArrayIndex(v.qNext));

            return new StatePDA($"{Name}_purged", names, Alphabet, WorkAlphabet, newT, translate.ArrayIndex(StartState), StartSymbol, aStates);
        }

        public IAutomat KleeneStern() {
            var pdaT = new PDATransform
            {
                { 0, null, null, null, this.StartState+1 } // accepted front state
			};

            // add transforms, +1
            foreach (var item in this.Transforms) {
                PDATransformValue[] qnext = new PDATransformValue[item.Value.Length];
                for (int i = 0; i < item.Value.Length; i++)
                    pdaT.Add(item.Key.q + 1, item.Key.ci, item.Key.cw, item.Value[i].cw2, item.Value[i].qNext + 1);
            }

            var acceptedStates = new uint[this.AcceptedStates.Length + 1];
            acceptedStates[0] = 0;
            // add e-transform from org accepted to front state
            for (uint i = 0; i < this.AcceptedStates.Length; i++) {
                acceptedStates[i + 1] = this.AcceptedStates[i] + 1; // a little redundant, because all accepted states must go back to first
                pdaT.Add(this.AcceptedStates[i] + 1, null, null, null, 0);
            }

            return new StatePDA($"QPDA_KleeneStern({Name})", this.StatesCount + 1, this.Alphabet, this.WorkAlphabet, pdaT, 0, acceptedStates);
        }

    }
}