using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RevenueControl.Web.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Permissions = new HashSet<UserClientPermission>();
        }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserClientPermission> Permissions { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class UserClientPermission
    {
        public int Id { get; set; }

        //[Required]
        [StringLength(128)]
        public string UserId { get; set; }

        //[Required]
        [StringLength(75)]
        public string ClientName { get; set; }

        //[ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        //[ForeignKey("ClientName")]
        public virtual Client Client { get; set; }
    }

    public class Client
    {
        public Client()
        {
            Permissions = new HashSet<UserClientPermission>();
        }

        [Key]
        [MaxLength(75)]
        public string Name { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserClientPermission> Permissions { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", false)
        {
            Database.SetInitializer<ApplicationDbContext>(null);
        }

        public DbSet<UserClientPermission> UserClientPermissions { get; set; }

        public DbSet<Client> Clients { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.Permissions)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.Permissions)
                .WithRequired(e => e.Client)
                .HasForeignKey(e => e.ClientName);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}