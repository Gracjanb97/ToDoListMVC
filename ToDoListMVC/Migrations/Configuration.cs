namespace ToDoListMVC.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using ToDoListMVC.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<ToDoListMVC.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ToDoListMVC.Models.ApplicationDbContext context)
        {
            AddUsers(context);
        }

        void AddUsers(ToDoListMVC.Models.ApplicationDbContext context)
        {
            var user = new ApplicationUser { UserName = "user1@email.com" };
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            userManager.Create(user, "password");
        }
    }
}
