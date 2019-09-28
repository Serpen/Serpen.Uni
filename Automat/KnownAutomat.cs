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
        public static Finite.DFA[] GetDFAModels() {
            var list = GetTypes<Finite.DFA>();

            list.Add(KnownAutomat.DEA_BinFreq(3, 5));
            list.Add(KnownAutomat.DEA_ContainsOnes(6));
            list.Add(KnownAutomat.DEA_EndsWithNulls(4));
            return list.ToArray();
        }

        public static ContextFree.CFGrammer[] GetCFGs() {
            var list = GetTypes<ContextFree.CFGrammer>();
            return list.ToArray();
        }

        /// <summary>
        /// Returns a list of all KnowAutomat NEA_ Fields, current not dyn methods
        /// </summary>
        /// <returns></returns>
        public static Finite.NFA[] GetNFAModels() {
            var Neas = GetTypes<Finite.NFA>();
            
            Neas.Add(KnownAutomat.NEA_XlastIsOne(5));
            return Neas.ToArray();
        }

        /// <summary>
        /// Returns a list of all KnowAutomat NEA_ Fields, current not dyn methods
        /// </summary>
        /// <returns></returns>
        public static Finite.NFAe[] GetNFAeModels() {
            var NeaEs = GetTypes<Finite.NFAe>();
            return NeaEs.ToArray();
        }

        public static ContextFree.CFGrammer[] GetCFGrammer() {
            var list = GetTypes<ContextFree.CFGrammer>();
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
            AList.AddRange(GetTypes<ContextFree.PDA>());
            AList.AddRange(GetTypes<ContextFree.StackPDA>());
            AList.AddRange(GetTypes<ContextFree.StatePDA>());
            return AList.ToArray();
        }

        public static ContextFree.PDA[] GetAllContextFreeAutomats() {
            var AList = new List<ContextFree.PDA>();
            AList.AddRange(GetTypes<ContextFree.PDA>());
            AList.AddRange(GetTypes<ContextFree.StackPDA>());
            AList.AddRange(GetTypes<ContextFree.StatePDA>());
            return AList.ToArray();
        }

        static bool MemberTypeFilter(MemberInfo objMemberInfo, object objSearch) {
            if (objMemberInfo is PropertyInfo objPropertyInfo) 
                if (objSearch is System.Type objSearchType)
                    return objPropertyInfo.PropertyType == objSearchType;
            return false;            
        }

        static bool MemberNameFilter(MemberInfo objMemberInfo, object objSearch) 
            => objMemberInfo.Name.StartsWith(objSearch.ToString());

        
    }
}