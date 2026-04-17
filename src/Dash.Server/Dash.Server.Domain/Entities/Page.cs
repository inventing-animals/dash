namespace Dash.Server.Domain.Entities;

public sealed class Page
{
    public Guid PageId { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public User User { get; set; } = null!;

    public ICollection<Widget> Widgets { get; set; } = [];
}
