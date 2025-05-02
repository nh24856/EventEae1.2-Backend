using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventEae1._2_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddEventandtickettype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "Role" },
                keyValues: new object[] { 1, "Admin" });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "Role" },
                keyValues: new object[] { 2, "Admin" });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "Role" },
                keyValues: new object[] { 3, "Admin" });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "Role" },
                keyValues: new object[] { 9, "Admin" });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "Role" },
                keyValues: new object[] { 7, "client" });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "Role" },
                keyValues: new object[] { 8, "client" });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "Role" },
                keyValues: new object[] { 9, "client" });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "Role" },
                keyValues: new object[] { 4, "Manager" });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "Role" },
                keyValues: new object[] { 5, "Manager" });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "Role" },
                keyValues: new object[] { 6, "Manager" });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "Role" },
                keyValues: new object[] { 9, "Manager" });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "CanApproveManagers" },
                    { 2, "CanManageUsers" },
                    { 3, "CanManageSettings" },
                    { 4, "CanCreateEvents" },
                    { 5, "CanViewOwnEvents" },
                    { 6, "CanViewTicketSales" },
                    { 7, "CanBrowseEvents" },
                    { 8, "CanViewOwnTickets" },
                    { 9, "CanManageProfile" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "Role", "Id" },
                values: new object[,]
                {
                    { 1, "Admin", 0 },
                    { 2, "Admin", 0 },
                    { 3, "Admin", 0 },
                    { 9, "Admin", 0 },
                    { 7, "client", 0 },
                    { 8, "client", 0 },
                    { 9, "client", 0 },
                    { 4, "Manager", 0 },
                    { 5, "Manager", 0 },
                    { 6, "Manager", 0 },
                    { 9, "Manager", 0 }
                });
        }
    }
}
