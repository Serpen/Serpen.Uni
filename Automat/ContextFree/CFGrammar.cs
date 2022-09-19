using System;
using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat.ContextFree {
    [Serializable]
    public class CFGrammer : GrammerBase, IUnion {

        public CFGrammer(string name, char[] variables, char[] terminals, RuleSet rules, char startSymbol)
            : base(name, variables, terminals, rules, startSymbol) {

            CheckConstraints();
        }

        public bool IsChomskey { get; internal set; }

        public static CFGrammer GenerateRandom(bool removeUnused = true) {
            var rnd = Uni.Utils.RND;
            const byte MAX_CHAR = 10;
            const byte MAX_VAR = 5;
            const byte MAX_RUL = 5;
            const byte MAX_WLEN = 5;
            const byte MAX_BODY = 5;

            var rs = new RuleSet();
            var Vars = new char[rnd.Next(1, MAX_VAR)];
            for (int i = 0; i < Vars.Length; i++)
                Vars[i] = (char)(rnd.Next((int)'A', (int)'Z'));

            var Terms = new char[rnd.Next(1, MAX_CHAR)];
            for (int i = 0; i < Terms.Length; i++)
                Terms[i] = (char)(rnd.Next((int)'a', (int)'z'));

            Vars = Vars.Distinct().OrderBy(s => s).ToArray();
            Terms = Terms.Distinct().OrderBy(s => s).ToArray();

            int rulesCount = rnd.Next(1, MAX_RUL);

            for (int i = 0; i < Math.Min(rulesCount, Vars.Length); i++) {
                char rKey = Vars[rnd.Next(0, Vars.Length)];
                var Vals = new string[rnd.Next(1, MAX_BODY)];
                for (int j = 0; j < Vals.Length; j++) {
                    string w = "";
                    var wLen = rnd.Next(0, MAX_WLEN);
                    for (int k = 0; k < wLen; k++) {
                        if (rnd.NextDouble() > 0.7)
                            w = w.Insert(k, Terms.RndElement().ToString());
                        else
                            w = w.Insert(k, Vars.RndElement().ToString());
                    }
                    Vals[j] = w;
                }

                if (!rs.ContainsKey(rKey))
                    rs.Add(rKey, Vals.Distinct().OrderBy(s => s).ToArray());
                else
                    rs[rKey] = Enumerable.Concat(rs[rKey], Vals).Distinct().OrderBy(s => s).ToArray();
            }

            var headVars = (from r in rs select r.Key).Distinct().ToArray();

            var varList = new List<char>(Vars);
            if (removeUnused)
                rs = rs.RemoveUnusedSymbols(varList, Terms);

            return new CFGrammer("CFG_Random", varList.ToArray(), Terms, rs, headVars.RndElement());
        }


        [AlgorithmSource("EAFK_S6.14_P253")]
        public static explicit operator CFGrammer(StackPDA spda) {
            var vars = new List<char>();
            char startSymbol;
            var translate = new Dictionary<string, char>(Uni.Utils.Pow((int)spda.StatesCount, 2));
            if (spda.StartSymbol.HasValue)
                startSymbol = spda.StartSymbol.Value;
            else { }
            startSymbol = Utils.NextFreeCapitalLetter(spda.WorkAlphabet, null);


            vars.Add(startSymbol);

            for (int i = 0; i < spda.StatesCount; i++)
                for (int j = 0; j < spda.StatesCount; j++) {
                    foreach (char c in spda.WorkAlphabet.Append(startSymbol).Distinct()) {
                        char l = Utils.NextFreeCapitalLetter(vars, null);
                        vars.Add(l);
                        translate.Add($"{i}{c}{j}", l);
                    }
                    char le = Utils.NextFreeCapitalLetter(vars, null);
                    vars.Add(le);
                    translate.Add($"{i}{j}", le);
                }

            var rs = new RuleSet();

            var sbody = new List<string>();
            for (int i = 0; i < spda.StatesCount; i++)
                sbody.Add(translate[$"{spda.StartState}{startSymbol}{i}"].ToString());

            rs.Add(startSymbol, sbody.ToArray());

            foreach (var t in spda.Transforms) {
                var bodys = new List<string>(t.Value.Length);
                foreach (var v in t.Value) {
                    if (v.cw2 != "")
                        for (int i = 0; i < spda.StatesCount; i++)
                            for (int j = 0; j < spda.StatesCount; j++)
                                bodys.Add($"{t.Key.ci}{translate[$"{i}{startSymbol}{j}"]}{translate[$"{i}{startSymbol}{j}"]}");
                    else
                        bodys.Add("");
                    rs.AddM(translate[$"{t.Key.q}{t.Key.cw}{v.qNext}"], bodys.Distinct().ToArray());
                }
            }

            return new CFGrammer($"CFG_({spda.Name})", vars.ToArray(), spda.Alphabet, rs, startSymbol);
        }

        internal GrammerBase org;

        public CFGrammer ToChomskyNF(AlgSourceMode mode = AlgSourceMode.EAFK) {

            var finalVars = new List<char>(Variables);
            var newRS = Rules;

            Utils.DebugMessage($"0.before: {newRS}", this, Uni.Utils.eDebugLogLevel.Verbose);

            if (mode == AlgSourceMode.EAFK) {
                newRS = newRS.RemoveUnusedSymbols(finalVars, Terminals);
                Utils.DebugMessage($"0.remove: {newRS}", this, Uni.Utils.eDebugLogLevel.Verbose);
            }

            newRS = Chomskey1_StartSymbol(newRS, finalVars);
            Utils.DebugMessage($"1.starts: {newRS}", this, Uni.Utils.eDebugLogLevel.Verbose);

            newRS = Chomskey2_Epsilon(newRS);
            Utils.DebugMessage($"2.epsilo: {newRS}", this, Uni.Utils.eDebugLogLevel.Verbose);

            newRS = Chomskey3_UnitRules(newRS, finalVars);
            Utils.DebugMessage($"3.unitru: {newRS}", this, Uni.Utils.eDebugLogLevel.Verbose);

            newRS = Chomskey4_Length(newRS, finalVars);
            Utils.DebugMessage($"4.length: {newRS}", this, Uni.Utils.eDebugLogLevel.Verbose);

            newRS = Chomskey5_Terminals(newRS, finalVars);
            Utils.DebugMessage($"5.termin: {newRS}", this, Uni.Utils.eDebugLogLevel.Verbose);

            return new CFGrammer($"CCFG{mode}_({Name})", finalVars.ToArray(), Terminals, newRS, StartSymbol) { IsChomskey = true, org = this };
        }

        /// <summary>
        /// Chomskey Transformation 1, Remove all StartSymbols on right side
        /// </summary>
        /// <param name="rs">Old RuleSet</param>
        /// <param name="newVars">Reference to new Variables</param>
        /// <returns>CNF Step 1</returns>
        RuleSet Chomskey1_StartSymbol(RuleSet rs, ICollection<char> newVars) {
            var newRS = new RuleSet();

            char? newStartSymbol = null;

            // check all rules for startbody
            foreach (var r in rs)
                foreach (string body in r.Value) {
                    if (body.Contains(StartSymbol)) {
                        //get new Symbol, ONCE
                        if (!newStartSymbol.HasValue) {
                            newStartSymbol = Utils.NextFreeCapitalLetter(newVars.Union(Terminals).ToArray(), StartSymbol);
                            newVars.Add(newStartSymbol.Value);
                            break;
                        }
                    }
                }

            // no symbol found, return old and exit
            if (!newStartSymbol.HasValue)
                return rs;

            // first add new startsymbole
            newRS.Add(StartSymbol, new string[] { newStartSymbol.Value.ToString() });


            // iterate rules and replace S->S'
            foreach (var r in rs) {
                char newrKey = r.Key != StartSymbol ? r.Key : newStartSymbol.Value;
                var valList = new List<string>(r.Value.Length);

                foreach (string body in r.Value)
                    valList.Add(body.Replace(StartSymbol, newStartSymbol.Value));

                newRS.Add(newrKey, valList.ToArray());
            }

            return newRS;
        }

        /// <summary>
        /// Move e-productions to Start
        /// </summary>
        RuleSet Chomskey2_Epsilon(RuleSet rs) {
            RuleSet newRS = new RuleSet();
            RuleSet roRS = null;

            var ListEpsilonHead = new Stack<char>();
            var alreadyProcessed = new List<char>();

            const string rChar = "";

            // find all rules with and empty body, S=>e is allowed
            foreach (var r in rs)
                if (r.Key != StartSymbol && r.Value.Contains(""))
                    ListEpsilonHead.Push(r.Key);

            // no E rule outside S found, return old RS and exit
            if (!ListEpsilonHead.Any())
                return rs;

            // as long as an Variable with Epsilon body exits
            while (ListEpsilonHead.Any()) {
                if (roRS == null)
                    roRS = rs;
                else {
                    roRS = newRS;
                    newRS = new RuleSet();
                }

                var VarEpsilonHead = ListEpsilonHead.Pop();

                // iterate rules
                foreach (var r in roRS) {
                    var newVals = new List<string>(r.Value.Length);
                    foreach (string body in r.Value) {
                        if (body.Contains(VarEpsilonHead) && !alreadyProcessed.Contains(r.Key)) { //replace Var with e-Power
                            newVals.AddRange(ReplaceStringEpsilonPowerSet(body, VarEpsilonHead));
                        } else if (body == rChar && r.Key == VarEpsilonHead) {
                            // should be removed
                        } else
                            newVals.Add(body);
                    }

                    if (alreadyProcessed.Contains(r.Key) && newVals.Contains(""))
                        newVals.Remove("");

                    newRS.Add(r.Key, newVals.Distinct().ToArray());
                }

                // search for new Epsilon Productions
                foreach (var r in newRS)
                    if (r.Key != StartSymbol && r.Value.Contains(rChar))
                        ListEpsilonHead.Push(r.Key);
                alreadyProcessed.Add(VarEpsilonHead);
            }

            return newRS;
        }

        /// <summary>
        /// Eliminate Rules Single Var->Single Var
        /// </summary>
        RuleSet Chomskey3_UnitRules(RuleSet rs, IEnumerable<char> newVars) {
            RuleSet newRS = new RuleSet();
            RuleSet roRS = null;

            var Repl = new Stack<char>();
            string[] replaceStrings;

            // find all rules with VAR->Var
            foreach (var r in rs)
                foreach (string body in r.Value)
                    if (body.Length == 1 && newVars.Contains(body[0]))
                        if (!Repl.Contains(body[0]))
                            Repl.Push(body[0]);

            if (!Repl.Any())
                return rs;

            while (Repl.Any()) {
                if (roRS == null)
                    roRS = rs;
                else {
                    roRS = newRS;
                    newRS = new RuleSet();
                }
                var singleVar = Repl.Pop();

                if (roRS.ContainsKey(singleVar)) {
                    replaceStrings = roRS[singleVar];
                } else {
                    replaceStrings = null;
                    Utils.DebugMessage($"{singleVar} not a head", this, Uni.Utils.eDebugLogLevel.Always);
                }

                foreach (var r in roRS) {
                    var newVals = new List<string>(r.Value.Length);
                    foreach (string body in r.Value)
                        if (replaceStrings != null && body == singleVar.ToString())
                            newVals.AddRange(replaceStrings);
                        else if (body.Length == 0 || body.Length != 1 || body[0] != singleVar) // don't readd singlevar
                            newVals.Add(body);
                        else
                            Utils.DebugMessage($"{singleVar} not re-added", this, Uni.Utils.eDebugLogLevel.Verbose);

                    if (newVals.Contains(r.Key.ToString()))
                        newVals.Remove(r.Key.ToString());
                    newRS.Add(r.Key, newVals.Distinct().ToArray());
                }

                foreach (var r in newRS)
                    if (r.Key != StartSymbol && r.Value.Contains(""))
                        Repl.Push(r.Key);

            }
            return newRS;
        }

        /// <summary>
        /// Length <= 2, introduce new Var for all |body|>2
        /// </summary>
        RuleSet Chomskey4_Length(RuleSet rs, ICollection<char> newVars) {
            var newRs = new RuleSet();
            var translate = new Dictionary<string, char>();

            foreach (var r in rs) {
                var newVals = new List<string>(r.Value.Length);
                foreach (string orgbody in r.Value) {
                    string newbody = orgbody;

                    // translate every new word, with all existing translations
                    foreach (var t in translate) {
                        var oldbody = newbody;
                        if (newbody.Length > 2)
                            // FIX: replace should start right
                            newbody = newbody.Replace(t.Key, t.Value.ToString());
                        else
                            break;

                        // body gets to small, Var only
                        if (newbody.Length == 1 && newVars.Contains(newbody[0]))
                            newbody = oldbody;
                    }


                    while (newbody.Length > 2) {
                        var replacePart = newbody.Substring(newbody.Length - 2, 2);

                        if (!translate.TryGetValue(replacePart, out char nL)) {
                            nL = Utils.NextFreeCapitalLetter(newVars.Union(Terminals).ToArray(), 'X');
                            newVars.Add(nL);
                            translate.Add(replacePart, nL);
                        }
                        // FIX: replace should start right
                        newbody = newbody.Replace(replacePart, nL.ToString());
                    }
                    newVals.Add(newbody);
                }
                newRs.Add(r.Key, newVals.ToArray());
            }

            foreach (var la in translate)
                newRs.Add(la.Value, new string[] { la.Key });

            return newRs;
        }

        RuleSet Chomskey5_Terminals(RuleSet rs, ICollection<char> newVars) {
            var newRs = new RuleSet();
            var translate = new Dictionary<char, char>();

            foreach (var r in rs) {
                var newVals = new List<string>(r.Value.Length);
                foreach (string ibody in r.Value) {
                    string newBody = ibody;
                    if (ibody.Length == 2) {

                        var c2Replaces = new List<char>();
                        if (Terminals.Contains(newBody[0]))
                            c2Replaces.Add(newBody[0]);
                        if (Terminals.Contains(newBody[1]))
                            c2Replaces.Add(newBody[1]);

                        foreach (var c2Replace in c2Replaces) {
                            if (translate.TryGetValue(c2Replace, out char nl)) { } else {
                                nl = Utils.NextFreeCapitalLetter(newVars.Union(Terminals).ToArray(), c2Replace.ToString().ToUpper()[0]);
                                newVars.Add(nl);
                                translate.Add(c2Replace, nl);
                            }
                            newBody = newBody.Replace(c2Replace, nl);
                        }
                    } //end if body.Length = 2
                    newVals.Add(newBody);
                } //next body
                newRs.Add(r.Key, newVals.ToArray());
            } //next r

            foreach (var la in translate)
                newRs.Add(la.Value, new string[] { la.Key.ToString() });

            return newRs;
        } //end function

        /// <summary>
        /// Replaces all chars with Epsilon or itself, so that all PowerSet possibilites are used
        /// </summary>
        /// <returns></returns>
        static string[] ReplaceStringEpsilonPowerSet(string s, char c) {
            var cPos = new List<int>(s.Length);
            var strs = new List<string>(s.Length);

            //save all c occurendes in cPos
            for (int i = 0; i < s.Length; i++)
                if (s[i] == c)
                    cPos.Add(i);

            var ps = Uni.Utils.GetPowerSet(cPos.Count);

            for (int i = 0; i < ps.GetLength(0); i++) {
                var newString = s;
                for (int j = ps.GetLength(1) - 1; j >= 0; j--) //iterate backwards, so remove positions stays corret
                {
                    if (ps[i, j])
                        newString = newString.Remove(cPos[j], 1);

                }
                strs.Add(newString);
            }

            return strs.Distinct().ToArray();
        }

        [AlgorithmSource("1659_3.4 CYK,Alg2")]
        public override bool AcceptWord(string w) {
            if (!IsChomskey) throw new NotSupportedException("Only supported in Chomskey normalform");

            if (string.IsNullOrEmpty(w)) {
                if (Rules.ContainsKey(StartSymbol))
                    return Rules[StartSymbol].Contains("");
                else
                    return false;
            } else {
                char[,][] table = new char[w.Length, w.Length][];

                for (int i = 0; i < w.Length; i++)
                    table[i, i] = Rules.FindProdHeads(w.Substring(i, 1));

                for (int d = 1; d < w.Length; d++) //distance i,j
                    for (int i = 0; i < w.Length - d; i++) {
                        int j = i + d;
                        var Heads = new List<char>();
                        for (int t = i; t < j; t++)
                            foreach (char tit in table[i, t])
                                foreach (char tt1j in table[t + 1, j])
                                    Heads.AddRange(Rules.FindProdHeads(
                                        new string(new char[] { tit, tt1j }
                                    )));

                        table[i, j] = Heads.Distinct().ToArray();
                    }
                return table[0, w.Length - 1].Contains(StartSymbol);
            }
        }

        [AlgorithmSource("EAFK_7.4.3")]
        public bool IsEmpty() => throw new NotImplementedException();

        #region Abgeschlossenheitseigenschaften


        CFGrammer Combine(IAcceptWord ia, string[] startBodysFormat, string nameExt) {
            if (!(ia is CFGrammer cfg2))
                throw new System.NotSupportedException();

            var newRules = new RuleSet();

            var finalVars = this.Variables.ToList();
            var finalTerms = this.Terminals.ToList();
            char newStart = Utils.NextFreeCapitalLetter(this.VarAndTerm.Union(cfg2.VarAndTerm), this.StartSymbol);

            finalVars.Add(newStart);

            var translate = new Dictionary<char, char>(cfg2.VarAndTerm.Count);
            foreach (char V in cfg2.Variables) {
                if (!finalVars.Contains(V)) {
                    finalVars.Add(V);
                    translate.Add(V, V);
                } else {
                    char newChar = Utils.NextFreeCapitalLetter(finalTerms.Union(finalVars), V);
                    translate.Add(V, newChar);
                    finalVars.Add(newChar);
                }
            }

            // assume char as terminal is equal when its equal
            foreach (char t in cfg2.Terminals) {
                if (!finalTerms.Contains(t)) {
                    finalTerms.Add(t);
                }
                // else {
                //     char newChar = Utils.NextFreeLowerLetter(finalTerms.Union(finalVars)); //against convention!
                //     translate.Add(t, newChar);
                //     finalTerms.Add(newChar);
                // }
                if (!translate.ContainsKey(t))
                    translate.Add(t, t);
            }


            for (int i = 0; i < startBodysFormat.Length; i++)
                startBodysFormat[i] = string.Format(startBodysFormat[i], this.StartSymbol, cfg2.StartSymbol, newStart);

            //New Start Rule, build 
            newRules.Add(newStart, startBodysFormat);


            foreach (var r in this.Rules) {
                newRules.Add(r.Key, r.Value);
            }


            foreach (var r in cfg2.Rules) {
                var newVals = new List<string>(r.Value.Length);
                foreach (string body in r.Value) {
                    var sw = new System.Text.StringBuilder();
                    foreach (char c in body)
                        sw.Append(translate[c]);

                    newVals.Add(sw.ToString());
                }
                newRules.Add(translate[r.Key], newVals.ToArray());
            }

            return new CFGrammer($"CFG_Combine({nameExt},{Name})", finalVars.ToArray(), finalTerms.ToArray(), newRules, newStart);

        }

        public CFGrammer KleeneStern() {
            var newRules = new RuleSet();

            char newStart = Utils.NextFreeCapitalLetter(this.VarAndTerm, this.StartSymbol);

            newRules.Add(newStart, new string[] { this.StartSymbol.ToString(), "", $"{this.StartSymbol}{newStart}" });

            foreach (var r in this.Rules) {
                newRules.Add(r.Key, r.Value);
            }
            return new CFGrammer($"CFG_KleeneStern({Name})-", this.Variables.Append(newStart).ToArray(), this.Terminals, newRules, newStart);
        }

        public IAcceptWord Union(IUnion cfg) => Combine(cfg, new string[] { "{0}", "{1}" }, "union");
        public CFGrammer Diff(CFGrammer cfg) => throw new NotSupportedException();
        public CFGrammer Diff(Finite.FABase A) => throw new NotImplementedException();
        public CFGrammer Intersect(CFGrammer cfg) => throw new NotSupportedException();
        public CFGrammer Intersect(Finite.FABase A) => throw new NotImplementedException();
        public CFGrammer Complement() => throw new NotSupportedException();
        public IAcceptWord Concat(IConcat cfg) => Combine(cfg, new string[] { "{0}{1}" }, "concat");
        public IAcceptWord Join(IJoin cfg) => Combine(cfg, new string[] { "{0}" }, "join");

        public CFGrammer HomomorphismChar(Dictionary<char, char> translate) => throw new NotImplementedException();

        #endregion
    } //end class
} //end ns