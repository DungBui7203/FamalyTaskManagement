using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "families",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__families__3213E83F44C52995", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "labels",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    family_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__labels__3213E83F3B7B06A1", x => x.id);
                    table.ForeignKey(
                        name: "FK_labels_family",
                        column: x => x.family_id,
                        principalTable: "families",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    family_id = table.Column<long>(type: "bigint", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__users__3213E83FAACB7A61", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_families",
                        column: x => x.family_id,
                        principalTable: "families",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "activity_logs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    family_id = table.Column<long>(type: "bigint", nullable: false),
                    entity_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    entity_id = table.Column<long>(type: "bigint", nullable: true),
                    action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    actor_id = table.Column<long>(type: "bigint", nullable: false),
                    data_json = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__activity__3213E83FE0E95989", x => x.id);
                    table.ForeignKey(
                        name: "FK_activity_logs_actor",
                        column: x => x.actor_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_activity_logs_family",
                        column: x => x.family_id,
                        principalTable: "families",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    family_id = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    priority = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true, defaultValue: "Medium"),
                    due_date = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Pending"),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    verified_by = table.Column<long>(type: "bigint", nullable: true),
                    verified_at = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    is_archived = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    updated_at = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tasks__3213E83F512E2804", x => x.id);
                    table.ForeignKey(
                        name: "FK_tasks_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_tasks_families",
                        column: x => x.family_id,
                        principalTable: "families",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tasks_verified_by",
                        column: x => x.verified_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "task_assignments",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    task_id = table.Column<long>(type: "bigint", nullable: false),
                    assignee_id = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__task_ass__3213E83FA7F869F4", x => x.id);
                    table.ForeignKey(
                        name: "FK_task_assignments_task",
                        column: x => x.task_id,
                        principalTable: "tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_task_assignments_user",
                        column: x => x.assignee_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "task_attachments",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    task_id = table.Column<long>(type: "bigint", nullable: false),
                    file_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    url = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    uploaded_by = table.Column<long>(type: "bigint", nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__task_att__3213E83F3811C559", x => x.id);
                    table.ForeignKey(
                        name: "FK_task_attachments_task",
                        column: x => x.task_id,
                        principalTable: "tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_task_attachments_user",
                        column: x => x.uploaded_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "task_comments",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    task_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__task_com__3213E83F68F08BD5", x => x.id);
                    table.ForeignKey(
                        name: "FK_task_comments_task",
                        column: x => x.task_id,
                        principalTable: "tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_task_comments_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "task_labels",
                columns: table => new
                {
                    task_id = table.Column<long>(type: "bigint", nullable: false),
                    label_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_labels", x => new { x.task_id, x.label_id });
                    table.ForeignKey(
                        name: "FK_task_labels_label",
                        column: x => x.label_id,
                        principalTable: "labels",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_task_labels_task",
                        column: x => x.task_id,
                        principalTable: "tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "time_entries",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    task_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    minutes = table.Column<int>(type: "int", nullable: true),
                    note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    logged_at = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__time_ent__3213E83F94B2FE50", x => x.id);
                    table.ForeignKey(
                        name: "FK_time_entries_task",
                        column: x => x.task_id,
                        principalTable: "tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_time_entries_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_activity_logs_actor_id",
                table: "activity_logs",
                column: "actor_id");

            migrationBuilder.CreateIndex(
                name: "IX_activity_logs_family_created",
                table: "activity_logs",
                columns: new[] { "family_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "UQ_labels_family_name",
                table: "labels",
                columns: new[] { "family_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_task_assignments_assignee_task",
                table: "task_assignments",
                columns: new[] { "assignee_id", "task_id" });

            migrationBuilder.CreateIndex(
                name: "UQ_task_assignments",
                table: "task_assignments",
                columns: new[] { "task_id", "assignee_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_task_attachments_task_uploaded",
                table: "task_attachments",
                columns: new[] { "task_id", "uploaded_at" });

            migrationBuilder.CreateIndex(
                name: "IX_task_attachments_uploaded_by",
                table: "task_attachments",
                column: "uploaded_by");

            migrationBuilder.CreateIndex(
                name: "IX_task_comments_task_created",
                table: "task_comments",
                columns: new[] { "task_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_task_comments_user_id",
                table: "task_comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_task_labels_label_id",
                table: "task_labels",
                column: "label_id");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_created_by",
                table: "tasks",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_family_status_due",
                table: "tasks",
                columns: new[] { "family_id", "status", "due_date" });

            migrationBuilder.CreateIndex(
                name: "IX_tasks_verified_by",
                table: "tasks",
                column: "verified_by");

            migrationBuilder.CreateIndex(
                name: "IX_time_entries_task_user",
                table: "time_entries",
                columns: new[] { "task_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "IX_time_entries_user_id",
                table: "time_entries",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_family_id",
                table: "users",
                column: "family_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activity_logs");

            migrationBuilder.DropTable(
                name: "task_assignments");

            migrationBuilder.DropTable(
                name: "task_attachments");

            migrationBuilder.DropTable(
                name: "task_comments");

            migrationBuilder.DropTable(
                name: "task_labels");

            migrationBuilder.DropTable(
                name: "time_entries");

            migrationBuilder.DropTable(
                name: "labels");

            migrationBuilder.DropTable(
                name: "tasks");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "families");
        }
    }
}
