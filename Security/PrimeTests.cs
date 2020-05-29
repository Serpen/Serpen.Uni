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

        public static bool mrt(int n, int a) { // n ungerade, 1 < a < n-1
            int m = n - 1;
            int d = m >> 1, e = 1;
            while ((d & 1) == 0) {d >>= 1; ++e;}
            int p = a, q = a;
            while ((d >>= 1) != 0) { // potenziere modular: p = a^d mod n
                q *= q; q %= n; // quadriere modular: q = q^2 mod n
                if ((d & 1) != 0) {p *= q; p %= n;} // multipliziere modular: p = (p * q) mod n
            }
            if (p == 1 || p == m) return true; // n ist wahrscheinlich prim
            while (--e != 0) {
                p *= p; p %= n;
                if (p == m) return true;
                if (p <= 1) break;
            }
            return false; // n ist nicht prim
        }

    }
}