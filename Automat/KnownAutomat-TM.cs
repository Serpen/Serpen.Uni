using Serpen.Uni.Automat.Turing;

namespace Serpen.Uni.Automat {
    public static partial class KnownAutomat {

        const char BLK = TuringMachineSingleBand1659.BLANK;

        [AcceptedWordSamples("aba")]
        public static TuringMachineSingleBand1659 TM_1659_A44_M1 {
            get {
                var t = new TuringTransformSingleBand {
                    { 0, 'a', 0, 'b', TMDirection.Right },
                    { 0, 'b', 0, 'a', TMDirection.Right },
                    { 0, BLK, 1, 'b', TMDirection.Left },
                    { 1, 'b', 3, 'b', TMDirection.Right },
                    { 1, 'a', 2, 'a', TMDirection.Left },
                    { 2, 'b', 0, 'b', TMDirection.Left },
                    { 2, 'a', 4, 'a', TMDirection.Right }
                };

                char[] inputAlphabet = new char[] { 'a', 'b' };
                char[] bandAlphabet = new char[] { 'a', 'b', BLK };
                return new TuringMachineSingleBand1659(nameof(TM_1659_A44_M1), 5, inputAlphabet, bandAlphabet, t, 0, BLK, 3, 4);
            }
        }

        [AcceptedWordSamples("aa")]
        public static TuringMachineSingleBand1659 TM_1659_A46_M2_a2n {
            get {
                const uint qA = 6;
                const uint qD = 5;

                var t = new TuringTransformSingleBand {
                    { 0, 'a', 1, 's', TMDirection.Right },
                    { 0, BLK, qD, BLK, TMDirection.Right },

                    { 1, 'x', 1, 'x', TMDirection.Right },
                    { 1, 'a', 2, 'x', TMDirection.Right },
                    { 1, BLK, qA, BLK, TMDirection.Right },

                    { 2, 'x', 2, 'x', TMDirection.Right },
                    { 2, 'a', 3, 'a', TMDirection.Right },
                    { 2, BLK, 4, BLK, TMDirection.Left },

                    { 3, 'x', 3, 'x', TMDirection.Right },
                    { 3, 'a', 2, 'x', TMDirection.Right },
                    { 3, BLK, qD, BLK, TMDirection.Right },

                    { 4, 'a', 4, 'a', TMDirection.Left },
                    { 4, 'x', 4, 'x', TMDirection.Left },
                    { 4, 's', 1, 's', TMDirection.Right }
                };

                char[] inputAlphabet = new char[] { 'a' };
                char[] bandAlphabet = new char[] { 'a', 'x', 's', BLK };

                return new TuringMachineSingleBand1659(nameof(TM_1659_A46_M2_a2n), 7, inputAlphabet, bandAlphabet, t, 0, BLK, qA, qD);
            }
        }

        public static TuringMachineSingleBand1659 TM_1659_T42_A416_u_u {
            get {
                const uint D = 8;
                const uint F = 7;

                var t = new TuringTransformSingleBand {
                    {0, 'x', 0, 'x', TMDirection.Right},
                    {0, 'a', 1, 'x', TMDirection.Right},
                    {0, 'b', 2, 'x', TMDirection.Right},
                    {0, BLK, D, BLK, TMDirection.Right},
                    {0, '$', 6, '$', TMDirection.Right},

                    {1, 'a', 1, 'a', TMDirection.Right},
                    {1, 'b', 1, 'b', TMDirection.Right},
                    {1, '$', 3, '$', TMDirection.Right},

                    {3, 'x', 3, 'x', TMDirection.Right},
                    {3, 'a', 5, 'x', TMDirection.Left},
                    {3, 'b', D, 'b', TMDirection.Right},

                    {2, 'a', 2, 'a', TMDirection.Right},
                    {2, 'b', 2, 'b', TMDirection.Right},
                    {2, '$', 4, '$', TMDirection.Right},

                    {4, 'x', 4, 'x', TMDirection.Right},
                    {4, 'a', D, 'a', TMDirection.Right},
                    {4, 'b', 5, 'x', TMDirection.Left},

                    {5, 'a', 5, 'a', TMDirection.Left},
                    {5, 'b', 5, 'b', TMDirection.Left},
                    {5, 'x', 5, 'x', TMDirection.Left},
                    {5, '$', 5, '$', TMDirection.Left},
                    {5, BLK, 0, BLK, TMDirection.Right},

                    {6, BLK, F, BLK, TMDirection.Right},
                    {6, 'x', 6, 'x', TMDirection.Right},
                };

                return new TuringMachineSingleBand1659(nameof(TM_1659_T42_A416_u_u), 9, 
                new char[] {'a', 'b', '$'}, new char[] {'a', 'b', 'x', '$', BLK}, t, 0, BLK, 7, 8);
            }
        }

