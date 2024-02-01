using CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.AbstractProducts;
using CubeInCube.Backend.Domain.Entities.BasicElements;

namespace CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.Shapes
{
    public class Parallelepiped : Shape
    {
        public Plane[] Sides { get; private set; }
        public Position[] Points { get; private set; }

        public Parallelepiped(Position center, double length, double width, double heigth) : base(center, length, width, heigth)
        {
            Center = center;
            Sides = new Plane[6];
            Points = new Position[8];
            SetSides();
        }

        public Parallelepiped(Position center, double length) : base(center, length)
        {
            Center = center;
            Sides = new Plane[6];
            Points = new Position[8];
            SetSides();
        }

        protected override double GetVolume()
            => Dimension.Length * Dimension.Width * Dimension.Heigth;

        private void SetSides()
        {
            var PointsOfSides = GetPointsOfSides();

            for (int j = 0; j < 3; j++)
            {
                var PointsOfTwoSides = new List<Position>();
                var i = 0;
                while (i < PointsOfSides.Count)
                {
                    var index = i + j;

                    PointsOfTwoSides.Add(PointsOfSides[index]);

                    i += 3;
                }
                Points = PointsOfTwoSides.ToArray();

                Sides[j] = new Plane(PointsOfTwoSides[0], PointsOfTwoSides[1], PointsOfTwoSides[2], PointsOfTwoSides[3]);
                Sides[Sides.Length - 1 - j] = new Plane(PointsOfTwoSides[4], PointsOfTwoSides[5], PointsOfTwoSides[6], PointsOfTwoSides[7]);
            }
        }

        private List<Position> GetPointsOfSides()
        {
            var temp = new int[2] { -1, 1 };
            var PointsOfSides = new List<Position>();

            foreach (var i in temp)
            {
                foreach (var j in temp)
                {
                    foreach (var k in temp)
                    {
                        var tempArray = new int[] { i, j, k };

                        var x = Center.X + tempArray[0] * Dimension.Length * 0.5;
                        var y = Center.Y + tempArray[1] * Dimension.Width * 0.5;
                        var z = Center.Z + tempArray[2] * Dimension.Heigth * 0.5;

                        PointsOfSides.Add(new Position(x, y, z));

                        x = Center.X + tempArray[1] * Dimension.Length * 0.5;
                        y = Center.Y + tempArray[0] * Dimension.Width * 0.5;
                        z = Center.Z + tempArray[2] * Dimension.Heigth * 0.5;

                        PointsOfSides.Add(new Position(x, y, z));

                        x = Center.X + tempArray[2] * Dimension.Length * 0.5;
                        y = Center.Y + tempArray[1] * Dimension.Width * 0.5;
                        z = Center.Z + tempArray[0] * Dimension.Heigth * 0.5;

                        PointsOfSides.Add(new Position(x, y, z));

                    }
                    temp = temp.Reverse().ToArray();
                }
            }

            return PointsOfSides;
        }

    }
}
