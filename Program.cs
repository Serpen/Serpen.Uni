using Serpen.Uni.Automat;
using Serpen.Uni.CompSys;

#pragma warning disable CS0162

namespace Serpen.Uni {
    static class Program {
        static void Main() {
            // var sf = Schaltfunktion.SF_212 ;
            // System.Console.WriteLine(sf.Invoke(true, false, false));
            var wt = (WerteTabelle.T25_2);
            System.Console.WriteLine(wt);
            var kv = new KVDiagramm(wt);
            System.Console.WriteLine(kv);
            var qmcf = QuineMcCluskeyRow.QuineMcCluskey(wt);
            System.Console.WriteLine(string.Join(';', qmcf));
        }


    }
}
