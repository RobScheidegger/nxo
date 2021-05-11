using Microsoft.Extensions.DependencyInjection;
using NXO.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NXO.Shared
{
    public interface INXOModule
    {
        string Name { get; }
        void RegisterServices(IServiceCollection services);
        string GameType { get; }
    }
}
