using System;
using Serpen.Uni.Automat.Finite;

namespace Serpen.Uni.Automat {
    public static partial class KnownAutomat {
        public static DFA DEA_Contains01 {
            get {
                var deaT = new FATransform();
                deaT.AddBinTuple(0, 1, 0);
                deaT.AddBinTuple(1, 1, 2);
                deaT.AddBinTuple(2, 2, 2);
                return new DFA(nameof(DEA_Contains01), 3, binAlp, deaT, 0, new uint[] { 2 });
            }
        }
        public static DFA DEA_StartOrEndsWith01 {
            get {
                var deaT = new FATransform();
                deaT.AddBinTuple(0, 1, 3);
                deaT.AddBinTuple(1, 4, 2);
                deaT.AddBinTuple(2, 2, 2);
                deaT.AddBinTuple(3, 4, 3);
                deaT.AddBinTuple(4, 4, 5);
                deaT.AddBinTuple(5, 4, 3);
                return new DFA(nameof(DEA_StartOrEndsWith01), 6, binAlp, deaT, 0, 2, 5);
            }
        }
        public static DFA DEA_SameCount0And1 {
            get {
                var deaT = new FATransform();
                deaT.AddBinTuple(0, 1, 2);
                deaT.AddBinTuple(1, 0, 3);
                deaT.AddBinTuple(2, 3, 0);
                deaT.AddBinTuple(3, 2, 1);
                return new DFA(nameof(DEA_SameCount0And1), 4, binAlp, deaT, 0, 0);
            }
        }
        public static DFA DEA_1659_M1_A22_0even {
            get {
                var deat = new FATransform();
                deat.AddBinTuple(0, 1, 0);
                deat.AddBinTuple(1, 0, 1);
                return new DFA(nameof(DEA_1659_M1_A22_0even), 2, binAlp, deat, 0, 0);
            }
        }

