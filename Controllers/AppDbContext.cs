using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace rest1.Controllers
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        //public DbSet<User> Users { get; set; }  // 예제 모델
        //public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomUser> RoomUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>(entity =>
            //{
            //    entity.ToTable("user", schema: "talk");   // 🎯 PostgreSQL 테이블명 + 스키마

            //    entity.HasKey(e => e.UsrNo)
            //          .HasName("user_pk");

            //    entity.Property(e => e.UsrNo).HasColumnName("usr_no");
            //    entity.Property(e => e.UsrNm).HasColumnName("usr_nm");
            //    entity.Property(e => e.DivNo).HasColumnName("div_no");
            //    entity.Property(e => e.UsrId).HasColumnName("usr_id");
            //    entity.Property(e => e.UsrPw).HasColumnName("usr_pw");
            //});

            //modelBuilder.Entity<Room>()
            //    .ToTable("room", "talk")
            //    .HasKey(r => r.RoomNo);
            //modelBuilder.Entity<Room>()
            //    .Property(r => r.RoomNo).HasColumnName("room_no");

            modelBuilder.Entity<RoomUser>()
                .ToTable("roomuser", "talk")
                .HasKey(ru => new { ru.RoomNo, ru.UsrNo });
            // modelBuilder.Entity<RoomUser>().Property(ru => ru.RoomNo).HasColumnName("room_no");
            // modelBuilder.Entity<RoomUser>().Property(ru => ru.UsrNo).HasColumnName("usr_no");
            // modelBuilder.Entity<RoomUser>().Property(ru => ru.Title).HasColumnName("title");
            // modelBuilder.Entity<RoomUser>().Property(ru => ru.DelYn).HasColumnName("del_yn");
            //modelBuilder.Entity<RoomUser>()
            //    .HasOne(ru => ru.Room)
            //    .WithMany(r => r.RoomUsers)
            //    .HasForeignKey(ru => ru.RoomNo);
        }
    }

    //public class User
    //{
    //    public long UsrNo { get; set; }
    //    public string UsrNm { get; set; }
    //    public long? DivNo { get; set; }
    //    public string? UsrId { get; set; }
    //    public string? UsrPw { get; set; }
    //}

    //public class Room
    //{
    //    [Key]
    //    public long RoomNo { get; set; }
    //    public ICollection<RoomUser> RoomUsers { get; set; }
    //}

    public class RoomUser
    {
        public long RoomNo { get; set; }
        public long UsrNo { get; set; }
        public string Title { get; set; }
        public string DelYn { get; set; } = "N";

        // 네비게이션 속성
        //public Room Room { get; set; }
    }
}
