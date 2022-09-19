using System.Numerics;

namespace Serpen.Uni.Security {
    public static class RSA {

        public struct RsaParams {
            public long n;
            public long e;
            public long d;
        }
        public static RsaParams generateRSAPair(long defp = 0, long defq = 0, long defe = 2) {
            var rnds = new System.Collections.Generic.List<long>();

            var ps = PrimeTests.generatePrimes(2, short.MaxValue);
            long p = ps[0], q = ps[1];
            if (defp > 0) p = defp;
            if (defq > 0) q = defq;

            long n = p * q;
            long phiN = (p - 1) * (q - 1);

            long e = 2;

            while (BigInteger.GreatestCommonDivisor(e, phiN) != 1) {
                e = Utils.PseudoRandoms(1, phiN, 2)[0];
            }

            if (defe > 0) e = defe;

            long k = 1;
            while (((k * phiN + 1) % e) != 0) // || ((k * phiN + 1) / e) == e)
                k++;

            long d = (k * phiN + 1) / e;

            return new RsaParams() { n = n, d = d, e = e };
        }

        public static long Encrypt(long s, long e, long n) {
            return ((long)BigInteger.ModPow(s, e, n));
        }

        public static long Decrypt(long c, long d, long n) {
            return (long)BigInteger.ModPow(c, d, n);
        }
    }
}