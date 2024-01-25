using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeInCube.Backend.Contracts
{
    public class CaseForCreationDto
    {
        public int Count { get; set; }
        public string InnerShape { get; set; } = null!;
        public DimensionForCreationDto MaxInnerShapeDimension { get; set; } = null!;
        public DimensionForCreationDto MinInnerShapeDimension { get; set; } = null!;
        public bool IsSortingEnable { get; set; } = false;
        public string BoundingShape { get; set; } = null!;
        public DimensionForCreationDto BoundingShapeDimension { get; set; } = null!;
        public string DistributionOfPosition { get; set; } = null!;
        public string DistributionOfLength { get; set; } = null!;
    }
}
