using System.ComponentModel;
using FluentMigrator;

namespace Dash.Server.Migrations.Migrations;

[Migration(202604190001)]
[Description("Add nullable StateJson column to Widgets for caching last-known widget state.")]
public sealed class Bootstrap_202604190001_AddWidgetState : Migration
{
    public override void Up()
    {
        Alter.Table("Widgets")
            .AddColumn("StateJson").AsString(int.MaxValue).Nullable();
    }

    public override void Down()
    {
        Delete.Column("StateJson").FromTable("Widgets");
    }
}
