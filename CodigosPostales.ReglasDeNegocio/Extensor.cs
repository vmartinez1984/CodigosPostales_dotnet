using CodigosPostales.Repositorios;
using Microsoft.Extensions.DependencyInjection;

namespace CodigosPostales.ReglasDeNegocio
{
    public static class Extensor
    {
        public static void AgregarReglasDeNegocio(this IServiceCollection services)
        {
            services.AddSingleton<IRepositorio, Repositorio>();
            services.AddSingleton<ICodigoPostalRdn, CodigoPostalRdn>();
        }
    }
}
