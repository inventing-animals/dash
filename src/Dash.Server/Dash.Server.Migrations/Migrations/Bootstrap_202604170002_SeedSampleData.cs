using System.ComponentModel;
using FluentMigrator;

namespace Dash.Server.Migrations.Migrations;

[Migration(202604170002)]
[Description("Seed sample users, pages, and widgets.")]
public sealed class Bootstrap_202604170002_SeedSampleData : Migration
{
    private static readonly Guid AliceUserId = Guid.Parse("6a827d52-d15a-4944-9f71-0d925e538001");
    private static readonly Guid BobUserId = Guid.Parse("6a827d52-d15a-4944-9f71-0d925e538002");

    private static readonly Guid AliceHomePageId = Guid.Parse("7b938e63-e26b-4a55-af82-1ea36f649101");
    private static readonly Guid AliceWorkPageId = Guid.Parse("7b938e63-e26b-4a55-af82-1ea36f649102");
    private static readonly Guid BobTravelPageId = Guid.Parse("7b938e63-e26b-4a55-af82-1ea36f649103");

    public override void Up()
    {
        Insert.IntoTable("Users")
            .Row(new
            {
                UserId = AliceUserId,
                Name = "Alice Example",
            })
            .Row(new
            {
                UserId = BobUserId,
                Name = "Bob Example",
            });

        Insert.IntoTable("Pages")
            .Row(new
            {
                PageId = AliceHomePageId,
                UserId = AliceUserId,
                Name = "Home",
            })
            .Row(new
            {
                PageId = AliceWorkPageId,
                UserId = AliceUserId,
                Name = "Work",
            })
            .Row(new
            {
                PageId = BobTravelPageId,
                UserId = BobUserId,
                Name = "Travel",
            });

        Insert.IntoTable("Widgets")
            .Row(new
            {
                WidgetId = Guid.Parse("8ca49f74-f37c-4b66-b093-2fb47075a201"),
                PageId = AliceHomePageId,
                WidgetType = "dash.widgets.digital-clock",
                ConfigurationJson = """{"TimeZoneId":"Europe/Bucharest","Format":"HH:mm:ss"}""",
            })
            .Row(new
            {
                WidgetId = Guid.Parse("8ca49f74-f37c-4b66-b093-2fb47075a202"),
                PageId = AliceHomePageId,
                WidgetType = "dash.widgets.email-checker",
                ConfigurationJson = """{"AccountLabel":"Personal inbox","Folder":"Inbox","PollingInterval":"00:05:00"}""",
            })
            .Row(new
            {
                WidgetId = Guid.Parse("8ca49f74-f37c-4b66-b093-2fb47075a203"),
                PageId = AliceWorkPageId,
                WidgetType = "dash.widgets.email-checker",
                ConfigurationJson = """{"AccountLabel":"Work mail","Folder":"Priority","PollingInterval":"00:02:00"}""",
            })
            .Row(new
            {
                WidgetId = Guid.Parse("8ca49f74-f37c-4b66-b093-2fb47075a204"),
                PageId = AliceWorkPageId,
                WidgetType = "dash.widgets.digital-clock",
                ConfigurationJson = """{"TimeZoneId":"UTC","Format":"HH:mm"}""",
            })
            .Row(new
            {
                WidgetId = Guid.Parse("8ca49f74-f37c-4b66-b093-2fb47075a205"),
                PageId = BobTravelPageId,
                WidgetType = "dash.widgets.digital-clock",
                ConfigurationJson = """{"TimeZoneId":"America/New_York","Format":"hh:mm tt"}""",
            });
    }

    public override void Down()
    {
        Delete.FromTable("Widgets").Row(new { WidgetId = Guid.Parse("8ca49f74-f37c-4b66-b093-2fb47075a201") });
        Delete.FromTable("Widgets").Row(new { WidgetId = Guid.Parse("8ca49f74-f37c-4b66-b093-2fb47075a202") });
        Delete.FromTable("Widgets").Row(new { WidgetId = Guid.Parse("8ca49f74-f37c-4b66-b093-2fb47075a203") });
        Delete.FromTable("Widgets").Row(new { WidgetId = Guid.Parse("8ca49f74-f37c-4b66-b093-2fb47075a204") });
        Delete.FromTable("Widgets").Row(new { WidgetId = Guid.Parse("8ca49f74-f37c-4b66-b093-2fb47075a205") });

        Delete.FromTable("Pages").Row(new { PageId = AliceHomePageId });
        Delete.FromTable("Pages").Row(new { PageId = AliceWorkPageId });
        Delete.FromTable("Pages").Row(new { PageId = BobTravelPageId });

        Delete.FromTable("Users").Row(new { UserId = AliceUserId });
        Delete.FromTable("Users").Row(new { UserId = BobUserId });
    }
}