        public static DFA DEA_1659_M2_A24_01 {
            get {
                var deat = new FATransform();
                deat.AddBinTuple(0, 1, 0);
                deat.AddBinTuple(1, 1, 2);
                deat.AddBinTuple(2, 2, 2);
                return new DFA(nameof(DEA_1659_M2_A24_01), 3, binAlp, deat, 0, 2);
            }
        }
        public static DFA DEA_1659_A223_B29_RegExp {
            get {
                var deaT = new FATransform();
                deaT.Add(0, 'b', 0);
                deaT.Add(0, 'a', 1);
                deaT.Add(0, 'c', 1);
                deaT.Add(1, 'a', 1);
                deaT.Add(1, 'b', 1);
                deaT.Add(1, 'c', 0);
                return new DFA(nameof(DEA_1659_A223_B29_RegExp), 2, new char[] { 'a', 'b', 'c' }, deaT, 0, 0);
            }
        }
        public static DFA DEA_1659_T27_A224 {
            get {
                var deaT = new FATransform();
                deaT.AddBinTuple(0, 1, 2);
                deaT.AddBinTuple(1, 2, 1);
                deaT.AddBinTuple(2, 2, 2);
                return new DFA(nameof(DEA_1659_T27_A224), 3, binAlp, deaT, 0, 0, 2); ;
            }
        }
        public static DFA DEA_1659_M3_A25_c {
            get {
                var deaT = new FATransform();
                deaT.Add(0, 'c', 1);
                deaT.Add(1, 'c', 2);
                deaT.Add(2, 'c', 3);
                deaT.Add(3, 'c', 4);
                deaT.Add(4, 'c', 0);
                return new DFA(nameof(DEA_1659_M3_A25_c), 5, new char[] { 'c' }, deaT, 0, 2);
            }
        }
        public static DFA DEA_1659_T21_AE {
            get {
                var deaT = new FATransform();
                deaT.AddBinTuple(0, 1, 3);
                deaT.AddBinTuple(1, 1, 2);
                deaT.AddBinTuple(2, 1, 2);
                deaT.AddBinTuple(3, 4, 3);
                deaT.AddBinTuple(4, 4, 3);
                return new DFA(nameof(DEA_1659_T21_AE), 5, binAlp, deaT, 0, 2, 4);
            }
        }
        public static DFA DEA_1659_A26_Tree {
            get {
                var deaT = new FATransform();
                deaT.AddBinTuple(0, 1, 2);
                deaT.AddBinTuple(1, 3, 4);
                deaT.AddBinTuple(2, 5, 6);
                deaT.AddBinTuple(3, 7, 7);
                deaT.AddBinTuple(4, 7, 7);
                deaT.AddBinTuple(5, 7, 7);
                deaT.AddBinTuple(6, 7, 7);
                deaT.AddBinTuple(7, 7, 7);
                return new DFA(nameof(DEA_1659_A26_Tree), 8, binAlp, deaT, 0, 0, 1, 6);
            }
        }
        public static DFA DEA_1659_A213_M1_a {
            get {
                var deaT = new FATransform();
                deaT.Add(0, 'a', 1);
                deaT.Add(1, 'a', 2);
                deaT.Add(2, 'a', 3);
                deaT.Add(3, 'a', 4);
                deaT.Add(4, 'a', 5);
                deaT.Add(5, 'a', 0);
                return new DFA(nameof(DEA_1659_A213_M1_a), 6, new char[] { 'a' }, deaT, 0, 0, 3);
            }
        }
        public static DFA DEA_1659_A213_M1_b {
            get {
                var deaT = new FATransform();
                deaT.Add(0, 'a', 1);
                deaT.Add(1, 'a', 2);
                deaT.Add(2, 'a', 0);
                return new DFA(nameof(DEA_1659_A213_M1_b), 3, new char[] { 'a' }, deaT, 0, 0);
            }
        }
        public static DFA DEA_A21_B25_MN {
            get {
                var deaT = new FATransform();
                deaT.AddBinTuple(0,1,0);
                deaT.AddBinTuple(1,2,0);
                deaT.AddBinTuple(2,2,0);
                return new DFA(nameof(DEA_A21_B25_MN), new string[] {$"[{Utils.EPSILON.ToString()}]", "[0]", "[00]"}, binAlp, deaT, 0, 2);
            }
        }
        public static DFA DEA_1659_A215_M3_TF {
            get {
                var deaT = new FATransform();
                deaT.AddBinTuple(0, 1, 2);
                deaT.AddBinTuple(1, 3, 2);
                deaT.AddBinTuple(2, 4, 0);
                deaT.AddBinTuple(3, 3, 3);
                deaT.AddBinTuple(4, 3, 0);
                return new DFA(nameof(DEA_1659_A215_M3_TF), 5, binAlp, deaT, 0, 0, 2, 3);
            }
        }
        public static DFA DEA_1659_A225_M4_pump {
            get {
                var deaT = new FATransform();
                deaT.AddBinTuple(0, 0, 1);
                deaT.AddBinTuple(1, 0, 2);
                deaT.AddBinTuple(2, 3, 2);
                deaT.AddBinTuple(3, 0, 1);
                return new DFA(nameof(DEA_1659_A225_M4_pump), 4, binAlp, deaT, 0, 3);
            }
        }
        public static DFA DEA_1659_A227_T21 {
            get {
                var deaT = new FATransform();
                deaT.AddBinTuple(0, 1, 3);
                deaT.AddBinTuple(1, 1, 2);
                deaT.AddBinTuple(2, 1, 2);
                deaT.AddBinTuple(3, 4, 3);
                deaT.AddBinTuple(4, 4, 3);
                return new DFA(nameof(DEA_1659_A227_T21), 5, binAlp, deaT, 0, 2,4);
            }
        }
        
        public static DFA DEA_1659_A228_T22_g_ {
            get => Converter.Nea2TeilmengenDea(NEAe_1659_T22_A212_N3);
        }
        public static DFA DEA_1659_A228_T22 {
            get {
                var deaT = new FATransform();
                deaT.Add(0, '0', 5);
                deaT.Add(0, '1', 1);
                deaT.Add(1, '0', 2);
                deaT.Add(1, '1', 1);
                deaT.Add(2, '0', 5);
                deaT.Add(2, '1', 3);
                deaT.Add(3, '0', 2);
                deaT.Add(3, '1', 3);
                deaT.Add(4, '0', 0);
                deaT.Add(4, '1', 4);
                deaT.Add(5, '0', 6);
                deaT.Add(5, '1', 4);
                deaT.Add(6, '0', 6);
                deaT.Add(6, '1', 6);
                return new DFA(nameof(DEA_1659_A228_T22), new string[] {"A,B","A,B,C","A,B,D","A,B,C,D","D,C","D", "0"}, binAlp, deaT, 0, 1,3,4);
            }
        }
        public static DFA DEA_1659_A219 {
            get => DFA.UnionProduct(DEA_1659_M1_A22_0even, DEA_1659_M2_A24_01);
        }
        public static NFAe NEAe_1659_A220 {
            get => DEA_1659_M1_A22_0even.UnionNEA(DEA_1659_M2_A24_01);
        }

