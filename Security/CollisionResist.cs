namespace Serpen.Uni.Security {
    public class CollisionResist {
        public static int CalcCollisionResist(int l) {
            int n = l / 2;
            double result = 1;
            while (result > .5) {
                long of = 1;
                for (int i = l; i > l - n + 1; i--)
                    of *= i;
                result = 1 - (of / System.Math.Pow(l, n));
                n--;
            }
            return n + 1;
        }
    }
}