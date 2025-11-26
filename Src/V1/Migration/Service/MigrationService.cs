using MarkItDoneApi.Infra.Data;
using MarkItDoneApi.V1.Core.DomainExceptions;
using Microsoft.EntityFrameworkCore;

namespace MarkItDoneApi.V1.Migration.Service;

public class MigrationService
{
  private readonly DatabaseContext _context;

  public MigrationService(DatabaseContext context)
  {
    _context = context;
  }

  public async Task<List<string>> ListPendingMigrationsAsync()
  {
    try
    {
      var allMigrations = _context.Database.GetMigrations();
      
      var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();
      
      var pendingMigrations = allMigrations
        .Except(appliedMigrations)
        .ToList();
    
      return pendingMigrations;
    }
    catch (Exception ex)
    {
      throw new ServiceException(
        message: "Erro ao consultar migrations pendentes.",
        action: "Verifique a conexão com o banco de dados e tente novamente.",
        statusCode: 500,
        innerException: ex
      );
    }
  }

  public async Task<List<string>> RunPendingMigrationsAsync()
  {
    try
    {
      var pendingMigrations = await ListPendingMigrationsAsync();

      if (!pendingMigrations.Any())
      {
        return new List<string>();
      }

      await _context.Database.MigrateAsync();

      return pendingMigrations;
    }
    catch (ServiceException)
    {
      throw;
    }
    catch (Exception ex)
    {
      throw new ServiceException(
        message: "Erro ao aplicar migrations pendentes.",
        action: "Verifique a conexão com o banco de dados e tente novamente. Se o problema persistir, contacte o suporte.",
        statusCode: 500,
        innerException: ex
      );
    }
  }
}
