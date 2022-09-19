using IntTuple = System.Tuple<int, int>;
using System.Linq;
using System.Collections.Generic;

namespace Serpen.Uni.Knownledge {
    public static class CF {

        static IDictionary<string, float> cfs = new Dictionary<string, float>() { { "a", 1f }, { "c", 0.5f }, {"h", 0.9f}, {"f", 0.8f} };
        static IDictionary<string, float> imp = new Dictionary<string, float>() {
            {"a->b", 0.8f}, 
            {"c->d", 0.5f}, 
            {"b&d->e", 0.9f}, 
            {"e|f->g", 0.25f}, 
            {"h->g", 0.3f}, 

            
        };

        public static void B420() {
            var ef = Disjunktion("e","f");
            var G = Konjunktion("h", "e");
        }

        // function
        static float Konjunktion(params string[] a) => (from x in a select cfs[x]).Min();
        static float Disjunktion(params string[] a) => (from x in a select cfs[x]).Max();
        static float serielleKombination(string a, string b) {
            return imp[a + "->" + b] * System.MathF.Max(0, cfs[a]);
        }
    }
}