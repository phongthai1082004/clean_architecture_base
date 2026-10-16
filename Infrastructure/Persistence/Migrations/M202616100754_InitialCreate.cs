using FluentMigrator;

namespace Infrastructure.Persistence.Migrations
{
    [Migration(202616100754)]
    public class M202616100754_InitialCreate : Migration
    {
        public override void Up()
        {
            Create.Table("users")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Email").AsString(256).NotNullable().Unique()
                .WithColumn("FullName").AsString(150).NotNullable()
                .WithColumn("PasswordHash").AsString(255).Nullable()
                .WithColumn("PhoneNumber").AsString(50).Nullable()
                .WithColumn("AvatarUrl").AsString(500).NotNullable().WithDefaultValue("")
                .WithColumn("Status").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("CreatedAt").AsDateTimeOffset().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("UpdatedAt").AsDateTimeOffset().Nullable();

            Create.Table("external_tokens")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("UserId").AsGuid().NotNullable()
                    .ForeignKey("fk_external_tokens_users_user_id", "users", "Id")
                .WithColumn("Provider").AsString(50).NotNullable()
                .WithColumn("ProviderUserId").AsString(100).NotNullable()
                .WithColumn("AccessToken").AsCustom("TEXT").NotNullable()
                .WithColumn("RefreshToken").AsCustom("TEXT").Nullable()
                .WithColumn("ExpiresAt").AsDateTimeOffset().Nullable();

            Create.Table("audit_logs")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("UserId").AsGuid().Nullable()
                    .ForeignKey("fk_audit_logs_users_user_id", "users", "Id")
                .WithColumn("Action").AsString(100).NotNullable()
                .WithColumn("TableName").AsString(100).NotNullable()
                .WithColumn("RecordId").AsString(100).NotNullable()
                .WithColumn("Message").AsCustom("TEXT").NotNullable()
                .WithColumn("OldValues").AsCustom("TEXT").Nullable()
                .WithColumn("NewValues").AsCustom("TEXT").Nullable();

            Create.Table("notifications")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("UserId").AsGuid().NotNullable()
                    .ForeignKey("fk_notifications_users_user_id", "users", "Id")
                .WithColumn("Title").AsString(200).NotNullable()
                .WithColumn("Message").AsCustom("TEXT").NotNullable()
                .WithColumn("Type").AsString(50).NotNullable()
                .WithColumn("IsRead").AsBoolean().NotNullable().WithDefaultValue(false);

            Create.Table("file_metadata")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("FileName").AsString(255).NotNullable()
                .WithColumn("FilePath").AsString(500).NotNullable()
                .WithColumn("ContentType").AsString(100).NotNullable()
                .WithColumn("FileSize").AsInt64().NotNullable();

            Create.Table("inbox_messages")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("ProcessedAt").AsDateTimeOffset().NotNullable()
                .WithColumn("Error").AsCustom("TEXT").Nullable();

            Create.Table("outbox_messages")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("EventType").AsString(255).NotNullable()
                .WithColumn("Payload").AsCustom("TEXT").NotNullable()
                .WithColumn("OccurredOn").AsDateTimeOffset().NotNullable()
                .WithColumn("ProcessedAt").AsDateTimeOffset().Nullable()
                .WithColumn("Error").AsCustom("TEXT").Nullable();
        }

        public override void Down()
        {
            Delete.Table("outbox_messages");
            Delete.Table("inbox_messages");
            Delete.Table("file_metadata");
            Delete.Table("notifications");
            Delete.Table("audit_logs");
            Delete.Table("external_tokens");
            Delete.Table("users");
        }
    }
}