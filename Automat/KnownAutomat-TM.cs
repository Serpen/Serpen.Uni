using System.Linq;
using Serpen.Uni.Automat.Turing;

namespace Serpen.Uni.Automat {
    public partial class KnownAutomat {
        public static TuringMachineBase TM_1659_A44_M1 {
            get {
                var t = new TuringTransformSingleBand();
                t.Add(new TuringTransformSingleBand.TuringKey(0, 'a'), new TuringTransformSingleBand.TuringVal(0, 'b', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(0, 'b'), new TuringTransformSingleBand.TuringVal(0, 'a', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(0, ' '), new TuringTransformSingleBand.TuringVal(1, 'b', TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(1, 'b'), new TuringTransformSingleBand.TuringVal(3, 'b', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(1, 'a'), new TuringTransformSingleBand.TuringVal(2, 'a', TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(2, 'b'), new TuringTransformSingleBand.TuringVal(0, 'b', TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(2, 'a'), new TuringTransformSingleBand.TuringVal(4, 'a', TMDirection.Right));
                return new TuringMachineSingleBand(nameof(TM_1659_A44_M1), 5, new char[] {'a', 'b'}, new char[] {'a', 'b', ' '}, t, 0, ' ', new uint[] {3});
            }
        }

        public static TuringMachineBase TM_EFAK_A89_B82_T81 {
            get {
                var t = new TuringTransformSingleBand();
                t.Add(new TuringTransformSingleBand.TuringKey(0, '0'), new TuringTransformSingleBand.TuringVal(1, 'X', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(1, 'Y'), new TuringTransformSingleBand.TuringVal(1, 'Y', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(1, '0'), new TuringTransformSingleBand.TuringVal(1, '0', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(1, '1'), new TuringTransformSingleBand.TuringVal(2, 'Y', TMDirection.Left ));
                t.Add(new TuringTransformSingleBand.TuringKey(2, 'Y'), new TuringTransformSingleBand.TuringVal(2, 'Y', TMDirection.Left ));
                t.Add(new TuringTransformSingleBand.TuringKey(2, '0'), new TuringTransformSingleBand.TuringVal(2, '0', TMDirection.Left ));
                t.Add(new TuringTransformSingleBand.TuringKey(2, 'X'), new TuringTransformSingleBand.TuringVal(0, 'X', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(0, 'Y'), new TuringTransformSingleBand.TuringVal(3, 'Y', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(3, 'Y'), new TuringTransformSingleBand.TuringVal(3, 'Y', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(3, 'B'), new TuringTransformSingleBand.TuringVal(4, 'B', TMDirection.Right));
                return new TuringMachineSingleBand(nameof(TM_EFAK_A89_B82_T81), 5, binAlp, new char[] {'0', '1', 'X', 'Y', 'B'}, t, 0, 'B', new uint[] {4});
            }
        }

        public static TuringMachineSingleBand TM_EFAK_A810_B84_Monus {
            get {
                var t = new TuringTransformSingleBand();
                t.Add(new TuringTransformSingleBand.TuringKey(0, '0'), new TuringTransformSingleBand.TuringVal(1, 'B', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(0, '1'), new TuringTransformSingleBand.TuringVal(5, 'B', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(1, '0'), new TuringTransformSingleBand.TuringVal(1, '0', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(1, '1'), new TuringTransformSingleBand.TuringVal(2, '1', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(2, '0'), new TuringTransformSingleBand.TuringVal(3, '1', TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(2, '1'), new TuringTransformSingleBand.TuringVal(2, '1', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(2, 'B'), new TuringTransformSingleBand.TuringVal(4, 'B', TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(3, '0'), new TuringTransformSingleBand.TuringVal(3, '0', TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(3, '1'), new TuringTransformSingleBand.TuringVal(3, '1', TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(3, 'B'), new TuringTransformSingleBand.TuringVal(0, 'B', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(4, '0'), new TuringTransformSingleBand.TuringVal(4, '0', TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(4, '1'), new TuringTransformSingleBand.TuringVal(4, 'B', TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(4, 'B'), new TuringTransformSingleBand.TuringVal(6, '0', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(5, '0'), new TuringTransformSingleBand.TuringVal(5, 'B', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(5, '1'), new TuringTransformSingleBand.TuringVal(5, 'B', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(5, 'B'), new TuringTransformSingleBand.TuringVal(6, 'B', TMDirection.Right));
                return new TuringMachineSingleBand(nameof(TM_EFAK_A810_B84_Monus), 7, binAlp, new char[] {'0', '1', 'B'}, t, 0, 'B', new uint[] {});
            }
        }

        public static TuringMachineBase TM_EFAK_B86 {
            get {
                string[] states = new string[] {"0,0", "0,1", "0,B", "1,0", "1,1", "1,B"};
                
                var t = new TuringTransformSingleBand();
                t.AddByStateStore(states, "0,B", '0', "1,0", '0', TMDirection.Right);
                t.AddByStateStore(states, "0,B", '1', "1,1", '1', TMDirection.Right);
                t.AddByStateStore(states, "1,0", '1', "1,0", '1', TMDirection.Right);
                t.AddByStateStore(states, "1,1", '0', "1,1", '1', TMDirection.Right);
                t.AddByStateStore(states, "1,0", 'B', "1,B", 'B', TMDirection.Right);
                t.AddByStateStore(states, "1,1", 'B', "1,B", 'B', TMDirection.Right);

                return new TuringMachineSingleBand(nameof(TM_EFAK_B86), states, binAlp, new char[] {'0','1','B'}, t, 2, 'B', new uint[] {5});
            }
        }

        public static TuringMachineBase TM_EFAK_B87_wcw {
            get {

                string[] states = new string[] {"1,0","1,1","1,B","2,0","2,1","2,B","3,0","3,1","3,B","4,0","4,1","4,B","5,0","5,1","5,B","6,0","6,1","6,B","7,0","7,1","7,B","8,0","8,1","8,B","9,0","9,1","9,B"};
                // string[] states = new string[] {"1,0", "1,1", "2,0", "2,1", "3,0", "3,1", "4,0", "4,1", "5,0", "5,1", "6,0", "6,1", "7,0", "7,1", "8,0", "8,1", "9,0", "9,1"};
                string[] bandSymbols  = new string[] {"B,0", "*,0", "B,1", "*,1", "B,c", "*,c", "B,B", "*,B"};
                // char[] bandAlphabetEx = new char[] {'B','*','B','*','B','*','B','*'};
                // char[] bandAlphabetEx = new char[] {'0','O','1','l','C','c','b','B'};
                var bandAlphabetTranslate = new System.Collections.Generic.Dictionary<char, string>();
                for (int i = 0; i < bandSymbols.Length; i++)
                    bandAlphabetTranslate.Add((char)(67+i), bandSymbols[i]);
                var bandAlphabetTranslateRev = new System.Collections.Generic.Dictionary<string, char>();
                for (int i = 0; i < bandSymbols.Length; i++)
                    bandAlphabetTranslateRev.Add(bandSymbols[i], (char)(67+i));


                char[] bandSymbols2 = new char[bandSymbols.Length];
                for (int i = 0; i < bandSymbols2.Length; i++)
                    bandSymbols2[i] = (char)(67+i);
                
                var t = new TuringTransformSingleBand();
                t.AddByStateStoreAndBand(states, bandSymbols, "1,B", "B,0", "2,0", "*,0", TMDirection.Right);
                t.AddByStateStoreAndBand(states, bandSymbols, "1,B", "B,1", "2,1", "*,1", TMDirection.Right);

                // 2
                t.AddByStateStoreAndBand(states, bandSymbols, "2,0", "B,0", "2,0", "B,0", TMDirection.Right);
                t.AddByStateStoreAndBand(states, bandSymbols, "2,0", "B,1", "2,0", "B,1", TMDirection.Right);
                t.AddByStateStoreAndBand(states, bandSymbols, "2,1", "B,0", "2,1", "B,0", TMDirection.Right);
                t.AddByStateStoreAndBand(states, bandSymbols, "2,1", "B,1", "2,1", "B,1", TMDirection.Right);

                // 3
                t.AddByStateStoreAndBand(states, bandSymbols, "2,0", "B,c", "3,0", "B,c", TMDirection.Right);
                t.AddByStateStoreAndBand(states, bandSymbols, "2,1", "B,c", "3,1", "B,c", TMDirection.Right);

                // 4
                t.AddByStateStoreAndBand(states, bandSymbols, "3,0", "*,0", "3,0", "*,0", TMDirection.Right);
                t.AddByStateStoreAndBand(states, bandSymbols, "3,0", "*,1", "3,0", "*,1", TMDirection.Right);
                t.AddByStateStoreAndBand(states, bandSymbols, "3,1", "*,0", "3,1", "*,0", TMDirection.Right);
                t.AddByStateStoreAndBand(states, bandSymbols, "3,1", "*,1", "3,1", "*,1", TMDirection.Right);

                // 5
                t.AddByStateStoreAndBand(states, bandSymbols, "3,0", "B,0", "4,B", "*,0", TMDirection.Left);
                t.AddByStateStoreAndBand(states, bandSymbols, "3,1", "B,1", "4,B", "*,1", TMDirection.Left);

                // 6
                t.AddByStateStoreAndBand(states, bandSymbols, "4,B", "*,0", "4,B", "*,0", TMDirection.Left);
                t.AddByStateStoreAndBand(states, bandSymbols, "4,B", "*,1", "4,B", "*,1", TMDirection.Left);

                // 7
                t.AddByStateStoreAndBand(states, bandSymbols, "4,B", "B,c", "5,B", "B,c", TMDirection.Left);

                //8
                t.AddByStateStoreAndBand(states, bandSymbols, "5,B", "B,0", "6,B", "B,0", TMDirection.Left);
                t.AddByStateStoreAndBand(states, bandSymbols, "5,B", "B,1", "6,B", "B,1", TMDirection.Left);

                //9
                t.AddByStateStoreAndBand(states, bandSymbols, "6,B", "B,0", "6,B", "B,0", TMDirection.Left);
                t.AddByStateStoreAndBand(states, bandSymbols, "6,B", "B,1", "6,B", "B,1", TMDirection.Left);

                //10
                t.AddByStateStoreAndBand(states, bandSymbols, "6,B", "*,0", "1,B", "*,0", TMDirection.Right);
                t.AddByStateStoreAndBand(states, bandSymbols, "6,B", "*,1", "1,B", "*,1", TMDirection.Right);

                //11
                t.AddByStateStoreAndBand(states, bandSymbols, "5,B", "*,0", "7,B", "*,0", TMDirection.Right);
                t.AddByStateStoreAndBand(states, bandSymbols, "5,B", "*,1", "7,B", "*,1", TMDirection.Right);

                //12
                t.AddByStateStoreAndBand(states, bandSymbols, "7,B", "B,c", "8,B", "B,c", TMDirection.Right);

                //13
                t.AddByStateStoreAndBand(states, bandSymbols, "8,B", "*,0", "8,B", "*,0", TMDirection.Right);
                t.AddByStateStoreAndBand(states, bandSymbols, "8,B", "*,1", "8,B", "*,1", TMDirection.Right);

                //14
                t.AddByStateStoreAndBand(states, bandSymbols, "8,B", "B,B", "9,B", "B,B", TMDirection.Right);


                var tm = new TuringMachineSingleBand(nameof(TM_EFAK_B87_wcw), states, new char[] {'0','1','c'}, bandSymbols2 , t, Utils.ArrayIndex(states,"1,B"), 'I', new uint[] {Utils.ArrayIndex(states,"9,B")});
                tm.RealBandAlphabet = bandAlphabetTranslate;
                tm.RealBandAlphabetRev = bandAlphabetTranslateRev;
                return tm;
            }
        }
    }
}