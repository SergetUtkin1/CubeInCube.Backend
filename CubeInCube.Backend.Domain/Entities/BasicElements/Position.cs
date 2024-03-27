using MathNet.Numerics.LinearAlgebra;
using System.Drawing;

namespace CubeInCube.Backend.Domain.Entities.BasicElements
{
    public class Position
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Position(Vector<double> vector)
        {
            double[] coordinates = vector.ToArray();

            if(coordinates.Length < 3)
            {
                throw new Exception("Less than 3 values in vector");
            }

            X = coordinates[0];
            Y = coordinates[1];
            Z = coordinates[2];
        }

        public override string ToString()
        {
            return $"({Math.Round(X,2)}; {Math.Round(Y,2)}; {Math.Round(Z,2)};)";
        }

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

        public double GetDistance(Edge line)
        {
            var pointP = Vector<double>.Build.DenseOfArray(new double[] { X, Y, Z });

            var pointA = Vector<double>.Build.DenseOfArray(new double[] { line.Start.X, line.Start.Y, line.Start.Z });
            var pointB = Vector<double>.Build.DenseOfArray(new double[] { line.End.X, line.End.Y, line.End.Z });
            // Векторы направления прямой и от точки A до точки P
            var vecAB = pointB - pointA;
            var vecAP = pointP - pointA;

            var crossProduct = VectorCrossProduct(vecAB, vecAP);

            // Находим длину отрезка AB
            var lengthAB = vecAB.L2Norm();

            // Находим расстояние от точки P до линии AB
            double distance = crossProduct.L2Norm() / lengthAB;

            return distance;
        }


        public Position ProjectPointOntoPlane(Plane plane)
        {
            var normalVector = plane.Normal.ToVector();
            var point = this.ToVector();
            // Находим расстояние от точки до плоскости
            var distanceToPlane = GetDistance(plane);

            // Вычисляем проекцию точки на плоскость
            var projection = point - distanceToPlane * normalVector;
            var result = new Position(projection);
            Console.WriteLine($"Проекция точки {this} на плоскость {plane}: {result}");
            return result;
        }

        private Vector<double> VectorCrossProduct(Vector<double> vector1, Vector<double> vector2)
        {
            var result = Vector<double>.Build.Dense(3);
            result[0] = vector1[1] * vector2[2] - vector1[2] * vector2[1];
            result[1] = vector1[2] * vector2[0] - vector1[0] * vector2[2];
            result[2] = vector1[0] * vector2[1] - vector1[1] * vector2[0];
            return result;
        }

        public Vector<double> VectorCrossProduct(Vector<double> vector2)
        {
            var result = Vector<double>.Build.Dense(3);
            var vector1 = this.ToVector();
            result[0] = vector1[1] * vector2[2] - vector1[2] * vector2[1];
            result[1] = vector1[2] * vector2[0] - vector1[0] * vector2[2];
            result[2] = vector1[0] * vector2[1] - vector1[1] * vector2[0];
            return result;
        }

        public Vector<double> ToVector()
        {
            var vector = Vector<double>.Build.DenseOfArray(new double[] { X, Y, Z });
            return vector;
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
