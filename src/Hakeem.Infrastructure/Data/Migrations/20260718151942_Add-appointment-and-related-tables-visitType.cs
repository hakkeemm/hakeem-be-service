using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hakeem.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddappointmentandrelatedtablesvisitType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "QuickVisitFee",
                table: "AspNetUsers",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "NewVisitFee",
                table: "AspNetUsers",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "FollowUpFee",
                table: "AspNetUsers",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ConsultationFee",
                table: "AspNetUsers",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AverageRating",
                table: "AspNetUsers",
                type: "numeric(3,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.Sql("ALTER TABLE \"Appointments\" ALTER COLUMN \"VisitType\" TYPE integer USING 0;");

            migrationBuilder.AlterColumn<int>(
                name: "VisitType",
                table: "Appointments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.Sql("ALTER TABLE \"Appointments\" ALTER COLUMN \"VisitPurpose\" TYPE integer USING 0;");

            migrationBuilder.AlterColumn<int>(
                name: "VisitPurpose",
                table: "Appointments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "Fee",
                table: "Appointments",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "QuickVisitFee",
                table: "AspNetUsers",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "NewVisitFee",
                table: "AspNetUsers",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "FollowUpFee",
                table: "AspNetUsers",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ConsultationFee",
                table: "AspNetUsers",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AverageRating",
                table: "AspNetUsers",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(3,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VisitType",
                table: "Appointments",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "VisitPurpose",
                table: "Appointments",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "Fee",
                table: "Appointments",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");
        }
    }
}
