namespace CustomerService.Tests;

[CollectionDefinition(nameof(DatabaseCollection))]
public class DatabaseCollection : ICollectionFixture<CustomerPostgresDatabaseFixture>
{
    
}