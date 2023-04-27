using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Northwind.Foundation.Api;

/// <summary>
/// Represents and API response with a single object payload.
/// </summary>
/// <typeparam name="T">The type of the response.</typeparam>
[DataContract]
public class Response<T>
{
    // Required by serializers.
    Response()
    {}
    
    /// <summary>
    /// Creates a new successful response.
    /// </summary>
    /// <param name="data"></param>
    [JsonConstructor]
    public Response(T? data)
    {
        Data = data;
    }

    /// <summary>
    /// Creates a new failed response.
    /// </summary>
    /// <param name="errors">Errors.</param>
    public Response(Error[] errors)
    {
        Errors = errors;
    }
    
    /// <summary>
    /// Indicates whether or not the request was successful.
    /// </summary>
    [DataMember(Order = 1)]
    public bool IsSuccessful
    {
        get { return !Errors.Any(); }
        // ReSharper disable once ValueParameterNotUsed
        private set
        {
            // Purposely a no-op.
            // This allows protobuf to deserialize the property.
        }
    }
    
    /// <summary>
    /// A collection of errors that occurred during the request.
    /// </summary>
    [DataMember(Order = 2)]
    public Error[] Errors { get; set; } = Array.Empty<Error>(); //JsonSerializer requires a public setter if the property is not set in the JsonConstructor.
    
    /// <summary>
    /// The response payload.
    /// </summary>
    [DataMember(Order = 3)]
    public T? Data { get; }
}