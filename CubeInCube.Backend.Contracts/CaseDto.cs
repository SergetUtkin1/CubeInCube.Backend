using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeInCube.Backend.Contracts
{
    public class CaseDto
    {
        public int Count { get; set; }
        public List<ShapeLookUpDto>? Shapes { get; set; }
    }
}
