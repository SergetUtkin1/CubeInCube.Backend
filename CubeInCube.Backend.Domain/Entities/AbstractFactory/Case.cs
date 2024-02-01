using CubeInCube.Backend.Domain.Entities.BasicElements;
using CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.AbstractProducts;
using CubeInCube.Backend.Domain.Utils;

namespace CubeInCube.Backend.Domain.Entities.AbstractFactory
{
    public class Case
    {
        private Shape[] _shapes;
        private List<Shape> _goodShapes;
        private CaseConfiguration Configuration { get; set; }
        private CaseFactory Factory { get; set; }
        private FileWriter FileWriter { get; set; }

        public Case(CaseFactory factory, CaseConfiguration configuration, FileWriter fileWriter)
        {
            _goodShapes = new List<Shape>();
            Configuration = configuration;
            Factory = factory;
            FileWriter = fileWriter;
            Factory.CreateBoundingShape(Configuration.BoundDimension);
            Factory.SetCountOfInnerShapes(Configuration.Count);
            _shapes = FillArrayOfInnerShapes(Configuration.IsSortingEnable);
        }

        public List<Shape> Run()
        {
            var curCount = 0;
            var attemptCount = 0;
            var shapeIndex = 0;

            while (curCount < Configuration.Count && attemptCount < 1000000000 && shapeIndex < _shapes.Length - 1)
            {
                if (attemptCount > 10000000)
                    shapeIndex += 1;

                var center = Factory.CreatePoint(Configuration.DistributionOfPosition); 
                Factory.CreateInnerShape(center, _shapes[shapeIndex].Dimension);
                _shapes[shapeIndex].Center = center;

                if (Factory.CheckIntersection())
                {
                    attemptCount += 1;
                }
                else
                {
                    Factory.ConfirmAdding();
                    curCount += 1;
                    attemptCount = 0;
                    _goodShapes.Add(_shapes[shapeIndex]);
                    FileWriter.Write(center, _shapes[shapeIndex].Dimension);
                    Console.WriteLine($"N-{curCount}: ({center.X}, {center.Y}, {center.Z}) H:{_shapes[shapeIndex].Dimension.Heigth} W:{_shapes[shapeIndex].Dimension.Width}  L:{_shapes[shapeIndex].Dimension.Length} ");
                    shapeIndex += 1;
                }
            }
            FileWriter.Count += 1;
            Console.WriteLine($"Удалось вместить: {curCount}. \nПройдено фигур: {shapeIndex}");

            return _goodShapes;
        }

        private Shape[] FillArrayOfInnerShapes(bool isSortingEnable)
        {
            var count = Configuration.Count;
            var dimensions = new Dimension[count];

            for (int i = 0; i < count; i++)
            {
                var dimension = new Dimension()
                {
                    Length = CreateLength(),
                    Width = CreateLength(),
                    Heigth = CreateLength(),
                    Theta = CreateLength(),
                    Fi = CreateLength(),
                };
                dimensions[i] = dimension;
            }
            var shapes = Factory.GetArrayOfInnerShapes(dimensions, isSortingEnable);

            return shapes;
        }

        private double CreateLength()
        {
            double length;

            do
            {
                length = Configuration.DistributionOfLength
                        .GetValue(Configuration.MinLength, Configuration.MaxLength);
            } while (!(Configuration.MinLength <= length && length <= Configuration.MaxLength && length > 0)); ;

            return length;
        }
           

    }
}
