using CubeInCube.Backend.Domain.Entities.BasicElements;
using CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.AbstractProducts;

namespace CubeInCube.Backend.Domain.Entities
{
    public class CaseConfiguration
    {
        public int Count { get; set; }
        public double MaxLength { get; set; }
        public double MinLength { get; set; }
        public bool IsSortingEnable { get; set; } = false;
        public Dimension BoundDimension { get; set; } = null!;
        public Distribution DistributionOfPosition { get; set; } = null!;
        public Distribution DistributionOfLength { get; set; } = null!;
    }
}
