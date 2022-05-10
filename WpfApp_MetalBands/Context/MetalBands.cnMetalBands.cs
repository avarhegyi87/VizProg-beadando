﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Entity Developer tool using EF Core template.
// Code is generated on: 2022. 05. 10. 20:40:22
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

namespace enMetalBands
{

    public partial class cnMetalBands : DbContext
    {

        public cnMetalBands() :
            base()
        {
            OnCreated();
        }

        public cnMetalBands(DbContextOptions<cnMetalBands> options) :
            base(options)
        {
            OnCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured ||
                (!optionsBuilder.Options.Extensions.OfType<RelationalOptionsExtension>().Any(ext => !string.IsNullOrEmpty(ext.ConnectionString) || ext.Connection != null) &&
                 !optionsBuilder.Options.Extensions.Any(ext => !(ext is RelationalOptionsExtension) && !(ext is CoreOptionsExtension))))
            {
                optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MetalBands;Integrated Security=True;Persist Security Info=True");
            }
            CustomizeConfiguration(ref optionsBuilder);
            base.OnConfiguring(optionsBuilder);
        }

        partial void CustomizeConfiguration(ref DbContextOptionsBuilder optionsBuilder);

        public virtual DbSet<enMetalBand> enMetalBands
        {
            get;
            set;
        }

        public virtual DbSet<enAlbum> enAlbums
        {
            get;
            set;
        }

        public virtual DbSet<enMusician> enMusicians
        {
            get;
            set;
        }

        internal virtual DbSet<enUsers> enUsers
        {
            get;
            set;
        }

        public virtual DbSet<enGenre> enGenres
        {
            get;
            set;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            this.enMetalBandMapping(modelBuilder);
            this.CustomizeenMetalBandMapping(modelBuilder);

            this.enAlbumMapping(modelBuilder);
            this.CustomizeenAlbumMapping(modelBuilder);

            this.enMusicianMapping(modelBuilder);
            this.CustomizeenMusicianMapping(modelBuilder);

            this.enUsersMapping(modelBuilder);
            this.CustomizeenUsersMapping(modelBuilder);

            this.enGenreMapping(modelBuilder);
            this.CustomizeenGenreMapping(modelBuilder);

            RelationshipsMapping(modelBuilder);
            CustomizeMapping(ref modelBuilder);
        }

        #region enMetalBand Mapping

