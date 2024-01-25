using CubeInCube.Backend.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeInCube.Backend.Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<ICaseService> _lazyCaseService;

        public ServiceManager()
        {
            _lazyCaseService = new Lazy<ICaseService>(() => new CaseService());
        }

        public ICaseService CaseService => _lazyCaseService.Value;
    }
}
