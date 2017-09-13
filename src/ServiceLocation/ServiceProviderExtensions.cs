using System;

namespace SuperClean.ServiceLocation
{
    public static class ServiceProviderExtensions
    {
        public static T GetService<T>(this IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            return (T)serviceProvider.GetService(typeof(T));
        }
    }
}