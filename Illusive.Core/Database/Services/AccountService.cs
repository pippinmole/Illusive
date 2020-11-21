using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Illusive.Illusive.Database.Interfaces;
using Illusive.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Illusive.Database {
    public class AccountService : IAccountService {
        
        private readonly ILogger<AccountService> _logger;
        private readonly IMongoCollection<AccountData> _accounts;

        public AccountService(ILogger<AccountService> logger, IDatabaseContext ctx) {
            this._logger = logger;
            this._accounts = ctx.GetDatabase("IllusiveDatabase").GetCollection<AccountData>("user_info");

        }

        public AccountData AddRecord(AccountData account) {
            this._accounts.InsertOne(account);
            return account;
        }

        public AccountData GetAccount(string email) {
            var accounts = this._accounts.Find(x => x.Email == email);
            return accounts.FirstOrDefault();
        }

        public AccountData GetAccountWhere(Expression<Func<AccountData, bool>> expression) {
            return this._accounts.Find(expression).FirstOrDefault();
        }

        public void UpdateAccount(string accountId, UpdateDefinition<AccountData> update) {
            this._accounts.UpdateOne(x => x.Id == accountId, update);
        }
        
        public bool RemoveAccountWhere(Expression<Func<AccountData, bool>> expression) {
            var result = this._accounts.DeleteOne(expression);
            return result.DeletedCount == 1;
        }

        public async Task<bool> RemoveAccountWhereAsync(Expression<Func<AccountData, bool>> expression) {
            var result = await this._accounts.DeleteOneAsync(expression);
            return result.DeletedCount == 1;
        }
    }
}