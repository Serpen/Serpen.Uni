using System;

namespace Serpen.Uni.Compiler {
    public static class Utils {
        public static void WriteGrammerFirstSets(this Serpen.Uni.Automat.ContextFree.CFGrammer g) {
            System.Console.WriteLine(g);
            var first = new FirstSet(g);
            foreach (var Var in g.Variables)
                System.Console.WriteLine($"First({Var}) = " + String.Join(',', first.SetFor(Var)));
        }

        public static void WriteGrammerClosures(this Serpen.Uni.Automat.ContextFree.CFGrammer g) {
            System.Console.WriteLine(g);
            var lr0c = new LR0Closure(g, g.StartSymbol);
            System.Console.WriteLine($"Closure0({g.StartSymbol}) = {{" + String.Join(',', lr0c.ToString()) + "}");
            foreach (var item in g.VarAndTerm) {
                var gotoclosure = new LR0Closure(g, lr0c, item);
                if (gotoclosure.Count > 0)
                    System.Console.WriteLine($"GotoClosure0({item}) = {{" + String.Join(',', gotoclosure.ToString()) + "}");
                foreach (var gotoc in gotoclosure) {
                    foreach (var Var in g.VarAndTerm) {
                        var gotogoto = new LR0Closure(g, gotoclosure, Var);
                        if (gotogoto.Count > 0)
                            System.Console.WriteLine($" GotoClosure0({Var}) = {{" + String.Join(',', gotogoto.ToString()) + "}");
                    }
                }
            }
        }
    }
}