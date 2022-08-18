using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni {

    public static class Utils {

        public static readonly System.Random RND = new System.Random();

        public static long[] PseudoRandoms(int count, long maxValue = System.Int64.MaxValue, long minValue = 0) {
            var rndgen = System.Security.Cryptography.RandomNumberGenerator.Create();
            byte[] bytes;

            System.Func<int, long> convertfunc;
            if (maxValue <= System.Byte.MaxValue) {
                bytes = new byte[count];
                convertfunc = (i) => bytes[i];
            } else if (maxValue <= System.Int16.MaxValue) {
                bytes = new byte[2 * count];
                convertfunc = (i) => System.BitConverter.ToInt16(bytes, i * 2);
            } else if (maxValue <= System.Int32.MaxValue) {
                bytes = new byte[4 * count];
                convertfunc = (i) => System.BitConverter.ToInt32(bytes, i * 4);
            } else {
                bytes = new byte[8 * count];
                convertfunc = (i) => System.BitConverter.ToInt64(bytes, i * 8);
            }

            long[] nums = new long[count];

            rndgen.GetBytes(bytes);
            for (int i = 0; i < count; i++) {
                nums[i] = convertfunc(i);
                if (nums[i] == 0) nums[i] = minValue; //dirty fix

                if (minValue >= 0 && nums[i] < 0)
                    nums[i] *= -1;
                while (nums[i] > maxValue) //fix 0, -1
                    nums[i] >>= 1;
                while (nums[i] < minValue)
                    nums[i] <<= 1;
            }

            return nums;
        }

        public static int Pow(this int bas, int exp) => (int)System.Math.Pow(bas, exp);
        public static int Pow2(this int exp) => Pow(2, exp);
        public static int Sqrt(this int exp) => (int)(System.Math.Sqrt(exp));
        public static int Log(this int z, int bas) => (int)(System.Math.Log(z, bas) + .5);
        public static int Log2(this int z) => Log(z, 2);

        public static bool HasBitSet(this int num, byte bit) {
            if (bit > 31)
                throw new System.ArgumentOutOfRangeException(nameof(bit));
            return (num & (1 << bit)) != 0;
        }
        public static int SetBit(this int num, byte bit) {
            if (bit > 31)
                throw new System.ArgumentOutOfRangeException(nameof(bit));
            return num | (1 << bit);
        }
        public static int UnSetBit(this int num, byte bit) {
            if (bit > 31)
                throw new System.ArgumentOutOfRangeException(nameof(bit));
            return num | (~(1 << bit));
        }

        public static bool[,] GetPowerSet(int bitscount, int additionalCols = 0) {
            var ret = new bool[Pow2(bitscount), bitscount + additionalCols];

            for (int i = 0; i < ret.GetLength(0); i++)
                for (byte j = 0; j < bitscount; j++)
                    ret[i, j] = i.HasBitSet(j);
            return ret;
        }

        public static IEnumerable<IEnumerable<T>> GetPowerSet<T>(this IReadOnlyList<T> list) {
            var ret = new List<List<T>>();

            int count = Utils.Pow2(list.Count);

            for (int i = 0; i < count; i++) {
                ret.Add(new List<T>());
                for (byte j = 0; j < count; j++)
                    if (i.HasBitSet(j))
                        ret[i].Add(list[j]);
            }
            return ret;
        }

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length) {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        public static bool[] ToBoolArray(this int integer, int minLen) {
            bool[] ret = new bool[System.Math.Max(Log2(integer), minLen)];
            for (byte i = 0; i < ret.Length; i++)
                ret[i] = HasBitSet((byte)integer, i);
            return ret;
        }
        internal enum eDebugLogLevel { Always, Normal, Verbose }
        internal static eDebugLogLevel DebugLogLevel = eDebugLogLevel.Normal;

        [System.Diagnostics.DebuggerStepThrough()]
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

        public static (int, int) CantorIndexToIndex(int i) {
            int j = (int)System.Math.Floor(System.Math.Sqrt(0.25 + 2 * i) - 0.5);
            int y = i - j * (j + 1) / 2;
            return (j - y, y);
        }

        public static int IndexToCantorIndex(int r, int c) => (r + c) * (r + c + 1) / 2 + c;

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

            var ret = new Dictionary<T, T[]>();
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
                        sb.Append($"{array[r, c].ToString().PadLeft(2)}, ");
                    else
                        sb.Append("".ToString().PadLeft(2));
                sb.AppendLine();
            }
            return sb.ToString();
        }

        [AlgorithmSource("HD FEdS P117 D221")]
        public static byte HammingDistance(long val1, long val2) {
            byte ret = 0;
            for (byte i = 0; i < 64; i++) {
                long shift = 1L << i;
                if ((val1 & shift) != (val2 & shift))
                    ret++;
            }
            return ret;
        }

        public static bool ContainsSubset<T>(this IEnumerable<T> supset, IEnumerable<T> subset) {
            return subset.All(x => supset.Contains(x));
        }

        public static string asHex(this long num, bool dots = true, int fillup = 0, bool withPrefix = true) {
            var s = num.ToString("X").PadLeft(fillup, '0');
            if (dots)
                for (int i = s.Length - 1; i >= 0; i -= 4)
                    s = s.Insert(i, ".");

            return (withPrefix ? "0x" : "") + s;
        }

        public static string asBin(this long num, bool dots = true, int fillup = 0, bool withPrefix = true) {
            var s = System.Convert.ToString(num, 2).PadLeft(fillup, '0');

            if (dots)
                for (int i = s.Length - 1; i >= 0; i -= 4)
                    s = s.Insert(i, ".");

            return (withPrefix ? "0b" : "") + s;
        }

    } //end class Utils 
}