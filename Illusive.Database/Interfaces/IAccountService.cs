using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Illusive.Illusive.Database.Models;

namespace Illusive.Illusive.Database.Interfaces {
    public interface IAccountService {

        AccountData AddRecord(AccountData account);
        AccountData GetAccount(string email);
        bool AccountExists(string email, out AccountData account);
        bool AccountExists(Expression<Func<AccountData, bool>> predicate, [NotNull] out AccountData account);
    }
}