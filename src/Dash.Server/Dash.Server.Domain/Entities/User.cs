namespace Dash.Server.Domain.Entities;

public sealed class User
{
    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Page> Pages { get; set; } = [];
}
