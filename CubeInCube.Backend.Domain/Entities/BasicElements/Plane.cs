namespace CubeInCube.Backend.Domain.Entities.BasicElements
{
    public class Plane
    {
        private Edge[] _edges;

        public Position[] Points { get; set; }


        public Edge[] Edges
        {
            get { return _edges; }
            set { _edges = value; }
        }


        public Plane(Position position1, Position position2, Position position3, Position position4)
        {
            Points = new Position[4] { position1, position2, position3, position4 };
            var edges = new Edge[4];
            edges[0] = new Edge(position1, position2);
            edges[1] = new Edge(position2, position3);
            edges[2] = new Edge(position3, position4);
            edges[3] = new Edge(position4, position1);
            _edges = edges;
        }
    }
}
