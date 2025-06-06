using FluentMigrator;

namespace TestProject.Infra.Implements.Migrations;

[Migration(202506060000)] // Год-Месяц-День-Час-Минута
public class CreateTestEntityTable : Migration
{
    public override void Up()
    {
        Create.Table("test_entities")
            .WithColumn("id").AsInt32().PrimaryKey().NotNullable()
            .WithColumn("sum").AsInt32().NotNullable()
            .WithColumn("name").AsString().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("test_entities");
    }
}