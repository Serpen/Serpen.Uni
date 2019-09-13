namespace Serpen.Uni {
    
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class AlgorithmComplexityAttribute : System.Attribute
    {
        public string LandauComplexity {get;}
        public AlgorithmComplexityAttribute(string landauComplexity) => this.LandauComplexity = landauComplexity;
    }

    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    sealed class AlgorithmSourceAttribute : System.Attribute
    {
        public string Source {get;}
        public AlgorithmSourceAttribute(string landauComplexity) => this.Source = landauComplexity;
    }
}