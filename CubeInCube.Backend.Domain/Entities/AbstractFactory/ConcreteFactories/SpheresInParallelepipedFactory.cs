using CubeInCube.Backend.Domain.Entities.AbstractFactory;
using CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.AbstractProducts;
using CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.Shapes;
using CubeInCube.Backend.Domain.Entities.BasicElements;

namespace ShapesInShape.Models.AbstractFactory.ConcreteFactories
{
    public class SpheresInParallelepipedFactory : CaseFactory
    {
        public Sphere[] InnerShapes { get; set; } = null!;
        public Parallelepiped BoundingShape { get; set; } = null!;
        
        public override void CreateBoundingShape(Dimension dimension) =>
            BoundingShape = new Parallelepiped(new Position(), dimension.Length, dimension.Width, dimension.Heigth);

        public override void CreateInnerShape(Position center, Dimension dimension)
        {
            var sphere = new Sphere(center, dimension.Length, dimension.Width, dimension.Heigth);
            InnerShapes[_currentIndex] = sphere;
        }

        public override void SetCountOfInnerShapes(int count)
        {
            InnerShapes = new Sphere[count];
        }
        
        public override Shape[] GetArrayOfInnerShapes(Dimension[] dimensions, bool isSortingEnable)
        {
            var spheres = new Sphere[dimensions.Length];

            for (int i = 0; i < dimensions.Length; i++)
            {
                spheres[i] = new Sphere(new Position(),
                                        dimensions[i].Length,
                                        dimensions[i].Width,
                                        dimensions[i].Heigth);
            }

            if (isSortingEnable)
                Array.Sort(spheres, (a, b) => ((int)(b.Volume - a.Volume)));

            return spheres;
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
                var distance = shape.Center.GetDistance(BoundingShape.Sides[i]);

                if (distance <= shape.Dimension.Length)
                {
                    flag = true;
                    break;
                }
            }

            return flag;
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
    }
}
