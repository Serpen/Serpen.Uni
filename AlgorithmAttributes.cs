using System;

namespace Serpen.Uni {
    
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class AlgorithmComplexityAttribute : Attribute
    {
        public string LandauComplexity {get;}
        public AlgorithmComplexityAttribute(string landauComplexity) => this.LandauComplexity = landauComplexity;
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
    sealed class AlgorithmSourceAttribute : Attribute
    {
        public string[] Source {get;}
        public AlgorithmSourceAttribute(params string[] source) => this.Source = source;
    }

    public enum AlgSourceMode { K1659, K1608, EAFK, Wiki }

}