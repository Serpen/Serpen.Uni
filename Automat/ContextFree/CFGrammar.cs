using System;
using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat.ContextFree {
    public class CFGrammer : GrammerBase, IAbgeschlossenheitseigenschaften<CFGrammer, CFGrammer> {

        public CFGrammer(string name, char[] variables, char[] terminals, RuleSet rules, char startSymbol)
            : base(name, variables, terminals, rules, startSymbol) {
            
            CheckConstraints();
        }

        public bool isChomskey { get; internal set; }
        
        public static CFGrammer GenerateRandom() {
            var rnd = Utils.RND;
            const byte MAX_CHAR = 10;
            const byte MAX_VAR = 5;
            const byte MAX_RUL = 5;
            const byte MAX_WLEN = 5;
            const byte MAX_BODY = 5;

            var rs = new RuleSet();
            var Vars = new char[rnd.Next(1, MAX_VAR)];
            var Terms = new char[rnd.Next(1, MAX_CHAR)];
            for (int i = 0; i < Vars.Length; i++)
                Vars[i] = (char)(rnd.Next((Int32)'A', (Int32)'Z'));

            for (int i = 0; i < Terms.Length; i++)
                Terms[i] = (char)(rnd.Next((Int32)'a', (Int32)'z'));

            Vars = Vars.Distinct().ToArray();
            Terms = Terms.Distinct().ToArray();

            int rulesCount = rnd.Next(1, MAX_RUL);

            for (int i = 0; i < Math.Min(rulesCount, Vars.Length); i++) {
                char rKey = Vars[rnd.Next(0, Vars.Length)];
                if (!rs.ContainsKey(rKey)) {
                    var Vals = new string[rnd.Next(1, MAX_BODY)];
                    for (int j = 0; j < Vals.Length; j++) {
                        string w = "";
                        var wLen = rnd.Next(0, MAX_WLEN);
                        for (int k = 0; k < wLen; k++) {
                            if (rnd.NextDouble() > 0.7)
                                w = w.Insert(k, Utils.GrAE(Terms).ToString());
                            else
                                w = w.Insert(k, Utils.GrAE(Vars).ToString());
                        }
                        Vals[j] = w;
                    }
                    rs.Add(rKey, Vals.Distinct().ToArray());
                }
            }

            var HeadVars = (from r in rs select r.Key).Distinct().ToArray();

            return new CFGrammer("CFG_Random", Vars, Terms, rs, Utils.GrAE(HeadVars));
        }

        
        public CFGrammer toChomskyNF(SourceMode mode = SourceMode.EAFK) {
            var newVars = new List<char>(Variables);
            var newRS = Rules;
            if (mode == SourceMode.EAFK)
                newRS = RemoveUnusedSymbols(newRS, ref newVars);
            newRS = Chomskey1_StartSymbol(newRS, ref newVars);
            newRS = Chomskey2_Epsilon(newRS);
            newRS = Chomskey3_UnitRules(newRS, newVars);
            newRS = Chomskey4_Length(newRS, ref newVars);
            newRS = Chomskey5_Terminals(newRS, ref newVars);
            return new CFGrammer($"CCFG_({Name})", newVars.ToArray(), Terminals, newRS, StartSymbol) { isChomskey = true };
        }

        /// <summary>
        /// Chomskey Transformation 1, Remove all StartSymbols on right side
        /// </summary>
        /// <param name="rs">Old RuleSet</param>
        /// <param name="newVars">Reference to new Variables</param>
        /// <returns>CNF Step 1</returns>
        RuleSet Chomskey1_StartSymbol(RuleSet rs, ref List<char> newVars) {
            var newRS = new RuleSet();

            char newStartSymbol = (char)0;

            //check all rules for startbody
            foreach (var r in rs)
                foreach (string body in r.Value) {
                    if (body.Contains(StartSymbol)) {
                        //get new Symbol, ONCE
                        if (newStartSymbol == (char)0) {
                            newStartSymbol = Utils.NextFreeCapitalLetter(newVars.Union(Terminals).ToArray(), StartSymbol);
                            newVars.Add(newStartSymbol);
                        }
                    }
                }

            //no symbol found, return old and exit
            if (newStartSymbol == (char)0)
                return rs;

            //first add new startsymbole
            newRS.Add(StartSymbol, new string[] { newStartSymbol.ToString() });


            //iterate rules and replace S->S'
            foreach (var r in rs) {
                char newrKey = r.Key != StartSymbol ? r.Key : newStartSymbol;
                var valList = new List<string>();

                foreach (string body in r.Value) {
                    string newBody = body;
                    newBody = newBody.Replace(StartSymbol, newStartSymbol);

                    valList.Add(newBody);
                }

                newRS.Add(newrKey, valList.ToArray());
            }

            return newRS;
        }


        RuleSet Chomskey2_Epsilon(RuleSet rs) {
            RuleSet newRS = new RuleSet();
            RuleSet workRS = null;

            var ListEpsilonHead = new Stack<char>();
            var alreadyProcessed = new List<char>();

            const string rChar = "";

            //find all rules with and empty body, S=>e is allowed
            foreach (var r in rs)
                if (r.Key != StartSymbol && r.Value.Contains(""))
                    ListEpsilonHead.Push(r.Key);

            //no E rule outside S found, return old RS and exit
            if (!ListEpsilonHead.Any())
                return rs;

            var itCount = 0;
            //as long as an Variable with Epsilon body exits
            while (ListEpsilonHead.Any()) {
                itCount++;
                if (itCount == 100) {
                    Console.WriteLine($"Chomskey2_Epsilon 100th run for {this}");
                    //{U}, {p,l,o,d}, {(U=>(UUUU,oUpUUU,loUUUU,,UUdUUUUUo,p,UUUUU,UlUUdUUl,opplU))}, U}
                    // {J}, {b,f}, {(J=>(fJJJffJ,,f,JJfJJJJJ,JJJJJbJ,JJJ,JJJJJbfJ,fJffbJJbJ))}, J}
                    // {J}, {v,w,c}, {(J=>(Jv,JJJc,,wvJvJ,JJJ,c))}, J}
                }
                if (workRS == null)
                    workRS = rs;
                else {
                    workRS = newRS;
                    newRS = new RuleSet();
                }

                var VarEpsilonHead = ListEpsilonHead.Pop();

                //iterate rules
                foreach (var r in workRS) {
                    var newVals = new List<string>();
                    foreach (string body in r.Value) {
                        if (body.Contains(VarEpsilonHead) && !alreadyProcessed.Contains(r.Key)) { //replace Var with e-Power
                            newVals.AddRange(ReplaceStringEpsilonPowerSet(body, VarEpsilonHead));
                        } else if (body == rChar && r.Key == VarEpsilonHead) {
                            //should be removed
                        } else
                            newVals.Add(body);
                    }

                    if (alreadyProcessed.Contains(r.Key) && newVals.Contains(""))
                        newVals.Remove("");

                    newRS.Add(r.Key, newVals.Distinct().ToArray());
                }

                //search for new Epsilon Productions
                foreach (var r in newRS)
                    if (r.Key != StartSymbol && r.Value.Contains(rChar))
                        ListEpsilonHead.Push(r.Key);
                alreadyProcessed.Add(VarEpsilonHead);
            }

            return newRS;
        }

        RuleSet Chomskey3_UnitRules(RuleSet rs, List<char> newVars) {
            RuleSet newRS = new RuleSet();
            RuleSet workRS = null;
            var Repl = new Stack<char>();
            string[] replaceStrings;

            //find all rules with VAR->Var
            foreach (var r in rs)
                foreach (string body in r.Value)
                    if (body.Length == 1 && newVars.Contains(body[0]))
                        Repl.Push(body[0]);

            if (!Repl.Any())
                return rs;

            while (Repl.Any()) {
                if (workRS == null)
                    workRS = rs;
                else {
                    workRS = newRS;
                    newRS = new RuleSet();
                }
                var var = Repl.Pop();
                replaceStrings = workRS[var];

                foreach (var r in workRS) {
                    var newVals = new List<string>();
                    foreach (string body in r.Value) {
                        if (body == var.ToString()) {
                            newVals.AddRange(replaceStrings);
                        } else
                            newVals.Add(body);
                    }
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

        RuleSet Chomskey4_Length(RuleSet rs, ref List<char> newVars) {
            var newRs = new RuleSet();
            var translate = new Dictionary<string, char>();

            foreach (var r in rs) {
                var newVals = new List<string>();
                foreach (string body in r.Value) {
                    string newbody = body;
                    int replaceStartPos = 0;

                    //translate every new word, with all existing translations
                    foreach (var t in translate) {
                        var oldbody = newbody;
                        if (newbody.Length > 2)
                            newbody = newbody.Replace(t.Key, t.Value.ToString());
                        else
                            break;
                        if (newbody.Length == 1 && newVars.Contains(newbody[0]))
                            newbody = oldbody;
                    }


                    while (newbody.Length > 2) {
                        var replacePart = newbody.Substring(Math.Min(replaceStartPos, newbody.Length - 2), 2);
                        replaceStartPos++;
                        char nL;

                        if (translate.TryGetValue(replacePart, out nL)) { } else {
                            nL = Utils.NextFreeCapitalLetter(newVars.Union(Terminals).ToArray(), 'X');
                            newVars.Add(nL);
                            translate.Add(replacePart, nL);
                        }
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
        // "{(R=>(RRaRRRR,,RRRoRRRR,RoaR,o,RRRRotRR,oRR,R))}"
        // "{U}, {d,e}, {(U=>(UUUUd,UUUUee,,UUUddUdU,e,UUeUe))}, U}"
        // "{Y,N}, {v,d,x,g,l,p,e}, {(N=>(NlYNdYpN,p,dgldvYp,Y,v,dN,YeNYYxegp))(Y=>(NNYg,dpYYxNNN,x,YNYpYYYN,NYplY,,pYNNx,Ye))}, N}"
        // "{(N=>(AlYAdYpA,lYAdYpA,AlYdYpA,lYdYpA,AlYAdYp,lYAdYp,AlYdYp,lYdYp,AlAdYpA,lAdYpA,AldYpA,ldYpA,AlAdYp,lAdYp,AldYp,ldYp,AlYAdpA,lYAdpA,AlYdpA,lYdpA,AlYAdp,lYAdp,AlYdp,lYdp,AlAdpA,lAdpA,AldpA,ldpA,AlAdp,lAdp,Aldp,ldp,p,dgldvYp,dgldvp,AAYg,AYg,Yg,AAg,Ag,g,dpYYxAAA,dpYYxAA,dpYYxA,dpYYx,dpYxAAA,dpYxAA,dpYxA,dpYx,dpxAAA,dpxAA,dpxA,dpx,x,YAYpYYYA,YYpYYYA,YAYpYYY,YYpYYY,AYpYYYA,YpYYYA,AYpYYY,YpYYY,YApYYYA,YApYYY,ApYYYA,pYYYA,ApYYY,pYYY,YAYpYYA,YYpYYA,YAYpYY,YYpYY,AYpYYA,YpYYA,AYpYY,YpYY,YApYYA,YApYY,ApYYA,pYYA,ApYY,pYY,YAYpYA,YYpYA,YAYpY,YYpY,AYpYA,YpYA,AYpY,YpY,YApYA,YApY,ApYA,pYA,ApY,pY,YAYpA,YYpA,YAYp,YYp,AYpA,YpA,AYp,Yp,YApA,YAp,ApA,pA,Ap,AYplY,YplY,AplY,plY,AYpl,Ypl,Apl,pl,pYAAx,pYAx,pYx,pAAx,pAx,px,Ye,e,v,dA,d,YeAYYxegp,YeYYxegp,eAYYxegp,eYYxegp,YeAYxegp,YeYxegp,eAYxegp,eYxegp,YeAxegp,Yexegp,eAxegp,exegp,))(A=>(AlYAdYpA,lYAdYpA,AlYdYpA,lYdYpA,AlYAdYp,lYAdYp,AlYdYp,lYdYp,AlAdYpA,lAdYpA,AldYpA,ldYpA,AlAdYp,lAdYp,AldYp,ldYp,AlYAdpA,lYAdpA,AlYdpA,lYdpA,AlYAdp,lYAdp,AlYdp,lYdp,AlAdpA,lAdpA,AldpA,ldpA,AlAdp,lAdp,Aldp,ldp,p,dgldvYp,dgldvp,AAYg,AYg,Yg,AAg,Ag,g,dpYYxAAA,dpYYxAA,dpYYxA,dpYYx,dpYxAAA,dpYxAA,dpYxA,dpYx,dpxAAA,dpxAA,dpxA,dpx,x,YAYpYYYA,YYpYYYA,YAYpYYY,YYpYYY,AYpYYYA,YpYYYA,AYpYYY,YpYYY,YApYYYA,YApYYY,ApYYYA,pYYYA,ApYYY,pYYY,YAYpYYA,YYpYYA,YAYpYY,YYpYY,AYpYYA,YpYYA,AYpYY,YpYY,YApYYA,YApYY,ApYYA,pYYA,ApYY,pYY,YAYpYA,YYpYA,YAYpY,YYpY,AYpYA,YpYA,AYpY,YpY,YApYA,YApY,ApYA,pYA,ApY,pY,YAYpA,YYpA,YAYp,YYp,AYpA,YpA,AYp,Yp,YApA,YAp,ApA,pA,Ap,AYplY,YplY,AplY,plY,AYpl,Ypl,Apl,pl,pYAAx,pYAx,pYx,pAAx,pAx,px,Ye,e,v,dA,d,YeAYYxegp,YeYYxegp,eAYYxegp,eYYxegp,YeAYxegp,YeYxegp,eAYxegp,eYxegp,YeAxegp,Yexegp,eAxegp,exegp))(Y=>(AAYg,AYg,Yg,AAg,Ag,g,dpYYxAAA,dpYYxAA,dpYYxA,dpYYx,dpYxAAA,dpYxAA,dpYxA,dpYx,dpxAAA,dpxAA,dpxA,dpx,x,YAYpYYYA,YYpYYYA,YAYpYYY,YYpYYY,AYpYYYA,YpYYYA,AYpYYY,YpYYY,YApYYYA,YApYYY,ApYYYA,pYYYA,ApYYY,pYYY,YAYpYYA,YYpYYA,YAYpYY,YYpYY,AYpYYA,YpYYA,AYpYY,YpYY,YApYYA,YApYY,ApYYA,pYYA,ApYY,pYY,YAYpYA,YYpYA,YAYpY,YYpY,AYpYA,YpYA,AYpY,YpY,YApYA,YApY,ApYA,pYA,ApY,pY,YAYpA,YYpA,YAYp,YYp,AYpA,YpA,AYp,Yp,YApA,YAp,ApA,pA,Ap,p,AYplY,YplY,AplY,plY,AYpl,Ypl,Apl,pl,pYAAx,pYAx,pYx,pAAx,pAx,px,Ye,e))}

        RuleSet Chomskey5_Terminals(RuleSet rs, ref List<char> newVars) {
            var newRs = new RuleSet();
            var translate = new Dictionary<char, char>();

            foreach (var r in rs) {
                var newVals = new List<string>();
                foreach (string body in r.Value) {
                    string newBody = body;
                    if (body.Length == 2) {

                        var c2Replaces = new List<char>();
                        if (Terminals.Contains(newBody[0]))
                            c2Replaces.Add(newBody[0]);
                        if (Terminals.Contains(newBody[1]))
                            c2Replaces.Add(newBody[1]);

                        foreach (var c2Replace in c2Replaces) {
                            char nl;
                            if (translate.TryGetValue(c2Replace, out nl)) { } else {
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
            var cPos = new List<int>();
            var strs = new List<string>();

            //save all c occurendes in cPos
            for (int i = 0; i < s.Length; i++)
                if (s[i] == c)
                    cPos.Add(i);

            var ps = Utils.GetPowerSet(cPos.Count);

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
            if (!isChomskey) throw new NotSupportedException("Only supported in Chomskey normalform");

            if (w == "") {
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

        
        CFGrammer Combine(CFGrammer cfg, string[] startBodysFormat, string nameExt) {
            var newRules = new RuleSet();

            List<char> bothVarAndTerm = this.VarAndTerm.Union(cfg.VarAndTerm).ToList();
            List<char> finalVars = this.Variables.ToList();
            List<char> finalTerms= this.Terminals.ToList();
            char newStart = Utils.NextFreeCapitalLetter(bothVarAndTerm, this.StartSymbol);
            
            bothVarAndTerm.Add(newStart);
            finalVars.Add(newStart);

            var translate = new Dictionary<char, char>();
            foreach (char c in cfg.Variables)
            {
                if (!finalVars.Contains(c)) {
                    translate.Add(c,c);
                } else {
                    char newChar = Utils.NextFreeCapitalLetter(finalVars, c);
                    translate.Add(c, newChar);
                    finalVars.Add(newChar);
                }
            }
            foreach (char c in cfg.Terminals)
            {
                if (!finalTerms.Contains(c)) {
                    translate.Add(c,c);
                } else {
                    char newChar = Utils.NextFreeCapitalLetter(finalTerms, c); //against convention!
                    translate.Add(c, newChar);
                    finalTerms.Add(newChar);
                }
            }


            for (int i = 0; i < startBodysFormat.Length; i++)
                startBodysFormat[i] = string.Format(startBodysFormat[i], this.StartSymbol, cfg.StartSymbol, newStart);

            //New Start Rule, build 
            newRules.Add(newStart, startBodysFormat);


            foreach (var r in this.Rules) {
                newRules.Add(r.Key, r.Value);
            }


            foreach (var r in cfg.Rules) {
                var newVals = new List<string>();
                foreach (string body in r.Value) {
                    var sw = new System.Text.StringBuilder();
                    foreach (char c in body) {
                        sw.Append(translate[c]);
                    }
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
            return new CFGrammer("$CFG_KleeneStern({Name})-", this.Variables.Append(newStart).ToArray(), this.Terminals, newRules, newStart);
        }

        public CFGrammer Union(CFGrammer cfg) => Combine(cfg, new string[] {"{0}","{1}"}, "union");

        public CFGrammer Reverse() {
            var newRules = new RuleSet();

            foreach (var r in this.Rules) {
                var newVals = new List<string>();
                foreach (string body in r.Value) {
                    newVals.Add(body.Reverse().ToString());
                }
                newRules.Add(r.Key, newVals.ToArray());
            }
            return new CFGrammer("CFG_Reverse({Name})", this.Variables, this.Terminals, newRules, this.StartSymbol);
        
        }

        public CFGrammer Diff(CFGrammer cfg) => throw new NotSupportedException();
        public CFGrammer Diff(Finite.FABase A) => throw new NotImplementedException();
        public CFGrammer Intersect(CFGrammer cfg) => throw new NotSupportedException();
        public CFGrammer Intersect(Finite.FABase A) => throw new NotImplementedException();
        public CFGrammer Complement() => throw new NotSupportedException();
        public CFGrammer Concat(CFGrammer cfg) => Combine(cfg, new string[] {"{0}{1}"}, "concat");
        public CFGrammer Join(CFGrammer cfg) => Combine(cfg, new string[] {"{0}"}, "join");
        public CFGrammer HomomorphismChar(Dictionary<char,char> translate) => throw new NotImplementedException();

        #endregion
    } //end class
} //end ns