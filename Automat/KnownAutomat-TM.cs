using Serpen.Uni.Automat.Turing;

namespace Serpen.Uni.Automat {
    public partial class KnownAutomat {

        const char BLK = TuringMachineSingleBand1659.BLANK;

        [AcceptedWordSamples("aba")]
        public static TuringMachineSingleBand TM_1659_A44_M1 {
            get {
                var t = new TuringTransformSingleBand();
                t.Add(new TuringTransformSingleBand.TuringKey(0, 'a'), new TuringTransformSingleBand.TuringVal(0, 'b', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(0, 'b'), new TuringTransformSingleBand.TuringVal(0, 'a', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(0, BLK), new TuringTransformSingleBand.TuringVal(1, 'b', TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(1, 'b'), new TuringTransformSingleBand.TuringVal(3, 'b', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(1, 'a'), new TuringTransformSingleBand.TuringVal(2, 'a', TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(2, 'b'), new TuringTransformSingleBand.TuringVal(0, 'b', TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(2, 'a'), new TuringTransformSingleBand.TuringVal(4, 'a', TMDirection.Right));

                char[] inputAlphabet = new char[] { 'a', 'b' };
                char[] bandAlphabet = new char[] { 'a', 'b', BLK };
                return new TuringMachineSingleBand(nameof(TM_1659_A44_M1), 5, inputAlphabet, bandAlphabet, t, 0, BLK, new uint[] { 3 });
            }
        }

        [AcceptedWordSamples("aa")]
        public static TuringMachineSingleBand1659 TM_1659_A46_M2_a2n {
            get {
                const uint qA = 6;
                const uint qD = 5;

                var t = new TuringTransformSingleBand();
                t.Add(0, 'a', 1, 's', TMDirection.Right);
                t.Add(0, BLK, qD, BLK, TMDirection.Right);

                t.Add(1, 'x', 1, 'x', TMDirection.Right);
                t.Add(1, 'a', 2, 'x', TMDirection.Right);
                t.Add(1, BLK, qA, BLK, TMDirection.Right);

                t.Add(2, 'x', 2, 'x', TMDirection.Right);
                t.Add(2, 'a', 3, 'a', TMDirection.Right);
                t.Add(2, BLK, 4, BLK, TMDirection.Left);

                t.Add(3, 'x', 3, 'x', TMDirection.Right);
                t.Add(3, 'a', 2, 'x', TMDirection.Right);
                t.Add(3, BLK, qD, BLK, TMDirection.Right);

                t.Add(4, 'a', 4, 'a', TMDirection.Left);
                t.Add(4, 'x', 4, 'x', TMDirection.Left);
                t.Add(4, 's', 1, 's', TMDirection.Right);

                char[] inputAlphabet = new char[] { 'a', 'x', 's' };
                char[] bandAlphabet = new char[] { 'a', 'x', 's', BLK };

                return new TuringMachineSingleBand1659(nameof(TM_1659_A46_M2_a2n), 7, inputAlphabet, bandAlphabet, t, 0, BLK, qA, qD);
            }
        }

        [AcceptedWordSamples("aa")]
        public static TuringMachineSingleBand1659 TM_1659_A414_M1 {
            get {
                const uint qA = 1;
                const uint qD = 2;

                var t = new TuringTransformSingleBand();
                t.Add(0, '0', 0, '0', TMDirection.Right);
                t.Add(0, '1', qD, BLK, TMDirection.Left);
                t.Add(0, BLK, qA, '0', TMDirection.Left);

                char[] bandAlphabet = new char[] { '0', '1', BLK };

                return new TuringMachineSingleBand1659(nameof(TM_1659_A414_M1), 3, binAlp, bandAlphabet, t, 0, BLK, qA, qD);
            }
        }

        [AcceptedWordSamples("aaba")]
        public static NTM1659 NTM_1659_A411_N1 {
            get {
                const uint qA = 4;
                const uint qD = 3;

                var t = new NTM1659Transform();
                t.Add(0, 'a', 0, 'a', TMDirection.Right);
                t.AddM(0, 'a', 1, 'a', TMDirection.Right);
                t.Add(0, 'b', 0, 'b', TMDirection.Right);
                t.Add(0, BLK, qD, BLK, TMDirection.Right);

                t.Add(1, 'a', 2, 'a', TMDirection.Right);
                t.Add(1, 'b', qD, 'b', TMDirection.Right);
                t.Add(1, BLK, qD, BLK, TMDirection.Right);

                t.Add(2, 'b', qA, 'b', TMDirection.Right);
                t.Add(2, 'a', qD, 'a', TMDirection.Right);
                t.Add(2, BLK, qD, BLK, TMDirection.Right);

                char[] alphabet = new char[] { 'a', 'b', BLK };

                return new NTM1659(nameof(NTM_1659_A411_N1), 5, new char[] { 'a', 'b' }, alphabet, t, 0, KnownAutomat.BLK, qA, qD);
            }
        }

        [AcceptedWordSamples("0011")]
        public static TuringMachineSingleBand TM_EFAK_A89_B82_T81_0n1n {
            get {
                var t = new TuringTransformSingleBand();
                t.Add(new TuringTransformSingleBand.TuringKey(0, '0'), new TuringTransformSingleBand.TuringVal(1, 'X', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(1, 'Y'), new TuringTransformSingleBand.TuringVal(1, 'Y', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(1, '0'), new TuringTransformSingleBand.TuringVal(1, '0', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(1, '1'), new TuringTransformSingleBand.TuringVal(2, 'Y', TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(2, 'Y'), new TuringTransformSingleBand.TuringVal(2, 'Y', TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(2, '0'), new TuringTransformSingleBand.TuringVal(2, '0', TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(2, 'X'), new TuringTransformSingleBand.TuringVal(0, 'X', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(0, 'Y'), new TuringTransformSingleBand.TuringVal(3, 'Y', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(3, 'Y'), new TuringTransformSingleBand.TuringVal(3, 'Y', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(3, BLK), new TuringTransformSingleBand.TuringVal(4, BLK, TMDirection.Right));

                char[] inputAlphabet = new char[] { '0', '1', 'X', 'Y'};
                char[] blankAlphabet = new char[] { '0', '1', 'X', 'Y', BLK};
                return new TuringMachineSingleBand(nameof(TM_EFAK_A89_B82_T81_0n1n), 5, inputAlphabet, blankAlphabet, t, 0, BLK, new uint[] { 4 });
            }
        }

        public static TuringMachineSingleBand TM_EFAK_A810_B84_Monus {
            get {
                var t = new TuringTransformSingleBand();
                t.Add(new TuringTransformSingleBand.TuringKey(0, '0'), new TuringTransformSingleBand.TuringVal(1, BLK, TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(0, '1'), new TuringTransformSingleBand.TuringVal(5, BLK, TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(1, '0'), new TuringTransformSingleBand.TuringVal(1, '0', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(1, '1'), new TuringTransformSingleBand.TuringVal(2, '1', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(2, '0'), new TuringTransformSingleBand.TuringVal(3, '1', TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(2, '1'), new TuringTransformSingleBand.TuringVal(2, '1', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(2, BLK), new TuringTransformSingleBand.TuringVal(4, BLK, TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(3, '0'), new TuringTransformSingleBand.TuringVal(3, '0', TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(3, '1'), new TuringTransformSingleBand.TuringVal(3, '1', TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(3, BLK), new TuringTransformSingleBand.TuringVal(0, BLK, TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(4, '0'), new TuringTransformSingleBand.TuringVal(4, '0', TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(4, '1'), new TuringTransformSingleBand.TuringVal(4, BLK, TMDirection.Left));
                t.Add(new TuringTransformSingleBand.TuringKey(4, BLK), new TuringTransformSingleBand.TuringVal(6, '0', TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(5, '0'), new TuringTransformSingleBand.TuringVal(5, BLK, TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(5, '1'), new TuringTransformSingleBand.TuringVal(5, BLK, TMDirection.Right));
                t.Add(new TuringTransformSingleBand.TuringKey(5, BLK), new TuringTransformSingleBand.TuringVal(6, BLK, TMDirection.Right));
                return new TuringMachineSingleBand(nameof(TM_EFAK_A810_B84_Monus), 7, binAlp, new char[] { '0', '1', BLK }, t, 0, BLK, new uint[] { });
            }
        }

        public static TuringMachineSingleBand TM_EFAK_U825a {
            get {
                const char BLK = TuringMachineSingleBand1659.BLANK;

                var t = new TuringTransformSingleBand();
                t.Add(0,'0',1,'1',TMDirection.Right);
                t.Add(1,'1',0,'0',TMDirection.Right);
                t.Add(1,BLK,3,BLK,TMDirection.Right);
                
                return new TuringMachineSingleBand(nameof(TM_EFAK_U825a), 4, binAlp, new char[] { '0', '1', BLK }, t, 0, BLK, new uint[] { 3 });
            }
        }

        public static TuringMachineSingleBand TM_EFAK_U825b {
            get {
                const char BLK = TuringMachineSingleBand1659.BLANK;

                var t = new TuringTransformSingleBand();

                t.Add(0,'0',0,BLK,TMDirection.Right);
                t.Add(0,'1',1,BLK,TMDirection.Right);
                t.Add(1,'1',1,BLK,TMDirection.Right);
                t.Add(1,BLK,3,BLK,TMDirection.Right);

                
                return new TuringMachineSingleBand(nameof(TM_EFAK_U825b), 4, binAlp, new char[] { '0', '1', BLK }, t, 0, BLK, new uint[] { 3 });
            }
        }

        public static TuringMachineSingleBand TM_EFAK_U825c {
            get {
                const char BLK = TuringMachineSingleBand1659.BLANK;

                var t = new TuringTransformSingleBand();

                t.Add(0,'0',1,'1',TMDirection.Right);
                t.Add(1,'1',2,'0',TMDirection.Left);
                t.Add(2,'1',0,'1',TMDirection.Right);
                t.Add(1,BLK,3,BLK,TMDirection.Right);
                
                return new TuringMachineSingleBand(nameof(TM_EFAK_U825c), 4, binAlp, new char[] { '0', '1', BLK }, t, 0, BLK, new uint[] { 3 });
            }
        }

        public static TuringMachineSingleBand TM_EFAK_B86 {
            get {
                string[] states = new string[] { "0,0", "0,1", "0,B", "1,0", "1,1", "1,B" };

                var t = new TuringTransformSingleBand();
                t.AddByStateStore(states, "0,B", '0', "1,0", '0', TMDirection.Right);
                t.AddByStateStore(states, "0,B", '1', "1,1", '1', TMDirection.Right);
                t.AddByStateStore(states, "1,0", '1', "1,0", '1', TMDirection.Right);
                t.AddByStateStore(states, "1,1", '0', "1,1", '1', TMDirection.Right);
                t.AddByStateStore(states, "1,0", BLK, "1,B", BLK, TMDirection.Right);
                t.AddByStateStore(states, "1,1", BLK, "1,B", BLK, TMDirection.Right);

                return new TuringMachineSingleBand(nameof(TM_EFAK_B86), states, binAlp, new char[] { '0', '1', BLK }, t, 2, BLK, new uint[] { 5 });
            }
        }

        public static TuringMachineMultiTrack TMk_EFAK_B87_wcw {
            get {
                string[] stateTracks = { "1,0", "1,1", "1,B", "2,0", "2,1", "2,B", "3,0", "3,1", "3,B", "4,0", "4,1", "4,B", "5,0", "5,1", "5,B", "6,0", "6,1", "6,B", "7,0", "7,1", "7,B", "8,0", "8,1", "8,B", "9,0", "9,1", "9,B" };

                // string[] bandTracks = { "B,0", "*,0", "B,1", "*,1", "B,c", "*,c", "B,B", "*,B" };
                char[] bandSymbols = { '0', 'A', '1', 'D', 'c', 'E', 'B', 'F' };

                char[] inpAlph = { '0', '1', 'c' };

                uint[] acceptedStates = { Utils.ArrayIndex(stateTracks, "9,B") };
                uint startState = Utils.ArrayIndex(stateTracks, "1,B");

                var t = new TuringTransformMultiTrack(stateTracks);
                t.AddByStateStoreAndTracks("1,B", "B,0", "2,0", "*,0", TMDirection.Right);
                t.AddByStateStoreAndTracks("1,B", "B,1", "2,1", "*,1", TMDirection.Right);

                // 2
                t.AddByStateStoreAndTracks("2,0", "B,0", "2,0", "B,0", TMDirection.Right);
                t.AddByStateStoreAndTracks("2,0", "B,1", "2,0", "B,1", TMDirection.Right);
                t.AddByStateStoreAndTracks("2,1", "B,0", "2,1", "B,0", TMDirection.Right);
                t.AddByStateStoreAndTracks("2,1", "B,1", "2,1", "B,1", TMDirection.Right);

                // 3
                t.AddByStateStoreAndTracks("2,0", "B,c", "3,0", "B,c", TMDirection.Right);
                t.AddByStateStoreAndTracks("2,1", "B,c", "3,1", "B,c", TMDirection.Right);

                // 4
                t.AddByStateStoreAndTracks("3,0", "*,0", "3,0", "*,0", TMDirection.Right);
                t.AddByStateStoreAndTracks("3,0", "*,1", "3,0", "*,1", TMDirection.Right);
                t.AddByStateStoreAndTracks("3,1", "*,0", "3,1", "*,0", TMDirection.Right);
                t.AddByStateStoreAndTracks("3,1", "*,1", "3,1", "*,1", TMDirection.Right);

                // 5
                t.AddByStateStoreAndTracks("3,0", "B,0", "4,B", "*,0", TMDirection.Left);
                t.AddByStateStoreAndTracks("3,1", "B,1", "4,B", "*,1", TMDirection.Left);

                // 6
                t.AddByStateStoreAndTracks("4,B", "*,0", "4,B", "*,0", TMDirection.Left);
                t.AddByStateStoreAndTracks("4,B", "*,1", "4,B", "*,1", TMDirection.Left);

                // 7
                t.AddByStateStoreAndTracks("4,B", "B,c", "5,B", "B,c", TMDirection.Left);

                //8
                t.AddByStateStoreAndTracks("5,B", "B,0", "6,B", "B,0", TMDirection.Left);
                t.AddByStateStoreAndTracks("5,B", "B,1", "6,B", "B,1", TMDirection.Left);

                //9
                t.AddByStateStoreAndTracks("6,B", "B,0", "6,B", "B,0", TMDirection.Left);
                t.AddByStateStoreAndTracks("6,B", "B,1", "6,B", "B,1", TMDirection.Left);

                //10
                t.AddByStateStoreAndTracks("6,B", "*,0", "1,B", "*,0", TMDirection.Right);
                t.AddByStateStoreAndTracks("6,B", "*,1", "1,B", "*,1", TMDirection.Right);

                //11
                t.AddByStateStoreAndTracks("5,B", "*,0", "7,B", "*,0", TMDirection.Right);
                t.AddByStateStoreAndTracks("5,B", "*,1", "7,B", "*,1", TMDirection.Right);

                //12
                t.AddByStateStoreAndTracks("7,B", "B,c", "8,B", "B,c", TMDirection.Right);

                //13
                t.AddByStateStoreAndTracks("8,B", "*,0", "8,B", "*,0", TMDirection.Right);
                t.AddByStateStoreAndTracks("8,B", "*,1", "8,B", "*,1", TMDirection.Right);

                //14
                t.AddByStateStoreAndTracks("8,B", "B,B", "9,B", "B,B", TMDirection.Right);

                var tm = new TuringMachineMultiTrack(nameof(TMk_EFAK_B87_wcw), 2, stateTracks, inpAlph, bandSymbols, t, startState, 'B', acceptedStates);
                return tm;
            }
        }
    }
}