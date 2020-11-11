using System;
using System.Linq.Expressions;
using Illusive.Models;
using MongoDB.Driver;

namespace Illusive.Database {
    public interface IAccountService {
        AccountData AddRecord(AccountData account);
        AccountData GetAccountWhere(Expression<Func<AccountData, bool>> expression);
        void UpdateAccount(string accountId, UpdateDefinition<AccountData> update);
    }
}