namespace Serpen.Uni.CompSys {
    public enum ePROMSet { Unset, PositiveSet, NegativeSet, BothSet }

    /// <summary>
    /// Programmable Logic Array
    /// </summary>
    public class PROM {

        /// <param name="andMatrix">which input is set for AND matrix</param>
        /// <param name="orMatrix">which and is set for OR matrix</param>
        public PROM(ePROMSet[,] andMatrix, bool[,] orMatrix) {
            if (andMatrix.GetLength(1) != orMatrix.GetLength(1))
                throw new System.ArgumentOutOfRangeException();
            AndMatrix = andMatrix;
            OrMatrix = orMatrix;
            Utils.DebugMessage("PROM:\n" + ToString(), Utils.eDebugLogLevel.Normal);
        }

        public static PROM GenerateRandom(int min = 1, int max = 10) {
            var rnd = Utils.RND;

            int inputs = rnd.Next(min, max);
            int ands = rnd.Next(min, max);
            int orOut = rnd.Next(min, max);

            var andMatrix = new ePROMSet[inputs, ands];
            var orMatrix = new bool[orOut, ands];

            for (int a = 0; a < ands; a++) {
                for (int j = 0; j < inputs; j++)
                    andMatrix[j, a] = (ePROMSet)rnd.Next(0, 3);
                for (int j = 0; j < orOut; j++)
                    orMatrix[j, a] = rnd.Next(0, 2) == 0 ? false : true;
            }
            return new PROM(andMatrix, orMatrix);
        }

        public ePROMSet[,] AndMatrix { get; set; }
        public bool[,] OrMatrix { get; set; }

        public bool[] Invoke(params bool[] inputs) {
            bool?[] ands = new bool?[AndMatrix.GetLength(1)]; // nullable due to buggy init with true
            bool[] ors = new bool[OrMatrix.GetLength(0)];

            if (inputs.Length != AndMatrix.GetLength(0))
                throw new System.ArgumentOutOfRangeException();

            // calculate AND Elements, iterate through 
            for (int a = 0; a < ands.Length; a++) {
                for (int i = 0; i < inputs.Length; i++) {
                    if (AndMatrix[i, a] == ePROMSet.BothSet)
                        ands[a] = false; // a&-a = alwaysfalse
                    else if (AndMatrix[i, a] == ePROMSet.PositiveSet)
                        ands[a] = ands[a].HasValue ? ands[a] & inputs[i] : inputs[i];
                    else if (AndMatrix[i, a] == ePROMSet.NegativeSet)
                        ands[a] = ands[a].HasValue ? ands[a] & !inputs[i] : !inputs[i];
                    if (ands[a].HasValue && !ands[a].Value)
                        break; // performance
                }
                if (!ands[a].HasValue)
                    ands[a] = false; // wasn't processed at all, so set to false
            }

            // calculate or/outputs with AND inputs
            for (int o = 0; o < ors.Length; o++) {
                ors[o] = false;
                for (int a = 0; a < ands.Length; a++) {
                    if (OrMatrix[o, a])
                        ors[o] |= ands[a].Value;
                    if (ors[o])
                        break; // performance
                }

            }
            return ors;
        }

        public override string ToString() {
            string headlenHalf = new string('=', AndMatrix.GetLength(1)); 
            var sb = new System.Text.StringBuilder(headlenHalf + "AND" + headlenHalf + "\n");
            for (int i = 0; i < AndMatrix.GetLength(0); i++) {
                for (int a = 0; a < AndMatrix.GetLength(1); a++) {
                    switch (AndMatrix[i, a]) {
                        case ePROMSet.PositiveSet:
                            sb.Append("+ ");
                            break;
                        case ePROMSet.NegativeSet:
                            sb.Append("- ");
                            break;
                        case ePROMSet.BothSet:
                            sb.Append("# ");
                            break;
                        default:
                            sb.Append("  ");
                            break;
                    }
                }
                sb.AppendLine();
            }
            sb.AppendLine(headlenHalf + "OR" + headlenHalf + "\n");
            for (int a = 0; a < OrMatrix.GetLength(0); a++)
            {
                for (int o = 0; o < OrMatrix.GetLength(1); o++)
                {
                    sb.Append(OrMatrix[a, o] ? "1 " : "0 ");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }

}