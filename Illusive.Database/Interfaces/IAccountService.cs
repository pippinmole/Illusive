using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Illusive.Illusive.Database.Models;

namespace Illusive.Illusive.Database.Interfaces {
    public interface IAccountService {

        AccountData AddRecord(AccountData account);
        AccountData GetAccountWhere(Expression<Func<AccountData, bool>> expression);
    }
}