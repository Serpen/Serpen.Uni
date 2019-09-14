using System.Linq;
using Serpen.Uni.Automat.Turing;

namespace Serpen.Uni.Automat {
    public partial class KnownAutomat {
        public static TuringMachineBase TM_1659_A44_M1 {
            get {
                var t = new TuringTransform();
                t.Add(new TuringKey(0, 'a'), new TuringVal(0, 'b', TMDirection.Right));
                t.Add(new TuringKey(0, 'b'), new TuringVal(0, 'a', TMDirection.Right));
                t.Add(new TuringKey(0, ' '), new TuringVal(1, 'b', TMDirection.Left));
                t.Add(new TuringKey(1, 'b'), new TuringVal(3, 'b', TMDirection.Right));
                t.Add(new TuringKey(1, 'a'), new TuringVal(2, 'a', TMDirection.Left));
                t.Add(new TuringKey(2, 'b'), new TuringVal(0, 'b', TMDirection.Left));
                t.Add(new TuringKey(2, 'a'), new TuringVal(4, 'a', TMDirection.Right));
                return new TuringMachineSingleBand(nameof(TM_1659_A44_M1), 5, new char[] {'a', 'b'}, new char[] {'a', 'b', ' '}, t, 0, ' ', new uint[] {3});
            }
        }

        public static TuringMachineBase TM_EFAK_A89_B82_T81 {
            get {
                var t = new TuringTransform();
                t.Add(new TuringKey(0, '0'), new TuringVal(1, 'X', TMDirection.Right));
                t.Add(new TuringKey(1, 'Y'), new TuringVal(1, 'Y', TMDirection.Right));
                t.Add(new TuringKey(1, '0'), new TuringVal(1, '0', TMDirection.Right));
                t.Add(new TuringKey(1, '1'), new TuringVal(2, 'Y', TMDirection.Left ));
                t.Add(new TuringKey(2, 'Y'), new TuringVal(2, 'Y', TMDirection.Left ));
                t.Add(new TuringKey(2, '0'), new TuringVal(2, '0', TMDirection.Left ));
                t.Add(new TuringKey(2, 'X'), new TuringVal(0, 'X', TMDirection.Right));
                t.Add(new TuringKey(0, 'Y'), new TuringVal(3, 'Y', TMDirection.Right));
                t.Add(new TuringKey(3, 'Y'), new TuringVal(3, 'Y', TMDirection.Right));
                t.Add(new TuringKey(3, 'B'), new TuringVal(4, 'B', TMDirection.Right));
                return new TuringMachineSingleBand(nameof(TM_EFAK_A89_B82_T81), 5, binAlp, new char[] {'0', '1', 'X', 'Y', 'B'}, t, 0, 'B', new uint[] {4});
            }
        }

        public static TuringMachineSingleBand TM_EFAK_A810_B84_Monus {
            get {
                var t = new TuringTransform();
                t.Add(new TuringKey(0, '0'), new TuringVal(1, 'B', TMDirection.Right));
                t.Add(new TuringKey(0, '1'), new TuringVal(5, 'B', TMDirection.Right));
                t.Add(new TuringKey(1, '0'), new TuringVal(1, '0', TMDirection.Right));
                t.Add(new TuringKey(1, '1'), new TuringVal(2, '1', TMDirection.Right));
                t.Add(new TuringKey(2, '0'), new TuringVal(3, '1', TMDirection.Left));
                t.Add(new TuringKey(2, '1'), new TuringVal(2, '1', TMDirection.Right));
                t.Add(new TuringKey(2, 'B'), new TuringVal(4, 'B', TMDirection.Left));
                t.Add(new TuringKey(3, '0'), new TuringVal(3, '0', TMDirection.Left));
                t.Add(new TuringKey(3, '1'), new TuringVal(3, '1', TMDirection.Left));
                t.Add(new TuringKey(3, 'B'), new TuringVal(0, 'B', TMDirection.Right));
                t.Add(new TuringKey(4, '0'), new TuringVal(4, '0', TMDirection.Left));
                t.Add(new TuringKey(4, '1'), new TuringVal(4, 'B', TMDirection.Left));
                t.Add(new TuringKey(4, 'B'), new TuringVal(6, '0', TMDirection.Right));
                t.Add(new TuringKey(5, '0'), new TuringVal(5, 'B', TMDirection.Right));
                t.Add(new TuringKey(5, '1'), new TuringVal(5, 'B', TMDirection.Right));
                t.Add(new TuringKey(5, 'B'), new TuringVal(6, 'B', TMDirection.Right));
                return new TuringMachineSingleBand(nameof(TM_EFAK_A810_B84_Monus), 7, binAlp, new char[] {'0', '1', 'B'}, t, 0, 'B', new uint[] {});
            }
        }

        public static TuringMachineBase TM_EFAK_B86 {
            get {
                string[] states = new string[] {"0,0", "0,1", "0,B", "1,0", "1,1", "1,B"};
                
                var t = new TuringTransform();
                t.AddByStateStore(states, "0,B", '0', "1,0", '0', TMDirection.Right);
                t.AddByStateStore(states, "0,B", '1', "1,1", '1', TMDirection.Right);
                t.AddByStateStore(states, "1,0", '1', "1,0", '1', TMDirection.Right);
                t.AddByStateStore(states, "1,1", '0', "1,1", '1', TMDirection.Right);
                t.AddByStateStore(states, "1,0", 'B', "1,B", 'B', TMDirection.Right);
                t.AddByStateStore(states, "1,1", 'B', "1,B", 'B', TMDirection.Right);

                return new TuringMachineSingleBand(nameof(TM_EFAK_B86), states, binAlp, new char[] {'0','1','B'}, t, 2, 'B', new uint[] {5});
            }
        }
    }
}