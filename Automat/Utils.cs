using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat {

    public static class Utils {

        public const char EPSILON = 'ε';

        internal static void AcceptWordConsoleLine(this IAcceptWord A, string w, bool onlytrue = false) {
            try {
                bool accepts = A.AcceptWord(w);
                if (accepts || !onlytrue)
                    System.Console.WriteLine($"{A.Name} accepts |{w.Length}| '{w}': {accepts}");
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


        internal static char NextFreeLowerLetter(IEnumerable<char> alphabet) {
            for (int i = (int)'a'; i < (int)'z' + 1; i++)
                if (!alphabet.Contains((char)i))
                    return (char)i;

            for (int i = (int)'ß'; i < (int)'ÿ' + 1; i++)
                if (char.IsLetter((char)i) && !alphabet.Contains((char)i))
                    return (char)i;

            throw new System.ArgumentOutOfRangeException();

        }
        internal static char NextFreeCapitalLetter(IEnumerable<char> alphabet, char? inputChar) => NextFreeCapitalLetter(alphabet, inputChar, System.Array.Empty<char>());

        internal static char NextFreeCapitalLetter(IEnumerable<char> alphabet, char? inputChar, char[] wishChars) {
            if (inputChar.HasValue) {
                if (!alphabet.Contains(inputChar.ToString().ToUpper()[0]))
                    return inputChar.ToString().ToUpper()[0];
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
            }

            foreach (char w in wishChars) {
                if (!alphabet.Contains(w)) return w;
            }

            for (int i = (int)'A'; i < (int)'Z' + 1; i++) {
                if (!alphabet.Contains((char)i)) {
                    return (char)i;
                }
            }

            for (int i = (int)'Έ'; i < (int)'Ϋ' + 1; i++) {
                if (!char.IsLetter((char)i))
                    continue;
                if (!alphabet.Contains((char)i))
                    return (char)i;
            }

            for (int i = (int)'À'; i < (int)'Ý' + 1; i++) {
                if (!char.IsLetter((char)i))
                    continue;
                if (i == 0xd7 || i == 0xd8)
                    continue;
                if (!alphabet.Contains((char)i))
                    return (char)i;

            }

            throw new System.ArgumentOutOfRangeException();
        }

        public static void SaveAutomatImageToTemp(this IAutomat automat)
            => Visualization.DrawAutomat(automat).Save(System.Environment.ExpandEnvironmentVariables($@"%temp%\automat\{automat.Name}.png"));

        public static void ExportToTemp(this IAcceptWord automat) {
            if (automat is IAutomat a2)
                SaveAutomatImageToTemp(a2);
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var stream = new System.IO.FileStream(System.Environment.ExpandEnvironmentVariables($@"%temp%\automat\{automat.Name}.bin"), System.IO.FileMode.Create, System.IO.FileAccess.Write);
            formatter.Serialize(stream, automat);
            stream.Close();
        }

        internal static T ImportFromTemp<T>(string file) where T : class, IAcceptWord {
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var stream = new System.IO.FileStream(System.Environment.ExpandEnvironmentVariables($@"%temp%\automat\{file}"), System.IO.FileMode.Open);
            var obj = formatter.Deserialize(stream);
            stream.Close();
            return obj as T;
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

        internal static void DebugMessage(string message, Serpen.Uni.Utils.eDebugLogLevel level, params IAcceptWord[] As) {
            if (Serpen.Uni.Utils.DebugLogLevel >= level && System.Diagnostics.Debugger.IsAttached) {
                var stack = new System.Diagnostics.StackTrace(true);
                var sframe = stack.GetFrame(1);
                var smethod = sframe.GetMethod();
                System.Diagnostics.Debug.WriteLine(
                    smethod.DeclaringType.Name + "." +
                    smethod.Name +
                    ":" + sframe.GetFileLineNumber() + " " +
                    (As != null ? "[" + string.Join(",", (from a in As select a.Name)) + "] " : " ") +
                    message);
            }
        }

    } //end class Utils 
}