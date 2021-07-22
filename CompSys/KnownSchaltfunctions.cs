namespace Serpen.Uni.CompSys {

    public class KnownSchaltfunctions {
        public static Schaltfunktion AND2 = new Schaltfunktion(2, a => a[0] & a[1]);
        public static Schaltfunktion NAND2 = new Schaltfunktion(2, a => !(a[0] & a[1]));
        public static Schaltfunktion OR2 = new Schaltfunktion(2, a => a[0] | a[1]);
        public static Schaltfunktion NOR2 = new Schaltfunktion(2, a => !(a[0] | a[1]));
        public static Schaltfunktion XOR2 = new Schaltfunktion(2, a => (a[0] & !a[1]) | (!a[0] & a[1]));
        public static Schaltfunktion SF_1608_S22 = new Schaltfunktion(3, a => (a[0] & (a[1] | a[2]) | a[1] & a[2]));
        public static Schaltfunktion DNF_T23 = new Schaltfunktion(3, a => (!a[0] & !a[1] & !a[2]) |
                                                                          (a[0] & a[1] & !a[2]) |
                                                                          (!a[0] & !a[1] & a[2]) |
                                                                          (a[0] & !a[1] & a[2]));
        public static Schaltfunktion KNF_T23 = new Schaltfunktion(3, a => (!a[0] | a[1] | a[2]) &
                                                                          (a[0] | !a[1] | a[2]) &
                                                                          (a[0] | !a[1] | !a[2]) &
                                                                          (!a[0] | !a[1] | !a[2]));
        public static Schaltfunktion DNF_B21 = new Schaltfunktion(3, a => (!a[0] & !a[1] & !a[2]) |
                                                                          (a[0] & a[1] & !a[2]) |
                                                                          (!a[0] & !a[1] & a[2]) |
                                                                          (!a[0] & a[1] & a[2]));
        public static Schaltfunktion KNF_B22 = new Schaltfunktion(3, a => (!a[0] | a[1] | a[2])
                                                                        & (a[0] | !a[1] | a[2])
                                                                        & (a[0] | a[1] | !a[2])
                                                                        & (a[0] | !a[1] | !a[2]));
        public static Schaltfunktion SF_212 = new Schaltfunktion(3, a => (a[0] | a[1]) &
                                                                         !(a[0] & !a[2]) |
                                                                         (a[1] & a[2]));
        public static Schaltfunktion DigFun_Ex429 = new Schaltfunktion(4, a => (!a[1] & !a[2] & !a[3]) |
                                                                         (!a[0] & a[1] & !a[2] & !a[3]) |
                                                                         (a[0] & a[1] & !a[2] & !a[3]) |
                                                                         (!a[0] & !a[1] & a[2] & a[3]) |
                                                                         (a[0] & !a[1] & a[2] & a[3]) |
                                                                         (!a[0] & !a[1] & a[2] & !a[3]) |
                                                                         (!a[0] & a[1] & a[2] & !a[3]) |
                                                                         (a[0] & a[1] & a[2] & !a[3]) |
                                                                         (a[0] & !a[1] & a[2] & !a[3]));
        public static Schaltfunktion DigFun_Ex431 = new Schaltfunktion(4, a => (a[1]) |
                                                                         (!a[0] & a[2]) |
                                                                         (a[0] & !a[2] & a[3]));
        public static Schaltfunktion SF_1606_B23 = new Schaltfunktion(4, a => (a[0] & a[1]) |
                                                                         (a[0] & a[2] & !a[3]) |
                                                                         (!a[0] & !a[2]));
    }

}