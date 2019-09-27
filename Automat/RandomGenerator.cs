using System.Collections.Generic;
using System.Linq;


namespace Serpen.Uni.Automat
{
    internal static class RandomGenerator {

        public static System.Random rnd = Utils.RND;

        public static char[] RandomAlphabet(int min, int max) => RandomAlphabet(min, max, new char[] {}, null);
        public static char[] RandomAlphabet(int min, int max, IEnumerable<char> addChar, System.Index? ind) {
            int alphabetCount = rnd.Next(min, max);
            var alphabet = new List<char>(alphabetCount);

            for (int i = 0; i < alphabetCount; i++)
                alphabet.Add((char)rnd.Next('a', 'z'));
            
                alphabet.AddRange(addChar);

            return alphabet.Distinct().ToArray();
        }

        public static uint[] RandomAcceptedStates(int min, int max, int maxState) {
            max = System.Math.Max(min,max);
            uint[] accState = new uint[rnd.Next(min, max)];
            for (int i = 0; i < accState.Length; i++)
                accState[i] = (uint)rnd.Next(0, maxState);
            return accState.Distinct().ToArray();
        }
    }
}