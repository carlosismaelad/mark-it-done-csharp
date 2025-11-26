using MarkItDoneApi.V1.Migration.Service;
using Microsoft.AspNetCore.Mvc;

namespace MarkItDoneApi.V1.Migration.Rest;

[ApiController]
[Route("api/v1/migrations")]
public class MigrationController : ControllerBase
{
  private readonly MigrationService _migrationService;

  public MigrationController(MigrationService migrationService)
  {
    _migrationService = migrationService;
  }

  /// <summary>
  /// Lists all pending (unapplied) migrations
  /// Returns an empty list [] if there are no pending migrations
  /// </summary>
  /// <returns>List of pending migrations or empty list</returns>
  [HttpGet]
  public async Task<IActionResult> GetPendingMigrationsAsync()
  {
    var pendingMigrations = await _migrationService.ListPendingMigrationsAsync();
    return Ok(pendingMigrations);
  }

  /// <summary>
  /// Applies all pending migrations
  /// Returns an empty list [] if there are no migrations to apply
  /// </summary>
  /// <returns>List of migrations that have been applied or an empty list</returns>
  [HttpPost]
  public async Task<IActionResult> ApplyPendingMigrationsAsync()
  {
    var appliedMigrations = await _migrationService.RunPendingMigrationsAsync();
    return Ok(appliedMigrations);
  }
}