using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreacionInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categoria",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    url_icono = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categoria", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    fecha_registro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "auditoria_log",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    entidad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    entidad_id = table.Column<int>(type: "int", nullable: false),
                    accion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    usuario_id = table.Column<int>(type: "int", nullable: true),
                    detalle_json = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auditoria_log", x => x.id);
                    table.ForeignKey(
                        name: "FK_auditoria_log_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "billetera",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    usuario_id = table.Column<int>(type: "int", nullable: false),
                    saldo_total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    saldo_retenido = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    saldo_disponible = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_billetera", x => x.id);
                    table.ForeignKey(
                        name: "FK_billetera_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subasta",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendedorId = table.Column<int>(type: "int", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    url_imagen = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    precio_base = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    incremento_minimo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_fin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "Estado actual de la subasta (PROGRAMADA, ACTIVA, FINALIZADA, DESIERTA)"),
                    version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subasta", x => x.id);
                    table.ForeignKey(
                        name: "FK_subasta_categoria_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "categoria",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_subasta_usuario_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "puja",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subasta_id = table.Column<int>(type: "int", nullable: false),
                    comprador_id = table.Column<int>(type: "int", nullable: false),
                    monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    fecha_puja = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_puja", x => x.id);
                    table.ForeignKey(
                        name: "FK_puja_subasta_subasta_id",
                        column: x => x.subasta_id,
                        principalTable: "subasta",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_puja_usuario_comprador_id",
                        column: x => x.comprador_id,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "transaccion_ledger",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    billetera_id = table.Column<int>(type: "int", nullable: false),
                    tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    subasta_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaccion_ledger", x => x.id);
                    table.ForeignKey(
                        name: "FK_transaccion_ledger_billetera_billetera_id",
                        column: x => x.billetera_id,
                        principalTable: "billetera",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_transaccion_ledger_subasta_subasta_id",
                        column: x => x.subasta_id,
                        principalTable: "subasta",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_log_entidad_entidad_id",
                table: "auditoria_log",
                columns: new[] { "entidad", "entidad_id" });

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_log_fecha",
                table: "auditoria_log",
                column: "fecha");

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_log_usuario_id",
                table: "auditoria_log",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_billetera_usuario_id",
                table: "billetera",
                column: "usuario_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_puja_comprador_id",
                table: "puja",
                column: "comprador_id");

            migrationBuilder.CreateIndex(
                name: "IX_puja_subasta_id",
                table: "puja",
                column: "subasta_id");

            migrationBuilder.CreateIndex(
                name: "IX_puja_subasta_id_fecha_puja",
                table: "puja",
                columns: new[] { "subasta_id", "fecha_puja" });

            migrationBuilder.CreateIndex(
                name: "IX_subasta_CategoriaId",
                table: "subasta",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_subasta_Estado",
                table: "subasta",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_subasta_fecha_fin",
                table: "subasta",
                column: "fecha_fin");

            migrationBuilder.CreateIndex(
                name: "IX_subasta_fecha_inicio",
                table: "subasta",
                column: "fecha_inicio");

            migrationBuilder.CreateIndex(
                name: "IX_subasta_VendedorId",
                table: "subasta",
                column: "VendedorId");

            migrationBuilder.CreateIndex(
                name: "IX_transaccion_ledger_billetera_id",
                table: "transaccion_ledger",
                column: "billetera_id");

            migrationBuilder.CreateIndex(
                name: "IX_transaccion_ledger_fecha",
                table: "transaccion_ledger",
                column: "fecha");

            migrationBuilder.CreateIndex(
                name: "IX_transaccion_ledger_subasta_id",
                table: "transaccion_ledger",
                column: "subasta_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_email",
                table: "usuario",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auditoria_log");

            migrationBuilder.DropTable(
                name: "puja");

            migrationBuilder.DropTable(
                name: "transaccion_ledger");

            migrationBuilder.DropTable(
                name: "billetera");

            migrationBuilder.DropTable(
                name: "subasta");

            migrationBuilder.DropTable(
                name: "categoria");

            migrationBuilder.DropTable(
                name: "usuario");
        }
    }
}
