﻿using DataAccessLayer.Interface;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Common;

namespace DataAccessLayer
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly BookingPartyDataContext _bookingPartyDataContext;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(BookingPartyDataContext context)
        {
            _bookingPartyDataContext = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
             await _bookingPartyDataContext.SaveChangesAsync();
             return entity;
        }

        public async Task<int> DeleteAsync(T entity)
        {
           _dbSet.Remove(entity);
           return await _bookingPartyDataContext.SaveChangesAsync();
        }

        public async Task<ICollection<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
             await _bookingPartyDataContext.SaveChangesAsync();
             return entity;
        }
        
        public IQueryable<T> GetByCondition(Expression<Func<T, bool>> condition)
        {
            return _dbSet.Where(condition).AsQueryable().AsNoTracking();
        }
        public async Task<T> GetByProperty(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<ICollection<T>> GetListByProperty(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<PaginatedResult<T>> GetPaginatedListAsync(int page, int pageSize)
        {
            var totalCount = await _dbSet.CountAsync();
            var pageCount = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await _dbSet.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageCount = pageCount
            };
        }

        public T GetLast<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return _dbSet.OrderByDescending(keySelector).FirstOrDefault();
        }
    }
}