        public static DFA DEA_1659_A216_M3_min {
            get => DEA_1659_A215_M3_TF.MinimizeTF();
        }
        public static DFA DEA_1659_A217_TF {
            get {
                var deat = new FATransform();
                deat.AddBinTuple(0, 3, 2);
                deat.AddBinTuple(1, 0, 2);
                deat.AddBinTuple(2, 3, 3);
                deat.AddBinTuple(3, 0, 4);
                deat.AddBinTuple(4, 5, 3);
                deat.AddBinTuple(5, 0, 4);
                return new DFA(nameof(DEA_1659_A217_TF), 6, binAlp, deat, 0, 0);
            }
        }
        public static DFA DEA_EAFK_A48_max {
            get {
                var deat = new FATransform();
                deat.AddBinTuple(0, 1, 5);
                deat.AddBinTuple(1, 6, 2);
                deat.AddBinTuple(2, 0, 2);
                deat.AddBinTuple(3, 2, 6);
                deat.AddBinTuple(4, 7, 5);
                deat.AddBinTuple(5, 2, 6);
                deat.AddBinTuple(6, 6, 4);
                deat.AddBinTuple(7, 6, 2);
                return new DFA(nameof(DEA_EAFK_A48_max), 8, binAlp, deat, 0, 2);
            }
        }
        public static DFA DEA_EAFK_A410_a {
            get {
                var deata = new FATransform();
                deata.AddBinTuple(0, 0, 1);
                deata.AddBinTuple(1, 0, 1);
                return new DFA(nameof(DEA_EAFK_A410_a), 2, binAlp, deata, 0, 0);
            }
        }
        public static DFA DEA_EAFK_A410_b {
            get {
                var deatb = new FATransform();
                deatb.AddBinTuple(0, 1, 2);
                deatb.AddBinTuple(1, 1, 2);
                deatb.AddBinTuple(2, 0, 2);
                return new DFA(nameof(DEA_EAFK_A410_b), 3, binAlp, deatb, 0, 0, 1);
            }
        }
        public static NFA NEA_EndsWith01 {
            get {
                var neaT = new NFAeTransform();
                neaT.Add(0, '0', 0, 1);
                neaT.Add(0, '1', 0);
                neaT.Add(1, '1', 2);
                return new NFA(nameof(NEA_EndsWith01), 3, binAlp, neaT, 0, new uint[] { 2 });
            }
        }
        public static NFA NEA_EAFK_A211_webay {
            get {
                var WebEbayAlphabet = new char[] { 'w', 'e', 'b', 'a', 'y' };
                var neaT = new NFAeTransform();
                neaT.Add(0, 'b', 0);
                neaT.Add(0, 'a', 0);
                neaT.Add(0, 'y', 0);
                neaT.Add(0, 'w', 0, 1);
                neaT.Add(0, 'e', 0, 4);

                neaT.Add(1, 'e', 2);
                neaT.Add(2, 'b', 3);

                neaT.Add(4, 'b', 5);
                neaT.Add(5, 'a', 6);
                neaT.Add(6, 'y', 7);

                return new NFA(nameof(NEA_EAFK_A211_webay), 8, WebEbayAlphabet, neaT, 0, 3, 7);
            }
        }
        public static NFAe NEAe_EAFK_A216_EpsilonTest {
            get {
                NFAeTransform neaT = new NFAeTransform();
                neaT.Add(0, null, 1, 3);
                neaT.Add(1, null, 2);
                neaT.Add(2, null, 5);
                neaT.Add(3, '0', 4);
                neaT.Add(4, '1', 5);
                neaT.Add(4, null, 6);

                return new NFAe(nameof(NEAe_EAFK_A216_EpsilonTest), 7, binAlp, neaT, 0, 6);
            }
        }
        public static NFAe NEAe_Simple {
            get {
                NFAeTransform neaT = new NFAeTransform();
                neaT.Add(0, '0', 0);
                neaT.Add(0, '1', 0,1);
                neaT.Add(1, '0', 1);
                neaT.Add(1, '1', 1);

                return new NFAe(nameof(NEAe_Simple), 2, binAlp, neaT, 0, 1);
            }
        }
        public static NFAe NEAe_EAFK_A213_Dec {
            get {
                var Decimals = new char[] { 'D', '+', '-', '.' };
                NFAeTransform neaET = new NFAeTransform();
                neaET.Add(0, null, 1);
                neaET.Add(0, '+', 1);
                neaET.Add(0, '-', 1);
                neaET.Add(1, 'D', 1, 4);
                neaET.Add(1, '.', 2);
                neaET.Add(2, 'D', 3);
                neaET.Add(3, 'D', 3);
                neaET.Add(3, null, 4); //->5?
                neaET.Add(4, '.', 3);
                return new NFAe(nameof(NEAe_EAFK_A213_Dec), 5, Decimals, neaET, 0, 5);
            }
        }
        public static NFAe NEAe_1659_A27_N1 {
            get {
                var neaET = new NFAeTransform();
                neaET.Add(0, '0', 0, 1);
                neaET.Add(0, '1', 0);
                neaET.Add(1, '0', 2);
                neaET.Add(1, null, 2);
                neaET.Add(2, '1', 3);
                neaET.Add(3, '0', 3);
                neaET.Add(3, '1', 3);
                return new NFAe(nameof(NEAe_1659_A27_N1), 4, binAlp, neaET, 0, new uint[] { 3 });
            }
        }
        public static DFA DEA_1659_A210_PotAut {
            get => Converter.Nea2TeilmengenDea(NEAe_1659_A27_N1);
        }
        public static NFAe NEAe_1659_T22_A212_N3 {
            get {
                var neaET = new NFAeTransform();
                neaET.Add(0, null, 1);
                neaET.Add(0, '1', 0);
                neaET.Add(1, '0', 3);
                neaET.Add(1, '1', 2);
                neaET.Add(2, '0', 0);
                neaET.Add(3, '1', 2, 3);
                return new NFAe(nameof(NEAe_1659_T22_A212_N3), 4, binAlp, neaET, 0, 2);
            }
        }

