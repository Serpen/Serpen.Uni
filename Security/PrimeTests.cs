using System.Numerics;

namespace Serpen.Uni.Security {
    public static class PrimeTests {
        public static bool MillerRabinTest(long number, long a) {
            if (number % 2 == 0)
                return false;
            if (number == 2 || number == 3 || number == 5) return true;
            if (number == 0 || number == 1) return false;

            if (a < 2 || a > number - 2)
                throw new System.ArgumentOutOfRangeException(nameof(a));
            if (number < 5)
                throw new System.ArgumentOutOfRangeException(nameof(number));

            long n1 = number - 1;
            int j = 0;

            while (n1 % 2 == 0) {
                j++;
                n1 /= 2;
            }

            long d = (number - 1) / Serpen.Uni.Utils.Pow2(j);

            if (BigInteger.ModPow(a, d, number) == 1)
                return true;

            for (int r = j; r > 0; r--)
                if (BigInteger.ModPow(a, d * Serpen.Uni.Utils.Pow2(r), number) == 1)
                    return true;

            return false;
        }

        public static bool MillerRabinTests(long number, int randoms = 10) {
            if (number == 2 || number == 3) return true;
            if (number == 0 || number == 1) return false;
            long[] rndnums = Serpen.Uni.Utils.PseudoRandoms(randoms, number - 2, 2);
            for (int i = 0; i < rndnums.Length; i++) {
                if (!MillerRabinTest(number, rndnums[i]))
                    return false;

            }
            return true;
        }

        public static bool NaiveSimplePrimeTest(long number) {
            long max = (long)System.Math.Sqrt(number);
            if (number % 2 == 0)
                return false;

            for (int i = 3; i <= max; i += 2) // *5 not needed
                if (number % i == 0)
                    return false;

            return true;
        }

        public static long[] generatePrimes(byte count, int maxValue = ushort.MaxValue) {
            var rnds = new System.Collections.Generic.List<long>();
            var p = new long[count];
            byte found = 0;

            if (maxValue / count < System.Math.Log(maxValue))
                throw new System.ArgumentOutOfRangeException();

            while (found < count) {
                rnds.AddRange(Utils.PseudoRandoms(100, maxValue));
                foreach (var num in rnds)
                {
                    if (found < count && PrimeTests.MillerRabinTests(num)) {
                        p[found] = num;
                        found++;
                    }
                }
                rnds.Clear();
            }
            return p;
        }
    }
}