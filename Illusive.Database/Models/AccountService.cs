using System;
using System.Collections.Generic;
using Illusive.Illusive.Database.Interfaces;
using MongoDB.Driver;

namespace Illusive.Illusive.Database.Models {
    public class AccountService : IAccountService {

        private readonly IMongoCollection<AccountData> _accounts;

        public AccountService(IDatabaseContext ctx) {
            this._accounts = ctx.GetDatabase("IllusiveDatabase").GetCollection<AccountData>("user_info");
            Console.WriteLine($"Accounts: {this._accounts.CollectionNamespace}");
        }

        public AccountData AddRecord(AccountData account) {
            this._accounts.InsertOne(account);
            return account;
        }

        public AccountData GetAccount(string email) {
            Console.WriteLine($"Trying to get account from email: {email}");
            return this._accounts.Find(x => x.Email == email).FirstOrDefault();
        }

        public bool AccountExists(string email, out AccountData account) {
            account = this._accounts.Find(x => x.Email == email).FirstOrDefault();
            return account != null;
        }
    }
}