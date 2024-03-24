using CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.Shapes;
using CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.AbstractProducts;
using CubeInCube.Backend.Domain.Entities.BasicElements;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using CubeInCube.Backend.Domain.Entities.AbstractFactory;
using CubeInCube.Backend.Domain.Entities.Models.AbstractFactory.Products.Shapes;

namespace ShapesInShape.ConsoleApplication.Models.AbstractFactory.ConcreteFactories
{
    public class CylinderInParallelepipedFactory : CaseFactory
    {
        public Cylinder[] InnerShapes { get; set; } = null!;
        public Parallelepiped BoundingShape { get; set; } = null!;

        public override void CreateBoundingShape(Dimension dimension) =>
            BoundingShape = new Parallelepiped(new Position(), dimension.Length, dimension.Width, dimension.Heigth);

        public override void CreateInnerShape(Position center, Dimension dimension)
        {
            var cylinder = new Cylinder(center, dimension.Length, dimension.Width, dimension.Heigth, dimension.Theta, dimension.Fi);
            InnerShapes[_currentIndex] = cylinder;
        }

        public override void SetCountOfInnerShapes(int count)
        {
            InnerShapes = new Cylinder[count];
        }

        public override Shape[] GetArrayOfInnerShapes(Dimension[] dimensions, bool isSortingEnable)
        {
            var cylinders = new Cylinder[dimensions.Length];

            for (int i = 0; i < dimensions.Length; i++)
            {
                cylinders[i] = new Cylinder(new Position(),
                                        dimensions[i].Length,
                                        dimensions[i].Width,
                                        dimensions[i].Heigth,
                                        dimensions[i].Theta,
                                        dimensions[i].Fi);
            }

            if (isSortingEnable)
                Array.Sort(cylinders, (a, b) => ((int)(b.Volume - a.Volume)));

            return cylinders;
        }

        public override Position CreatePoint(Distribution distributionOfPosition)
        {
            double x, y, z;
            Position position;

            do
            {
                x = distributionOfPosition.GetValue(BoundingShape.Center.X - 0.5 * BoundingShape.Dimension.Length, BoundingShape.Center.X + 0.5 * BoundingShape.Dimension.Length);
                y = distributionOfPosition.GetValue(BoundingShape.Center.Y - 0.5 * BoundingShape.Dimension.Width, BoundingShape.Center.Y + 0.5 * BoundingShape.Dimension.Width);
                z = distributionOfPosition.GetValue(BoundingShape.Center.Z - 0.5 * BoundingShape.Dimension.Heigth, BoundingShape.Center.Z + 0.5 * BoundingShape.Dimension.Heigth);
                position = new Position(x, y, z);
            } while (!CheckPointInsideBounding(position));

            return position;
        }

        protected override bool CheckPointInsideBounding(Position position)
        {
            var flag = false;
            var xPlanes = (BoundingShape.Center.X - 0.5 * BoundingShape.Dimension.Length, BoundingShape.Center.X + 0.5 * BoundingShape.Dimension.Length);
            var yPlanes = (BoundingShape.Center.Y - 0.5 * BoundingShape.Dimension.Width, BoundingShape.Center.Y + 0.5 * BoundingShape.Dimension.Width);
            var zPlanes = (BoundingShape.Center.Z - 0.5 * BoundingShape.Dimension.Heigth, BoundingShape.Center.Z + 0.5 * BoundingShape.Dimension.Heigth);

            var xCondition = (xPlanes.Item1 < position.X && position.X < xPlanes.Item2);
            var yCondition = (yPlanes.Item1 < position.Y && position.Y < yPlanes.Item2);
            var zCondition = (zPlanes.Item1 < position.Z && position.Z < zPlanes.Item2);
            if (xCondition && yCondition && zCondition)
            {
                flag = true;
            }

            return flag;
        }

        protected override bool HasIntersectionWithOtherShape(Shape shape, Shape otherShape)
        {
            var flag = false;
            var distanceBetweenCenteres = shape.Center.GetDistance(otherShape.Center);

            if (distanceBetweenCenteres <= shape.Dimension.Length + otherShape.Dimension.Length)
            {
                flag = true;
            }

            return flag;
        }

        protected override bool HasIntersectionWithBound(Shape shape)
        {
            var flag = false;

            for (int i = 0; i < BoundingShape.Sides.Length; i++)
            {
                var sides = BoundingShape.Sides;
                var points = GetPointsOfAxis(shape);
                Plane top;
                Plane bottom;
                (top, bottom) = GetPlanesOfCylinder(shape);
                foreach (var side in sides)
                {
                    var intersectionPoints = FindIntersectionPoints(top, side) ?? FindIntersectionPoints(bottom, side);
                    if(intersectionPoints != null)
                    {
                        var line = new Edge(intersectionPoints.Value.Item1, intersectionPoints.Value.Item2);
                        var distanceFromTopCenter = top.Points[0].GetDistance(line);
                        var distanceFromBottomCenter = bottom.Points[0].GetDistance(line);

                        if (distanceFromBottomCenter <= shape.Dimension.Width || distanceFromTopCenter <= shape.Dimension.Width)
                        {
                            flag = true;
                            return flag;
                        }
                    }
                    
                    foreach (var point in points)
                    {
                        var distance = point.GetDistance(BoundingShape.Sides[i]);

                        if (distance <= shape.Dimension.Length)
                        {
                            var projectPoint = point.ProjectPointOntoPlane(side);
                            var distanceForTop = projectPoint.GetDistance(top);
                            var distanceForBottom = projectPoint.GetDistance(bottom);
                            if (distanceForTop <= shape.Dimension.Heigth && distanceForBottom <= shape.Dimension.Heigth)
                            {
                                flag = true;
                                return flag;
                            }

                        }
                    }
                }
            }

            return flag;
        }

