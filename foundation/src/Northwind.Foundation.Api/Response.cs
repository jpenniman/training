using System.Runtime.Serialization;

namespace Northwind.Foundation.Api;

/// <summary>
/// Represents and API response with a single object payload.
/// </summary>
/// <typeparam name="TEntity">The type of the response.</typeparam>
[DataContract]
public class Response<TEntity> where TEntity : class
{
    // Required by serializers.
    Response()
    {
        Errors = Array.Empty<Error>();
    }

    /// <summary>
    /// Creates a new successful response.
    /// </summary>
    /// <param name="data"></param>
    public Response(bool isSuccessful, TEntity? data = null, Error[]? errors = null)
    {
        IsSuccessful = isSuccessful;
        Data = data;
        Errors = errors ?? Array.Empty<Error>();
    }

    /// <summary>
    /// Indicates whether or not the request was successful.
    /// </summary>
    [DataMember(Order = 1)]
    public bool IsSuccessful { get; private set; }

    /// <summary>
    /// A collection of errors that occurred during the request.
    /// </summary>
    [DataMember(Order = 2)]
    public Error[] Errors { get; private set; }
    
    /// <summary>
    /// The response payload.
    /// </summary>
    [DataMember(Order = 3)]
    public TEntity? Data { get; private set; }
}