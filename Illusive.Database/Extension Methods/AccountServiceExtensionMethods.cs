using System;
using System.Linq.Expressions;
using Illusive.Illusive.Database.Interfaces;
using Illusive.Illusive.Database.Models;

namespace Illusive.Illusive.Database.Extension_Methods {
    public static class AccountServiceExtensionMethods {
        public static AccountData GetAccountByEmail(this IAccountService service, string email) {
            return service.GetAccountWhere(x => x.Email == email);
        }
        
        public static AccountData GetAccountById(this IAccountService service, string id) {
            return service.GetAccountWhere(x => x.Id == id);
        }

        public static bool AccountExistsWhere(this IAccountService service, Expression<Func<AccountData, bool>> predicate, out AccountData account) {
            account = service.GetAccountWhere(predicate);
            return account != null;
        }
    }
}