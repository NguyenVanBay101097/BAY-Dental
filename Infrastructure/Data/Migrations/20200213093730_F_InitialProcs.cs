using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F_InitialProcs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("CREATE FUNCTION nop_splitstring_to_table " +
					"( " +
						"@string NVARCHAR(MAX), " +
						"@delimiter CHAR(1) " +
					") " +
					"RETURNS @output TABLE( " +
						"data NVARCHAR(MAX) " +
					") " +
					"BEGIN " +
						"DECLARE @start INT, @end INT " +
						"SELECT @start = 1, @end = CHARINDEX(@delimiter, @string) " +
						"WHILE @start < LEN(@string) + 1 BEGIN " +
							"IF @end = 0 " +
								"SET @end = LEN(@string) + 1 " +
							"INSERT INTO @output(data) " +
							"VALUES(SUBSTRING(@string, @start, @end - @start)) " +
							"SET @start = @end + 1 " +
							"SET @end = CHARINDEX(@delimiter, @string, @start) " +
						"END " +
						"RETURN " +
					"END");

			migrationBuilder.Sql("CREATE PROCEDURE SearchIrProperty " +
						"@ResIds nvarchar(max) = null, " +
						"@FieldId nvarchar(max) = null, " +
						"@CompanyId nvarchar(max) = null " +
					"AS " +
					"BEGIN " +
						"SET NOCOUNT ON; " +
								"DECLARE @sql nvarchar(max) " +
						"SET @sql = ' " +
						"select* from IRProperties " +
					   "where 1 = 1 ' " +
						"CREATE TABLE #FilteredResIds " +
						"( " +
							"ResId nvarchar(max) " +
						") " +
						"INSERT INTO #FilteredResIds (ResId) " +
						"SELECT CAST(data as nvarchar) FROM nop_splitstring_to_table(@ResIds, ';') " +
						"DECLARE @ResIdsCount int " +
						"SET @ResIdsCount = (SELECT COUNT(1) FROM #FilteredResIds) " +
						"IF @ResIdsCount > 0 " +
						"BEGIN " +
							"SET @sql = @sql + ' " +
							"AND ResId IN(SELECT ResId FROM #FilteredResIds)' " +
						"END " +
						"IF @FieldId != 0 " +
						"BEGIN " +
							"SET @sql = @sql + ' " +
							"AND FieldId = ' + @FieldId + ' ' " +
						"END " +
						"IF @CompanyId != 0 " +
						"BEGIN " +
							"SET @sql = @sql + ' " +
							"AND CompanyId = ' + @CompanyId + ' ' " +
						"END " +
						"SET @sql = @sql + ' " +
						"ORDER BY CompanyId asc' " +
						"EXEC sp_executesql @sql " +
						"DROP TABLE #FilteredResIds " +
					"END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("DROP FUNCTION nop_splitstring_to_table");
			migrationBuilder.Sql("DROP PROCEDURE SearchIrProperty");
		}
    }
}
