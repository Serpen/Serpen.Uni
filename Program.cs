using Serpen.Uni.Automat;
using Serpen.Uni.CompSys;

#pragma warning disable CS0162

namespace Serpen.Uni {
    static class Program {
        static void Main() {
            // var sf = Schaltfunktion.SF_212 ;
            // System.Console.WriteLine(sf.Invoke(true, false, false));
            var wt = (WerteTabelle.VT_1608_T25);
            System.Console.WriteLine(wt);
            var kv = new KVDiagramm(wt);
            System.Console.WriteLine(kv);
            var qmcf = QuineMcCluskeyRow.QuineMcCluskey(wt, AlgSourceMode.Wiki);
            System.Console.WriteLine(string.Join("; ", qmcf));
            qmcf = QuineMcCluskeyRow.QuineMcCluskey(wt, AlgSourceMode.K1608);
            System.Console.WriteLine(string.Join("; ", qmcf));
        }


    }
}
