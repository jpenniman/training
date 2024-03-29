namespace CustomerService.Tests;

public class CustomerPostgresDatabaseFixture : PostgresDatabaseFixture
{
    public CustomerPostgresDatabaseFixture()
    {
        Seed("../../../../../database/customers.sql");
    }
}