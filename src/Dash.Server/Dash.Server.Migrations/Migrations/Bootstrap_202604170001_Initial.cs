using System.ComponentModel;
using FluentMigrator;

namespace Dash.Server.Migrations.Migrations;

[Migration(202604170001)]
[Description("Create initial Users, Pages, and Widgets tables.")]
public sealed class Bootstrap_202604170001_Initial : Migration
{
    public override void Up()
    {
        Create.Table("Users")
            .WithColumn("UserId").AsGuid().PrimaryKey()
            .WithColumn("Name").AsString(200).NotNullable();

        Create.Table("Pages")
            .WithColumn("PageId").AsGuid().PrimaryKey()
            .WithColumn("UserId").AsGuid().NotNullable().Indexed()
            .WithColumn("Name").AsString(200).NotNullable();

        Create.Table("Widgets")
            .WithColumn("WidgetId").AsGuid().PrimaryKey()
            .WithColumn("PageId").AsGuid().NotNullable().Indexed()
            .WithColumn("WidgetType").AsString(200).NotNullable()
            .WithColumn("ConfigurationJson").AsString(int.MaxValue).NotNullable();

        Create.Index("IX_Pages_UserId_Name")
            .OnTable("Pages")
            .OnColumn("UserId").Ascending()
            .OnColumn("Name").Ascending();

        Create.Index("IX_Widgets_PageId_WidgetType")
            .OnTable("Widgets")
            .OnColumn("PageId").Ascending()
            .OnColumn("WidgetType").Ascending();
    }

    public override void Down()
    {
        Delete.Index("IX_Widgets_PageId_WidgetType").OnTable("Widgets");
        Delete.Index("IX_Pages_UserId_Name").OnTable("Pages");
        Delete.Table("Widgets");
        Delete.Table("Pages");
        Delete.Table("Users");
    }
}
