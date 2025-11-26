using MarkItDoneApi.Infra.Data;
using Microsoft.AspNetCore.Mvc;

namespace MarkItDoneApi.V1.Status;

[ApiController]
[Route("api/v1/status")]
public class StatusController : ControllerBase
{
    public readonly DatabaseStatusChecker _statusChecker;
    public StatusController(DatabaseStatusChecker statusChecker)
    {
        _statusChecker = statusChecker;
    }

    [HttpGet]
    public async Task<IActionResult> GetStatus()
    {
        var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "postgres";
        var status = await _statusChecker.GetDatabaseStatusAsync(dbName);

        return StatusCode(200, new
        {
            updated_at = DateTime.Now,
            dependencies = new
            {
                version = status.Version,
                max_connections = status.MaxConnections,
                opened_connections = status.OpenedConnections
            }
        });
    }
    
}