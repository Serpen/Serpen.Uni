using System.Linq;

namespace Serpen.Uni.Automat {
    public static class PumpingLemma {


        //should return certificate instead of true/false, cover more possible outcomes 
        /// <returns>bool empiric _guess_ if its pumpbar</returns>
        public static PumpResult TestPumpbar(IAutomat Automat, int pumpLaenge, int words = 200, int maxWordLen = 50, int exponentMax = 20, int exponentCount = 10) {
            int allAcceptedWordsPass = 0;

            if (Automat.AcceptedStates.Length == 0)
                return PumpResult.NoAcceptedWordExists;

            bool foundAtLeastOneAcceptedWordForTest = false;
            foreach (string w in Automat.GetRandomWords(words, pumpLaenge, maxWordLen)) {
                if (Automat.AcceptWord(w)) {
                    foundAtLeastOneAcceptedWordForTest = true;
                    bool foundAccepted = false;
                    allAcceptedWordsPass = 0;
                    foreach (string[] strParts in StringParts3(w)) {

                        if (!(w.Length >= pumpLaenge)) {
                            Uni.Utils.DebugMessage($"word |w={w}|>={pumpLaenge}", Automat, Uni.Utils.eDebugLogLevel.Verbose);
                            continue; // throw new PumpingException($"|w|={w.Length}<{pumpLaenge}");
                        }

                        string x = strParts[0];
                        string y = strParts[1];
                        string z = strParts[2];

                        Uni.Utils.DebugMessage($"word {w}=({x}.{y}.{z})", Automat, Uni.Utils.eDebugLogLevel.Verbose);
                        if (!((x + y).Length <= pumpLaenge)) {
                            Uni.Utils.DebugMessage($"word |x+y={x.Length}+{y.Length}|<={pumpLaenge}", Automat, Uni.Utils.eDebugLogLevel.Verbose);
                            continue; // throw new PumpingException($"|xy|={(x + y).Length}>{pumpLaenge}");
                        }
                        if (y.Length == 0) {
                            Uni.Utils.DebugMessage($"word |y={y}|=0", Automat, Uni.Utils.eDebugLogLevel.Verbose);
                            continue; // throw new PumpingException($"|y|=0, y={Utils.EPSILON}");
                        }


                        for (int k = 0; k < exponentCount; k++) {
                            int r = Uni.Utils.RND.Next(1, exponentMax);
                            string wi = x + string.Concat(System.Linq.Enumerable.Repeat(y, r)) + z;
                            foundAccepted = Automat.AcceptWord(wi);
                            Uni.Utils.DebugMessage($"word {wi}: {foundAccepted}", Automat, Uni.Utils.eDebugLogLevel.Verbose);
                            if (foundAccepted) {
                                Uni.Utils.DebugMessage($"{w} {pumpLaenge}-pumpbar for {x}.{y}.{z}", Automat, Uni.Utils.eDebugLogLevel.Verbose);
                                allAcceptedWordsPass++;
                            }
                        }
                        if (allAcceptedWordsPass >= exponentCount)
                            break;
                    } //next strParts
                    if (allAcceptedWordsPass < exponentCount)
                        return PumpResult.NotPumpable;
                } //endif AcceptWord
            } //next w
            if (!foundAtLeastOneAcceptedWordForTest)
                return PumpResult.NoAcceptedWordFound;

            if (allAcceptedWordsPass > 0) {
                Uni.Utils.DebugMessage($"{pumpLaenge}-pumpbar for |{allAcceptedWordsPass}| words", Automat, Uni.Utils.eDebugLogLevel.Verbose);
                return PumpResult.Pumpable;
            } else
                Uni.Utils.DebugMessage($"not {pumpLaenge}-pumpbar", Automat, Uni.Utils.eDebugLogLevel.Verbose);
            return PumpResult.NotPumpable;
        }

        static string[][] StringParts3(string w) {
            var words = new System.Collections.Generic.List<string[]>(w.Length*w.Length);
            for (int i = 0; i <= w.Length; i++) {
                for (int j = i; j <= w.Length; j++) {
                    string[] parts = new string[3];
                    parts[0] = w.Substring(0, i);
                    parts[1] = w.Substring(i, j - i);
                    parts[2] = w.Substring(j, w.Length - j);
                    words.Add(parts);
                }
            }
            return words.Distinct().ToArray();
        }

        static string[][] StringParts5(string w) {
            var words = new System.Collections.Generic.List<string[]>(w.Length*w.Length);
            for (int i = 0; i <= w.Length; i++) {
                for (int j = i; j <= w.Length; j++) {
                    for (int k = j; k <= w.Length; k++) {
                        for (int l = k; l <= w.Length; l++) {
                            string[] parts = new string[5];
                            parts[0] = w.Substring(0, i);
                            parts[1] = w.Substring(i, j - i);
                            parts[2] = w.Substring(j, k - j);
                            parts[3] = w.Substring(k, l - k);
                            parts[4] = w.Substring(l, w.Length - l);
                            words.Add(parts);
                        }
                    }
                }
            }
            return words.Distinct().ToArray();
        }
    }

    public enum PumpResult {
        Unknown, Pumpable, NotPumpable, NoAcceptedWordFound, NoAcceptedWordExists
    }

    public sealed class PumpingException : System.ApplicationException {
        public PumpingException(string message) : base(message) { }
    }
}