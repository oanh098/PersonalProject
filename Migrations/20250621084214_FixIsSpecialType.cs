using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalProject.Migrations
{
    /// <inheritdoc />
    public partial class FixIsSpecialType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE ""RestaurantMenu"" ALTER COLUMN ""IsSpecial"" TYPE boolean USING CASE WHEN ""IsSpecial"" IN (1, '1') THEN TRUE ELSE FALSE END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE ""RestaurantMenu"" ALTER COLUMN ""IsSpecial"" TYPE integer USING CASE WHEN ""IsSpecial"" = TRUE THEN 1 ELSE 0 END;");
        }
    }
}
