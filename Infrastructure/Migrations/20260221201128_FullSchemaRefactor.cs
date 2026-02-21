using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FullSchemaRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FruverZones_Users_FruverId",
                table: "FruverZones");

            migrationBuilder.DropForeignKey(
                name: "FK_FruverZones_Zones_ZoneId",
                table: "FruverZones");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_FruverId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_FruverId_Status",
                table: "Orders");

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
                name: "FruverId",
                table: "Orders");

            migrationBuilder.AddColumn<decimal>(
                name: "RefundAmount",
                table: "Payments",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundReason",
                table: "Payments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefundedAt",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FruverId",
                table: "OrderItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "DeliveryStatusHistory",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,7)",
                oldPrecision: 10,
                oldScale: 7,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "DeliveryStatusHistory",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,7)",
                oldPrecision: 10,
                oldScale: 7,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "Addresses",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,7)",
                oldPrecision: 10,
                oldScale: 7,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "Addresses",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,7)",
                oldPrecision: 10,
                oldScale: 7,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Addresses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Addresses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LinkUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    StartsAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndsAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FruverProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_FruverProducts_FruverProductId",
                        column: x => x.FruverProductId,
                        principalTable: "FruverProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryPersonZones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliveryPersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ZoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryPersonZones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryPersonZones_Users_DeliveryPersonId",
                        column: x => x.DeliveryPersonId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryPersonZones_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FruverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.CheckConstraint("CK_Reviews_Rating", "[Rating] >= 1 AND [Rating] <= 5");
                    table.ForeignKey(
                        name: "FK_Reviews_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_FruverId",
                        column: x => x.FruverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "BeeFiPlans",
                columns: new[] { "Id", "BonusPointsMultiplier", "CreatedAt", "Description", "DiscountPercentage", "FreeDeliveriesPerMonth", "HasEarlyAccess", "HasFreeDelivery", "HasPrioritySupport", "IsActive", "MonthlyPrice", "Name", "SpeedMbps" },
                values: new object[,]
                {
                    { new Guid("3ddfad6c-fa4f-49ed-a33f-4a294d555c03"), 1, new DateTime(2026, 2, 21, 20, 11, 27, 979, DateTimeKind.Utc).AddTicks(4652), "Plan básico de internet con beneficios en BeeFi", 5m, 1, false, true, false, true, 50000m, "Básico", 50 },
                    { new Guid("b58b466d-64af-488f-b74c-90ddbd4825ad"), 3, new DateTime(2026, 2, 21, 20, 11, 27, 979, DateTimeKind.Utc).AddTicks(4656), "Plan premium con todos los beneficios", 15m, 5, true, true, true, true, 120000m, "Premium", 200 },
                    { new Guid("ddccdbc7-ba86-4789-bd38-3f5fc33090d5"), 2, new DateTime(2026, 2, 21, 20, 11, 27, 979, DateTimeKind.Utc).AddTicks(4655), "Plan plus con más beneficios", 10m, 3, false, true, true, true, 80000m, "Plus", 100 }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "DisplayOrder", "IconUrl", "IsActive", "Name", "ParentCategoryId" },
                values: new object[,]
                {
                    { new Guid("8d0d4260-3c22-4a82-9f3b-784cfa790654"), new DateTime(2026, 2, 21, 20, 11, 27, 981, DateTimeKind.Utc).AddTicks(5009), "Productos lácteos", 3, "/icons/dairy.png", true, "Lácteos", null },
                    { new Guid("d4f7e198-5a6a-4ae4-a6a9-fa3a3b33acf5"), new DateTime(2026, 2, 21, 20, 11, 27, 981, DateTimeKind.Utc).AddTicks(5005), "Frutas frescas", 1, "/icons/fruits.png", true, "Frutas", null },
                    { new Guid("f25d815f-2594-4e30-8a2f-6176fdc2c994"), new DateTime(2026, 2, 21, 20, 11, 27, 981, DateTimeKind.Utc).AddTicks(5010), "Carnes y embutidos", 4, "/icons/meat.png", true, "Carnes", null },
                    { new Guid("fe31a0fe-2b4a-4157-a209-1a193f64eb52"), new DateTime(2026, 2, 21, 20, 11, 27, 981, DateTimeKind.Utc).AddTicks(5008), "Verduras frescas", 2, "/icons/vegetables.png", true, "Verduras", null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("22a05b31-2317-4716-8b9e-d7fdcd642ea9"), new DateTime(2026, 2, 21, 20, 11, 27, 997, DateTimeKind.Utc).AddTicks(3373), "Gestión completa del sistema", "Administrador" },
                    { new Guid("24af6f6c-1269-449b-9b63-fd6d1e49433a"), new DateTime(2026, 2, 21, 20, 11, 27, 997, DateTimeKind.Utc).AddTicks(3371), "Vendedor que publica y gestiona productos", "FruverAliado" },
                    { new Guid("a45227e0-cb30-4924-9338-2ad0de80661c"), new DateTime(2026, 2, 21, 20, 11, 27, 997, DateTimeKind.Utc).AddTicks(3372), "Personal de entregas y logística", "Empleado" },
                    { new Guid("e93bd156-71f2-4d7e-836b-224752e64a66"), new DateTime(2026, 2, 21, 20, 11, 27, 997, DateTimeKind.Utc).AddTicks(3365), "Usuario final que compra productos", "Cliente" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_FruverId",
                table: "OrderItems",
                column: "FruverId");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_IsActive_DisplayOrder",
                table: "Banners",
                columns: new[] { "IsActive", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_FruverProductId",
                table: "CartItems",
                column: "FruverProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId",
                table: "CartItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId_FruverProductId",
                table: "CartItems",
                columns: new[] { "UserId", "FruverProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPersonZones_DeliveryPersonId",
                table: "DeliveryPersonZones",
                column: "DeliveryPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPersonZones_DeliveryPersonId_ZoneId",
                table: "DeliveryPersonZones",
                columns: new[] { "DeliveryPersonId", "ZoneId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPersonZones_ZoneId",
                table: "DeliveryPersonZones",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_FruverId",
                table: "Reviews",
                column: "FruverId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_OrderId",
                table: "Reviews",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId_OrderId",
                table: "Reviews",
                columns: new[] { "UserId", "OrderId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FruverZones_Users_FruverId",
                table: "FruverZones",
                column: "FruverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FruverZones_Zones_ZoneId",
                table: "FruverZones",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Users_FruverId",
                table: "OrderItems",
                column: "FruverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FruverZones_Users_FruverId",
                table: "FruverZones");

            migrationBuilder.DropForeignKey(
                name: "FK_FruverZones_Zones_ZoneId",
                table: "FruverZones");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Users_FruverId",
                table: "OrderItems");

            migrationBuilder.DropTable(
                name: "Banners");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "DeliveryPersonZones");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_FruverId",
                table: "OrderItems");

            migrationBuilder.DeleteData(
                table: "BeeFiPlans",
                keyColumn: "Id",
                keyValue: new Guid("3ddfad6c-fa4f-49ed-a33f-4a294d555c03"));

            migrationBuilder.DeleteData(
                table: "BeeFiPlans",
                keyColumn: "Id",
                keyValue: new Guid("b58b466d-64af-488f-b74c-90ddbd4825ad"));

            migrationBuilder.DeleteData(
                table: "BeeFiPlans",
                keyColumn: "Id",
                keyValue: new Guid("ddccdbc7-ba86-4789-bd38-3f5fc33090d5"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("8d0d4260-3c22-4a82-9f3b-784cfa790654"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("d4f7e198-5a6a-4ae4-a6a9-fa3a3b33acf5"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("f25d815f-2594-4e30-8a2f-6176fdc2c994"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("fe31a0fe-2b4a-4157-a209-1a193f64eb52"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("22a05b31-2317-4716-8b9e-d7fdcd642ea9"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("24af6f6c-1269-449b-9b63-fd6d1e49433a"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a45227e0-cb30-4924-9338-2ad0de80661c"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("e93bd156-71f2-4d7e-836b-224752e64a66"));

            migrationBuilder.DropColumn(
                name: "RefundAmount",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "RefundReason",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "RefundedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "FruverId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Addresses");

            migrationBuilder.AddColumn<Guid>(
                name: "FruverId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "DeliveryStatusHistory",
                type: "decimal(10,7)",
                precision: 10,
                scale: 7,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "DeliveryStatusHistory",
                type: "decimal(10,7)",
                precision: 10,
                scale: 7,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Addresses",
                type: "decimal(10,7)",
                precision: 10,
                scale: 7,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Addresses",
                type: "decimal(10,7)",
                precision: 10,
                scale: 7,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

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
                name: "IX_Orders_FruverId_Status",
                table: "Orders",
                columns: new[] { "FruverId", "Status" });

            migrationBuilder.AddForeignKey(
                name: "FK_FruverZones_Users_FruverId",
                table: "FruverZones",
                column: "FruverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FruverZones_Zones_ZoneId",
                table: "FruverZones",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_FruverId",
                table: "Orders",
                column: "FruverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