        [AcceptedWordSamples("0","0000")]
        public static TuringMachineSingleBand1659 TM_1659_A414_M1 {
            get {
                const uint qA = 1;
                const uint qD = 2;

                var t = new TuringTransformSingleBand {
                    { 0, '0', 0, '0', TMDirection.Right },
                    { 0, '1', qD, BLK, TMDirection.Left },
                    { 0, BLK, qA, '0', TMDirection.Left }
                };

                char[] bandAlphabet = new char[] { '0', '1', BLK };

                return new TuringMachineSingleBand1659(nameof(TM_1659_A414_M1), 3, binAlp, bandAlphabet, t, 0, BLK, qA, qD);
            }
        }

        public static TuringMachineSingleBand1659 TM_1659_46_M0_D {
            get {
                var t = new TuringTransformSingleBand {
                    {0, '0', 2, BLK, TMDirection.Right},
                    {0, '1', 2, BLK, TMDirection.Right},
                    {0, BLK, 2, BLK, TMDirection.Right}
                };
                char[] bandAlphabet = new char[] { '0', '1', BLK };
                return new TuringMachineSingleBand1659(nameof(TM_1659_46_M0_D), 3, binAlp, bandAlphabet, t, 0, BLK, 1, 2);
            }
        }

        [AcceptedWordSamples("aaba")]
        public static NTM1659 NTM_1659_A411_N1 {
            get {
                const uint qA = 4;
                const uint qD = 3;

                var t = new NTM1659Transform {
                    { 0, 'a', 0, 'a', TMDirection.Right },
                    { 0, 'b', 0, 'b', TMDirection.Right },
                    { 0, BLK, qD, BLK, TMDirection.Right },

                    { 1, 'a', 2, 'a', TMDirection.Right },
                    { 1, 'b', qD, 'b', TMDirection.Right },
                    { 1, BLK, qD, BLK, TMDirection.Right },

                    { 2, 'b', qA, 'b', TMDirection.Right },
                    { 2, 'a', qD, 'a', TMDirection.Right },
                    { 2, BLK, qD, BLK, TMDirection.Right }
                };
                t.AddM(0, 'a', 1, 'a', TMDirection.Right);

                char[] alphabet = new char[] { 'a', 'b', BLK };

                return new NTM1659(nameof(NTM_1659_A411_N1), 5, new char[] { 'a', 'b' }, alphabet, t, 0, KnownAutomat.BLK, qA, qD);
            }
        }

        [AcceptedWordSamples("0011")]
        public static TuringMachineSingleBand TM_EFAK_A89_B82_T81_0n1n {
            get {
                var t = new TuringTransformSingleBand {
                    { 0, '0', 1, 'X', TMDirection.Right },
                    { 1, 'Y', 1, 'Y', TMDirection.Right },
                    { 1, '0', 1, '0', TMDirection.Right },
                    { 1, '1', 2, 'Y', TMDirection.Left },
                    { 2, 'Y', 2, 'Y', TMDirection.Left },
                    { 2, '0', 2, '0', TMDirection.Left },
                    { 2, 'X', 0, 'X', TMDirection.Right },
                    { 0, 'Y', 3, 'Y', TMDirection.Right },
                    { 3, 'Y', 3, 'Y', TMDirection.Right },
                    { 3, BLK, 4, BLK, TMDirection.Right }
                };

                char[] inputAlphabet = new char[] { '0', '1', 'X', 'Y' };
                char[] blankAlphabet = new char[] { '0', '1', 'X', 'Y', BLK };
                return new TuringMachineSingleBand(nameof(TM_EFAK_A89_B82_T81_0n1n), 5, inputAlphabet, blankAlphabet, t, 0, BLK, new uint[] { 4 });
            }
        }

        public static TuringMachineSingleBand TM_EFAK_A810_B84_Monus {
            get {
                var t = new TuringTransformSingleBand {
                    { 0, '0', 1, BLK, TMDirection.Right },
                    { 0, '1', 5, BLK, TMDirection.Right },
                    { 1, '0', 1, '0', TMDirection.Right },
                    { 1, '1', 2, '1', TMDirection.Right },
                    { 2, '0', 3, '1', TMDirection.Left },
                    { 2, '1', 2, '1', TMDirection.Right },
                    { 2, BLK, 4, BLK, TMDirection.Left },
                    { 3, '0', 3, '0', TMDirection.Left },
                    { 3, '1', 3, '1', TMDirection.Left },
                    { 3, BLK, 0, BLK, TMDirection.Right },
                    { 4, '0', 4, '0', TMDirection.Left },
                    { 4, '1', 4, BLK, TMDirection.Left },
                    { 4, BLK, 6, '0', TMDirection.Right },
                    { 5, '0', 5, BLK, TMDirection.Right },
                    { 5, '1', 5, BLK, TMDirection.Right },
                    { 5, BLK, 6, BLK, TMDirection.Right }
                };
                return new TuringMachineSingleBand(nameof(TM_EFAK_A810_B84_Monus), 7, binAlp, new char[] { '0', '1', BLK }, t, 0, BLK, new uint[] { });
            }
        }

