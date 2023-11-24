using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SportMeApp.Models;

    public class SportMeContext : DbContext
    {
        public SportMeContext(DbContextOptions<SportMeContext> options)
            : base(options)
        {
        }

        public DbSet<SportMeApp.Models.Message> Message { get; set; } = default!;
        public DbSet<SportMeApp.Models.Event> Event { get; set; } = default!;
        public DbSet<SportMeApp.Models.User> User { get; set; } = default!;
        public DbSet<SportMeApp.Models.UserEvent> UserEvent { get; set; } = default!;

        public DbSet<SportMeApp.Models.Locations> Locations { get; set; } = default!;
        public DbSet<SportMeApp.Models.Sport> Sport { get; set; } = default!;

}
