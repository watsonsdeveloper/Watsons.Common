﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watsons.Common.TrafficLogHelpers;
public partial class TrafficContext : DbContext
{
    public TrafficContext()
    {
    }

    public TrafficContext(DbContextOptions<TrafficContext> options) : base(options)
    {
        
    }

    // onConfiguration
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // connect with connection string
            //optionsBuilder.UseSqlServer("Data Source=10.98.32.248;Initial Catalog=TRV2_UAT;User ID=sa;Password=!QAZ2wsx#EDC;Trust Server Certificate=True"); // add connection string
        }
    }

    internal virtual DbSet<TrafficLog> TrafficLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrafficLog>(entity =>
        {

            entity.HasKey(e => e.RequestId);

            entity.ToTable("TrafficLog");

            //entity.Property(e => e.RequestId)
            //    .HasMaxLength(50)
            //    .IsUnicode(false);
            //entity.Property(e => e.SessionId)
            //    .HasMaxLength(50)
            //    .IsUnicode(false);
            //entity.Property(e => e.Action)
            //    .HasMaxLength(250)
            //    .IsUnicode(true);
            //entity.Property(e => e.Headers)
            //    .IsUnicode(true);
            //entity.Property(e => e.Request)
            //    .IsUnicode(true);
            //entity.Property(e => e.Response)
            //    .IsUnicode(true);
            //entity.Property(e => e.HttpStatus)
            //    .HasColumnType("int");
            //entity.Property(e => e.RequestDT)
            //.HasColumnType("datetime")
            //.HasDefaultValueSql("(getdate())");
            //entity.Property(e => e.ResponseDT).HasColumnType("datetime");
        });


        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
