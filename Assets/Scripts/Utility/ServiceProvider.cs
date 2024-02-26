using System;
using System.Collections.Generic;

public class ServiceProvider : IServiceProvider
{
    Dictionary<Type, object> servicesDictionary;

    public ServiceProvider()
    {
        servicesDictionary = new Dictionary<Type, object>();
    }
    public void RegisterService<Type>(Type service)
    {
        servicesDictionary.Add(typeof(Type), service);
    }
    public object GetService(Type serviceType)
    {
        return servicesDictionary[serviceType];
    }
    public Type GetService<Type>()
    {
        return (Type)servicesDictionary[typeof(Type)];
    }
}