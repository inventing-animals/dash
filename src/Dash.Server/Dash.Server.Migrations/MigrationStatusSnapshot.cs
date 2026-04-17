namespace Dash.Server.Migrations;

public sealed record MigrationStatusSnapshot(
    string EnvironmentName,
    string ContentRootPath,
    string DatabaseTarget,
    IReadOnlyList<MigrationStatusItem> Items)
{
    public int AppliedCount => Items.Count(x => x.IsApplied);

    public int PendingCount => Items.Count - AppliedCount;
}

public sealed record MigrationStatusItem(
    long Version,
    string Description,
    bool IsApplied);
