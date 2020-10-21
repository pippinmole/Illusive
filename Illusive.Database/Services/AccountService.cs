﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Illusive.Illusive.Database.Interfaces;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using ILogger = DnsClient.Internal.ILogger;

namespace Illusive.Illusive.Database.Models {
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
            Console.WriteLine($"Trying to get account from email: {email}");
            
            var accounts = this._accounts.Find(x => x.Email == email);
            return accounts.FirstOrDefault();
        }

        public bool AccountExists(string email, out AccountData account) {
            account = this._accounts.Find(x => x.Email == email).FirstOrDefault();
            return account != null;
        }

        public bool AccountExists(Expression<Func<AccountData, bool>> predicate, [NotNull] out AccountData account) {
            account = this._accounts.Find(predicate).FirstOrDefault();
            return account != null;
        }
    }
}