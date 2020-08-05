using Microsoft.EntityFrameworkCore.Migrations;

namespace NoticeBoard.Data.Migrations
{
    public partial class ChangeIdName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Notifications_NotificationId",
                table: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            // migrationBuilder.DropColumn(
            //     name: "NotificationId",
            //     table: "Notifications");

            // migrationBuilder.DropColumn(
            //     name: "CommentId",
            //     table: "Comments");

            // migrationBuilder.AddColumn<int>(
            //     name: "Id",
            //     table: "Notifications",
            //     nullable: false,
            //     defaultValue: 0)
            //     .Annotation("SqlServer:Identity", "1, 1");

            // migrationBuilder.AddColumn<int>(
            //     name: "Id",
            //     table: "Comments",
            //     nullable: false,
            //     defaultValue: 0)
            //     .Annotation("SqlServer:Identity", "1, 1");
            migrationBuilder.RenameColumn(
                name:"NotificationId",
                table:"Notifications",
                newName:"Id"
            );
            migrationBuilder.RenameColumn(
                name:"CommentId",
                table:"Comments",
                newName:"Id"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Notifications_NotificationId",
                table: "Comments",
                column: "NotificationId",
                principalTable: "Notifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Notifications_NotificationId",
                table: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            // migrationBuilder.DropColumn(
            //     name: "Id",
            //     table: "Notifications");

            // migrationBuilder.DropColumn(
            //     name: "Id",
            //     table: "Comments");

            // migrationBuilder.AddColumn<int>(
            //     name: "NotificationId",
            //     table: "Notifications",
            //     type: "int",
            //     nullable: false,
            //     defaultValue: 0)
            //     .Annotation("SqlServer:Identity", "1, 1");

            // migrationBuilder.AddColumn<int>(
            //     name: "CommentId",
            //     table: "Comments",
            //     type: "int",
            //     nullable: false,
            //     defaultValue: 0)
            //     .Annotation("SqlServer:Identity", "1, 1");
            migrationBuilder.RenameColumn(
                name:"Id",
                table:"Notifications",
                newName:"NotificationId"
            );
            migrationBuilder.RenameColumn(
                name:"Id",
                table:"Comments",
                newName:"CommentId"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "NotificationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Notifications_NotificationId",
                table: "Comments",
                column: "NotificationId",
                principalTable: "Notifications",
                principalColumn: "NotificationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
