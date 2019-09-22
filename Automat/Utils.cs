using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Automat {


    public static class Utils {

        public const char EPSILON = 'ε';
        public static readonly System.Random RND = new System.Random();
        public static int Pow(int bas, int exp) => (int)System.Math.Pow(bas, exp);
        public static int Pow2(int exp) => (int)System.Math.Pow(2, exp);
        public static int Log2(int z) => (int)System.Math.Log2(z);
        public static bool HasBitSet(byte i, int b) {
            return (b & (1 << i)) > 0;
        }

        /// <summary>
        /// Get Random Array Elemet
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GrAE<T>(T[] array) {
            return array[RND.Next(0, array.Length)];
        }

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

        internal static bool SameAlphabet(IAutomat A1, IAutomat A2) {
            if (A1.Alphabet.Length != A2.Alphabet.Length) return false;
            for (int i = 0; i < A1.Alphabet.Length; i++)
                if (A1.Alphabet[i] != A2.Alphabet[i])
                    return false;
            return true;
        }

        internal static void DebugMessage(string message) {
            var stack = new System.Diagnostics.StackTrace();
            System.Diagnostics.Debug.WriteLine("DBG: " + 
                stack.GetFrame(1).GetMethod().DeclaringType.Name + "." +
                stack.GetFrame(1).GetMethod().Name + 
                ":" + stack.GetFrame(1).GetILOffset() + " " +
                message);
        }

        internal static void AcceptWordConsoleLine(IAutomat A, string w) {
            try {
                System.Console.WriteLine($"{A.Name} accepts '{w}': {A.AcceptWord(w)}");
            } catch (Serpen.Uni.Automat.TuringCycleException e) {
                System.Console.WriteLine($"{A.Name} {e.Message}");
            }
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

        public static uint ArrayIndex(System.Array array, object value) 
            => (uint)System.Array.IndexOf(array, value);

    } //end class Utils 


}