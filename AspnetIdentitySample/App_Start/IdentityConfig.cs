﻿using AspnetIdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AspnetIdentitySample
{
    // This is useful if you do not want to tear down the database each time you run the application.
    // You want to create a new database if the Model changes
    // public class MyDbInitializer : DropCreateDatabaseIfModelChanges<MyDbContext>
    public class MyDbInitializer : CreateDatabaseIfNotExists<MyDbContext>
    {
        protected override void Seed(MyDbContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        private void InitializeIdentityForEF(MyDbContext context)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var myinfo = new MyUserInfo() { FirstName = "John", LastName = "McDonald" };
            string name = "Admin";
            string password = "Fleet2015.";
            string roleName = "Admin";
            //string test = "test";

            //Create Role Test and User Test
            //RoleManager.Create(new IdentityRole(test));
            //UserManager.Create(new ApplicationUser() { UserName = test });

            //Create Role Admin if it does not exist
            var role = RoleManager.FindByName(roleName);
            if (!RoleManager.RoleExists(name))
            {
                var roleresult = RoleManager.Create(new IdentityRole(name));
            }
            //Create Role User if it does not exist
            if (!RoleManager.RoleExists("User"))
            {
                var roleresult = RoleManager.Create(new IdentityRole("User"));
            }

            //Create User=Admin with password=Fleet2015.
            var user = new ApplicationUser();
            user.UserName = name;
            
            user.MyUserInfo = myinfo;
            var adminresult = UserManager.Create(user, password);

            //Add User Admin to Role Admin
            if (adminresult.Succeeded)
            {
                var result = UserManager.AddToRole(user.Id, name);
            }


            // Add user admin to Role Admin if not already added
            var rolesForUser = UserManager.GetRoles(user.Id);
            var resultfinal = UserManager.AddToRole(user.Id, "Admin");
        }
    }
}