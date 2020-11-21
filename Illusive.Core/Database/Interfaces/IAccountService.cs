using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Illusive.Models;
using MongoDB.Driver;

namespace Illusive.Database {
    public interface IAccountService {
        AccountData AddRecord(AccountData account);
        AccountData GetAccountWhere(Expression<Func<AccountData, bool>> expression);
        void UpdateAccount(string accountId, UpdateDefinition<AccountData> update);
        
        /// <summary>
        /// <para> Removes the first account that matches the expression. </para>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>True when </returns>
        bool RemoveAccountWhere(Expression<Func<AccountData, bool>> expression);
        
        /// <summary>
        /// <para> Removes the first account that matches the expression. </para>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>True when </returns>
        Task<bool> RemoveAccountWhereAsync(Expression<Func<AccountData, bool>> expression);
    }
}