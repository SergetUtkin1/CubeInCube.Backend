using CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.AbstractProducts;

namespace CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.Distributions
{
    public class UniformDistribution : Distribution
    {
        public override double GetValue(double minValue, double maxValue) =>
            minValue + (maxValue - minValue) * Random.Shared.NextDouble();
    }
}
