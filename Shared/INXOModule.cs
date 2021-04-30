using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Shared
{
    public interface INXOModule
    {
        string Name { get; }
        void RegisterServices(IServiceCollection services);
        string GameType { get; }
        Type Settings { get; }
    }
}
