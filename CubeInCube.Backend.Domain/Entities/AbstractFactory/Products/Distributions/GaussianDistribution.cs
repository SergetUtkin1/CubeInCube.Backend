using MathNet.Numerics.Distributions;
using CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.AbstractProducts;

namespace CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.Distributions
{
    public class GaussianDistribution : Distribution
    {
        public override double GetValue(double minValue, double maxValue)
        {
            var dist = new Normal();
            return minValue + (maxValue - minValue) * dist.Sample();
        }            
    }
}
