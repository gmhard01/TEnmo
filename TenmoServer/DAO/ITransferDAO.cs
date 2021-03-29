using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        Transfer MakeTransfer(int acctToTransferTo, int acctToTransferFrom, decimal amtToTransfer);

        bool UpdateBalance(int acctToTransferTo, int acctToTransferFrom, decimal amtToTransfer);

        List<Transfer> GetTranfersForCurrentUser(int userId);

        List<Transfer> GetPendingRequestsForCurrentUser(int userId);

        public bool CheckBalance(int acctId, decimal amtTransfer);

        public Transfer MakeTransferRequest(int acctToTransferTo, int acctToTransferFrom, decimal amtToTransfer);

        public string RespondToTransferRequest(int transferId, int updatedStatusId, int acctToTransferTo, int acctToTransferFrom, decimal amtToTransfer);

        public Transfer UpdateTransfer(int transferId, int updatedStatusId);

        public Transfer GetTransferById(int transferId);
    }
}
