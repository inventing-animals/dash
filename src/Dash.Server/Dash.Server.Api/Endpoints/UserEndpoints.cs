using Dash.Server.Api.Contracts.Users;
using Dash.Server.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Dash.Server.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/api/users")
            .WithTags("Users");

        users.MapGet(
                "/{userId:guid}/pages",
                GetUserPagesAsync)
            .WithName("GetUserPages")
            .WithSummary("Fetch pages and widgets for a user.")
            .WithDescription("Returns all pages for the supplied user GUID together with each page's configured widgets.");
    }

    private static async Task<Results<Ok<GetUserPagesResponse>, NotFound>> GetUserPagesAsync(
        Guid userId,
        DashDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var response = await dbContext.Users
            .AsNoTracking()
            .AsSplitQuery()
            .Where(user => user.UserId == userId)
            .Select(user => new GetUserPagesResponse(
                user.UserId,
                user.Name,
                user.Pages
                    .OrderBy(page => page.Name)
                    .Select(page => new PageSummaryResponse(
                        page.PageId,
                        page.Name,
                        page.Widgets
                            .OrderBy(widget => widget.WidgetType)
                            .ThenBy(widget => widget.WidgetId)
                            .Select(widget => new WidgetSummaryResponse(
                                widget.WidgetId,
                                widget.WidgetType,
                                widget.ConfigurationJson))
                            .ToList()))
                    .ToList()))
            .SingleOrDefaultAsync(cancellationToken);

        return response is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(response);
    }
}
