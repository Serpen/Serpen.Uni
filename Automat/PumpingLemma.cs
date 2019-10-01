namespace Serpen.Uni.Automat {
    public class PumpingLemma {


        public static bool TestPumpbar(IAutomat Automat, int pumpLaenge) {
            // string w = Automat.GetRandomWord();
            // pumpLaenge = w.Length;
            for (int l = 1; l < 10; l++) {
                foreach (string w in Automat.GetRandomWords(10000, 10 * l)) {
                    for (int i = 0; i < w.Length; i++) {
                        for (int j = 0; j < w.Length - i; j++) {
                            string x = w.Substring(0, i);
                            string y = w.Substring(i, j);
                            string z = w.Substring(i + j, w.Length - i - j);
                            // System.Console.WriteLine($"{x} {y} {z}".Trim());

                            Utils.DebugMessage($"word {w}=({x}.{y}.{z})", Automat, Utils.eDebugLogLevel.Verbose);

                            if (!(w.Length >= pumpLaenge)) {
                                Utils.DebugMessage($"word |w={w}|>={pumpLaenge}", Automat, Utils.eDebugLogLevel.Verbose);
                                continue; // throw new PumpingException($"|w|={w.Length}<{pumpLaenge}");
                            }
                            if (!((x + y).Length <= pumpLaenge)) {
                                Utils.DebugMessage($"word |x+y={x.Length}+{y.Length}|<={pumpLaenge}", Automat, Utils.eDebugLogLevel.Verbose);
                                continue; // throw new PumpingException($"|xy|={(x + y).Length}>{pumpLaenge}");
                            }
                            if (y.Length == 0) {
                                Utils.DebugMessage($"word |y={y}|=0", Automat, Utils.eDebugLogLevel.Verbose);
                                continue; // throw new PumpingException($"|y|=0, y={Utils.EPSILON}");
                            }

                            bool first = Automat.AcceptWord(w);
                            if (first) {
                                bool second = false;

                                for (int k = 0; k < 10; k++) {
                                    int r = Utils.RND.Next(1, 100);
                                    string wi = x + string.Concat(System.Linq.Enumerable.Repeat(y, r)) + z;
                                    second = Automat.AcceptWord(wi);
                                    Utils.DebugMessage($"word {wi}: {second}", Automat, Utils.eDebugLogLevel.Verbose);
                                    if (!second)
                                        break;
                                }

                                if (first && second) {
                                    // Utils.DebugMessage($"{w} {pumpLaenge}-pumpbar for {x}.{y}.{z}", Automat, Utils.eDebugLogLevel.Normal);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
    }

    public sealed class PumpingException : System.ApplicationException {
        public PumpingException(string message) : base(message) { }
    }
}