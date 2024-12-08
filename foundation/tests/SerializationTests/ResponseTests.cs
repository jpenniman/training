using System.Runtime.Serialization;
using System.Text.Json;
using FluentAssertions;
using Northwind.Foundation;
using Northwind.Foundation.Api;

namespace SerializationTests;

public class ResponseTests
{
    [DataContract]
    class Foo : IEquatable<Foo>
    {
        [DataMember(Order = 1)]
        public string Bar { get; set; }

        public bool Equals(Foo? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Bar == other.Bar;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Foo)obj);
        }

        public override int GetHashCode()
        {
            return Bar.GetHashCode();
        }
    }
    
    [Fact]
    public void ProtobufWithData()
    {
        var original = new Response<Foo>(true, new Foo() {Bar= "test"});
        using var ms = new MemoryStream(); 
        ProtoBuf.Serializer.Serialize(ms, original);
        ms.Position = 0;
        var deserialized = ProtoBuf.Serializer.Deserialize<Response<Foo>>(ms);
        original.Data.Should().Be(deserialized.Data);
    }
    
    [Fact]
    public void ProtobufWithErrors()
    {
        var original = new Response<string>(false, errors: new[] { new Error("TEST_CODE", "Test Title", "Test Detail") });
        using var ms = new MemoryStream(); 
        ProtoBuf.Serializer.Serialize(ms, original);
        ms.Position = 0;
        var deserialized = ProtoBuf.Serializer.Deserialize<Response<string>>(ms);
        original.Errors.SequenceEqual(deserialized.Errors).Should().BeTrue();
    }
    
    [Fact]
    public void JsonWithErrors()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new Error.ErrorJsonConverter());
        
        var original = new Response<string>(false, errors: new[] { new Error("TEST_CODE", "Test Title", "Test Detail") });
        var json = JsonSerializer.Serialize(original, options);
        var deserialized = JsonSerializer.Deserialize<Response<string>>(json, options);
        deserialized.IsSuccessful.Should().BeFalse();
        deserialized.Errors[0].Id.Should().Be(original.Errors[0].Id);
        deserialized.Errors[0].Code.Should().Be(original.Errors[0].Code);
        deserialized.Errors[0].Title.Should().Be(original.Errors[0].Title);
        deserialized.Errors[0].Detail.Should().Be(original.Errors[0].Detail);
    }
}