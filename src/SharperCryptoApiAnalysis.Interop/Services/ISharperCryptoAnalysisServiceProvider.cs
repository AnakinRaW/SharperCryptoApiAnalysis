using System;
using System.ComponentModel.Composition.Hosting;

namespace SharperCryptoApiAnalysis.Interop.Services
{
    /// <summary>
    /// Provides methods to get MEF exported instances and VS registered services
    /// </summary>
    /// <seealso cref="System.IServiceProvider" />
    public interface ISharperCryptoAnalysisServiceProvider : IServiceProvider
    {
        /// <summary>
        /// The MEF export provider.
        /// </summary>
        ExportProvider ExportProvider { get; }

        /// <summary>
        /// The current service provider.
        /// </summary>
        IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Gets a service.
        /// </summary>
        /// <typeparam name="T">The type of the service</typeparam>
        /// <returns>The service</returns>
        T GetService<T>() where T : class;

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="T">The base type of the service</typeparam>
        /// <typeparam name="TV">The type of the returned value</typeparam>
        /// <returns>The service</returns>
        TV GetService<T, TV>() where T : class where TV : class;

        /// <summary>
        /// Tries to get a service.
        /// </summary>
        /// <param name="t">The type of the service</param>
        /// <returns>The service</returns>
        object TryGetService(Type t);

        /// <summary>
        /// Tries to get a service.
        /// </summary>
        /// <param name="typename">The typename of the service.</param>
        /// <returns>The service</returns>
        object TryGetService(string typename);

        /// <summary>
        /// Tries to get a service.
        /// </summary>
        /// <typeparam name="T">The type of the service</typeparam>
        /// <returns>The service</returns>
        T TryGetService<T>() where T : class;

        /// <summary>
        /// Adds a service instance.
        /// </summary>
        /// <param name="t">The type of the service</param>
        /// <param name="owner">The owner.</param>
        /// <param name="instance">The instance.</param>
        void AddService(Type t, object owner, object instance);

        /// <summary>
        /// Adds a service instance.
        /// </summary>
        /// <typeparam name="T">The type of the service</typeparam>
        /// <param name="owner">The owner.</param>
        /// <param name="instance">The instance.</param>
        void AddService<T>(object owner, T instance) where T : class;

        /// <summary>
        /// Removes the service.
        /// </summary>
        /// <param name="t">The type of the service</param>
        /// <param name="owner">The owner.</param>
        void RemoveService(Type t, object owner);
    }
}
