using System.Collections.Generic;
using System.Linq;

public class DijkstraIterativ
{
    // globaler Link State
    [Serpen.Uni.AlgorithmSource("FUH1801_P180")]
    [Serpen.Uni.AlgorithmComplexity("O(n^2)")]
    public static void DoIt(IEnumerable<Edge> edges, Edge Q)
    {
        var distance = new System.Collections.Generic.Dictionary<Edge, int>();
        var predessor = new System.Collections.Generic.Dictionary<Edge, Edge>();
        var processed = new System.Collections.Generic.List<Edge>();
        processed.Add(Q);

        foreach (var edge in edges)
        {
            if (edge.isNeigbour(Q))
            {
                distance[edge] = edge.Cost(Q);
                predessor[edge] = Q;
            }
            else if (edge.Equals(Q))
                distance[edge] = 0;
            else
                distance[edge] = int.MaxValue;
        }

        while (processed.Count < edges.Count())
        {
            var i = edges.Except(processed).MinBy(e => distance[e]);
            processed.Add(i);
            foreach (var j in i.Neighbours)
            {
                var sum = distance[i] + i.Cost(j);
                if (sum < distance[j])
                {
                    Serpen.Uni.Utils.DebugMessage($"{j} better path over {i}: {sum}<{distance[j]}", Serpen.Uni.Utils.eDebugLogLevel.Normal);
                    distance[j] = sum;
                    predessor[j] = i;
                }
            }
        }
        Serpen.Uni.Utils.DebugMessage(new System.Lazy<string>(() =>
        {
            var sb = new System.Text.StringBuilder();
            foreach (var d in distance)
                sb.AppendLine($"{Q} -> {d.Key} = {d.Value}");
            return sb.ToString();
        }), Serpen.Uni.Utils.eDebugLogLevel.Always);
    }
}

public class Edge
{
    private Dictionary<Edge, int> neighbours = new();
    private readonly string Name;
    public Edge(string name) => Name = name;

    public void AddNeighbour(Edge edge, int cost)
    {
        neighbours.Add(edge, cost);
        edge.neighbours.Add(this, cost);
    }
    internal int Cost(Edge q) => neighbours[q];

    internal bool isNeigbour(Edge q) => neighbours.ContainsKey(q);

    internal IEnumerable<Edge> Neighbours => neighbours.Keys;

    public override string ToString() => Name;

    public override bool Equals(object obj)
    {
        if (obj is Edge e)
            return e.Name == this.Name;
        return false;
    }

    public override int GetHashCode() => Name.GetHashCode();

    public static IEnumerable<Edge> Sample_A41_P180()
    {
        var t1 = new Edge("T1");
        var t2 = new Edge("T2");
        var t3 = new Edge("T3");
        var t4 = new Edge("T4");
        var t5 = new Edge("T5");
        var t6 = new Edge("T6");

        t1.AddNeighbour(t2, 2);
        t1.AddNeighbour(t3, 5);
        t1.AddNeighbour(t4, 1);

        t2.AddNeighbour(t3, 3);
        t2.AddNeighbour(t4, 2);

        t3.AddNeighbour(t4, 3);
        t3.AddNeighbour(t5, 1);
        t3.AddNeighbour(t6, 5);

        t4.AddNeighbour(t5, 1);

        t5.AddNeighbour(t6, 2);

        yield return t1;
        yield return t2;
        yield return t3;
        yield return t4;
        yield return t5;
        yield return t6;

    }
}
