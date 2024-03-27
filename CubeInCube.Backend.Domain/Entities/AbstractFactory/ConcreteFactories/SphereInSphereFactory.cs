using CubeInCube.Backend.Domain.Entities.AbstractFactory;
using CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.AbstractProducts;
using CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.Shapes;
using CubeInCube.Backend.Domain.Entities.BasicElements;

namespace CubeInCube.Backend.Domain.Entities.AbstractFactory.ConcreteFactories
{
    public class SphereInSphereFactory : CaseFactory
    {
        public Sphere[] InnerShapes { get; set; } = null!;
        public Sphere BoundingShape { get; set; } = null!;

        public override void CreateBoundingShape(Dimension dimension)
            => BoundingShape = new Sphere(new Position(), dimension.Length);

        public override void CreateInnerShape(Position center, Dimension dimension)
        {
            var sphere = new Sphere(center, dimension.Length);
            InnerShapes[_currentIndex] = sphere;
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

        public override Shape[] GetArrayOfInnerShapes(Dimension[] dimensions, bool isSortingEnable)
        {
            var spheres = new Sphere[dimensions.Length];

            for (int i = 0; i < dimensions.Length; i++)
            {
                spheres[i] = new Sphere(new Position(),
                                        dimensions[i].Length,
                                        dimensions[i].Width,
                                        dimensions[i].Height);
            }
            if (isSortingEnable)
                Array.Sort(spheres, (a, b) => ((int)(b.Volume - a.Volume)));

            return spheres;
        }

        public override void SetCountOfInnerShapes(int count)
        {
            InnerShapes = new Sphere[count];
        }

        public override bool CheckIntersection()
        {
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

        protected override bool CheckPointInsideBounding(Position position)
        {
            var flag = false;

            if (position.GetDistance(BoundingShape.Center) < BoundingShape.Dimension.Length)
            {
                flag = true;
            }

            return flag;
        }

        protected override bool HasIntersectionWithBound(Shape shape)
        {
            var flag = false;
            var distanceBetweenCenteres = shape.Center.GetDistance(BoundingShape.Center);

            if (BoundingShape.Dimension.Length - distanceBetweenCenteres <= shape.Dimension.Length)
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
    }
}
