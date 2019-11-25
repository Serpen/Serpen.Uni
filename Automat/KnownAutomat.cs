using System.Collections.Generic;
using System.Reflection;

namespace Serpen.Uni.Automat {
    public static partial class KnownAutomat {

        public static readonly char[] binAlp = new char[] { '0', '1' };


        internal static List<T> GetTypes<T>() {
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
        public static Finite.DFA[] GetDFAModels(int randomCount = 10) {
            var list = GetTypes<Finite.DFA>();

            list.Add(KnownAutomat.DEA_BinFreq(3, 5));
            list.Add(KnownAutomat.DEA_ContainsOnes(6));
            list.Add(KnownAutomat.DEA_EndsWithNulls(4));

            for (int i = 0; i < randomCount; i++)
                list.Add(Finite.DFA.GenerateRandom());
            return list.ToArray();
        }

        public static ContextFree.CFGrammer[] GetCFGs(int randomCount = 10) {
            var list = GetTypes<ContextFree.CFGrammer>();

            for (int i = 0; i < randomCount; i++)
                list.Add(ContextFree.CFGrammer.GenerateRandom());

            return list.ToArray();
        }

        /// <summary>
        /// Returns a list of all KnowAutomat NEA_ Fields, current not dyn methods
        /// </summary>
        /// <returns></returns>
        public static Finite.NFA[] GetNFAModels(int randomCount = 10) {
            var list = GetTypes<Finite.NFA>();

            list.Add(KnownAutomat.NEA_XlastIsOne(5));

            for (int i = 0; i < randomCount; i++)
                list.Add(Finite.NFA.GenerateRandom());

            return list.ToArray();
        }

        public static Finite.NFAe[] GetNFAeModels(int randomCount = 10) {
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
            AList.AddRange(GetNFAeModels());
            AList.AddRange(GetTypes<ContextFree.DPDA>());
            AList.AddRange(GetTypes<ContextFree.PDA>());
            AList.AddRange(GetTypes<ContextFree.StackPDA>());
            AList.AddRange(GetTypes<ContextFree.StatePDA>());
            AList.AddRange(GetTypes<Turing.TuringMachineSingleBand>());
            AList.AddRange(GetTypes<Turing.TuringMachineSingleBand1659>());
            AList.AddRange(GetTypes<Turing.TuringMachineMultiTrack>());
            AList.AddRange(GetTypes<Turing.NTM1659>());
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

        static bool MemberNameFilter(MemberInfo objMemberInfo, object objSearch)
            => objMemberInfo.Name.StartsWith(objSearch.ToString());


    }
}