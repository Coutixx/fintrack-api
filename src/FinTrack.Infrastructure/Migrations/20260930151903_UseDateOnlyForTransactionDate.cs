using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UseDateOnlyForTransactionDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE "Transactions"
                ALTER COLUMN "Date" TYPE date
                USING ("Date" AT TIME ZONE 'UTC')::date;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE "Transactions"
                ALTER COLUMN "Date" TYPE timestamp with time zone
                USING ("Date"::timestamp AT TIME ZONE 'UTC');
                """);
        }
    }
}
