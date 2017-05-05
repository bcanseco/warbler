using System;

namespace Warbler.Misc
{
    /// <summary>
    ///   Parent class for services that directly interoperate with hubs.
    ///   Provides lazy-loading support (convention-friendly singleton).
    /// </summary>
    /// <typeparam name="TService">The service subclass.</typeparam>
    public abstract class HubResource<TService>
        where TService : new()
    {
        private static readonly Lazy<TService> Resource =
            new Lazy<TService>(() => new TService());

        /// <summary> Gets the singleton instance of the service. </summary>
        public static TService Instance => Resource.Value;
    }
}
