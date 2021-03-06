// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FossApps.KeyStore
{
    using Models;
    using Newtonsoft.Json;

    /// <summary>
    /// </summary>
    public partial interface IKeyStoreClient : System.IDisposable
    {
        /// <summary>
        /// The base URI of the service.
        /// </summary>
        System.Uri BaseUri { get; set; }

        /// <summary>
        /// Gets or sets json serialization settings.
        /// </summary>
        JsonSerializerSettings SerializationSettings { get; }

        /// <summary>
        /// Gets or sets json deserialization settings.
        /// </summary>
        JsonSerializerSettings DeserializationSettings { get; }


        /// <summary>
        /// Gets the IBasicPing.
        /// </summary>
        IBasicPing BasicPing { get; }

        /// <summary>
        /// Gets the IKeys.
        /// </summary>
        IKeys Keys { get; }

    }
}
