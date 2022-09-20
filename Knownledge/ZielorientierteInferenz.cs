using System.Linq;

namespace Serpen.Uni.Knownledge.Inferenz {

    [AlgorithmSource("1696_A4.5")]
    public class ZielorientierteInferenz {

        public readonly int[] Facts;
        public readonly Rule[] RuleSet;

        public ZielorientierteInferenz(Rule[] ruleSet, params int[] facts) {
            Facts = facts;
            RuleSet = ruleSet;
        }

        public bool Backchain(params int[] q) {
            Utils.DebugMessage("in:" + string.Join(",", Utils.intArraytoAlphabet(q)), Utils.eDebugLogLevel.Normal);

            if (q.Length == 0) return true;
            if (Facts.Contains(q[0])) {
                Utils.DebugMessage((char)(q[0] + 65) + " in Facts " + string.Join(",", Utils.intArraytoAlphabet(Facts)), Utils.eDebugLogLevel.Always);
                return Backchain(q.Skip(1).ToArray());
            } else {
                foreach (var R in RuleSet) {
                    if (R.Result == q[0]) {
                        Utils.DebugMessage("use rule " + R.ToString(), Utils.eDebugLogLevel.Always);
                        if (Backchain(R.RuleArray.Union(q.Skip(1)).ToArray()))
                            return true;
                    }
                }
            }
            return false;
        }
    }

    public struct Rule {
        public readonly int[] RuleArray;
        public readonly int Result;
        public Rule(int[] rule, int result) {
            RuleArray = rule;
            Result = result;
        }
        public Rule(params int[] rule) {
            Result = rule[0];
            RuleArray = rule.Skip(1).ToArray();
        }

        public override string ToString() {
            return string.Join(" & ", Utils.intArraytoAlphabet(RuleArray)) + " -> " + (char)(Result + 65);
        }

        public static Rule[] RB47 = {
            new Rule(7,0,1),
            new Rule(8,2),
            new Rule(8,3),
            new Rule(9,4,5,6),
            new Rule(10,7),
            new Rule(10,8),
            new Rule(11,8,9),
            new Rule(12,10,11)
        };

        public static Rule[] EA2_2_1_Rec = {
            new Rule(1, 0),
            new Rule(3,2),
            new Rule(2, 3)
        };

        public static Rule[] EA2_2_2 = {
            new Rule(3,1,2),
            new Rule(8,1,7),
            new Rule(5,1,8),
            new Rule(6,3),
            new Rule(7,6,5)
        };

        public static int[] B410Facts = new int[] { 7, 2, 4, 5, 6 };
        public static int[] S411Facts = new int[] { 0, 3, 4, 5, 6 };

    }
}