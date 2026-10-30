using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

using CRUD_Razor.Data;
using System.Data.Common;

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
            using var connection = BookDatabase.CreateConnection(configuration);
            if (string.IsNullOrWhiteSpace(connection.DataSource))
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
        if (executed.Exception is DbException error)
        {
            logger.LogWarning("Book database operation failed with error code {Code}.", error.ErrorCode);
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