        public static TuringMachineSingleBand TM_EFAK_U825a {
            get {
                const char BLK = TuringMachineSingleBand1659.BLANK;

                var t = new TuringTransformSingleBand {
                    { 0, '0', 1, '1', TMDirection.Right },
                    { 1, '1', 0, '0', TMDirection.Right },
                    { 1, BLK, 3, BLK, TMDirection.Right }
                };

                return new TuringMachineSingleBand(nameof(TM_EFAK_U825a), 4, binAlp, new char[] { '0', '1', BLK }, t, 0, BLK, new uint[] { 3 });
            }
        }

        public static TuringMachineSingleBand TM_EFAK_U825b {
            get {
                const char BLK = TuringMachineSingleBand1659.BLANK;

                var t = new TuringTransformSingleBand {
                    { 0, '0', 0, BLK, TMDirection.Right },
                    { 0, '1', 1, BLK, TMDirection.Right },
                    { 1, '1', 1, BLK, TMDirection.Right },
                    { 1, BLK, 3, BLK, TMDirection.Right }
                };

                return new TuringMachineSingleBand(nameof(TM_EFAK_U825b), 4, binAlp, new char[] { '0', '1', BLK }, t, 0, BLK, new uint[] { 3 });
            }
        }

        public static TuringMachineSingleBand TM_EFAK_U825c {
            get {
                const char BLK = TuringMachineSingleBand1659.BLANK;

                var t = new TuringTransformSingleBand {
                    { 0, '0', 1, '1', TMDirection.Right },
                    { 1, '1', 2, '0', TMDirection.Left },
                    { 2, '1', 0, '1', TMDirection.Right },
                    { 1, BLK, 3, BLK, TMDirection.Right }
                };

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

        [AcceptedWordSamples("00c00")]
        public static TuringMachineMultiTrack TMk_EFAK_B87_wcw {
            get {
                string[] stateTracks = { "1,0", "1,1", "1,B", "2,0", "2,1", "2,B", "3,0", "3,1", "3,B", "4,0", "4,1", "4,B", "5,0", "5,1", "5,B", "6,0", "6,1", "6,B", "7,0", "7,1", "7,B", "8,0", "8,1", "8,B", "9,0", "9,1", "9,B" };

                // string[] bandTracks = { "B,0", "*,0", "B,1", "*,1", "B,c", "*,c", "B,B", "*,B" };
                char[] bandSymbols = { '0', 'A', '1', 'D', 'c', 'E', 'B', 'F' };

                char[] inpAlph = { '0', '1', 'c' };

                uint[] acceptedStates = { stateTracks.ArrayIndex("9,B") };
                uint startState = stateTracks.ArrayIndex("1,B");

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


        public static TuringMachineSingleBand TM_EFAK_A812_copy {
            get {
                var t = new TuringTransformSingleBand {
                    { 0, '0', 1, 'X', TMDirection.Right },
                    { 0, '1', 3, '1', TMDirection.Left },
                    { 1, '0', 1, '0', TMDirection.Right },
                    { 1, '1', 1, '1', TMDirection.Right },
                    { 1, BLK, 2, '0', TMDirection.Left },
                    { 2, '0', 2, '0', TMDirection.Left },
                    { 2, '1', 2, '1', TMDirection.Left },
                    { 2, 'X', 0, 'X', TMDirection.Right },
                    { 3, 'X', 3, '0', TMDirection.Left },
                    { 3, '1', 4, '1', TMDirection.Right }
                };

                char[] bandAlphabet = new char[] { '0', '1', 'X', BLK };

                return new TuringMachineSingleBand(nameof(TM_EFAK_A812_copy), 5, binAlp, bandAlphabet, t, 0, BLK, new uint[] { 4 });
            }
        }

        public static TuringMachineSingleBand TM_EFAK_A813 {
            get {
                uint[] q = new uint[] { 0, 9, 6, 1, 5, 7, 8, 10, 11, 12 };

                var t = new TuringTransformSingleBand();
                t.Add(q[1], BLK, q[0], BLK, TMDirection.Right);
                t.Add(q[0], '0', q[2], BLK, TMDirection.Right);
                t.Add(q[1], '0', q[1], '0', TMDirection.Left);
                t.Add(q[2], '0', q[2], '0', TMDirection.Right);
                t.Add(q[2], '1', q[3], '1', TMDirection.Right);
                t.Insert((TuringTransformSingleBand)TM_EFAK_A812_copy.Transforms, 1);
                t.Add(q[4], '0', q[5], '0', TMDirection.Left);
                t.Add(q[5], '1', q[6], '1', TMDirection.Left);
                t.Add(q[6], BLK, q[7], BLK, TMDirection.Right);
                t.Add(q[6], '0', q[1], '0', TMDirection.Left);
                t.Add(q[7], '1', q[8], BLK, TMDirection.Right);
                t.Add(q[8], '0', q[8], BLK, TMDirection.Right);
                t.Add(q[8], '1', q[9], BLK, TMDirection.Right);

                char[] bandAlphabet = new char[] { '0', '1', 'X', BLK };

                return new TuringMachineSingleBand(nameof(TM_EFAK_A813), 13, binAlp, bandAlphabet, t, 0, BLK, new uint[] { 12 });
            }
        }
    }
}