using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SportMeApp.Models;

    public class SportMeAppContext : DbContext
    {
        public SportMeAppContext(DbContextOptions<SportMeAppContext> options)
            : base(options)
        {
        }

        public DbSet<SportMeApp.Models.Message> Message { get; set; } = default!;
        public DbSet<SportMeApp.Models.Group> Group { get; set; } = default!;
        public DbSet<SportMeApp.Models.User> User { get; set; } = default!;
        public DbSet<SportMeApp.Models.UserGroup> UserGroup { get; set; } = default!;
}
