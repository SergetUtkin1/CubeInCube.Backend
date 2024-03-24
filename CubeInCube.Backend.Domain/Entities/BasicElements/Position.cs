using MathNet.Numerics.LinearAlgebra;

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

            // Проекция вектора AP на вектор AB
            var projection = vecAP.DotProduct(vecAB) / vecAB.L2Norm() * vecAB;

            // Расстояние - длина проекции
            var distance = (vecAP - projection).L2Norm();

            return distance;
        }


        public Position ProjectPointOntoPlane(Plane plane)
        {
            // Три точки, задающие плоскость
            var pointA = Vector<double>.Build.DenseOfArray(new double[] { plane.Points[0].X, plane.Points[0].Y, plane.Points[0].Z });
            var pointB = Vector<double>.Build.DenseOfArray(new double[] { plane.Points[1].X, plane.Points[1].Y, plane.Points[1].Z });
            var pointC = Vector<double>.Build.DenseOfArray(new double[] { plane.Points[2].X, plane.Points[2].Y, plane.Points[2].Z });

            // Точка, для которой мы хотим найти проекцию на плоскость
            var pointP = Vector<double>.Build.DenseOfArray(new double[] { X, Y, Z });

            // Найдем векторы, образующие стороны плоскости
            var vecAB = pointB - pointA;
            var vecAC = pointC - pointA;

            // Нормаль к плоскости - векторное произведение vecAB и vecAC
            var normal = VectorCrossProduct(vecAB, vecAC);

            // Вектор от точки на плоскости до заданной точки
            var vecAP = pointP - pointA;

            // Проекция вектора AP на нормаль к плоскости
            var projection = (vecAP.DotProduct(normal) / normal.Norm(2)) * normal;

            // Найдем точку проекции, добавив проекцию к любой из точек на плоскости
            var projectedPoint = pointP - projection;

            return new Position(projectedPoint);
        }

        private Vector<double> VectorCrossProduct(Vector<double> vector1, Vector<double> vector2)
        {
            var result = Vector<double>.Build.Dense(3);
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