        public static NFA NEA_1659_N2_A28 {
            get {
                var neaT = new NFAeTransform();
                neaT.Add(0, '0', 0);
                neaT.Add(0, '1', 0, 1);
                neaT.Add(1, '0', 2);
                neaT.Add(2, '1', 3);
                neaT.Add(3, '0', 3);
                neaT.Add(3, '1', 3);
                return new NFA("NEA_N2_A28", 4, binAlp, neaT, 0, 3);
            }
        }

        public static DFA DEA_EndsWithNulls(uint count) {
            var t = new FATransform();
            for (uint i = 0; i < count + 1; i++) {
                t.Add(i, '1', 0);
                if (i < count)
                    t.Add(i, '0', i + 1);
                else
                    t.Add(i, '0', i);
            }

            return new DFA($"DEA_EndsWithNulls({count})", count + 1, binAlp, t, 0, count);
        }

        public static DFA DEA_ContainsOnes(uint count) {
            var t = new FATransform();
            for (uint i = 0; i < count + 1; i++) {
                t.Add(i, '0', i);
                if (i < count)
                    t.Add(i, '1', i + 1);
                else
                    t.Add(i, '1', i);
            }

            return new DFA($"DEA_ContainsOnes({count})", count + 1, binAlp, t, 0, count);
        }

        public static DFA DEA_BinFreq(uint zeros, uint ones) {
            if (zeros == 0 | ones == 0)
                throw new ArgumentOutOfRangeException();

            var t = new FATransform();
            for (uint i = 0; i < zeros * ones; i++)
                if (i < (zeros - 1) * ones)
                    t.Add(i, '0', i + ones);
                else
                    t.Add(i, '0', i % ones);

            for (uint i = 0; i < zeros; i++) {
                for (uint o = 0; o < ones - 1; o++)
                    t.Add(i * ones + o, '1', i * ones + o + 1);
                uint last = i * (ones) + ones - 1;
                t.Add(last, '1', last - (ones - 1));
            }

            //naming states
            string[] names = new string[zeros * ones];
            for (uint i = 0; i < zeros * ones; i++) {
                int remain, quotient;
                quotient = Math.DivRem((int)i, (int)ones, out remain);
                names[i] = $"{quotient}/{remain}";
            }
            return new DFA($"DEA_BinFreq({zeros}, {ones})", names, binAlp, t, 0, new uint[] { 0 });
        }

        public static NFA NEA_XlastIsOne(uint count) {
            var t = new NFAeTransform();
            t.Add(0, '0', 0);
            t.Add(0, '1', 0, 1);
            for (uint i = 1; i < count; i++) {
                t.Add(i, '0', i + 1);
                t.Add(i, '1', i + 1);
            }
            return new NFA($"NEA_XlastIsOne({count})", count + 1, binAlp, t, 0, count);
        }
    }
}