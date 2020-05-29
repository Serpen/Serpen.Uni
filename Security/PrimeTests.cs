using System.Numerics;

namespace Serpen.Uni.Security {
    public static class PrimeTests {
        public static bool MillerRabinTest(long number, long a) {
            if (number % 2 == 0)
                return false;
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

            int d = (int)(number - 1) / Serpen.Uni.Utils.Pow2(j);

            if (BigInteger.ModPow(a, d, number) == 1)
                return true;

            for (int r = j; r >= 0; r--)
                if (BigInteger.ModPow(a, d * Serpen.Uni.Utils.Pow2(r), number) == 1)
                    return true;

            return false;
        }

        public static bool MillerRabinTests(int number) {
            int[] rndnums = Serpen.Uni.Utils.PseudoRandoms(10, number-2, 2);
            for (int i = 0; i < 10; i++) {
                if (!MillerRabinTest(number, rndnums[i]))
                    return false;

            }
            return true;
        }

    }
}