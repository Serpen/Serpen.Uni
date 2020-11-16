using System.Collections.Generic;
using System.Reflection;

namespace Serpen.Uni.Automat {
    public static partial class KnownAutomat {

        public static readonly char[] binAlp = new char[] { '0', '1' };
        private const int RND_AUTOMAT_COUNT = 20;


        internal static List<T> GetTypes<T>() where T : IAcceptWord {
            var list = new List<T>();
            var mems = typeof(KnownAutomat).FindMembers(MemberTypes.Property, BindingFlags.Public | BindingFlags.Static,
                new MemberFilter(MemberTypeFilter), typeof(T));
            foreach (var mem in mems)
                list.Add((T)(((PropertyInfo)mem).GetValue(null)));
            return list;
        }

        /// <summary>
        /// Returns a list of all KnowAutomat DEA_ Fields, current not dyn methods
        /// </summary>
        /// <returns></returns>
        public static Finite.DFA[] GetDFAModels(int randomCount = RND_AUTOMAT_COUNT) {
            var list = GetTypes<Finite.DFA>();

            list.Add(KnownAutomat.DEA_BinFreq(3, 5));
            list.Add(KnownAutomat.DEA_ContainsOnes(6));
            list.Add(KnownAutomat.DEA_EndsWithNulls(4));

            for (int i = 0; i < randomCount; i++)
                list.Add(Finite.DFA.GenerateRandom());
            return list.ToArray();
        }


        public static ContextFree.CFGrammer[] GetCFGs(int randomCount = RND_AUTOMAT_COUNT) {
            var list = GetTypes<ContextFree.CFGrammer>();

            for (int i = 0; i < randomCount; i++)
                list.Add(ContextFree.CFGrammer.GenerateRandom());

            return list.ToArray();
        }

        /// <summary>
        /// Returns a list of all KnowAutomat NEA_ Fields, current not dyn methods
        /// </summary>
        /// <returns></returns>
        public static Finite.NFA[] GetNFAModels(int randomCount = RND_AUTOMAT_COUNT) {
            var list = GetTypes<Finite.NFA>();

            list.Add(KnownAutomat.NEA_XlastIsOne(5));

            for (int i = 0; i < randomCount; i++)
                list.Add(Finite.NFA.GenerateRandom());

            return list.ToArray();
        }

        public static Finite.NFAe[] GetNFAeModels(int randomCount = RND_AUTOMAT_COUNT) {
            var list = GetTypes<Finite.NFAe>();

            for (int i = 0; i < randomCount; i++)
                list.Add(Finite.NFAe.GenerateRandom());


            return list.ToArray();
        }

        public static Finite.FABase[] GetAllFiniteAutomats() {
            var AList = new List<Finite.FABase>();
            AList.AddRange(GetDFAModels());
            AList.AddRange(GetNFAModels());
            AList.AddRange(GetNFAeModels());
            return AList.ToArray();
        }

        public static IAutomat[] GetAllAutomats() {
            var AList = new List<IAutomat>();
            AList.AddRange(GetDFAModels());
            AList.AddRange(GetNFAModels());
            AList.AddRange(GetTypes<Finite.DFA>());
            AList.AddRange(GetTypes<Finite.NFA>());
            AList.AddRange(GetTypes<Finite.NFAe>());
            AList.AddRange(GetTypes<ContextFree.DPDA>());
            AList.AddRange(GetTypes<ContextFree.PDA>());
            AList.AddRange(GetTypes<ContextFree.StackPDA>());
            AList.AddRange(GetTypes<ContextFree.StatePDA>());
            AList.AddRange(GetTypes<Turing.TuringMachineSingleBand>());
            AList.AddRange(GetTypes<Turing.TuringMachineSingleBand1659>());
            AList.AddRange(GetTypes<Turing.TuringMachineMultiTrack>());
            AList.AddRange(GetTypes<Turing.NTM1659>());
            AList.AddRange(Tests.GenerateRandomAutomats(RND_AUTOMAT_COUNT));
            return AList.ToArray();
        }

        public static ContextFree.IPDA[] GetAllContextFreeAutomats() {
            var AList = new List<ContextFree.IPDA>();
            AList.AddRange(GetTypes<ContextFree.PDA>());
            AList.AddRange(GetTypes<ContextFree.DPDA>());
            AList.AddRange(GetTypes<ContextFree.StackPDA>());
            AList.AddRange(GetTypes<ContextFree.StatePDA>());
            return AList.ToArray();
        }

        static bool MemberTypeFilter(MemberInfo objMemberInfo, object objSearch) {
            if (objMemberInfo is PropertyInfo objPropertyInfo)
                if (objSearch is System.Type objSearchType)
                    return objPropertyInfo.PropertyType == objSearchType | objSearchType.IsAssignableFrom(objPropertyInfo.PropertyType);
            return false;
        }

        public static Finite.NFAe HammingGraph(byte count) {
            var transform = new Finite.NFAeTransform();
            for (uint v1 = 0; v1 < count; v1++)
                for (uint v2 = 0; v2 < count; v2++)
                    if (Serpen.Uni.Utils.HammingDistance(v1, v2) == 1)
                        transform.AddM(v1,'1', v2);
            var ret = new Finite.NFAe("Hamming-" + count, count, new char[] {'1'}, transform, 0, new uint[] {});
            for (int v = 0; v < count; v++)
                ret.States[v] = System.Convert.ToString(v,2).PadLeft(Uni.Utils.Log2(count),'0');
            return ret;
        }
    }
}