        static (Position, Position)? FindIntersectionPoints(Plane plane1, Plane plane2)
        {
            // Проверка на параллельность или совпадение плоскостей
            if (plane1.Normal.X * plane2.Normal.Y == plane2.Normal.X * plane1.Normal.Y 
                && plane1.Normal.X * plane2.Normal.Z == plane2.Normal.X * plane1.Normal.Z 
                && plane1.Normal.Y * plane2.Normal.Z == plane2.Normal.Y * plane1.Normal.Z 
                && plane1.Dvalue != plane2.Dvalue)
                return null;

            // Вычисление параметра t
            var intersectionPoints = new List<Position>();

            for (double t = 1.0; t < 3.0; t++)
            {
                // Проверка на ноль в знаменателе
                double denominator = plane1.Normal.X * plane2.Normal.Y - plane2.Normal.X * plane1.Normal.Y;
                if (denominator == 0)
                    return null;

                // По формулам находим координаты точек на линии пересечения
                var x = ((plane1.Normal.Y * plane2.Normal.Z - plane2.Normal.Y * plane1.Normal.Z) * t - (plane1.Dvalue * plane2.Normal.Z - plane2.Dvalue * plane1.Normal.Z)) / denominator;
                var y = ((plane2.Normal.X * plane1.Normal.Z - plane1.Normal.X * plane2.Normal.Z) * t - (plane2.Dvalue * plane1.Normal.X - plane1.Dvalue * plane2.Normal.X)) / denominator;
                var z = ((plane1.Normal.X * plane2.Normal.Y - plane2.Normal.X * plane1.Normal.Y) * t - (plane1.Dvalue * plane2.Normal.Y - plane2.Dvalue * plane1.Normal.Y)) / denominator;

                // Формируем точку на линии пересечения
                var intersectionPoint =  new Position(x, y, z);
                intersectionPoints.Add(intersectionPoint);
            }

            // Возвращаем точки
            return (intersectionPoints[0], intersectionPoints[1]);
        }

        private static List<Position> GetPointsOfAxis(Shape shape)
        {
            var count = 1000.0;
            var start = new Position(shape.Center.X, shape.Center.Y, shape.Center.Z - shape.Dimension.Heigth * 0.5);
            var end = new Position(shape.Center.X, shape.Center.Y, shape.Center.Z + shape.Dimension.Heigth * 0.5);
            var step = shape.Dimension.Heigth / count;
            var points = new List<Position>()
                    {
                        RotateVector(start, shape.Dimension.Theta, shape.Dimension.Fi),
                        RotateVector(end, shape.Dimension.Theta, shape.Dimension.Fi)
                    };

            for (int j = 0; j < count; j++)
            {
                var point = new Position(shape.Center.X, shape.Center.Y, start.Z + step);
                points.Add(RotateVector(point, shape.Dimension.Theta, shape.Dimension.Fi));
            }

            return points;
        }

        private static (Plane top, Plane bottom) GetPlanesOfCylinder(Shape shape)
        {
            var start = new Position(shape.Center.X, shape.Center.Y, shape.Center.Z - shape.Dimension.Heigth * 0.5);
            var end = new Position(shape.Center.X, shape.Center.Y, shape.Center.Z + shape.Dimension.Heigth * 0.5);

            var bottom = new Plane(RotateVector(start, shape.Dimension.Theta, shape.Dimension.Fi),
               RotateVector(new Position(start.X + shape.Dimension.Width, start.Y, start.Z), shape.Dimension.Theta, shape.Dimension.Fi),
               RotateVector(new Position(start.X, start.Y + shape.Dimension.Width, start.Z), shape.Dimension.Theta, shape.Dimension.Fi));
            var top = new Plane(RotateVector(end, shape.Dimension.Theta, shape.Dimension.Fi),
                                RotateVector(new Position(end.X + shape.Dimension.Width, end.Y, end.Z), shape.Dimension.Theta, shape.Dimension.Fi),
                                 RotateVector(new Position(end.X, end.Y + shape.Dimension.Width, end.Z), shape.Dimension.Theta, shape.Dimension.Fi));

            return (top, bottom);
        }

        public override bool CheckIntersection()
        {
            var flag = false;
            var shape = InnerShapes[_currentIndex];

            if (HasIntersectionWithBound(shape))
            {
                flag = true;
            }
            else
            {
                for (int i = 0; i < _currentIndex; i++)
                {
                    if (HasIntersectionWithOtherShape(shape, InnerShapes[i]))
                    {
                        flag = true;
                        break;
                    }
                }
            }

            return flag;
        }

        static Position RotateVector(Position position, double angleX, double angleY)
        {
            var vector = Vector<double>.Build.DenseOfArray(new double[] { position.X, position.Y, position.Z });

            // Создание матрицы поворота по оси X
            var rotationMatrixX = Matrix<double>.Build.DenseOfArray(new double[,] {
            { 1, 0, 0 },
            { 0, Math.Cos(angleX), -Math.Sin(angleX) },
            { 0, Math.Sin(angleX), Math.Cos(angleX) }
            });

            // Создание матрицы поворота по оси Y
            var rotationMatrixY = Matrix<double>.Build.DenseOfArray(new double[,] {
            { Math.Cos(angleY), 0, Math.Sin(angleY) },
            { 0, 1, 0 },
            { -Math.Sin(angleY), 0, Math.Cos(angleY) }
            });

            // Выполнение последовательного поворота вектора
            var rotatedVector = rotationMatrixX * rotationMatrixY * vector;
            double[] coordinates = rotatedVector.ToArray();

            return new Position(coordinates[0], coordinates[1], coordinates[2]);
        }
    } 
}
