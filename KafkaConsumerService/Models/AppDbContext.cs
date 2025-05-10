using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace KafkaConsumerService.Models;
// Represents the application's database context, managing access to Kafka queue and DISTLIST entities.
public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AP_KAFKA_QUEUE> AP_KAFKA_QUEUE { get; set; }

    public virtual DbSet<DISTLIST> DISTLIST { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AP_KAFKA_QUEUE>(entity =>
        {
            entity.HasKey(e => e.QUEUE_ID)
                //.HasName("PK__AP_KAFKA__B3E8C3C2F3FE6136")
                .HasFillFactor(80);

            entity.Property(e => e.QUEUE_ID)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();

            entity.Property(e => e.ACTION_NAME)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EVENT_TIME)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.KEY_VALUE)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProcessingStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TABLE_NAME)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DISTLIST>(entity =>
        {
            entity.HasKey(e => e.DISTLISTID)
                .IsClustered(false)
                .HasFillFactor(80);

            entity.HasIndex(e => e.NAME, "IDX_NAME").HasFillFactor(80);

            entity.Property(e => e.DISTLISTID)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.AKTIV)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.DATE_EDIT).HasColumnType("datetime");
            entity.Property(e => e.DATE_NEW).HasColumnType("datetime");
            entity.Property(e => e.DESCRIPTION).HasMaxLength(250);
            entity.Property(e => e.IS_DYNAMIC)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.NAME).HasMaxLength(100);
            entity.Property(e => e.SELECTION_ID)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.USER_EDIT)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.USER_NEW)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
