using Serpen.Uni.Automat.ContextFree;

namespace Serpen.Uni.Automat {
    public static partial class KnownAutomat {
        public static PDA QPDA_1659_A33_K1 {
            get {
                var pdaT = new PDATransform();
                pdaT.Add(0, null, PDA.START, PDA.START.ToString(), 1);
                pdaT.Add(1, 'a', null, "a", 1);
                pdaT.Add(1, null, null, null, 2);
                pdaT.Add(2, 'b', 'a', null, 2);
                pdaT.Add(2, null, PDA.START, null, 3);
                return new StatePDA(nameof(QPDA_1659_A33_K1), 4, new char[] { 'a', 'b' }, new char[] { 'a', PDA.START }, pdaT, 0, PDA.START, new uint[] { 3 });
            }
        }

        public static PDA QPDA_EAFK_A62_wwr_Palindrom {
            get {
                var pdaT = new PDATransform();
                pdaT.Add(0, '0', null, "0", 0);
                pdaT.Add(0, '1', null, "1", 0);
                pdaT.Add(0, null, null, null, 1);
                pdaT.Add(1, '0', '0', null, 1);
                pdaT.Add(1, '1', '1', null, 1);
                pdaT.Add(1, null, PDA.START, null, 2);
                return new StatePDA(nameof(QPDA_EAFK_A62_wwr_Palindrom), 3, binAlp, new char[] { '0', '1', DPDA.START }, pdaT, 0, PDA.START, new uint[] { 2 });
            }
        }

        public static DPDA DPDA_EAFK_A611_wcwr_Palindrom {
            get {
                var pdadT = new DPDATransform();
                pdadT.Add(0, '0', null, "0", 0);
                pdadT.Add(0, '1', null, "1", 0);
                pdadT.Add(0, 'c', '0', "0", 1);
                pdadT.Add(0, 'c', '1', "1", 1);
                pdadT.Add(0, 'c', DPDA.START, DPDA.START.ToString(), 1);
                pdadT.Add(1, '0', '0', null, 1);
                pdadT.Add(1, '1', '1', null, 1);
                pdadT.Add(1, null, DPDA.START, null, 2);
                return new DPDA(nameof(DPDA_EAFK_A611_wcwr_Palindrom), 3, new char[] { '0', '1', 'c' }, new char[] { '0', '1', DPDA.START }, pdadT, 0, DPDA.START, new uint[] { 2 });
            }
        }
        public static StackPDA SPDA_simple_10 {
            get {
                var pdaT = new PDATransform();
                pdaT.Add(0, '1', null, "1", 1);
                pdaT.Add(1, '0', '1', null, 0);
                return new StackPDA(nameof(SPDA_simple_10), 2, binAlp, binAlp, pdaT, 0, (char)0);
            }
        }

        public static StackPDA SPDA_EAFK_B612_T52_ArithmExp {
            get {
                var pdaT = new PDATransform();
                pdaT.Add(new PDATransformKey(0, null, 'I'), new PDATransformValue[] {
                    new PDATransformValue("a", 0),
                    new PDATransformValue("b", 0),
                    new PDATransformValue("Ia", 0),
                    new PDATransformValue("Ib", 0),
                    new PDATransformValue("I0", 0),
                    new PDATransformValue("I1", 0),
                });

                pdaT.Add(new PDATransformKey(0, null, 'E'), new PDATransformValue[] {
                    new PDATransformValue("I", 0),
                    new PDATransformValue("E+E", 0),
                    new PDATransformValue("E*E", 0),
                    new PDATransformValue("(E)", 0),
                });
                
                pdaT.Add(0, 'a', 'a', null, 0);
                pdaT.Add(0, 'b', 'b', null, 0);
                pdaT.Add(0, '0', '0', null, 0);
                pdaT.Add(0, '1', '1', null, 0);
                pdaT.Add(0, '(', '(', null, 0);
                pdaT.Add(0, ')', ')', null, 0);
                pdaT.Add(0, '+', '+', null, 0);
                pdaT.Add(0, '*', '*', null, 0);

                return new StackPDA(nameof(SPDA_EAFK_B612_T52_ArithmExp), 1, new char[] { 'a', 'b', '0', '1', '(', ')', '+', '*' },
                    new char[] { 'a', 'b', '0', '1', '(', ')', '+', '*', 'E', 'I' }, pdaT, 0, 'E');
            }
        }

