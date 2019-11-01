using Serpen.Uni.Automat;
using Serpen.Uni.CompSys;

#pragma warning disable CS0162

namespace Serpen.Uni {
    static class Program {
        static void Main() {
            var sf = Schaltfunktion.SF_1608_S22 ;
            System.Console.WriteLine(sf.Invoke(true, false, false));
            var wt = (WerteTabelle)sf;
            System.Console.WriteLine(wt);
            System.Console.WriteLine("KDFN: " + wt.toKDNF());
            System.Console.WriteLine("KKFN: " + wt.toKKNF());
            var kv = new KVDiagramm(wt);
            System.Console.WriteLine("KV:\n" + kv);
            var qmcf = QuineMcCluskeyRow.QuineMcCluskey(wt, AlgSourceMode.Wiki);
            System.Console.WriteLine(string.Join("; ", qmcf));
            qmcf = QuineMcCluskeyRow.QuineMcCluskey(wt, AlgSourceMode.K1608);
            System.Console.WriteLine(string.Join("; ", qmcf));
        }


    }
}
