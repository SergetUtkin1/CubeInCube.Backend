using CubeInCube.Backend.Contracts;
using CubeInCube.Backend.Domain.Entities.AbstractFactory.Products.Distributions;
using CubeInCube.Backend.Domain.Entities.AbstractFactory;
using CubeInCube.Backend.Domain.Entities.BasicElements;
using CubeInCube.Backend.Domain.Entities;
using CubeInCube.Backend.Domain.Utils;
using CubeInCube.Backend.Services.Abstractions;
using ShapesInShape.ConsoleApplication.Models.AbstractFactory.ConcreteFactories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShapesInShape.Models.AbstractFactory.ConcreteFactories;

namespace CubeInCube.Backend.Services
{
    public class CaseService : ICaseService
    {
        public async Task<CaseDto> CreateCase(CaseForCreationDto caseForCreationDto)
        {
            var factory = new SpheresInParallelepipedFactory();
            var fileWriter = new FileWriter();
            var configuration = new CaseConfiguration()
            {
                Count = caseForCreationDto.Count,
                BoundDimension = new Dimension(caseForCreationDto.BoundingShapeDimension.Length, caseForCreationDto.BoundingShapeDimension.Width, caseForCreationDto.BoundingShapeDimension.Heigth),
                MaxLength = caseForCreationDto.MaxInnerShapeDimension.Length,
                MinLength = caseForCreationDto.MinInnerShapeDimension.Length,
                IsSortingEnable = caseForCreationDto.IsSortingEnable,
                DistributionOfLength = new UniformDistribution(),
                DistributionOfPosition = new UniformDistribution()
            };

            var myCase = new Case(factory, configuration, fileWriter);

            var shapes = myCase.Run();
            var caseDto = new CaseDto();
            caseDto.Count = shapes.Count;
            foreach (var shape in shapes) 
            { 
                var shapeDto = new ShapeLookUpDto()
                {
                    Heigth = shape.Dimension.Heigth,
                    Length = shape.Dimension.Length,
                    Width = shape.Dimension.Width,
                    Fi = shape.Dimension.Fi,
                    Theta = shape.Dimension.Theta,
                    X = shape.Center.X,
                    Y = shape.Center.Y,
                    Z = shape.Center.Z,
                };
                caseDto.Shapes.Add(shapeDto);
            }

            return caseDto;
        }
    }
}
