using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Northwind.Foundation.Api;

/// <summary>
/// Error objects provide additional information about problems encountered while performing an operation.
/// Inspired by the JSON-API spec: https://jsonapi.org/format/#error-objects
/// </summary>
[DataContract]
public sealed class Error : IEquatable<Error>
{
    const int ID_LENGTH = 8;
    
    // Required for Protobuf-Net Serializer.
    Error() : this(IdGenerator.Generate(ID_LENGTH), "UNKNOWN", "Not provided.", "Not provided.")
    { }

    // Used for JSON Serialization
    Error(string id, string code, string title, string detail)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException(
                "An error ID must be provided. This should be a unique identifier for this particular occurrence of the problem.",
                nameof(id));
        
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "An error code must be provided. This should be an application-specific error code. E.g. INVALID_ORDER_QUANTITY",
                nameof(code));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(
                "Please provide a title for this error. This should be a short, human-readable summary of the problem that SHOULD NOT change from occurrence to occurrence of the problem.",
                nameof(title));

        if (string.IsNullOrWhiteSpace(detail))
            throw new ArgumentException(
                "Please provide a detailed description of this error. This should be A human-readable explanation specific to this occurrence of the problem.",
                nameof(detail));
        
        Id = id;
        Code = code;
        Title = title;
        Detail = detail;
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="code">Required. An application-specific error code, expressed as a string value.</param>
    /// <param name="title">Required. A short, human-readable summary of the problem that SHOULD NOT change
    /// from occurrence to occurrence of the problem, except for purposes of localization.</param>
    /// <param name="detail">Required. A human-readable explanation specific to this occurrence of the problem.
    /// Like title, this field’s value can be localized.</param>
    public Error(string code, string title, string detail) : this (IdGenerator.Generate(ID_LENGTH), code, title, detail)
    { }

    /// <summary>
    /// A unique identifier for this particular occurrence of the problem.
    /// This ID can be presented to the user to relay to support to facilitate ease of searching
    /// logs for the specific occurrence of the error.
    /// </summary>
    [DataMember(Order = 1)] 
    public string Id { get; }

    /// <summary>
    /// An application-specific error code, expressed as a string value.
    /// </summary>
    [DataMember(Order = 2)] 
    public string Code { get; }
    
    /// <summary>
    /// A short, human-readable summary of the problem that SHOULD NOT change from occurrence to occurrence of the problem, except for purposes of localization.
    /// </summary>
    [DataMember(Order = 3)] 
    public string Title { get; }

    /// <summary>
    /// A human-readable explanation specific to this occurrence of the problem. Like title, this field’s value can be localized.
    /// </summary>
    [DataMember(Order = 4)] 
    public string Detail { get; }

    [DataMember(Order = 5)]
    public Dictionary<string, string> Metadata { get; set; } = new();
    
    ///<inheritdoc cref="IEquatable{T}.Equals(T)"/>
    public bool Equals(Error? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id && Code == other.Code && Title == other.Title && Detail == other.Detail;
    }

    ///<inheritdoc cref="Object.Equals(object)"/>
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is Error other && Equals(other);
    }

    ///<inheritdoc cref="Object.GetHashCode"/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Code, Title, Detail);
    }

    /// <summary>
    /// System.Text.Json converter.
    /// </summary>
    public class ErrorJsonConverter : JsonConverter<Error>
    {
        ///<inheritdoc cref="JsonConverter{T}.Read"/>
        public override Error Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var id = string.Empty;
            var code = string.Empty;
            var title = string.Empty;
            var detail = string.Empty;
            
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;
                
                var propertyName = reader.GetString();
                reader.Read();
                var value = reader.GetString();

                switch (propertyName)
                {
                    case nameof(Id):
                        id = value ?? string.Empty;
                        break;
                    case nameof(Code):
                        code = value ?? string.Empty;
                        break;
                    case nameof(Title):
                        title = value ?? string.Empty;
                        break;
                    case nameof(Detail):
                        detail = value ?? string.Empty;
                        break;
                }
            }

            return new Error(id, code, title, detail);
        }

        ///<inheritdoc cref="JsonConverter{T}.Write"/>
        public override void Write(Utf8JsonWriter writer, Error value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(Id), value.Id);
            writer.WriteString(nameof(Code), value.Code);
            writer.WriteString(nameof(Title), value.Title);
            writer.WriteString(nameof(Detail), value.Detail);
            writer.WriteEndObject();
        }
    }
}