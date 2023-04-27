using System.Runtime.Serialization;

namespace Northwind.Foundation.Api;

/// <summary>
/// Defines a response that represents a subset (page) of the total entities found during a
/// search or "Get All" query.
/// </summary>
/// <typeparam name="TEntity">The return type of the <see cref="Data"/> property.</typeparam>
[DataContract]
public class PagedResponse<TEntity>
{
    // Required by serializers.
    PagedResponse()
    { }

    /// <summary>
    /// Creates a new successful response.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="totalCount"></param>
    public PagedResponse(TEntity[] data, int totalCount)
    {
        Data = data;
        TotalCount = totalCount;
    }

    /// <summary>
    /// Creates a new failed response
    /// </summary>
    /// <param name="errors"></param>
    public PagedResponse(Error[] errors)
    {
        Errors = errors;
    }
    
    [DataMember(Order = 1)]
    public bool IsSuccessful
    {
        get { return !Errors.Any(); }
        // ReSharper disable once ValueParameterNotUsed
        private set
        {
            // Purposely a no-op.
            // This allows serializers to deserialize the property.
        }
    }
    
    /// <summary>
    /// A collection of errors that occurred during the request.
    /// </summary>
    [DataMember(Order = 2)]
    public Error[] Errors { get; set; } = Array.Empty<Error>();

    /// <summary>
    /// The page of data.
    /// </summary>
    [DataMember(Order = 3)]
    public TEntity[] Data { get; set; } = Array.Empty<TEntity>();
    
    
    /// <summary>
    /// The total number of records/entities that met the original query criteria.
    /// </summary>
    [DataMember(Order = 4)]
    public int TotalCount { get; set; }
}