using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Processor.Migrations
{
    /// <inheritdoc />
    public partial class RenameFormFactoryToFormFactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FormFactory",
                table: "VehicleTypes",
                newName: "FormFactor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FormFactor",
                table: "VehicleTypes",
                newName: "FormFactory");
        }
    }
}
