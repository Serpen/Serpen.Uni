using System.Numerics;

namespace Serpen.Uni.Security {
    public static class RSA {

        public static (System.Tuple<long, long>, long) generateRSAPair() {
            var rnds = new System.Collections.Generic.List<long>();

            long p = 71, q = 0;

            while (p == 0 || q == 0) {
                rnds.Clear();
                rnds.AddRange(Utils.PseudoRandoms(100, ushort.MaxValue));
                foreach (var num in rnds)
                {
                    if ((p == 0 || q == 0) && PrimeTests.MillerRabinTests(num, 10)) {
                        if (p == 0)
                            p = num;
                        else if (q == 0)
                            q = num;
                        else
                            break;
                    }
                }
            }

            long n = p * q;
            long phiN = (p-1) * (q-1);

            long e = 2;

            while (BigInteger.GreatestCommonDivisor(e, phiN) != 1) {
                e = Utils.PseudoRandoms(1, phiN, 2)[0];
            }

            long k = 1;
            while (((k * phiN + 1) % e) != 0) // || ((k * phiN + 1) / e) == e)
                k++;

            long d = (k * phiN +1) / e;

            return (new System.Tuple<long, long>(n, e), d);
        }

        public static long Encrypt(long s, long e, long n) {
            return ((long)BigInteger.ModPow(s, e, n));       
        }

        public static long Decrypt(long c, long d, long n) {
            return (long)BigInteger.ModPow(c, d, n);
        }
    }
}