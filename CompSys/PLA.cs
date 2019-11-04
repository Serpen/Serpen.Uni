namespace Serpen.Uni.CompSys {
    public enum ePLASet { Unset, PositiveSet, NegativeSet, BothSet }
    
    /// <summary>
    /// Programmable Logic Array
    /// </summary>
    public class PLA {

        /// <param name="andSets">definition matrix: which input is set for AND matrix</param>
        /// <param name="orSets">definition matrix: which and is set for OR matrix</param>
        public PLA(ePLASet[,] andSets, bool[,] orSets) {
            AndSets = andSets;
            OrSets = orSets;
        }

        public readonly ePLASet[,] AndSets;
        public readonly bool[,] OrSets;

        public bool[] Invoke(params bool[] inputs) {
            bool[] ands = new bool[AndSets.GetLength(1)];
            bool[] ors = new bool[OrSets.GetLength(0)];
            
            if (inputs.Length != AndSets.GetLength(0))
                throw new System.ArgumentOutOfRangeException();

            for (int a = 0; a < ands.Length; a++) {
                ands[a] = true;
                for (int i = 0; i < inputs.Length; i++) {
                    if (AndSets[i, a] == ePLASet.BothSet)
                        ands[a] = false;
                    else if (AndSets[i, a] == ePLASet.PositiveSet)
                        ands[a] &= inputs[i];
                    else if (AndSets[i, a] == ePLASet.NegativeSet)
                        ands[a] &= !inputs[i];
                }
            }

            for (int o = 0; o < ors.Length; o++) {
                ors[o] = false;
                for (int a = 0; a < ands.Length; a++)
                    if (OrSets[o, a])
                        ors[o] |= ands[a];
                
            }
            return ors;
        }
    }

}