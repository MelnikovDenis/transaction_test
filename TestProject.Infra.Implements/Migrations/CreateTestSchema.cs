using FluentMigrator;

namespace TestProject.Infra.Implements.Migrations;

[Migration(202507040000)] // Год-Месяц-День-Час-Минута
public class CreateTestSchema : Migration
{
    public override void Up()
    {
        Create.Table("test_entities")
            .WithColumn("id").AsInt32().PrimaryKey().Identity().NotNullable()
            .WithColumn("sum").AsInt32().NotNullable()
            .WithColumn("name").AsString().NotNullable();

        Create.Table("test_sub_entities")
            .WithColumn("id").AsInt32().PrimaryKey().Identity().NotNullable()
            .WithColumn("name").AsString().NotNullable()
            .WithColumn("test_entity_id").AsInt32().NotNullable();

        Create.ForeignKey("fk_test_sub_entities_test_entity_id")
            .FromTable("test_sub_entities").ForeignColumn("test_entity_id")
            .ToTable("test_entities").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);
    }

    public override void Down()
    {
        Delete.ForeignKey("fk_test_sub_entities_test_entity_id").OnTable("test_sub_entities");
        Delete.Table("test_sub_entities");
        Delete.Table("test_entities");
    }
}