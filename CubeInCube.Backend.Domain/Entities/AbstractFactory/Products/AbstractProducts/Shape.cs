using CubeInCube.Backend.Domain.Entities.BasicElements;

namespace CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.AbstractProducts
{
    public abstract class Shape
    {
        public Position Center { get; set;   }
        public Dimension Dimension { get; set; }
        public double Volume { get; set; }

        public Shape(Position center, double length, double width, double heigth, double theta, double fi)
        {
            Dimension = new Dimension(length, width, heigth, theta, fi);
            Center = center;
            Volume = GetVolume();
        }

        public Shape(Position center, double length, double width, double heigth)
        {
            Dimension = new Dimension(length, width, heigth);
            Center = center;
            Volume = GetVolume();
        }

        public Shape(Position center, double length)
        {
            Dimension = new Dimension(length);
            Center = center;
            Volume = GetVolume();
        }

        protected abstract double GetVolume();
    }
}
