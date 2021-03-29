using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountDAO
    {
        int GetAccountId(int userId);

        int GetUserId(int acctId);

        decimal GetBalance(int accountId);

        List<User> ListUsers(int userId);
    }
}
