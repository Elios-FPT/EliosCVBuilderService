using CVBuilder.Core.Interfaces;
using CVBuilder.Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace CVBuilder.Infrastructure.Implementations
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly CVBuilderDataContext _context;
        private readonly IDbContextTransaction _transaction;

        public EfUnitOfWork(CVBuilderDataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            // Close transaction if another transaction existed
            var currentTransaction = _context.Database.CurrentTransaction;
            if (currentTransaction != null)
            {
                currentTransaction.Dispose();
            }

            // Begin new transaction 
            _transaction = _context.Database.BeginTransaction();
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}