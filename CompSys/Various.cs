namespace Serpen.Uni.CompSys {
    public static class GrayCode {
        public static byte DualToGray(byte dual) {
            return (byte)(dual ^ dual >> 1);
        }

        public static bool[] DualToGray(bool[] dual) {
            byte number = 0;
            for (int i = dual.Length - 1; i >= 0; i--)
                number += (byte)(dual[i] ? Utils.Pow2(i) : 0);

            byte gray = DualToGray(number);
            bool[] numbool = new bool[Utils.Log2(number)];
            for (byte i = 0; i < numbool.Length; i++)
                numbool[i] = Utils.HasBitSet(number, i);
            return numbool;
        }

    }
}