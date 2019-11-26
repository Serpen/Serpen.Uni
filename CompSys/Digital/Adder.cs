using System.Collections.Generic;

namespace Serpen.Uni.CompSys.Digital {
    public abstract class Gatter {
        public readonly List<Gatter> OutputsTo = new List<Gatter>();

        public abstract bool[] Calculate(params bool[] inputs);
    }

    public class InputGatter : Gatter {

        public InputGatter(bool not) => Not = not;
        bool Not = false;
        public override bool[] Calculate(params bool[] inputs) {
            if (!Not)
                return inputs;
            else {
                bool[] ret = new bool[inputs.Length];
                for (int i = 0; i < inputs.Length; i++)
                    ret[i] = !inputs[i];
                return ret;
            }
        }

        public InputGatter NOT => new InputGatter(false);

    }

    public class And : Gatter {
        public override bool[] Calculate(params bool[] inputs) {
            bool? result = null;
            for (int i = 0; i < inputs.Length; i++)
                result = result.HasValue ? result.Value & inputs[i] : inputs[i];
            if (result.HasValue)
                return new bool[] { result.Value };
            else
                return null;
        }
    }

    public class Or : Gatter {
        public override bool[] Calculate(params bool[] inputs) {
            bool? result = null;
            for (int i = 0; i < inputs.Length; i++)
                result = result.HasValue ? result.Value | inputs[i] : inputs[i];
            if (result.HasValue)
                return new bool[] { result.Value };
            else
                return null;
        }
    }

    public class Schaltnetz {
        InputGatter[] Inputs;
        Gatter[] Outputs;

        public Schaltnetz(int inputs, Gatter[] outputs) {
            Inputs = new InputGatter[inputs];
            Outputs = outputs;
        }

        public bool[] Invoke(params bool[] vals) {
            for (int i = 0; i < Inputs.Length; i++)
            {
                
            }
            throw new System.NotImplementedException();
        }

        public static Schaltnetz Tests() {
            var and1 = new And();
            var and2 = new And();
            var and3 = new And();

            var or4 = new Or();

            var sn = new Schaltnetz(2, new Gatter[] {or4});
            sn.Inputs[0].OutputsTo.Add(and1);
            sn.Inputs[1].NOT.OutputsTo.Add(and1);

            sn.Inputs[0].OutputsTo.Add(and2);
            sn.Inputs[0].NOT.OutputsTo.Add(and2);

            sn.Inputs[0].NOT.OutputsTo.Add(and3);
            sn.Inputs[1].OutputsTo.Add(and3);

            and1.OutputsTo.Add(or4);
            and2.OutputsTo.Add(or4);
            and3.OutputsTo.Add(or4);

            return sn;

        }

    }
}