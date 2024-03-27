using MathNet.Numerics.LinearAlgebra;
using System.Drawing;

namespace CubeInCube.Backend.Domain.Entities.BasicElements
{
    public class Plane
    {
        private Edge[] _edges;

        public Position[] Points { get; set; }
            
        public Position Normal { get; private set; }
        public double Dvalue { get; private set; }

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
            SetNormalVectorValue(position1, position2, position3);
        }

        public Plane(Position position1, Position position2, Position position3)
        {
            Points = new Position[3] { position1, position2, position3 };
            var edges = new Edge[4];
            edges[0] = new Edge(position1, position2);
            edges[1] = new Edge(position2, position3);
            edges[2] = new Edge(position3, position1);
            _edges = edges;
            SetNormalVectorValue(position1, position2, position3);
        }

        private void SetNormalVectorValue(Position position1, Position position2, Position position3) 
        {
            var point1 = Vector<double>.Build.DenseOfArray(new double[] { position1.X, position1.Y, position1.Z });
            var point2 = Vector<double>.Build.DenseOfArray(new double[] { position2.X, position2.Y, position2.Z });
            var point3 = Vector<double>.Build.DenseOfArray(new double[] { position3.X, position3.Y, position3.Z });
            var vec1 = point2 - point1;
            var vec2 = point3 - point1;

            // Вычисляем векторное произведение этих векторов
            var normalVector = VectorCrossProduct(vec1, vec2).Normalize(2);
            Normal = new Position(normalVector);
            Dvalue = -(Normal.X * position1.X + Normal.Y * position1.Y + Normal.Z * position1.Z);
        }

        public Position? FindIntersectionDirectionVector(Vector<double> normal1, Vector<double> normal2)
        {
            // Найдем вектор, параллельный линии пересечения плоскостей
            var direction = VectorCrossProduct(normal1, normal2).Normalize(2); ;

            // Если вектор направления линии равен нулю, то плоскости параллельны или совпадают
            if (direction.L2Norm() < 1e-10)
                return null;

            // Найдем точки на линии пересечения
            // Мы можем выбрать любую точку на линии, например, точку (0, 0, 0)
            // и перемещать ее вдоль направляющего вектора
            var point1 = Vector<double>.Build.DenseOfArray(new double[] { 0, 0, 0 });
            var point2 = point1 + direction;
                
            return new Position(point2);
        }

        private double FindDValue(double A, double B, double C, Position point)
        {
            // Находим значение D
            double D = -(A * point.X + B * point.Y + C * point.Z);
            return D;
        }

        Vector<double> VectorCrossProduct(Vector<double> vector1, Vector<double> vector2)
        {
            var result = Vector<double>.Build.Dense(3);
            result[0] = vector1[1] * vector2[2] - vector1[2] * vector2[1];
            result[1] = vector1[2] * vector2[0] - vector1[0] * vector2[2];
            result[2] = vector1[0] * vector2[1] - vector1[1] * vector2[0];
            return result;
        }

        public bool IsOthersidePoints(Position point1, Position point2)
        {
            // Значения уравнения плоскости для двух точек
            double value1 = Normal.X * point1.X + Normal.Y * point1.Y + Normal.Z * point1.Z + Dvalue;
            double value2 = Normal.X * point2.X + Normal.Y * point2.Y + Normal.Z * point2.Z + Dvalue;

            // Проверка расположения сторон плоскости относительно точек
            if ((value1 > 0 && value2 < 0) || (value1 < 0 && value2 > 0))
            {
                return true;
            }
            else if (value1 == 0 && value2 == 0)
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return $"{Math.Truncate(Normal.X)}x + ({Math.Truncate(Normal.Y)}y) + ({Normal.Z}z) + ({Math.Truncate(Dvalue)}) = 0";
        }
    }
}
