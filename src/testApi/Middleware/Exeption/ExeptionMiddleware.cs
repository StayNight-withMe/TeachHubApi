namespace testApi.Middleware.Exeption
{
    public class ExeptionMiddleware
    {

        private RequestDelegate _next;

        private ILogger _logger;

        public ExeptionMiddleware(RequestDelegate next, ILogger<ExeptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }


        public async Task InvokeAsync(HttpContext httpContext)
        {

            try
            {
                await _next(httpContext);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogDebug("сработал токен отмены");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
            }
        }



    }
}
