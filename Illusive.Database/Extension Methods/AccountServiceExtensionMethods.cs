using System;
using System.Linq.Expressions;
using Illusive.Illusive.Database.Interfaces;
using Illusive.Illusive.Database.Models;

namespace Illusive.Illusive.Database.Extension_Methods {
    public static class AccountServiceExtensionMethods {

        public static AccountData GetAccount(this IAccountService service, string email) {
            return service.GetAccountWhere(x => x.Email == email);
        }
        
        public static bool AccountExists(this IAccountService service, string email, out AccountData account) {
            account = service.GetAccountWhere(x => x.Email == email);
            return account != null;
        }

        public static bool AccountExists(this IAccountService service, Expression<Func<AccountData, bool>> predicate, out AccountData account) {
            account = service.GetAccountWhere(predicate);
            return account != null;
        }
    }
}