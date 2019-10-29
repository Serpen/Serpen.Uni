using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni {

    public static class Utils {

        public const char EPSILON = 'ε';
        public static readonly System.Random RND = new System.Random();
        public static int Pow(int bas, int exp) => (int)System.Math.Pow(bas, exp);
        public static int Pow2(int exp) => (int)System.Math.Pow(2, exp);
        public static int Log2(int z) => (int)System.Math.Log2(z);
        public static bool HasBitSet(this byte i, int b) => (b & (1 << i)) > 0;

        private static eDebugLogLevel DebugLogLevel = eDebugLogLevel.Always;

        /// <summary>
        /// Get Random Array Elemet
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>

        public static bool[,] GetPowerSet(int bitscount) {
            var ret = new bool[Pow2(bitscount), bitscount];

            for (int i = 0; i < ret.GetLength(0); i++) {
                for (byte j = 0; j < ret.GetLength(1); j++) {
                    ret[i, j] = CheckBitSet(i, j);
                }
            }
            return ret;
        }

        static bool CheckBitSet(this int b, int bitNumber) => (b & (1 << bitNumber)) > 0;

        public enum eDebugLogLevel { Always, Normal, Verbose }

        internal static void DebugMessage(string message, Automat.IAcceptWord a, eDebugLogLevel level) {
            if (DebugLogLevel >= level && System.Diagnostics.Debugger.IsAttached) {
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
    
        public static T[] Randomize<T>(this T[] array) {
            var rnd = Uni.Utils.RND;
            for (int i = 0; i < array.Length * 2 / 3; i++) {
                int swapi = rnd.Next(0, array.Length);
                (array[i], array[swapi]) = (array[swapi], array[i]);
            }
            return array;
        }

        internal static Dictionary<T, List<T>> EqualityClasses<T>(T[] array, System.Func<T, T, bool> comparer) {

            var ret = new Dictionary<T, List<T>>(array.Length);

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

        public static uint ArrayIndex(this System.Array array, object value) {
            uint index = (uint)System.Array.IndexOf(array, value);
            if (index > array.Length)
                throw new System.IndexOutOfRangeException();
            return index;
        }

        public static T RndElement<T>(this T[] array) => array[Utils.RND.Next(0, array.Length)];

    } //end class Utils 
}