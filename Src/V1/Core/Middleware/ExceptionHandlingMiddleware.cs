using MarkItDoneApi.V1.Core.DomainExceptions;

namespace MarkItDoneApi.V1.Core.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    
    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessException ex)
        {
            context.Response.StatusCode = ex.StatusCode;
            await context.Response.WriteAsJsonAsync(ex.ToJson());
        }
        catch (ServiceException ex)
        {
            context.Response.StatusCode = ex.StatusCode;
            await context.Response.WriteAsJsonAsync(ex.ToJson());
        }
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = ex.StatusCode;
            await context.Response.WriteAsJsonAsync(ex.ToJson());
        }
        catch (UnauthorizedException ex)
        {
            context.Response.StatusCode = ex.StatusCode;
            await context.Response.WriteAsJsonAsync(ex.ToJson());
        }
        catch (Exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { 
                Name = "InternalServerError",
                Message = "Erro interno do servidor.",
                Action = "Tente novamente mais tarde ou entre em contato com o suporte.",
                StatusCode = 500
            });
        }
    }
}