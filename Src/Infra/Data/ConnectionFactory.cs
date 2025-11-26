using System.Data;
using MarkItDoneApi.V1.Core.DomainExceptions;
using Npgsql;

namespace MarkItDoneApi.Infra.Data;

public interface IConnectionFactory
{
  IDbConnection CreateConnection();
}

public class ConnectionFactory : IConnectionFactory
{
  private readonly string _connectionString;

  public ConnectionFactory(IConfiguration configuration)
  {

    if (string.IsNullOrEmpty(configuration.GetConnectionString("DefaultConnection")))
    {
      throw new ServiceException("A connection string 'DefaultConnection' não foi encontrada ou está vazia.");
    }

    _connectionString = configuration.GetConnectionString("DefaultConnection")!;
  }

  public IDbConnection CreateConnection()
  {
    return new NpgsqlConnection(_connectionString);
  }
}