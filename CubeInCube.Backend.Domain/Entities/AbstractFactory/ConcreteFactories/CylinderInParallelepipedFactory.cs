using CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.Shapes;
using CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.AbstractProducts;
using CubeInCube.Backend.Domain.Entities.BasicElements;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using CubeInCube.Backend.Domain.Entities.AbstractFactory;
using CubeInCube.Backend.Domain.Entities.Models.AbstractFactory.Products.Shapes;
using System.Drawing;
using System.Reflection;
using MathNet.Numerics.Distributions;
using System.IO.Pipes;

namespace ShapesInShape.ConsoleApplication.Models.AbstractFactory.ConcreteFactories
{
    public class CylinderInParallelepipedFactory : CaseFactory
    {
        public Cylinder[] InnerShapes { get; set; } = null!;
        public Parallelepiped BoundingShape { get; set; } = null!;

        public override void CreateBoundingShape(Dimension dimension) =>
            BoundingShape = new Parallelepiped(new Position(), dimension.Length, dimension.Width, dimension.Height);

        public override void CreateInnerShape(Position center, Dimension dimension)
        {
            var cylinder = new Cylinder(center, dimension.Length, dimension.Width, dimension.Height, dimension.Theta, dimension.Fi);
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
                                        dimensions[i].Height,
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
                z = distributionOfPosition.GetValue(BoundingShape.Center.Z - 0.5 * BoundingShape.Dimension.Height, BoundingShape.Center.Z + 0.5 * BoundingShape.Dimension.Height);
                position = new Position(x, y, z);
            } while (!CheckPointInsideBounding(position));

            return position;
        }

        protected override bool CheckPointInsideBounding(Position position)
        {
            var flag = false;
            var xPlanes = (BoundingShape.Center.X - 0.5 * BoundingShape.Dimension.Length, BoundingShape.Center.X + 0.5 * BoundingShape.Dimension.Length);
            var yPlanes = (BoundingShape.Center.Y - 0.5 * BoundingShape.Dimension.Width, BoundingShape.Center.Y + 0.5 * BoundingShape.Dimension.Width);
            var zPlanes = (BoundingShape.Center.Z - 0.5 * BoundingShape.Dimension.Height, BoundingShape.Center.Z + 0.5 * BoundingShape.Dimension.Height);

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
            var axisPoints1 = GetPointsOfAxis(shape);
            Plane top1;
            Plane bottom1;
            (top1, bottom1) = GetPlanesOfCylinder(shape);

            var axisPoints2 = GetPointsOfAxis(otherShape);
            Plane top2;
            Plane bottom2;
            (top2, bottom2) = GetPlanesOfCylinder(otherShape);

            var distances = new List<double>();

            var intersectionPoints = FindIntersectionLine(top1, top2);
            if (intersectionPoints != null)
            {
                distances.Add(top1.Points[0].GetDistance(top2.Points[0]));
            }

            intersectionPoints = FindIntersectionLine(top1, bottom2);
            if (intersectionPoints != null)
            {
                distances.Add(top1.Points[0].GetDistance(bottom2.Points[0]));
            }

            intersectionPoints = FindIntersectionLine(bottom1, bottom2);
            if (intersectionPoints != null)
            {
                distances.Add(bottom1.Points[0].GetDistance(bottom2.Points[0]));
            }

            if (distances.Any(d => (d <= shape.Dimension.Width + otherShape.Dimension.Width)))
            {
                flag = true;
                return flag;
            }


            foreach (var point in axisPoints1)
            {
                var distance = point.GetDistance(top2);

                if (distance <= shape.Dimension.Length)
                {
                    var projectPoint = point.ProjectPointOntoPlane(top2);
                    var distanceForTop = projectPoint.GetDistance(top1);
                    var distanceForBottom = projectPoint.GetDistance(bottom1);
                    if (distanceForTop <= shape.Dimension.Height && distanceForBottom <= shape.Dimension.Height)
                    {
                        flag = true;
                        return flag;
                    }
                }

                distance = point.GetDistance(bottom2);

                if (distance <= shape.Dimension.Length)
                {
                    var projectPoint = point.ProjectPointOntoPlane(bottom2);
                    var distanceForTop = projectPoint.GetDistance(top1);
                    var distanceForBottom = projectPoint.GetDistance(bottom1);
                    if (distanceForTop <= shape.Dimension.Height && distanceForBottom <= shape.Dimension.Height)
                    {
                        flag = true;
                        return flag;
                    }
                }
            }

            foreach (var point in axisPoints2)
            {
                var distance = point.GetDistance(top1);

                if (distance <= shape.Dimension.Length)
                {
                    var projectPoint = point.ProjectPointOntoPlane(top1);
                    var distanceForTop = projectPoint.GetDistance(top2);
                    var distanceForBottom = projectPoint.GetDistance(bottom2);
                    if (distanceForTop <= shape.Dimension.Height && distanceForBottom <= shape.Dimension.Height)
                    {
                        flag = true;
                        return flag;
                    }
                }

                distance = point.GetDistance(bottom1);

                if (distance <= shape.Dimension.Length)
                {
                    var projectPoint = point.ProjectPointOntoPlane(bottom1);
                    var distanceForTop = projectPoint.GetDistance(top2);
                    var distanceForBottom = projectPoint.GetDistance(bottom2);
                    if (distanceForTop <= shape.Dimension.Height && distanceForBottom <= shape.Dimension.Height)
                    {
                        flag = true;
                        return flag;
                    }
                }
            }

            for (int i = 0; i < axisPoints1.Count; i++)
            {
                for (int j = 0; j < axisPoints2.Count; j++)
                {
                    var distance = axisPoints1[i].GetDistance(axisPoints2[j]);

                    if(distance <= shape.Dimension.Width + otherShape.Dimension.Width && 
                     !(top1.IsOthersidePoints(axisPoints1[i], axisPoints2[j]) && top2.IsOthersidePoints(axisPoints1[i], axisPoints2[j]) &&
                        bottom1.IsOthersidePoints(axisPoints1[i], axisPoints2[j]) && bottom2.IsOthersidePoints(axisPoints1[i], axisPoints2[j])))
                    {
                        flag = true;
                        return flag;
                    }
                }
            }

            return flag;
        }

