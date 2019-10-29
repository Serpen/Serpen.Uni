namespace Serpen.Uni.Automat.Finite {
    public struct EATuple : ITransformKey {
        public uint q {get;}
        public char? c {get;}
        char[] ITransformKey.c {
            get {
                if (c.HasValue)
                    return new char[] {c.Value};
                else
                    return new char[] {};
            }
        }

        public EATuple(uint i, char? c) {
            this.q = i;
            this.c = c;
        }

        public override string ToString() => $"({q}),'{c}')";

        public override int GetHashCode() => ToString().GetHashCode();

        public override bool Equals(object obj) => (obj is EATuple eat) && (eat.q == this.q && eat.c == this.c);
    }
}