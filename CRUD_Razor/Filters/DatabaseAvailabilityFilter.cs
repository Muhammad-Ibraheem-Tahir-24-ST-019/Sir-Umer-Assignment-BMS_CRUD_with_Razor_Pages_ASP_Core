using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace CRUD_Razor.Filters;

public sealed class DatabaseAvailabilityFilter(
    IConfiguration configuration,
    ILogger<DatabaseAvailabilityFilter> logger) : IAsyncPageFilter
{
    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context) => Task.CompletedTask;

    public async Task OnPageHandlerExecutionAsync(
        PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            ShowUnavailable(context);
            return;
        }

        try
        {
            var settings = new SqlConnectionStringBuilder(connectionString);
            if (string.IsNullOrWhiteSpace(settings.DataSource))
            {
                ShowUnavailable(context);
                return;
            }
        }
        catch (ArgumentException)
        {
            // Do not expose a connection string or its credentials in the response.
            logger.LogWarning("The book database connection configuration is invalid.");
            ShowUnavailable(context);
            return;
        }

        var executed = await next();
        if (executed.Exception is SqlException error)
        {
            logger.LogWarning("Book database operation failed with SQL error {Number}.", error.Number);
            ((PageModel)context.HandlerInstance).ViewData["DatabaseUnavailable"] = true;
            executed.Result = new PageResult { StatusCode = StatusCodes.Status503ServiceUnavailable };
            executed.ExceptionHandled = true;
        }
    }

    private static void ShowUnavailable(PageHandlerExecutingContext context)
    {
        ((PageModel)context.HandlerInstance).ViewData["DatabaseUnavailable"] = true;
        context.Result = new PageResult { StatusCode = StatusCodes.Status503ServiceUnavailable };
    }
}

