using DomainModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public sealed class StateStorageDbContext : DbContext
    {
        public StateStorageDbContext(DbContextOptions options) : base(options){ }
        public StateStorageDbContext(DbContextOptions options, int[] decimalValuePrecision) : base(options)
        {
            DecimalValuePrecision = decimalValuePrecision;
        }

        private int[]? DecimalValuePrecision;
        public DbSet<TransactionDto> TransactionsTable { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TransactionDto>().HasKey(p => p.Id);
            modelBuilder.Entity<FixedTransactionDto>().HasBaseType<TransactionDto>();
            modelBuilder.Entity<FixedTransactionDto>().Property(p => p.Duration).HasColumnName("Duration");
            modelBuilder.Entity<FixedTransactionDto>().Property(p => p.FixedTransactionId).HasColumnName("FT_CollectionId_NoPk");

            if(DecimalValuePrecision is not null)
            {
                modelBuilder.Entity<TransactionDto>().Property(p => p.Value).HasPrecision(DecimalValuePrecision[0], DecimalValuePrecision[1]);
            }
        }
    }
}
