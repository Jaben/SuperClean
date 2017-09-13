//  The MIT License (MIT)
//  
//  Copyright (c) 2016 CaptiveAire Limitied
//  
//  Permission is hereby granted, free of charge, to any person obtaining a copy of
//  this software and associated documentation files (the "Software"), to deal in
//  the Software without restriction, including without limitation the rights to
//  use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//  the Software, and to permit persons to whom the Software is furnished to do so,
//  subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//  FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//  COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//  IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//  CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//  

using System;
using System.Collections.Concurrent;

namespace SuperClean.ServiceLocation
{
    public class AppServiceLocation : IServiceProvider
    {
        protected AppServiceLocation()
        {
        }

        public static IServiceProvider Instance { internal get; set; } = new AppServiceLocation();

        static readonly ConcurrentDictionary<Type, Func<IServiceProvider, object>> _servicesFactories =
            new ConcurrentDictionary<Type, Func<IServiceProvider, object>>();

        static readonly ConcurrentDictionary<Type, object> _services = new ConcurrentDictionary<Type, object>();

        object IServiceProvider.GetService(Type serviceType)
        {
            if (!_services.TryGetValue(serviceType, out var instance))
            {
                if (_servicesFactories.TryGetValue(serviceType, out var factory))
                {
                    instance = factory(this);
                    _services.TryAdd(serviceType, instance);
                }
            }

            return instance;
        }

        internal static void RegisterService<T>(Func<IServiceProvider, T> instanceFunc, Action<T, IServiceProvider> onActivate = null)
            where T : class
        {
            _servicesFactories.TryAdd(
                typeof(T),
                serviceProvider =>
                    {
                        var createdInstance = instanceFunc(serviceProvider);
                        onActivate?.Invoke(createdInstance, serviceProvider);
                        return createdInstance;
                    });
        }
    }
}