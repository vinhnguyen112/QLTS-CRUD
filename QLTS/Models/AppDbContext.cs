using Microsoft.EntityFrameworkCore;

namespace QLTS.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Procurement> Procurements { get; set; }
        public DbSet<ProcurementDetail> ProcurementDetails { get; set; }
        public DbSet<AssetAssignment> AssetAssignments { get; set; }
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<AssetDisposal> AssetDisposals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //UNIQUE

            // Mã loại tài sản không được trùng
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.CategoryCode)
                .IsUnique();

            // Mã phòng ban không được trùng
            modelBuilder.Entity<Department>()
                .HasIndex(d => d.DepartmentCode)
                .IsUnique();

            // Mã nhân viên không được trùng
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.EmployeeCode)
                .IsUnique();

            // Mã tài sản không được trùng
            modelBuilder.Entity<Asset>()
                .HasIndex(a => a.AssetCode)
                .IsUnique();

            // Serial tài sản không được trùng
            modelBuilder.Entity<Asset>()
                .HasIndex(a => a.SerialNumber)
                .IsUnique();

            // Tên đăng nhập không được trùng
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Mã phiếu nhập không được trùng
            modelBuilder.Entity<Procurement>()
                .HasIndex(p => p.ProcurementCode)
                .IsUnique();

            //QUAN HỆ

            // Role - User
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);


            // Department - Employee
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);


            // Category - Asset
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Assets)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            // Department - Asset
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Department)
                .WithMany(d => d.Assets)
                .HasForeignKey(a => a.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);


            // Employee - Asset
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.Assets)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);


            // Procurement - Department
            modelBuilder.Entity<Procurement>()
                .HasOne(p => p.Department)
                .WithMany()
                .HasForeignKey(p => p.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);


            // Procurement - ProcurementDetail
            modelBuilder.Entity<ProcurementDetail>()
                .HasOne(pd => pd.Procurement)
                .WithMany(p => p.ProcurementDetails)
                .HasForeignKey(pd => pd.ProcurementId)
                .OnDelete(DeleteBehavior.Cascade);


            // Asset - ProcurementDetail
            modelBuilder.Entity<ProcurementDetail>()
                .HasOne(pd => pd.Asset)
                .WithMany(a => a.ProcurementDetails)
                .HasForeignKey(pd => pd.AssetId)
                .OnDelete(DeleteBehavior.Restrict);


              //  KHÔNG CHO TRÙNG
            // Asset trong một phiếu nhập

            modelBuilder.Entity<ProcurementDetail>()
                .HasIndex(pd => new
                {
                    pd.ProcurementId,
                    pd.AssetId
                })
                .IsUnique();



            // AssetAssignment


            // Asset - Assignment
            modelBuilder.Entity<AssetAssignment>()
                .HasOne(aa => aa.Asset)
                .WithMany(a => a.AssetAssignments)
                .HasForeignKey(aa => aa.AssetId)
                .OnDelete(DeleteBehavior.Restrict);


            // Phòng ban cũ
            modelBuilder.Entity<AssetAssignment>()
                .HasOne(aa => aa.FromDepartment)
                .WithMany()
                .HasForeignKey(aa => aa.FromDepartmentId)
                .OnDelete(DeleteBehavior.NoAction);


            // Phòng ban mới
            modelBuilder.Entity<AssetAssignment>()
                .HasOne(aa => aa.ToDepartment)
                .WithMany()
                .HasForeignKey(aa => aa.ToDepartmentId)
                .OnDelete(DeleteBehavior.NoAction);


            // Nhân viên cũ
            modelBuilder.Entity<AssetAssignment>()
                .HasOne(aa => aa.FromEmployee)
                .WithMany()
                .HasForeignKey(aa => aa.FromEmployeeId)
                .OnDelete(DeleteBehavior.NoAction);


            // Nhân viên mới
            modelBuilder.Entity<AssetAssignment>()
                .HasOne(aa => aa.ToEmployee)
                .WithMany()
                .HasForeignKey(aa => aa.ToEmployeeId)
                .OnDelete(DeleteBehavior.NoAction);


            // Người thực hiện
            modelBuilder.Entity<AssetAssignment>()
                .HasOne(aa => aa.Creator)
                .WithMany()
                .HasForeignKey(aa => aa.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);



            // MaintenanceRequest


            // Asset - Maintenance
            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(m => m.Asset)
                .WithMany(a => a.MaintenanceRequests)
                .HasForeignKey(m => m.AssetId)
                .OnDelete(DeleteBehavior.Restrict);


            // Người yêu cầu
            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(m => m.Requester)
                .WithMany()
                .HasForeignKey(m => m.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);


            // Kỹ thuật viên
            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(m => m.Technician)
                .WithMany()
                .HasForeignKey(m => m.TechnicianId)
                .OnDelete(DeleteBehavior.NoAction);


            // AssetDisposal

            // Asset - Disposal
            modelBuilder.Entity<AssetDisposal>()
                .HasOne(ad => ad.Asset)
                .WithMany(a => a.AssetDisposals)
                .HasForeignKey(ad => ad.AssetId)
                .OnDelete(DeleteBehavior.Restrict);


            // Người phê duyệt
            modelBuilder.Entity<AssetDisposal>()
                .HasOne(ad => ad.Approver)
                .WithMany()
                .HasForeignKey(ad => ad.ApprovedBy)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}