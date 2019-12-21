using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni {

    public static class Utils {

        public static readonly System.Random RND = new System.Random();
        public static int Pow(int bas, int exp) => (int)System.Math.Pow(bas, exp);
        public static int Pow2(int exp) => (int)System.Math.Pow(2, exp);
        public static int Sqrt(int exp) => (int)System.Math.Sqrt(exp);
        public static int Log2(int z) => (int)System.Math.Log2(z);
        public static bool HasBitSet(this int num, byte bit) {
            if (bit > 31)
                throw new System.ArgumentOutOfRangeException(nameof(bit));
            return (num & (1 << bit)) != 0;
        } 

        public static bool[,] GetPowerSet(int bitscount, int additionalCols = 0) {
            var ret = new bool[Pow2(bitscount), bitscount + additionalCols];

            for (int i = 0; i < ret.GetLength(0); i++)
                for (byte j = 0; j < bitscount; j++)
                    ret[i, j] = i.HasBitSet(j);
            return ret;
        }

        public static bool[] ToBoolArray(this int integer, int minLen) {
            bool[] ret = new bool[System.Math.Max(Log2(integer) + 1, minLen)];
            for (byte i = 0; i < ret.Length; i++)
                ret[i] = HasBitSet((byte)integer, i);
            return ret;
        }
        internal enum eDebugLogLevel { Always, Normal, Verbose }
        internal static eDebugLogLevel DebugLogLevel = eDebugLogLevel.Normal;

        internal static void DebugMessage(string message, eDebugLogLevel level) {
            if (DebugLogLevel >= level && System.Diagnostics.Debugger.IsAttached) {
                var stack = new System.Diagnostics.StackTrace(true);
                var sframe = stack.GetFrame(1);
                var smethod = sframe.GetMethod();
                System.Diagnostics.Debug.WriteLine(
                    smethod.DeclaringType.Name + "." +
                    smethod.Name +
                    ":" + sframe.GetFileLineNumber() + " " +
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

        internal static Dictionary<T, T[]> EqualityClasses<T>(T[] array, System.Func<T, T, bool> comparer) {

            var ret2 = new Dictionary<T, List<T>>(array.Length) {
                { array[0], new List<T>(new T[] { array[0] }) }
            };

            for (int i = 1; i < array.Length; i++) {
                int eqIndex = -1;
                for (int j = 0; j < ret2.Keys.Count; j++) {
                    if (comparer(array[i], ret2.Keys.ElementAt(j))) {
                        eqIndex = j;
                        break;
                    }
                }

                if (eqIndex != -1) {
                    ret2.ElementAt(eqIndex).Value.Add(array[i]);
                } else {
                    ret2.Add(array[i], new List<T>(new T[] { array[i] }));
                }
            }

            var ret = new Dictionary<T,T[]>();
            foreach (var item in ret2)
                ret.Add(item.Key, item.Value.ToArray());

            return ret;

        } //end function

        public static uint ArrayIndex(this System.Array array, object value) {
            uint index = (uint)System.Array.IndexOf(array, value);
            if (index > array.Length)
                throw new System.IndexOutOfRangeException();
            return index;
        }

        public static T RndElement<T>(this T[] array) => array[Utils.RND.Next(0, array.Length)];

        public static T[,] RemoveArrayCol<T>(this T[,] table, int column) {
            var newTable = new T[table.GetLength(0), table.GetLength(1) - 1];
            int indexModifier = 0;
            for (int c = 0; c < newTable.GetLength(1); c++) {
                if (c == column)
                    indexModifier++;
                for (int r = 0; r < newTable.GetLength(0); r++)
                    newTable[r, c] = table[r, c + indexModifier];
            }
            return newTable;
        }

        public static T[,] RemoveArrayRow<T>(this T[,] table, int row) {
            var newTable = new T[table.GetLength(0) - 1, table.GetLength(1)];
            int indexModifier = 0;
            for (int r = 0; r < newTable.GetLength(0); r++) {
                if (r == row)
                    indexModifier++;
                for (int c = 0; c < newTable.GetLength(1); c++)
                    newTable[r, c] = table[r + indexModifier, c];
            }
            return newTable;
        }

        public static string FormatArray<T>(T[,] array) {
            var sb = new System.Text.StringBuilder();
            for (int r = 0; r < array.GetLength(0); r++) {
                for (int c = 0; c < array.GetLength(1); c++)
                    if (array[r, c] != null)
                        sb.Append($"{array[r, c].ToString().PadLeft(2) }, ");
                    else
                        sb.Append("".ToString().PadLeft(2));
                sb.AppendLine();
            }
            return sb.ToString();
        }

    } //end class Utils 
}