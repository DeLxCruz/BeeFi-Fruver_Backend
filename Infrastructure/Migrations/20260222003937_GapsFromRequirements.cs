using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GapsFromRequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionAmount",
                table: "Orders",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "CommissionRuleId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommissionRuleName",
                table: "Orders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryMode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "AllowPreOrder",
                table: "FruverProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableFrom",
                table: "FruverProducts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableUntil",
                table: "FruverProducts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSeasonal",
                table: "FruverProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PreOrderAvailableDate",
                table: "FruverProducts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreparationTimeMinutes",
                table: "FruverProducts",
                type: "int",
                nullable: false,
                defaultValue: 30);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryMode",
                table: "Deliveries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "BeeFiLogistics");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryPin",
                table: "Deliveries",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryProofUrl",
                table: "Deliveries",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SellerDeliveryFee",
                table: "Deliveries",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SellerDeliveryPersonName",
                table: "Deliveries",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CommissionRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ZoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MinOrderAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxOrderAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CommissionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommissionValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MinCommission = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    MaxCommission = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommissionRules_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CommissionRules_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CommissionRules_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PriceReferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ZoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    P25 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    P50 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    P75 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitNorm = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SampleCount = table.Column<int>(type: "int", nullable: false),
                    ComputedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WindowDays = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceReferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FruverProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PriceAdjustment = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariants_FruverProducts_FruverProductId",
                        column: x => x.FruverProductId,
                        principalTable: "FruverProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReturnRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    EvidenceUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AdminNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefundType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReturnRequests_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesAggDaily",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ZoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    UnitsSold = table.Column<int>(type: "int", nullable: false),
                    Revenue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OrderCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesAggDaily", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BeeFiPlans",
                columns: new[] { "Id", "BonusPointsMultiplier", "CreatedAt", "Description", "DiscountPercentage", "FreeDeliveriesPerMonth", "HasEarlyAccess", "HasFreeDelivery", "HasPrioritySupport", "IsActive", "MonthlyPrice", "Name", "SpeedMbps" },
                values: new object[,]
                {
                    { new Guid("74017c8d-1b0a-449f-9b58-04325e5a4462"), 1, new DateTime(2026, 2, 22, 0, 39, 37, 150, DateTimeKind.Utc).AddTicks(3354), "Plan básico de internet con beneficios en BeeFi", 5m, 1, false, true, false, true, 50000m, "Básico", 50 },
                    { new Guid("a4a13f0d-5e58-4648-9d93-b43f924e8c1b"), 3, new DateTime(2026, 2, 22, 0, 39, 37, 150, DateTimeKind.Utc).AddTicks(3358), "Plan premium con todos los beneficios", 15m, 5, true, true, true, true, 120000m, "Premium", 200 },
                    { new Guid("cd10c437-a131-497f-95b8-b067b05fdc57"), 2, new DateTime(2026, 2, 22, 0, 39, 37, 150, DateTimeKind.Utc).AddTicks(3358), "Plan plus con más beneficios", 10m, 3, false, true, true, true, 80000m, "Plus", 100 }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "DisplayOrder", "IconUrl", "IsActive", "Name", "ParentCategoryId" },
                values: new object[,]
                {
                    { new Guid("67099ccd-7593-4cb3-9c80-672c8735d609"), new DateTime(2026, 2, 22, 0, 39, 37, 152, DateTimeKind.Utc).AddTicks(6121), "Frutas frescas", 1, "/icons/fruits.png", true, "Frutas", null },
                    { new Guid("681453cb-dae1-4fde-a25f-8203c2ca2615"), new DateTime(2026, 2, 22, 0, 39, 37, 152, DateTimeKind.Utc).AddTicks(6124), "Productos lácteos", 3, "/icons/dairy.png", true, "Lácteos", null },
                    { new Guid("be63aaa5-b9c7-46a9-83f5-2231a3e7bb1d"), new DateTime(2026, 2, 22, 0, 39, 37, 152, DateTimeKind.Utc).AddTicks(6125), "Carnes y embutidos", 4, "/icons/meat.png", true, "Carnes", null },
                    { new Guid("d10ec7f6-95e8-4d3d-88b7-00f344e7378f"), new DateTime(2026, 2, 22, 0, 39, 37, 152, DateTimeKind.Utc).AddTicks(6124), "Verduras frescas", 2, "/icons/vegetables.png", true, "Verduras", null }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("24af6f6c-1269-449b-9b63-fd6d1e49433a"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a45227e0-cb30-4924-9338-2ad0de80661c"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("e93bd156-71f2-4d7e-836b-224752e64a66"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[] { new Guid("a20b5b31-2317-4716-8b9e-d7fdcd642ea9"), new DateTime(2026, 2, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Gestión completa del sistema", "Administrador" });

            migrationBuilder.CreateIndex(
                name: "IX_CommissionRules_CategoryId",
                table: "CommissionRules",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionRules_IsActive_ValidFrom_ValidTo",
                table: "CommissionRules",
                columns: new[] { "IsActive", "ValidFrom", "ValidTo" });

            migrationBuilder.CreateIndex(
                name: "IX_CommissionRules_RoleId",
                table: "CommissionRules",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionRules_ZoneId",
                table: "CommissionRules",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceReferences_ProductKey",
                table: "PriceReferences",
                column: "ProductKey");

            migrationBuilder.CreateIndex(
                name: "IX_PriceReferences_ProductKey_ZoneId",
                table: "PriceReferences",
                columns: new[] { "ProductKey", "ZoneId" },
                unique: true,
                filter: "[ZoneId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_FruverProductId_IsActive",
                table: "ProductVariants",
                columns: new[] { "FruverProductId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_SKU",
                table: "ProductVariants",
                column: "SKU",
                unique: true,
                filter: "[SKU] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_OrderId",
                table: "ReturnRequests",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_UserId_Status",
                table: "ReturnRequests",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesAggDaily_ProductKey_Date_ZoneId",
                table: "SalesAggDaily",
                columns: new[] { "ProductKey", "Date", "ZoneId" },
                unique: true,
                filter: "[ZoneId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAggDaily_ProductKey_ZoneId",
                table: "SalesAggDaily",
                columns: new[] { "ProductKey", "ZoneId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommissionRules");

            migrationBuilder.DropTable(
                name: "PriceReferences");

            migrationBuilder.DropTable(
                name: "ProductVariants");

            migrationBuilder.DropTable(
                name: "ReturnRequests");

            migrationBuilder.DropTable(
                name: "SalesAggDaily");

            migrationBuilder.DeleteData(
                table: "BeeFiPlans",
                keyColumn: "Id",
                keyValue: new Guid("74017c8d-1b0a-449f-9b58-04325e5a4462"));

            migrationBuilder.DeleteData(
                table: "BeeFiPlans",
                keyColumn: "Id",
                keyValue: new Guid("a4a13f0d-5e58-4648-9d93-b43f924e8c1b"));

            migrationBuilder.DeleteData(
                table: "BeeFiPlans",
                keyColumn: "Id",
                keyValue: new Guid("cd10c437-a131-497f-95b8-b067b05fdc57"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("67099ccd-7593-4cb3-9c80-672c8735d609"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("681453cb-dae1-4fde-a25f-8203c2ca2615"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("be63aaa5-b9c7-46a9-83f5-2231a3e7bb1d"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("d10ec7f6-95e8-4d3d-88b7-00f344e7378f"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a20b5b31-2317-4716-8b9e-d7fdcd642ea9"));

            migrationBuilder.DropColumn(
                name: "CommissionAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CommissionRuleId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CommissionRuleName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryMode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AllowPreOrder",
                table: "FruverProducts");

            migrationBuilder.DropColumn(
                name: "AvailableFrom",
                table: "FruverProducts");

            migrationBuilder.DropColumn(
                name: "AvailableUntil",
                table: "FruverProducts");

            migrationBuilder.DropColumn(
                name: "IsSeasonal",
                table: "FruverProducts");

            migrationBuilder.DropColumn(
                name: "PreOrderAvailableDate",
                table: "FruverProducts");

            migrationBuilder.DropColumn(
                name: "PreparationTimeMinutes",
                table: "FruverProducts");

            migrationBuilder.DropColumn(
                name: "DeliveryMode",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryPin",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryProofUrl",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "SellerDeliveryFee",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "SellerDeliveryPersonName",
                table: "Deliveries");

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

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("24af6f6c-1269-449b-9b63-fd6d1e49433a"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 21, 20, 11, 27, 997, DateTimeKind.Utc).AddTicks(3371));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a45227e0-cb30-4924-9338-2ad0de80661c"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 21, 20, 11, 27, 997, DateTimeKind.Utc).AddTicks(3372));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("e93bd156-71f2-4d7e-836b-224752e64a66"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 21, 20, 11, 27, 997, DateTimeKind.Utc).AddTicks(3365));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[] { new Guid("22a05b31-2317-4716-8b9e-d7fdcd642ea9"), new DateTime(2026, 2, 21, 20, 11, 27, 997, DateTimeKind.Utc).AddTicks(3373), "Gestión completa del sistema", "Administrador" });
        }
    }
}
