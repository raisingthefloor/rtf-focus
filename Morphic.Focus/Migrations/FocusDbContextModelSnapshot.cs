﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Morphic.Focus.Services;

namespace Morphic.Focus.Migrations
{
    [DbContext(typeof(FocusDbContext))]
    partial class FocusDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.10");

            modelBuilder.Entity("Morphic.Focus.Models.BlockList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateDeleted")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("TEXT");

                    b.Property<string>("DeletedBy")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("BlockLists");
                });

            modelBuilder.Entity("Morphic.Focus.Models.BlockListAppWebsite", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AppWebsiteName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("BlockListId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateDeleted")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("TEXT");

                    b.Property<string>("DeletedBy")
                        .HasColumnType("TEXT");

                    b.Property<string>("IconLocation")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BlockListId");

                    b.ToTable("BlockListAppWebsites");
                });

            modelBuilder.Entity("Morphic.Focus.Models.GeneralSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("BlockScreen1stMinBreak")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateDeleted")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("TEXT");

                    b.Property<string>("DeletedBy")
                        .HasColumnType("TEXT");

                    b.Property<bool>("DontGive5MinScheduledSessionWarning")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ShowCountdownTimer")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("GeneralSettings");
                });

            modelBuilder.Entity("Morphic.Focus.Models.UnblockItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AppWebsiteName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateDeleted")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("TEXT");

                    b.Property<string>("DeletedBy")
                        .HasColumnType("TEXT");

                    b.Property<string>("IconLocation")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("UnblockItems");
                });

            modelBuilder.Entity("Morphic.Focus.Models.BlockListAppWebsite", b =>
                {
                    b.HasOne("Morphic.Focus.Models.BlockList", "BlockList")
                        .WithMany("BlockListAppWebsites")
                        .HasForeignKey("BlockListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BlockList");
                });

            modelBuilder.Entity("Morphic.Focus.Models.BlockList", b =>
                {
                    b.Navigation("BlockListAppWebsites");
                });
#pragma warning restore 612, 618
        }
    }
}