        protected override bool HasIntersectionWithBound(Shape shape)
        {
            var flag = false;

            var sides = BoundingShape.Sides;
            var points = GetPointsOfAxis(shape);
            Plane top;
            Plane bottom;
            (top, bottom) = GetPlanesOfCylinder(shape);

            foreach (var side in sides)
            {
                if (side.IsOthersidePoints(top.Points[0], bottom.Points[0]))
                {
                    Console.WriteLine($"Otherside Points");
                    flag = true;
                    return flag;
                }
                var planesAndLines = new Dictionary<Plane, Edge?>();

                planesAndLines[top] = FindIntersectionLine(top, side);
                planesAndLines[bottom] = FindIntersectionLine(bottom, side);
                foreach (var item in planesAndLines)
                {
                    double distanceFromCenter = double.MaxValue;

                    if (item.Value is not null)
                    {
                        distanceFromCenter = item.Key.Points[0].GetDistance(item.Value);
                    }

                    if (distanceFromCenter <= shape.Dimension.Width)
                    {
                        Console.WriteLine($"Top: {top.Points[0]} \nBottom:{bottom.Points[0]}\n Radius:{shape.Dimension.Width}");
                        Console.WriteLine($"Intersection: Distance from center to line: {Math.Truncate(distanceFromCenter)}");
                        flag = true;
                        return flag;
                    }
                }


                foreach (var point in points)
                {
                    var distance = point.GetDistance(side);

                    if (distance <= shape.Dimension.Width)
                    {
                        var projectPoint = point.ProjectPointOntoPlane(side);
                        var distanceForTop = projectPoint.GetDistance(top);
                        var distanceForBottom = projectPoint.GetDistance(bottom);
                        if (distanceForTop <= shape.Dimension.Height && distanceForBottom <= shape.Dimension.Height)
                        {
                            Console.WriteLine($"Intersection: Distance from axis point: {Math.Truncate(distance)}\n" +
                                $"Project point to side:{projectPoint}\n" +
                                $"distanceForTop:{Math.Truncate(distanceForTop)}\n" +
                                $"distanceForBottom:{Math.Truncate(distanceForBottom)}");
                            flag = true;
                            return flag;
                        }

                    }
                }
            }

            return flag;
        }

        private double GetDistanceFromCenter(Plane plane, Edge? line)
        {
            if(line is null)
            {
                throw new ArgumentNullException(nameof(line));
            }
            return plane.Points[0].GetDistance(line);
        }

        static Edge? FindIntersectionLine(Plane plane1, Plane plane2)
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
            return new Edge(intersectionPoints[0], intersectionPoints[1]);
        }

        private static List<Position> GetPointsOfAxis(Shape cylindr)
        {
            var count = 100.0;
            var start = new Position(cylindr.Center.X, cylindr.Center.Y, cylindr.Center.Z - cylindr.Dimension.Height * 0.5);
            var end = new Position(cylindr.Center.X, cylindr.Center.Y, cylindr.Center.Z + cylindr.Dimension.Height * 0.5);
            var step = cylindr.Dimension.Height / count;
            var points = new List<Position>()
                    {
                        RotateVector(start, cylindr.Dimension.Theta, cylindr.Dimension.Fi),
                        RotateVector(end, cylindr.Dimension.Theta, cylindr.Dimension.Fi)
                    };
            var z = start.Z;
            for (int j = 0; j < count; j++)
            {
                z += step;
                var point = new Position(cylindr.Center.X, cylindr.Center.Y, z);
                points.Add(RotateVector(point, cylindr.Dimension.Theta, cylindr.Dimension.Fi));
            }

            return points;
        }

        private static (Plane top, Plane bottom) GetPlanesOfCylinder(Shape shape)
        {
            var start = new Position(shape.Center.X, shape.Center.Y, shape.Center.Z - shape.Dimension.Height * 0.5);
            var end = new Position(shape.Center.X, shape.Center.Y, shape.Center.Z + shape.Dimension.Height * 0.5);

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
