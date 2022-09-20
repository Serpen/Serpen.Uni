using System.Collections.Generic;

namespace Serpen.Uni.Knownledge.Inferenz {

    [AlgorithmSource("1696_A4.3")]
    public class DataDrivenInferenz {

        public static IEnumerable<int> Invoke(Rule[] RuleSet, params int[] facts) {
            bool changed;
            var Facts = new List<int>(facts);
            
            do {
                changed = false;
                foreach (var r in RuleSet) {
                    if (!Facts.Contains(r.Result) && Facts.ContainsSubset(r.RuleArray)) {
                        Utils.DebugMessage(("rule '" + r.ToString() + "' ").PadRight(25) + string.Join(",", Utils.intArraytoAlphabet(Facts.ToArray())) + " + " + (char)(r.Result + 65), Utils.eDebugLogLevel.Always);
                        Facts.Add(r.Result);
                        changed = true;
                    }
                }
            } while (changed);
            return Facts;
        }

    }
}