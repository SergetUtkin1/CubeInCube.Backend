using CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.AbstractProducts;
using CubeInCube.Backend.Domain.Entities.BasicElements;

namespace CubeInCube.Backend.Domain.Entities.AbstractFactory
{
    public abstract class CaseFactory
    {
        protected int _currentIndex = 0;

        public abstract void CreateBoundingShape(Dimension dimension);
        public abstract void SetCountOfInnerShapes(int count);
        public abstract void CreateInnerShape(Position center, Dimension dimension);
        public abstract Shape[] GetArrayOfInnerShapes(Dimension[] dimensions, bool isSortingEnable);
        public abstract Position CreatePoint(Distribution distributionOfPosition);
        protected abstract bool CheckPointInsideBounding(Position position);
        protected abstract bool HasIntersectionWithOtherShape(Shape shape, Shape otherShape);
        protected abstract bool HasIntersectionWithBound(Shape shape);
        public abstract bool CheckIntersection();
        public void ConfirmAdding() =>
            _currentIndex += 1;
    }
}
