using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountStatusToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BeeFiPlans",
                keyColumn: "Id",
                keyValue: new Guid("063420df-97b2-447e-b9a9-dc3e621d93e8"));

            migrationBuilder.DeleteData(
                table: "BeeFiPlans",
                keyColumn: "Id",
                keyValue: new Guid("563c6a31-0744-477b-80cc-3315f2b6261b"));

            migrationBuilder.DeleteData(
                table: "BeeFiPlans",
                keyColumn: "Id",
                keyValue: new Guid("999af319-e324-4f35-84cb-731199427dbe"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("480b3147-e6b0-4f41-9102-cd2f9eb7fbf0"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("74ebfa5f-96dc-4c43-bf61-6cfc032030e2"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("c9fd7fbf-29bd-4e56-a0a2-bf27de139186"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("fabe19e3-ecf9-4312-a3cc-938ef673d012"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("0d59eb98-8f1a-4ae0-ba95-3f2db1f8e91f"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("13695b0c-1734-4824-b07e-5ff0ddf042a8"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("1ee47f48-bc78-42fa-ae89-f722fedac391"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2dd8b391-e495-449f-9d5b-c6e00ec7f7d6"));

            migrationBuilder.AddColumn<int>(
                name: "AccountStatus",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedBy",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.InsertData(
                table: "BeeFiPlans",
                columns: new[] { "Id", "BonusPointsMultiplier", "CreatedAt", "Description", "DiscountPercentage", "FreeDeliveriesPerMonth", "HasEarlyAccess", "HasFreeDelivery", "HasPrioritySupport", "IsActive", "MonthlyPrice", "Name", "SpeedMbps" },
                values: new object[,]
                {
                    { new Guid("5496fff2-f866-49f1-9dbb-969f49b01999"), 3, new DateTime(2025, 11, 3, 14, 9, 46, 940, DateTimeKind.Utc).AddTicks(2521), "Plan premium con todos los beneficios", 15m, 5, true, true, true, true, 120000m, "Premium", 200 },
                    { new Guid("7ce37961-f9e0-48f8-bcb7-62b557771f5c"), 1, new DateTime(2025, 11, 3, 14, 9, 46, 940, DateTimeKind.Utc).AddTicks(2517), "Plan básico de internet con beneficios en BeeFi", 5m, 1, false, true, false, true, 50000m, "Básico", 50 },
                    { new Guid("e1bcaeae-7364-4651-a0df-a7fd114528af"), 2, new DateTime(2025, 11, 3, 14, 9, 46, 940, DateTimeKind.Utc).AddTicks(2520), "Plan plus con más beneficios", 10m, 3, false, true, true, true, 80000m, "Plus", 100 }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "DisplayOrder", "IconUrl", "IsActive", "Name", "ParentCategoryId" },
                values: new object[,]
                {
                    { new Guid("0aacce53-73d5-43aa-b001-21068c5bd59b"), new DateTime(2025, 11, 3, 14, 9, 46, 941, DateTimeKind.Utc).AddTicks(2151), "Productos lácteos", 3, "/icons/dairy.png", true, "Lácteos", null },
                    { new Guid("1a618b32-a98d-4ee4-9f90-d7771eb7a494"), new DateTime(2025, 11, 3, 14, 9, 46, 941, DateTimeKind.Utc).AddTicks(2148), "Frutas frescas", 1, "/icons/fruits.png", true, "Frutas", null },
                    { new Guid("8aba14fb-9a6d-41e5-b8ed-251fbf3da196"), new DateTime(2025, 11, 3, 14, 9, 46, 941, DateTimeKind.Utc).AddTicks(2152), "Carnes y embutidos", 4, "/icons/meat.png", true, "Carnes", null },
                    { new Guid("9134d0f7-22bd-45e3-aff7-ec780ce33343"), new DateTime(2025, 11, 3, 14, 9, 46, 941, DateTimeKind.Utc).AddTicks(2151), "Verduras frescas", 2, "/icons/vegetables.png", true, "Verduras", null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("0eb3c4c1-f67e-4feb-a765-a6539452c71a"), new DateTime(2025, 11, 3, 14, 9, 46, 954, DateTimeKind.Utc).AddTicks(1190), "Personal de entregas y logística", "Empleado" },
                    { new Guid("1f1a9117-76f6-4041-b452-3f14c74bf6f3"), new DateTime(2025, 11, 3, 14, 9, 46, 954, DateTimeKind.Utc).AddTicks(1175), "Usuario final que compra productos", "Cliente" },
                    { new Guid("478de043-fc0a-4481-ace6-d388fc840ce8"), new DateTime(2025, 11, 3, 14, 9, 46, 954, DateTimeKind.Utc).AddTicks(1178), "Vendedor que publica y gestiona productos", "FruverAliado" },
                    { new Guid("84a29846-7568-48d5-bc7b-ef852b8075a9"), new DateTime(2025, 11, 3, 14, 9, 46, 954, DateTimeKind.Utc).AddTicks(1191), "Gestión completa del sistema", "Administrador" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_AccountStatus",
                table: "Users",
                column: "AccountStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_AccountStatus",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "BeeFiPlans",
                keyColumn: "Id",
                keyValue: new Guid("5496fff2-f866-49f1-9dbb-969f49b01999"));

            migrationBuilder.DeleteData(
                table: "BeeFiPlans",
                keyColumn: "Id",
                keyValue: new Guid("7ce37961-f9e0-48f8-bcb7-62b557771f5c"));

            migrationBuilder.DeleteData(
                table: "BeeFiPlans",
                keyColumn: "Id",
                keyValue: new Guid("e1bcaeae-7364-4651-a0df-a7fd114528af"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("0aacce53-73d5-43aa-b001-21068c5bd59b"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("1a618b32-a98d-4ee4-9f90-d7771eb7a494"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("8aba14fb-9a6d-41e5-b8ed-251fbf3da196"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("9134d0f7-22bd-45e3-aff7-ec780ce33343"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("0eb3c4c1-f67e-4feb-a765-a6539452c71a"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("1f1a9117-76f6-4041-b452-3f14c74bf6f3"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("478de043-fc0a-4481-ace6-d388fc840ce8"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("84a29846-7568-48d5-bc7b-ef852b8075a9"));

            migrationBuilder.DropColumn(
                name: "AccountStatus",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Users");

            migrationBuilder.InsertData(
                table: "BeeFiPlans",
                columns: new[] { "Id", "BonusPointsMultiplier", "CreatedAt", "Description", "DiscountPercentage", "FreeDeliveriesPerMonth", "HasEarlyAccess", "HasFreeDelivery", "HasPrioritySupport", "IsActive", "MonthlyPrice", "Name", "SpeedMbps" },
                values: new object[,]
                {
                    { new Guid("063420df-97b2-447e-b9a9-dc3e621d93e8"), 1, new DateTime(2025, 10, 26, 20, 55, 21, 732, DateTimeKind.Utc).AddTicks(6879), "Plan básico de internet con beneficios en BeeFi", 5m, 1, false, true, false, true, 50000m, "Básico", 50 },
                    { new Guid("563c6a31-0744-477b-80cc-3315f2b6261b"), 3, new DateTime(2025, 10, 26, 20, 55, 21, 732, DateTimeKind.Utc).AddTicks(6882), "Plan premium con todos los beneficios", 15m, 5, true, true, true, true, 120000m, "Premium", 200 },
                    { new Guid("999af319-e324-4f35-84cb-731199427dbe"), 2, new DateTime(2025, 10, 26, 20, 55, 21, 732, DateTimeKind.Utc).AddTicks(6882), "Plan plus con más beneficios", 10m, 3, false, true, true, true, 80000m, "Plus", 100 }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "DisplayOrder", "IconUrl", "IsActive", "Name", "ParentCategoryId" },
                values: new object[,]
                {
                    { new Guid("480b3147-e6b0-4f41-9102-cd2f9eb7fbf0"), new DateTime(2025, 10, 26, 20, 55, 21, 733, DateTimeKind.Utc).AddTicks(6704), "Frutas frescas", 1, "/icons/fruits.png", true, "Frutas", null },
                    { new Guid("74ebfa5f-96dc-4c43-bf61-6cfc032030e2"), new DateTime(2025, 10, 26, 20, 55, 21, 733, DateTimeKind.Utc).AddTicks(6709), "Carnes y embutidos", 4, "/icons/meat.png", true, "Carnes", null },
                    { new Guid("c9fd7fbf-29bd-4e56-a0a2-bf27de139186"), new DateTime(2025, 10, 26, 20, 55, 21, 733, DateTimeKind.Utc).AddTicks(6707), "Productos lácteos", 3, "/icons/dairy.png", true, "Lácteos", null },
                    { new Guid("fabe19e3-ecf9-4312-a3cc-938ef673d012"), new DateTime(2025, 10, 26, 20, 55, 21, 733, DateTimeKind.Utc).AddTicks(6707), "Verduras frescas", 2, "/icons/vegetables.png", true, "Verduras", null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("0d59eb98-8f1a-4ae0-ba95-3f2db1f8e91f"), new DateTime(2025, 10, 26, 20, 55, 21, 748, DateTimeKind.Utc).AddTicks(8089), "Usuario final que compra productos", "Cliente" },
                    { new Guid("13695b0c-1734-4824-b07e-5ff0ddf042a8"), new DateTime(2025, 10, 26, 20, 55, 21, 748, DateTimeKind.Utc).AddTicks(8094), "Gestión completa del sistema", "Administrador" },
                    { new Guid("1ee47f48-bc78-42fa-ae89-f722fedac391"), new DateTime(2025, 10, 26, 20, 55, 21, 748, DateTimeKind.Utc).AddTicks(8093), "Personal de entregas y logística", "Empleado" },
                    { new Guid("2dd8b391-e495-449f-9d5b-c6e00ec7f7d6"), new DateTime(2025, 10, 26, 20, 55, 21, 748, DateTimeKind.Utc).AddTicks(8092), "Vendedor que publica y gestiona productos", "FruverAliado" }
                });
        }
    }
}
