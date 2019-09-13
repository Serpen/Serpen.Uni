namespace Serpen.Uni.Automat.Finite {
    public struct EATuple {
        public uint q {get;}
        public char? c {get;}

        public EATuple(uint i, char? c) {
            this.q = i;
            this.c = c;
        }

        public override string ToString() => $"({q}),'{c}')";

        public override int GetHashCode() => ToString().GetHashCode();

        public override bool Equals(object obj) {
            if (obj is EATuple eat)
                return eat.q == this.q && eat.c == this.c;
            return false;
        }
    }

    public struct DFATuple {

        public DFATuple(uint i, char c) {
            this.q = i;
            this.c = c;
        }

        public uint q {get;}
        public char c {get;}

        public override string ToString() => $"({q}),'{c}')";

        public override int GetHashCode() => ToString().GetHashCode();

        public override bool Equals(object obj) {
            if (obj is EATuple eat)
                return eat.q == this.q && eat.c == this.c;
            return false;
        }
    }
}