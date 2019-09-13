namespace Serpen.Uni.Graph {
    public class UndirectedUnwightedGraph {
 
        public UndirectedUnwightedGraph(int nodesCount, EdgeBase<int,int>[] edges) {
            Nodes = new string[nodesCount];
            for (int i = 0; i < nodesCount; i++)
                Nodes[i] = i.ToString();

            this.Edges = edges;
        }

        public string[] Nodes {get;}
        public EdgeBase<int,int>[] Edges {get;}

    }
}