using DomainModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    internal sealed class StateStorageDbContext(DbContextOptions options, int[] DecimalValuePrecision) : DbContext(options)
    {
        public DbSet<TransactionDto> TransactionsTable { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TransactionDto>().HasKey(p => p.Id);
            modelBuilder.Entity<TransactionDto>().Property(p => p.Value).HasPrecision(DecimalValuePrecision[0], DecimalValuePrecision[1]);
            modelBuilder.Entity<FixedTransactionDto>().HasBaseType<TransactionDto>();
            modelBuilder.Entity<FixedTransactionDto>().Property(p => p.Duration).HasColumnName("Duration");
            modelBuilder.Entity<FixedTransactionDto>().Property(p => p.FixedTransactionId).HasColumnName("FT_CollectionId_NoPk");
        }
    }
}
