using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using Core.Utilities.Interceptors;
using Microsoft.Extensions.Hosting;

namespace Core.Extensions;
public static class CoreDependecyResolvers
{
    public static IHostBuilder AddDependecyResolvers(this IHostBuilder services)
    {
        services.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        services.ConfigureContainer<ContainerBuilder>((hostContext, builder) =>
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces().
                EnableInterfaceInterceptors(new ProxyGenerationOptions()
                {
                    Selector = new AspectInterceptorSelector()
                }).SingleInstance();
        });
        return services;
    }
}