        public static StatePDA QPDA_simple_10 {
            get {
                var pdaT = new PDATransform();
                pdaT.Add(0, '1', null, "1", 1);
                pdaT.Add(1, '0', '1', null, 0);
                return new StatePDA(nameof(QPDA_simple_10), 2, binAlp, binAlp, pdaT, 0, (char)0, new uint[] { 0 });
            }
        }

        public static CFGrammer CFG_1659_G1 {
            get {
                var rs = new RuleSet();
                rs.Add('S', new string[] { "aSb", "" });
                return new CFGrammer(nameof(CFG_1659_G1), new char[] { 'S' }, new char[] { 'a', 'b' }, rs, 'S');
            }
        }

        public static CFGrammer CFG_1659_B31_G2 {
            get {
                var rs2 = new RuleSet();
                rs2.Add('S', new string[] { "BA" });
                rs2.Add('A', new string[] { "aA", "B" });
                rs2.Add('B', new string[] { "Bbb" });
                return new CFGrammer(nameof(CFG_1659_B31_G2), new char[] { 'S', 'A', 'B' }, new char[] { 'a', 'b' }, rs2, 'S');
                //b^(2n+1)*a^m*b^(2k+1)
            }
        }

        public static CFGrammer CFG_1659_B32_G3 {
            get {
                var rs2 = new RuleSet();
                rs2.Add('S', new string[] { "SS", "[S]", "" });
                return new CFGrammer(nameof(CFG_1659_B32_G3), new char[] { 'S' }, new char[] { '[', ']' }, rs2, 'S');
                //b^(2n+1)*a^m*b^(2k+1)
            }
        }

        public static CFGrammer CFG_1659_T32 {
            get {
                var rs2 = new RuleSet();
                rs2.Add('S', new string[] { "aAbbbBa" });
                rs2.Add('A', new string[] { "aAbb", "" });
                rs2.Add('B', new string[] { "bbBa", "b" });
                return new CFGrammer(nameof(CFG_1659_T32), new char[] { 'S', 'A', 'B' }, new char[] { 'a', 'b' }, rs2, 'S');
                //b^(2n+1)*a^m*b^(2k+1)
            }
        }

        public static CFGrammer CFG_1659_S_G4 {
            get {
                var rs2 = new RuleSet();
                rs2.Add('S', new string[] { "S+S", "S*S", "a" });
                return new CFGrammer(nameof(CFG_1659_S_G4), new char[] { 'S' }, new char[] { 'a', '+', '*' }, rs2, 'S');
                //b^(2n+1)*a^m*b^(2k+1)
            }
        }

        public static CFGrammer CFG_1659_T33_G5 {
            get {
                var rs2 = new RuleSet();
                rs2.Add('S', new string[] { "aXb", "Sab", "X" });
                rs2.Add('X', new string[] { "ab", "ba" });
                return new CFGrammer(nameof(CFG_1659_T33_G5), new char[] { 'S', 'X' }, new char[] { 'a', 'b' }, rs2, 'S');
                //b^(2n+1)*a^m*b^(2k+1)
            }
        }

        public static CFGrammer CFG_1659_P_G6_RG {
            get {
                var rs2 = new RuleSet();
                rs2.Add('S', new string[] { "aS", "aA" });
                rs2.Add('A', new string[] { "bA", "aS", "" });
                return new CFGrammer(nameof(CFG_1659_P_G6_RG), new char[] { 'S', 'A' }, new char[] { 'a', 'b' }, rs2, 'S');
                //b^(2n+1)*a^m*b^(2k+1)
            }
        }


        public static CFGrammer CFG_1659_S88 {
            get {
                var rs = new RuleSet();
                rs.Add('S', new string[] { "AA", "b" });
                rs.Add('A', new string[] { "S", "bAa", "" });
                return new CFGrammer(nameof(CFG_1659_S88), new char[] { 'S', 'A' }, new char[] { 'a', 'b' }, rs, 'S');
            }
        }

