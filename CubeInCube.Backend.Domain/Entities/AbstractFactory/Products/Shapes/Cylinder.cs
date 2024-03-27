using CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.AbstractProducts;
using CubeInCube.Backend.Domain.Entities.BasicElements;

namespace CubeInCube.Backend.Domain.Entities.Models.AbstractFactory.Products.Shapes
{
    public class Cylinder : Shape
    {
        public Cylinder(Position center, double length, double width, double heigth, double theta, double fi) : base(center, length, width, heigth, theta, fi)
        {
            Center = center;
            Dimension = new Dimension()
            {
                Height = heigth,
                Length = length,
                Width = length,
                Theta = theta,
                Fi = fi,
            };
        }

        protected override double GetVolume()
        {
            double volume = Dimension.Length * double.Pi * Math.Pow(Dimension.Width, 2);
            return volume;
        }
    
    }
}
