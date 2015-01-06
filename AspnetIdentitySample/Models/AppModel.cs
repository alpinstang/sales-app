using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AspnetIdentitySample.Models
{
    public class ApplicationUser : IdentityUser
    {
        // HomeTown will be stored in the same table as Users
        //public string HomeTown { get; set; }
        public virtual ICollection<ToDo> ToDoes { get; set; }

        // FirstName & LastName will be stored in a different table called MyUserInfo
        public virtual MyUserInfo MyUserInfo { get; set; }
    }

    public class MyUserInfo
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class ToDo
    {
        public int Id { get; set; }
        public virtual ApplicationUser User { get; set; }

        //These identities will comprise the Appointments form
        public string Date { get; set; }
        public string Company { get; set; }
        public string Description { get; set; }
        public string ApptType { get; set; }
        public string CustomerContact { get; set; }
    }

    public class AfterVisit
    {
        public int Id { get; set; }
        public virtual ApplicationUser User { get; set; }

        //These identities will comprise the After Visit Info form
        public string Date { get; set; }
        public string Company { get; set; }
        public string Competition { get; set; }
        public string PurchaseMethod { get; set; }
        public string PurchaseDate { get; set; }
        public string FutureDate { get; set; }
        public string NewVehicles { get; set; }
        public string NewVehiclesCount { get; set; }
        public string Discussion { get; set; }

        public string VisitDate { get; set; }
    }

    public class MyDbContext : IdentityDbContext<ApplicationUser>
    {
        public MyDbContext()
            : base("DefaultConnection")
        {
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Change the name of the table to be Users instead of AspNetUsers
            modelBuilder.Entity<IdentityUser>()
                .ToTable("Users");
            modelBuilder.Entity<ApplicationUser>()
                .ToTable("Users");
        }

        public DbSet<ToDo> ToDoes { get; set; }

        public DbSet<AfterVisit> AfterVisit { get; set; }

        public DbSet<MyUserInfo> MyUserInfo { get; set; }
    }
}