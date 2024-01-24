namespace CubeInCube.Backend.Domain.Entities.BasicElements
{
    public class Position
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double GetDistance(Position other) =>
            Math.Sqrt(Math.Pow((X - other.X), 2) + Math.Pow((Y - other.Y), 2) + Math.Pow((Z - other.Z), 2));

        public double GetDistance(Plane plane)
        {
            var pos1 = plane.Points[0];
            var pos2 = plane.Points[1];
            var pos3 = plane.Points[2];

            var a = pos1.Y * (pos2.Z - pos3.Z) + pos2.Y * (pos3.Z - pos1.Z) + pos3.Y * (pos1.Z - pos2.Z);
            var b = pos1.Z * (pos2.X - pos3.X) + pos2.Z * (pos3.X - pos1.X) + pos3.Z * (pos1.X - pos2.X);
            var c = pos1.X * (pos2.Y - pos3.Y) + pos2.X * (pos3.Y - pos1.Y) + pos3.X * (pos1.Y - pos2.Y);
            var d = -(pos1.X * (pos2.Y * pos3.Z - pos3.Y * pos2.Z) +
                    pos2.X * (pos3.Y * pos1.Z - pos1.Y * pos3.Z) +
                    pos3.X * (pos1.Y * pos2.Z - pos2.Y * pos1.Z));

            var dist = Math.Abs(a * X + b * Y + c * Z + d) / Math.Sqrt(a * a + b * b + c * c);

            return dist;
        }

        public Position()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public Position(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
