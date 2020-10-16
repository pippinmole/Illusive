using Illusive.Illusive.Database.Models;

namespace Illusive.Illusive.Database.Interfaces {
    public interface IAccountService {

        AccountData AddRecord(AccountData account);
        AccountData GetAccount(string email);
        bool AccountExists(string email, out AccountData account);
    }
}