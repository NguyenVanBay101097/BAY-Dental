using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F42 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("if exists (SELECT * FROM sys.views WHERE name='model_access_report') DROP VIEW model_access_report");
            migrationBuilder.Sql("CREATE VIEW model_access_report AS ( " +
"SELECT gu.UserId, m.Model, a.GroupId, a.Active, a.PermRead, a.PermCreate, a.PermWrite, a.PermUnlink FROM IRModelAccesses a " +
"INNER JOIN IRModels m ON m.Id = a.ModelId " +
"INNER JOIN ResGroupsUsersRels gu ON gu.GroupId = a.GroupId)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW model_access_report");
        }
    }
}
