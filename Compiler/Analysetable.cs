namespace Serpen.Uni.Compiler {

    public class AnalyseTable {

        public readonly string[,] table;
        public AnalyseTable(Automat.GrammerBase grammer) {
            table = new string[grammer.Variables.Length, grammer.Terminals.Length];

            var D = Steuermengen(grammer);

            for (int r = 0; r < grammer.Variables.Length; r++) {
                for (int c = 0; c < grammer.Terminals.Length; c++) {

                }
            }
            throw new System.NotImplementedException();

        }

        static string[] Steuermengen(Automat.GrammerBase grammer) {
            throw new System.NotImplementedException();
        }
    }
}