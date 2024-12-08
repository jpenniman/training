using Microsoft.Data.Sqlite;

namespace CustomerService.Tests.Lookup;

public class LookupSqliteDatabaseFixture : AbstractSqliteDatabaseFixture
{
    protected override string GetFileName()
    {
        return "test_customer_lookup.db";
    }

    protected override void Seed(string connectionString)
    {
        const string ddl = 
	        @"CREATE TABLE customers (
					customer_id char(5) NOT NULL,
					company_name varchar(40) NOT NULL,
					contact_name varchar(30) NULL,
					contact_title varchar(30) NULL,
					address varchar(60) NULL,
					city varchar(15) NULL,
					region varchar(15) NULL,
					postal_code varchar(10) NULL,
					country varchar(15) NULL,
					phone varchar(24) NULL,
					fax varchar(24) NULL,
					version int not null default (1),
					CONSTRAINT pk_customers PRIMARY KEY (customer_id)
				);";

        const string data =
	        @"INSERT INTO customers (customer_id, company_name, contact_name, contact_title, address, city, region, postal_code, country, phone, fax)
				VALUES
				     ('ALFKI','Alfreds Futterkiste','Maria Anders','Sales Representative','Obere Str. 57','Berlin',NULL,'12209','Germany','030-0074321','030-0076545')
				    ,('ANATR','Ana Trujillo Emparedados y helados','Ana Trujillo','Owner','Avda. de la Constitución 2222','México D.F.',NULL,'05021','Mexico','(5) 555-4729','(5) 555-3745')
				    ,('ANTON','Antonio Moreno Taquería','Antonio Moreno','Owner','Mataderos  2312','México D.F.',NULL,'05023','Mexico','(5) 555-3932',NULL)";
        
        using var cn = new SqliteConnection(connectionString);
        using var cmd = new SqliteCommand(ddl, cn);
        cn.Open();
        cmd.ExecuteNonQuery();
        cmd.CommandText = data;
        cmd.ExecuteNonQuery();
    }
}