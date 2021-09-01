using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateCustomerReceiptReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("if exists (SELECT * FROM sys.views WHERE name='customer_receipt_report') DROP VIEW customer_receipt_report");
			migrationBuilder.Sql(@"
                CREATE VIEW customer_receipt_report
                AS (
                    Select cr.* , 
                    (CASE WHEN cr.DateExamination IS NOT NULL THEN (DATEDIFF(MINUTE, cr.DateWaiting, cr.DateExamination))  ELSE NULL END) MinuteWaiting,
                    (CASE WHEN cr.DateDone IS NOT NULL THEN (DATEDIFF(MINUTE, cr.DateExamination, cr.DateDone))  ELSE NULL END) MinuteExamination,
                    (CASE 
                    WHEN cr.DateExamination IS NOT NULL and cr.DateDone IS NOT NULL  THEN ( (DATEDIFF(MINUTE, cr.DateWaiting, cr.DateExamination)) + (DATEDIFF(MINUTE, cr.DateExamination, cr.DateDone))) 
                    WHEN cr.DateExamination IS NOT NULL and cr.DateDone IS NULL  THEN ((DATEDIFF(MINUTE, cr.DateWaiting, cr.DateExamination))) 
                    ELSE NULL 
                    END) MinuteTotal,
                    SUBSTRING((SELECT coalesce(', '+ ps.Name, '') [text()] 
                    FROM CustomerReceiptProductRels AS rels
                       inner join Products ps 
                                  ON ps.Id = rels.ProductId 
                    WHERE  rels.CustomerReceiptId = cr.id
                    ORDER BY 1
                    FOR XML PATH ('')), 2, 1000) AS Products
                    from CustomerReceipts cr
                    left join Employees emp on emp.Id = cr.DoctorId
                    left join Partners pn on pn.Id = cr.PartnerId
                    left join Companies comp on comp.Id = cr.CompanyId
					)
                ");
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW customer_receipt_report ");
        }
    }
}
