using FluentMigrator;

namespace Survey.Persistence
{
    [Migration(20251007131400)]
    public class InitMigration : Migration
    {
        public override void Up()
        {
            Create.Table("surveis")
                  .WithColumn("id").AsGuid().PrimaryKey()
                  .WithColumn("question1").AsInt32()
                  .WithColumn("question2").AsInt32()
                  .WithColumn("question3").AsInt32()
                  .WithColumn("question4").AsInt32()
                  .WithColumn("question5").AsInt32()
                  .WithColumn("question6").AsInt32()
                  .WithColumn("question7").AsInt32()
                  .WithColumn("question8").AsInt32()
                  .WithColumn("question9").AsInt32()
                  .WithColumn("question10").AsInt32()
                  .WithColumn("question11").AsInt32()
                  .WithColumn("question12").AsInt32()
                  .WithColumn("question13").AsInt32()
                  .WithColumn("question14").AsInt32()
                  .WithColumn("question15").AsInt32()
                  .WithColumn("opinion").AsString()
                  .WithColumn("created_on").AsDateTime();
        }

        public override void Down()
        {
            Delete.Table("surveis");
        }
    }
}
