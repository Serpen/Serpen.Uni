using System.Collections.Generic;

namespace Serpen.Uni.Automat {

    public static class Utils {

        public const char EPSILON = 'ε';

        internal static void AcceptWordConsoleLine(this IAcceptWord A, string w) {
            try {
                System.Console.WriteLine($"{A.Name} accepts |{w.Length}| '{w}': {A.AcceptWord(w)}");
            } catch (Turing.TuringCycleException e) {
                System.Console.WriteLine($"{A.Name} {e.Message}");
            } catch (ContextFree.PDAStackException e) {
                System.Console.WriteLine($"{A.Name} {e.Message}");
            }
        }

        internal static bool TestEqualWithWord(IAutomat a1, IAutomat a2, string w) {
            bool ac1 = a1.AcceptWord(w);
            bool ac2 = a2.AcceptWord(w);
            if (ac1 != ac2) {
                Utils.DebugMessage($"word '{w}' is {ac1}:{a1.Name} != {ac2}:{a2.Name}", a1, Uni.Utils.eDebugLogLevel.Normal);
                return false;
            } else
                return true;
        }

        internal static char NextFreeCapitalLetter(ICollection<char> alphabet, char inputChar) => NextFreeCapitalLetter(alphabet, inputChar, System.Array.Empty<char>());
        internal static char NextFreeCapitalLetter(ICollection<char> alphabet, char inputChar, char[] wishChars) {
            if (inputChar == 'S' & !alphabet.Contains('Ŝ')) return 'Ŝ';
            if (inputChar == 'A' & !alphabet.Contains('Â')) return 'Â';
            if (inputChar == 'B' & !alphabet.Contains('Ɓ')) return 'Ɓ';
            if (inputChar == 'C' & !alphabet.Contains('Ĉ')) return 'Ĉ';
            if (inputChar == 'D' & !alphabet.Contains('Ď')) return 'Ď';
            if (inputChar == 'R' & !alphabet.Contains('Ř')) return 'Ř';
            if (inputChar == '0' & !alphabet.Contains('O')) return 'O';
            if (inputChar == '1' & !alphabet.Contains('L')) return 'L';
            if (inputChar == '+' & !alphabet.Contains('P')) return 'P';
            if (inputChar == '*' & !alphabet.Contains('M')) return 'M';
            if (!alphabet.Contains(inputChar.ToString().ToUpper()[0]))
                return inputChar.ToString().ToUpper()[0];

            foreach (char w in wishChars) {
                if (!alphabet.Contains(w)) return w;
            }

            for (int i = (int)'A'; i < (int)'Z' + 1; i++) {
                if (!alphabet.Contains((char)i)) {
                    return (char)i;
                }
            }

            for (int i = (int)'Έ'; i < (int)'Ϋ' + 1; i++) {
                if (i == 0x03a2 || i == 907 | i == 909)
                    continue;
                if (!alphabet.Contains((char)i))
                    return (char)i;
            }

            throw new System.ArgumentOutOfRangeException();
        }
        
        public static void SaveAutomatImageToTemp(this IAutomat automat)
            => Visualization.DrawAutomat(automat).Save(System.Environment.ExpandEnvironmentVariables($@"%temp%\automat\{automat.Name}.png"));
        
        public static void ExportToTemp(this IAutomat automat) {
            SaveAutomatImageToTemp(automat);
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var stream = new System.IO.FileStream(System.Environment.ExpandEnvironmentVariables($@"%temp%\automat\{automat.Name}.bin"), System.IO.FileMode.Create, System.IO.FileAccess.Write);
            formatter.Serialize(stream, automat);
            stream.Close();
        }

        internal static void DebugMessage(string message, IAcceptWord a, Serpen.Uni.Utils.eDebugLogLevel level) {
            if (Serpen.Uni.Utils.DebugLogLevel >= level && System.Diagnostics.Debugger.IsAttached) {
                var stack = new System.Diagnostics.StackTrace(true);
                var sframe = stack.GetFrame(1);
                var smethod = sframe.GetMethod();
                System.Diagnostics.Debug.WriteLine(
                    smethod.DeclaringType.Name + "." +
                    smethod.Name +
                    ":" + sframe.GetFileLineNumber() + " " +
                    (a != null ? "[" + a.Name + "] " : " ") +
                    message);
            }
        }

    } //end class Utils 
}