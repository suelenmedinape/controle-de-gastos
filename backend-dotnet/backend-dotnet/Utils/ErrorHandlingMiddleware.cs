namespace backend_dotnet.Utils;

//Ele vai processar as requisições HTTP antes de chegar no controller e vai processar as respostas antes de retornar ao cliente.
/*
    Este método formata a resposta de erro e a envia de volta.
    Fluxo completo:
        1. Requisição chega
        2. ErrorHandlingMiddleware.InvokeAsync() executa
        3. await _next(context) → passa para o controller
        4. Se não der erro, resposta volta normal
        5. Se der erro em qualquer lugar, catch captura
        6. HandleExceptionAsync() formata e retorna o erro no seu formato JSON
*/
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new ErrorResponse
        {
            Message = exception.Message,
            Metadata = null
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}