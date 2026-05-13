using Domain.Entities;
using Domain.Exceptions;
using Domain.Repository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using System.Linq.Expressions;

namespace Infrastructure.Repository
{
    public class FinancialOperationRepository(
        IDbContextFactory<FinancialDbContext> dbContextFactory)
        : AbstractRepository<FinancialOperation>(dbContextFactory), IFinancialOperationRepository
    {
        public override async Task<FinancialOperation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Id cannot be empty", nameof(id));
            }

            await using var context = _dbContextFactory.CreateDbContext();

            return await context.FinancialOperations.FindAsync([id], cancellationToken);
        }

        public override async Task<IEnumerable<FinancialOperation>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            await using var context = _dbContextFactory.CreateDbContext();

            return await context.FinancialOperations
                            .AsNoTracking()
                            .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<FinancialOperation>> GetAllAsync(string accountId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("User id cannot be empty", nameof(accountId));
            }

            await using var context = _dbContextFactory.CreateDbContext();

            return await context.FinancialOperations
                            .AsNoTracking()
                            .Where(x => x.UserId == accountId)
                            .ToListAsync(cancellationToken);
        }

        public async Task<PagedList<FinancialOperation>> GetAllAsync(string accountId, PaginationParameters parameters, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("User id cannot be empty", nameof(accountId));
            }

            ArgumentNullException.ThrowIfNull(parameters);

            await using var context = _dbContextFactory.CreateDbContext();

            var query = context.FinancialOperations
                            .AsNoTracking()
                            .Where(x => x.UserId == accountId)
                            .OrderByDescending(x => x.Date)
                            .ThenBy(x => x.Id);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedList<FinancialOperation>(items, parameters.PageNumber, parameters.PageSize, totalCount);
        }

        public override async Task<IEnumerable<FinancialOperation>> WhereAsync(Expression<Func<FinancialOperation, bool>> predicate, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            await using var context = _dbContextFactory.CreateDbContext();

            return await context.FinancialOperations
                            .AsNoTracking()
                            .Where(predicate)
                            .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<FinancialOperation>> WhereAsync(Expression<Func<FinancialOperation, bool>> predicate, string accountId, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            await using var context = _dbContextFactory.CreateDbContext();

            return await context.FinancialOperations
                            .AsNoTracking()
                            .Where(x => x.UserId == accountId)
                            .Where(predicate)
                            .ToListAsync(cancellationToken);
        }

        protected override async Task CheckIfAlreadyExistAsync(FinancialOperation operation, FinancialDbContext dbContext, CancellationToken cancellationToken = default)
        {
            if (await dbContext.FinancialOperations.AnyAsync(x =>
                    x.Id != operation.Id &&
                    x.Date == operation.Date &&
                    x.Type == operation.Type &&
                    x.Description == operation.Description &&
                    x.Amount == operation.Amount &&
                    x.UserId == operation.UserId,
                    cancellationToken))
            {
                throw new OperationAlreadyExistException(
                    $"Financial {operation.Type} operation with date {operation.Date}, " +
                    $"description '{operation.Description}' and amount {operation.Amount} already exists!");
            }
        }
    }
}
