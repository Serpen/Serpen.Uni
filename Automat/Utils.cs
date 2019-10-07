using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat {

    public static class Utils {

        public const char EPSILON = 'ε';
        public static readonly System.Random RND = new System.Random();
        public static int Pow(int bas, int exp) => (int)System.Math.Pow(bas, exp);
        public static int Pow2(int exp) => (int)System.Math.Pow(2, exp);
        public static int Log2(int z) => (int)System.Math.Log2(z);
        public static bool HasBitSet(byte i, int b) => (b & (1 << i)) > 0;

        private static eDebugLogLevel DebugLogLevel = eDebugLogLevel.Always;

        /// <summary>
        /// Get Random Array Elemet
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GrAE<T>(T[] array) => array[RND.Next(0, array.Length)];

        public static bool[,] GetPowerSet(int bitscount) {
            var ret = new bool[Pow2(bitscount), bitscount];

            for (int i = 0; i < ret.GetLength(0); i++) {
                for (byte j = 0; j < ret.GetLength(1); j++) {
                    ret[i, j] = CheckBitSet(i, j);
                }
            }
            return ret;
        }

        public static bool CheckBitSet(int b, int bitNumber)
            => (b & (1 << bitNumber)) > 0;

        public enum eDebugLogLevel { Always, Normal, Verbose }

        internal static void DebugMessage(string message, IAcceptWord a, eDebugLogLevel level) {
            if (DebugLogLevel >= level) {
                var stack = new System.Diagnostics.StackTrace();
                System.Diagnostics.Debug.WriteLine("DBG: " +
                    stack.GetFrame(1).GetMethod().DeclaringType.Name + "." +
                    stack.GetFrame(1).GetMethod().Name +
                    ":" + stack.GetFrame(1).GetILOffset() + " " +
                    (a != null ? "[" + a.Name + "] " : " ") +
                    message);
            }
        }
        internal static void AcceptWordConsoleLine(IAcceptWord A, string w) {
            try {
                System.Console.WriteLine($"{A.Name} accepts '{w}': {A.AcceptWord(w)}");
            } catch (Serpen.Uni.Automat.TuringCycleException e) {
                System.Console.WriteLine($"{A.Name} {e.Message}");
            } catch (Serpen.Uni.Automat.PDAStackException e) {
                System.Console.WriteLine($"{A.Name} {e.Message}");
            }
        }

        internal static bool TestEqualWithWord(IAutomat a1, IAutomat a2, string w) {
            bool ac1 = a1.AcceptWord(w);
            bool ac2 = a2.AcceptWord(w);
            if (ac1 != ac2) {
                Utils.DebugMessage($"word '{w}' is {ac1}:{a1.Name} != {ac2}:{a2.Name}", a1, eDebugLogLevel.Normal);
                return false;
            } else
                return true;
        }

        internal static char NextFreeCapitalLetter(ICollection<char> alphabet, char inputChar) => NextFreeCapitalLetter(alphabet, inputChar, new char[] { });
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
                if (!alphabet.Contains((char)i)) {
                    return (char)i;
                }
            }


            throw new System.ArgumentOutOfRangeException();
        }

        public static T[] RandomizeArray<T>(T[] array) {
            var rnd = Utils.RND;
            for (int i = 0; i < array.Length * 2 / 3; i++) {
                int swapi = rnd.Next(0, array.Length);
                (array[i], array[swapi]) = (array[swapi], array[i]);
            }
            return array;
        }

        internal static Dictionary<T, List<T>> EqualityClasses<T>(T[] array, System.Func<T, T, bool> comparer) {

            var ret = new Dictionary<T, List<T>>();

            ret.Add(array[0], new List<T>(new T[] { array[0] }));

            for (int i = 1; i < array.Length; i++) {
                int eqIndex = -1;
                for (int j = 0; j < ret.Keys.Count; j++) {
                    if (comparer(array[i], ret.Keys.ElementAt(j))) {
                        eqIndex = j;
                        break;
                    }
                }

                if (eqIndex != -1) {
                    ret.ElementAt(eqIndex).Value.Add(array[i]);
                } else {
                    ret.Add(array[i], new List<T>(new T[] { array[i] }));
                }
            }

            return ret;

        } //end function

        public static uint ArrayIndex(System.Array array, object value) {
            uint index = (uint)System.Array.IndexOf(array, value);
            if (index > array.Length)
                throw new System.IndexOutOfRangeException();
            return index;
        }

        public static void SaveAutomatImageToTemp(IAutomat automat)
            => Visualization.DrawAutomat(automat).Save(System.Environment.ExpandEnvironmentVariables($@"%temp%\automat\{automat.Name}.png"));


    } //end class Utils 

    public static class ExtensionMethods {
        public static bool EqualAlphabets(this char[] alphabet, char[] anotherAlphabet) {
            if (alphabet.Length != anotherAlphabet.Length) return false;
            for (int i = 0; i < alphabet.Length; i++)
                if (alphabet[i] != anotherAlphabet[i])
                    return false;
            return true;
        }
    }
}