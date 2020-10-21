namespace Illusive.Illusive.Database.Models {
    public static class AccountDataExtensions {
        
        public static bool VerifyPasswordHashEquals(this AccountData accountData, string plainTextPassword) =>
            BCrypt.Net.BCrypt.Verify(plainTextPassword, accountData.HashedPassword);
    }
}