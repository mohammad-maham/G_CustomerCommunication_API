using Microsoft.EntityFrameworkCore;

namespace G_CustomerCommunication_API.Models;

public partial class GCustomerCommunicationDbContext : DbContext
{
    private readonly IConfiguration _config;

    public GCustomerCommunicationDbContext()
    {
        _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
    }

    public GCustomerCommunicationDbContext(DbContextOptions<GCustomerCommunicationDbContext> options)
        : base(options)
    {
        _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
    }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationLink> NotificationLinks { get; set; }

    public virtual DbSet<NotificationType> NotificationTypes { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Survey> Surveys { get; set; }

    public virtual DbSet<SurveyTemplate> SurveyTemplates { get; set; }

    public virtual DbSet<SurveyTemplateQuestion> SurveyTemplateQuestions { get; set; }

    public virtual DbSet<ValueType> ValueTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_config.GetConnectionString("GCustomerCommunicationDbContext"), x => x.UseNodaTime());

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Notification_pkey");

            entity.ToTable("Notification");

            entity.Property(e => e.Id)
              .UseIdentityAlwaysColumn()
              .HasIdentityOptions(null, null, 100000000L, 1000000000000000000L, null, 30L);
            entity.Property(e => e.DestinationAddress).HasMaxLength(100);
            entity.Property(e => e.SenderUnit).HasColumnType("character varying");
            entity.Property(e => e.NotificationResult).HasColumnType("json");
        });

        modelBuilder.Entity<NotificationLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("NotificationLink_pkey");

            entity.ToTable("NotificationLink");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Config).HasColumnType("json");
            entity.Property(e => e.Name).HasColumnType("character varying");
        });

        modelBuilder.Entity<NotificationType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("NotificationType_pkey");

            entity.ToTable("NotificationType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasColumnType("character varying");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Status_pkey");

            entity.ToTable("Status");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Caption).HasColumnType("character varying");
            entity.Property(e => e.Name).HasColumnType("character varying");
        });

        modelBuilder.Entity<Survey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Survey_pkey");

            entity.ToTable("Survey");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.QuestionValues).HasColumnType("json");
        });

        modelBuilder.Entity<SurveyTemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SurveyTemplate_pkey");

            entity.ToTable("SurveyTemplate");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasColumnType("character varying");
        });

        modelBuilder.Entity<SurveyTemplateQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SurveyTemplateQuestion_pkey");

            entity.ToTable("SurveyTemplateQuestion");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Question).HasColumnType("character varying");
            entity.Property(e => e.Values).HasColumnType("character varying[]");
        });

        modelBuilder.Entity<ValueType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ValueType_pkey");

            entity.ToTable("ValueType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Caption).HasColumnType("character varying");
            entity.Property(e => e.Name).HasColumnType("character varying");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
