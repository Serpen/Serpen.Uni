using System.Collections.Generic;
using System.Linq;
using RegExText = System.Text.RegularExpressions.Regex;

namespace Serpen.Uni.Automat.Finite {
    public class Converter {

        [AlgorithmSource("EAFK-2.3.5", "EAFK-2.5.5","1659-D-2.8")]
        public static Finite.DFA Nea2TeilmengenDea(Finite.INFA N) {
            var States = new List<uint[]>((int)N.StatesCount);
            var DeaStatesNames2Index = new Dictionary<string, uint>((int)N.StatesCount);
            var States2Check = new Queue<uint[]>((int)N.StatesCount);
            var t = new Finite.DFATransform();
            var accStates = new List<uint>(N.AcceptedStates.Length);

            // Add first state to DFA states, for check and naming
            uint[] qStart;
            if (N is Finite.NFAe Ne)
                qStart = Ne.EpsilonHuelle(N.StartState);
            else
                qStart = new uint[] { N.StartState };

            States.Add(qStart);
            States2Check.Enqueue(qStart);
            DeaStatesNames2Index.Add(string.Join(',', qStart), 0);

            // if start State is also acceptable
            if (qStart.Intersect(N.AcceptedStates).Any())
                accStates.Add(0);

            // ?find all states for checking
            while (States2Check.Any()) {
                var q = States2Check.Dequeue();

                foreach (var w in N.Alphabet) { // every existing transform for char
                    var qNext = N.GoChar(q, w);
                    var qStr = string.Join(",", qNext);
                    if (!DeaStatesNames2Index.ContainsKey(qStr)) {
                        States.Add(qNext);
                        DeaStatesNames2Index.Add(qStr, (uint)States.Count - 1);
                        if (qNext.Intersect(N.AcceptedStates).Any())
                            accStates.Add((uint)States.Count - 1);

                        if (!(from s in States2Check where s.SequenceEqual(qNext) select s).Any())
                            States2Check.Enqueue(qNext);
                    }
                }
            }

            // calc transform
            for (uint i = 0; i < States.Count; i++) { // iterate every state
                foreach (var w in N.Alphabet) { // iterate alphabet
                    var x = N.GoChar(States[(int)i], w);
                    var XStr = string.Join(',', x);
                    var xPos = DeaStatesNames2Index[XStr];

                    if (!t.ContainsKey(i, w))
                        t.Add(i, w, xPos);
                }
            }

            // Naming the DFA states, with the NFA state set
            string[] names = new string[States.Count];
            for (int i = 0; i < names.Length; i++)
                names[i] = string.Join(",", States[i]);

            return new Finite.DFA($"DEA_PotenzMenge({N.Name})", names, N.Alphabet, t, 0, accStates.ToArray());

        }

        /// <summary>
        /// Returns the NOT minimal RegEx for a given DFA
        /// </summary>
        /// <param name="D"></param>
        /// <returns></returns>
        [AlgorithmSource("1659-S-2.11")]
        public static string DEA2RegExp(Finite.DFA D) {
            //represents R<sub>ij</sub><sup>(k)</sup> From i to j, without State higher than k
            string[,,] R = new string[D.States.Length, D.States.Length, D.States.Length + 1];

            //first k! then i, j
            for (uint k = 0; k < R.GetLength(2); k++) {
                for (uint i = 0; i < R.GetLength(0); i++) {
                    for (uint j = 0; j < R.GetLength(0); j++) {
                        // k==0 calculates Transforms for direct contact
                        if (k == 0) {
                            var toAdd = new List<string>();
                            if (i == j)  //from i to i, means ε 
                                toAdd.Add(Uni.Utils.EPSILON.ToString());

                            //check if any char loops in this state
                            foreach (char c in D.Alphabet) {
                                if (((Finite.DFATransform)D.Transforms).TryGetValue(i, c, out uint qNext) & qNext == j)
                                    toAdd.Add(c.ToString());
                            }
                            R[i, j, 0] = string.Join('+', toAdd);
                            if (R[i, j, 0] == Uni.Utils.EPSILON.ToString()) R[i, j, 0] = "";

                        } else {
                            //R is now calculated by previous R^(k-1)
                            //Rijk = Rijk+Rikk Rkkk* Rjkk
                            var Rijk = R[i, j, k - 1];
                            var Rikk = R[i, k - 1, k - 1];
                            var Rkkk = R[k - 1, k - 1, k - 1];
                            var Rkjk = R[k - 1, j, k - 1];

                            //try some magic to shorten formula
                            if (Rikk == Rkkk | Rikk == $"({Rkkk})" | $"({Rikk}" == Rkkk)
                                Rikk = string.Empty;
                            if (Rkjk == Rkkk | Rkjk == $"({Rkkk})" | $"({Rkjk}" == Rkkk)
                                Rkjk = string.Empty;
                            if ($"{Rikk}{Rkkk}{Rkjk}" == Rijk)
                                Rijk = "";
                            if (Rijk != string.Empty) Rijk = $"({Rijk})+";
                            if (Rikk != string.Empty) Rikk = $"({Rikk})";
                            if (Rkjk != string.Empty) Rkjk = $"({Rkjk})";
                            if (Rkkk != string.Empty) Rkkk = $"({Rkkk})*";

                            R[i, j, k] = $"{Rijk}{Rikk}{Rkkk}{Rkjk}";

                            if (R[i, j, k] == "()*") R[i, j, k] = string.Empty;

                            //try some magic to shorten formula with regex
                            R[i, j, k] = RegExText.Replace(R[i, j, k], @"\((\w)\)", "$1");
                            R[i, j, k] = RegExText.Replace(R[i, j, k], @"\((\w)\*\)", "$1*");
                            //R[i,j,k] = RegExText.Replace(R[i,j,k],@"\(ε\+(\w)\)\*","$1*");

                        }
                    } //next j
                } //next i
            } //next k

            return R[D.StartState, D.AcceptedStates[0], R.GetLength(2) - 1];
        } //end function
    } //end class
} //end ns