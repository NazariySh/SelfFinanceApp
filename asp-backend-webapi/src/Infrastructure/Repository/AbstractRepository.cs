using Domain.Entities;
using Domain.Exceptions;
using Domain.Repository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repository
{
    public abstract class AbstractRepository<T>(IDbContextFactory<FinancialDbContext> dbContextFactory) : IAbstractRepository<T> where T : BaseEntity
    {
        protected readonly IDbContextFactory<FinancialDbContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            await using var context = _dbContextFactory.CreateDbContext();

            await CreateAsync(entity, context, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            if (entity.Id == default)
            {
                throw new ArgumentException("Id cannot be empty", nameof(entity));
            }

            await using var context = _dbContextFactory.CreateDbContext();

            if (!await IsEntityIdInDatabaseAsync(entity.Id, context, cancellationToken))
            {
                throw new NotFoundException($"{typeof(T).Name} with id {entity.Id} does not exist!");
            }

            await CheckIfAlreadyExistAsync(entity, context, cancellationToken);

            context.Set<T>().Update(entity);

            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveAsync(T entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            if (entity.Id == default)
            {
                throw new ArgumentException("Id cannot be empty", nameof(entity));
            }

            await using var context = _dbContextFactory.CreateDbContext();

            await CheckIfEntityAssignedAsync(entity, context, cancellationToken);

            context.Set<T>().Remove(entity);

            await context.SaveChangesAsync(cancellationToken);
        }

        public abstract Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        public abstract Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        public abstract Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        protected virtual Task CheckIfAlreadyExistAsync(T entity, FinancialDbContext dbContext, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        protected virtual Task CheckIfEntityAssignedAsync(T entity, FinancialDbContext dbContext, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        private async Task CreateAsync(T entity, FinancialDbContext dbContext, CancellationToken cancellationToken = default)
        {
            if (entity.Id == default)
            {
                throw new ArgumentException("Id cannot be empty", nameof(entity));
            }

            await CheckIfAlreadyExistAsync(entity, dbContext, cancellationToken);

            await dbContext.Set<T>().AddAsync(entity, cancellationToken);
        }

        private static Task<bool> IsEntityIdInDatabaseAsync(Guid id, FinancialDbContext dbContext, CancellationToken cancellationToken = default)
        {
            return dbContext.Set<T>().AnyAsync(x => x.Id == id, cancellationToken);
        }
    }
}
