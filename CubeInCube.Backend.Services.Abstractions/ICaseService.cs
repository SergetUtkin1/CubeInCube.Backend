using CubeInCube.Backend.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeInCube.Backend.Services.Abstractions
{
    public interface ICaseService
    {
        Task<CaseDto> CreateCase(CaseForCreationDto caseForCreationDto);
    }
}