        public static CFGrammer CFG_1659_T35 {
            get {
                var rs = new RuleSet();
                rs.Add('S', new string[] { "aSa", "AC" });
                rs.Add('A', new string[] { "ba", "B", "C", "" });
                rs.Add('B', new string[] { "BB", "BA" });
                rs.Add('C', new string[] { "B", "a" });
                return new CFGrammer(nameof(CFG_1659_T35), new char[] { 'S', 'A', 'B', 'C' }, new char[] { 'a', 'b' }, rs, 'S');
            }
        }

        public static CFGrammer CFG_1659_S91 {
            get {
                var rs = new RuleSet();
                rs.Add('S', new string[] { "BB", "AA" });
                rs.Add('A', new string[] { "AB", "AC", "a" });
                rs.Add('B', new string[] { "BA", "CB", "b" });
                rs.Add('C', new string[] { "BC", "c" });
                return new CFGrammer(nameof(CFG_1659_S91), new char[] { 'S', 'A', 'B', 'C' }, new char[] { 'a', 'b', 'c' }, rs, 'S') { isChomskey = true };
            }
        }

        public static CFGrammer CFG_EAFK_B71 {
            get {
                var rs = new RuleSet();
                rs.Add('S', new string[] { "AB", "a" });
                rs.Add('A', new string[] { "b" });
                return new CFGrammer(nameof(CFG_EAFK_B71), new char[] { 'S', 'A', 'B' }, new char[] { 'a', 'b' }, rs, 'S');
            }
        }

        public static CFGrammer CFG_EAFK_B78 {
            get {
                var rs = new RuleSet();
                rs.Add('S', new string[] { "AB" });
                rs.Add('A', new string[] { "aAA", "" });
                rs.Add('B', new string[] { "bBB", "" });
                return new CFGrammer(nameof(CFG_EAFK_B78), new char[] { 'S', 'A', 'B' }, new char[] { 'a', 'b' }, rs, 'S');
            }
        }

        public static CFGrammer CFG_EAFK_B712_Expr {
            get {
                var rs = new RuleSet();
                rs.Add('E', new string[] { "E+T", "T*F", "(E)", "a", "b", "Ia", "Ib", "I0", "I1" });
                rs.Add('T', new string[] { "T*F", "(E)", "a", "b", "Ia", "Ib", "I0", "I1" });
                rs.Add('F', new string[] { "(E)", "a", "b", "Ia", "Ib", "I0", "I1" });
                rs.Add('I', new string[] { "a", "b", "Ia", "Ib", "I0", "I1" });
                return new CFGrammer(nameof(CFG_EAFK_B712_Expr), new char[] { 'E', 'T', 'F', 'I' }, new char[] { 'a', 'b', '0', '1', '+', '*', ')', '(' }, rs, 'E');
            }
        }


        public static CFGrammer CFG_MY_Circular {
            get {
                var rs = new RuleSet();
                rs.Add('S', new string[] { "A", "B" });
                rs.Add('A', new string[] { "B", "BB", "b", "" });
                rs.Add('B', new string[] { "A", "BB", "a" });
                return new CFGrammer(nameof(CFG_MY_Circular), new char[] { 'S', 'A', 'B' }, new char[] { 'a', 'b' }, rs, 'S');
            }
        }

        //"{(R=>(RRaRRRR,,RRRoRRRR,RoaR,o,RRRRotRR,oRR,R))}"
        public static CFGrammer CFG_MY_TermProblem {
            get {
                var rs = new RuleSet();
                rs.Add('R', new string[] { "RRaRRRR", "RRRoRRRR", "RoaR", "o", "RRRRotRR", "oRR", "R", "" });
                // rs.Add('R', new string[] {"RRaRRRR","o","RRRRotRR","R",""});
                return new CFGrammer(nameof(CFG_MY_Circular), new char[] { 'R' }, new char[] { 'a', 'o', 't' }, rs, 'R');
            }
        }
    }
}