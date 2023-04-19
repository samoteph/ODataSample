//-----------------------------------------------------------------------------
// <copyright file="MyDataContext.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ODataRoutingSample.Models
{
    public class MyDataContext : DbContext
    {
        public MyDataContext(DbContextOptions<MyDataContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<SamCelebrity> Sams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().OwnsOne(c => c.HomeAddress).WithOwner();
            modelBuilder.Entity<Customer>().OwnsMany(c => c.FavoriteAddresses).WithOwner();

            modelBuilder.Entity<SamCelebrity>().Ignore(p=>p.Properties);
        }
    }
}
