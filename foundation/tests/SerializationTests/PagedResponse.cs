using FluentAssertions;
using Northwind.Foundation;
using Northwind.Foundation.Api;

namespace SerializationTests;

public class PagedResponse
{
    [Fact]
    public void ProtobufWithData()
    {
        var original = new PagedResponse<string>(new[] { "test" }, 10);
        using var ms = new MemoryStream(); 
        ProtoBuf.Serializer.Serialize(ms, original);
        ms.Position = 0;
        var deserialized = ProtoBuf.Serializer.Deserialize<PagedResponse<string>>(ms);
        Assert.Equal(original.TotalCount, deserialized.TotalCount);
        original.Data.SequenceEqual(deserialized.Data).Should().BeTrue();
    }
    
    [Fact]
    public void ProtobufWithErrors()
    {
        var original = new PagedResponse<string>(new[] { new Error("TEST_CODE", "Test Title", "Test Detail") });
        using var ms = new MemoryStream(); 
        ProtoBuf.Serializer.Serialize(ms, original);
        ms.Position = 0;
        var deserialized = ProtoBuf.Serializer.Deserialize<PagedResponse<string>>(ms);
        original.Errors.SequenceEqual(deserialized.Errors).Should().BeTrue();
    }
}