        private void enMetalBandMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<enMetalBand>().ToTable(@"MetalBands");
            modelBuilder.Entity<enMetalBand>().Property(x => x.Band_id).HasColumnName(@"band_id").IsRequired().ValueGeneratedOnAdd();
            modelBuilder.Entity<enMetalBand>().Property(x => x.Band_name).HasColumnName(@"band_name").IsRequired().ValueGeneratedNever().HasMaxLength(50);
            modelBuilder.Entity<enMetalBand>().Property(x => x.Date_founding).HasColumnName(@"date_founding").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<enMetalBand>().Property(x => x.Genre_id).HasColumnName(@"genre_id").ValueGeneratedNever();
            modelBuilder.Entity<enMetalBand>().HasKey(@"Band_id");
        }

        partial void CustomizeenMetalBandMapping(ModelBuilder modelBuilder);

        #endregion

        #region enAlbum Mapping

        private void enAlbumMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<enAlbum>().ToTable(@"Albums");
            modelBuilder.Entity<enAlbum>().Property(x => x.Album_id).HasColumnName(@"album_id").IsRequired().ValueGeneratedOnAdd();
            modelBuilder.Entity<enAlbum>().Property(x => x.Band_id).HasColumnName(@"band_id").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<enAlbum>().Property(x => x.Release_Year).HasColumnName(@"release_year").ValueGeneratedNever().HasMaxLength(4);
            modelBuilder.Entity<enAlbum>().Property(x => x.Album_rating).HasColumnName(@"album_rating").ValueGeneratedNever();
            modelBuilder.Entity<enAlbum>().Property(x => x.Album_title).HasColumnName(@"album_title").IsRequired().ValueGeneratedNever().HasMaxLength(50);
            modelBuilder.Entity<enAlbum>().HasKey(@"Album_id");
        }

        partial void CustomizeenAlbumMapping(ModelBuilder modelBuilder);

        #endregion

        #region enMusician Mapping

        private void enMusicianMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<enMusician>().ToTable(@"Musicians");
            modelBuilder.Entity<enMusician>().Property(x => x.Musician_id).HasColumnName(@"musician_id").IsRequired().ValueGeneratedOnAdd();
            modelBuilder.Entity<enMusician>().Property(x => x.First_name).HasColumnName(@"first_name").IsRequired().ValueGeneratedNever().HasMaxLength(30);
            modelBuilder.Entity<enMusician>().Property(x => x.Last_name).HasColumnName(@"last_name").IsRequired().ValueGeneratedNever().HasMaxLength(30);
            modelBuilder.Entity<enMusician>().HasKey(@"Musician_id");
        }

        partial void CustomizeenMusicianMapping(ModelBuilder modelBuilder);

        #endregion

        #region enUsers Mapping

        private void enUsersMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<enUsers>().ToTable(@"Credentials");
            modelBuilder.Entity<enUsers>().Property(x => x.UserId).HasColumnName(@"user_id").IsRequired().ValueGeneratedOnAdd();
            modelBuilder.Entity<enUsers>().Property(x => x.UserName).HasColumnName(@"user_name").IsRequired().ValueGeneratedNever().HasMaxLength(25);
            modelBuilder.Entity<enUsers>().Property(x => x.Password).HasColumnName(@"password").IsRequired().ValueGeneratedNever().HasMaxLength(255);
            modelBuilder.Entity<enUsers>().HasKey(@"UserId");
            modelBuilder.Entity<enUsers>().HasIndex(@"UserName").IsUnique(true);
        }

        partial void CustomizeenUsersMapping(ModelBuilder modelBuilder);

        #endregion

        #region enGenre Mapping

        private void enGenreMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<enGenre>().ToTable(@"Genres");
            modelBuilder.Entity<enGenre>().Property(x => x.Genre_id).HasColumnName(@"Genre_id").IsRequired().ValueGeneratedOnAdd();
            modelBuilder.Entity<enGenre>().Property(x => x.Genre_name).HasColumnName(@"Genre_name").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<enGenre>().HasKey(@"Genre_id");
        }

        partial void CustomizeenGenreMapping(ModelBuilder modelBuilder);

        #endregion

        private void RelationshipsMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<enMetalBand>().HasMany(x => x.Albums).WithOne(op => op.MetalBand).OnDelete(DeleteBehavior.Cascade).HasForeignKey(@"Band_id").IsRequired(true);
            modelBuilder.Entity<enMetalBand>().HasMany(x => x.Musicians).WithMany(op => op.MetalBands)
                .UsingEntity<Dictionary<string, object>>(
                    @"MusiciansInBands",
                    x => x.HasOne<enMusician>().WithMany().HasPrincipalKey(@"Musician_id").HasForeignKey(@"Musician_id"),
                    x => x.HasOne<enMetalBand>().WithMany().HasPrincipalKey(@"Band_id").HasForeignKey(@"Band_id")
                )
                .ToTable(@"MusiciansInBands");
            modelBuilder.Entity<enMetalBand>().HasOne(x => x.Genres).WithMany(op => op.MetalBand).HasForeignKey(@"Genre_id").IsRequired(false);

            modelBuilder.Entity<enAlbum>().HasOne(x => x.MetalBand).WithMany(op => op.Albums).OnDelete(DeleteBehavior.Cascade).HasForeignKey(@"Band_id").IsRequired(true);

            modelBuilder.Entity<enGenre>().HasMany(x => x.MetalBand).WithOne(op => op.Genres).HasForeignKey(@"Genre_id").IsRequired(false);
        }

        partial void CustomizeMapping(ref ModelBuilder modelBuilder);

        public bool HasChanges()
        {
            return ChangeTracker.Entries().Any(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Added || e.State == Microsoft.EntityFrameworkCore.EntityState.Modified || e.State == Microsoft.EntityFrameworkCore.EntityState.Deleted);
        }

        partial void OnCreated();
    }
}
