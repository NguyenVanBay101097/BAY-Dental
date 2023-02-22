using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F_InitialViews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("if exists (SELECT * FROM sys.views WHERE name='stock_history') DROP VIEW stock_history");
            migrationBuilder.Sql("CREATE VIEW stock_history AS ( " +
              "SELECT move_id, " +
                "location_id, " +
                "company_id, " +
                "product_id, " +
                "product_categ_id, " +
                "SUM(quantity) as quantity, " +
                "date, " +
                "price_unit_on_quant " +
                "FROM " +
                "((SELECT " +
                    "quant.Id AS quant_id, " +
                    "StockMoves.Id AS move_id, " +
                    "dest_location.Id AS location_id, " +
                    "dest_location.CompanyId AS company_id, " +
                    "StockMoves.ProductId AS product_id, " +
                    "Products.CategId AS product_categ_id, " +
                    "quant.Qty AS quantity, " +
                    "StockMoves.date AS date, " +
                    "quant.Cost as price_unit_on_quant " +
                "FROM StockQuants as quant, StockQuantMoveRel, StockMoves " +
                "LEFT JOIN StockLocations dest_location ON StockMoves.LocationDestId = dest_location.Id " +
                "LEFT JOIN StockLocations source_location ON StockMoves.LocationId = source_location.Id " +
                "LEFT JOIN Products ON Products.Id = StockMoves.ProductId " +
                "WHERE quant.Qty > 0 AND StockMoves.State = 'done' AND dest_location.Usage in ('internal', 'transit') AND StockQuantMoveRel.QuantId = quant.Id " +
                "AND StockQuantMoveRel.MoveId = StockMoves.Id AND( " +
                    "(source_location.CompanyId is null and dest_location.CompanyId is not null) or " +
                    "(source_location.CompanyId is not null and dest_location.CompanyId is null) or " +
                    "source_location.CompanyId != dest_location.CompanyId or " +
                    "source_location.Usage not in ('internal', 'transit')) " +
                ") UNION " +
                "(SELECT " +
                    "quant.Id AS quant_id, " +
                    "StockMoves.Id AS move_id, " +
                    "source_location.Id AS location_id, " +
                    "source_location.CompanyId AS company_id, " +
                    "StockMoves.ProductId AS product_id, " +
                    "Products.CategId AS product_categ_id, " +
                    "-quant.Qty AS quantity, " +
                    "StockMoves.date AS date, " +
                    "quant.cost as price_unit_on_quant " +
                "FROM StockQuants as quant, StockQuantMoveRel, StockMoves " +
                "LEFT JOIN StockLocations source_location ON StockMoves.LocationId = source_location.Id " +
                "LEFT JOIN StockLocations dest_location ON StockMoves.LocationDestId = dest_location.Id " +
                "LEFT JOIN Products ON Products.Id = StockMoves.ProductId " +
                "WHERE quant.Qty > 0 AND StockMoves.State = 'done' AND source_location.Usage in ('internal', 'transit') AND StockQuantMoveRel.QuantId = quant.Id " +
                "AND StockQuantMoveRel.MoveId = StockMoves.Id AND( " +
                    "(dest_location.CompanyId is null and source_location.CompanyId is not null) or " +
                    "(dest_location.CompanyId is not null and source_location.CompanyId is null) or " +
                    "dest_location.CompanyId != source_location.CompanyId or " +
                    "dest_location.Usage not in ('internal', 'transit')) " +
                ")) " +
                "AS foo " +
                "GROUP BY move_id, location_id, company_id, product_id, product_categ_id, date, price_unit_on_quant)");

            migrationBuilder.Sql("if exists (SELECT * FROM sys.views WHERE name='sale_report') DROP VIEW sale_report");
            migrationBuilder.Sql("CREATE VIEW sale_report AS ( " +
                "SELECT l.ProductId as ProductId, " +
                "p.UOMId as ProductUOMId, " +
                "SUM(l.ProductUOMQty) as ProductUOMQty, " +
                "SUM(l.QtyInvoiced) as QtyInvoiced, " +
                "SUM(l.QtyToInvoice) as QtyToInvoice, " +
                "SUM(l.PriceTotal) as PriceTotal, " +
                "SUM(l.PriceSubTotal) as PriceSubTotal, " +
                "COUNT(*) as Nbr, " +
                "s.Name as Name, " +
                "s.DateOrder as Date, " +
                "s.State as State, " +
                "s.IsQuotation as IsQuotation, " +
                "s.PartnerId as PartnerId, " +
                "s.UserId as UserId, " +
                "s.CompanyId as CompanyId, " +
                "p.CategId as CategId " +
                "FROM SaleOrderLines l " +
                "JOIN SaleOrders s on l.OrderId = s.Id " +
                "JOIN Partners partner on s.PartnerId = partner.Id " +
                "LEFT JOIN Products p on l.ProductId = p.Id " +
                "GROUP BY l.ProductId, " +
                "l.OrderId, " +
                "p.UOMId, " +
                "p.CategId, " +
                "s.Name, " +
                "s.IsQuotation, " +
                "s.DateOrder, " +
                "s.PartnerId, " +
                "s.UserId, " +
                "s.State, " +
                "s.CompanyId)");

            migrationBuilder.Sql("if exists (SELECT * FROM sys.views WHERE name='model_access_report') DROP VIEW model_access_report");
            migrationBuilder.Sql("CREATE VIEW model_access_report AS ( " +
                "SELECT gu.UserId, m.Model, a.GroupId, a.Active, a.PermRead, a.PermCreate, a.PermWrite, a.PermUnlink FROM IRModelAccesses a " +
                "INNER JOIN IRModels m ON m.Id = a.ModelId " +
                "INNER JOIN ResGroupsUsersRels gu ON gu.GroupId = a.GroupId)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW stock_history");
            migrationBuilder.Sql("DROP VIEW sale_report");
            migrationBuilder.Sql("DROP VIEW model_access_report");
        }
    }
}
