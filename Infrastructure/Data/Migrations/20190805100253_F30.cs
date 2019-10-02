using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F30 : Migration
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW stock_history");
        }
    }
}
