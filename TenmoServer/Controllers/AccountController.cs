using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Models;
using System;
using System.Collections.Generic;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountDAO accountDAO;
        private readonly ITransferDAO transferDAO;

        public AccountController(IAccountDAO _accountDAO, ITransferDAO _transferDAO)
        {
            accountDAO = _accountDAO;
            transferDAO = _transferDAO;
        }

        [HttpGet("balance")]
        public ActionResult<decimal> GetAccountBalance()
        {
            int acctId = accountDAO.GetAccountId(Convert.ToInt32(User.FindFirst("sub").Value));
            decimal balance = accountDAO.GetBalance(acctId);
            return Ok(balance);
        }

        [HttpGet("listaccountusers")]
        public ActionResult<List<Models.User>> ListAccountUsers()
        {
            int userId = Convert.ToInt32(User.FindFirst("sub").Value);
            return Ok(accountDAO.ListUsers(userId));
        }

        [HttpGet("listtransfers")]
        public ActionResult<List<Models.Transfer>> ListTransfersForCurrentUser()
        {
            int userId = Convert.ToInt32(User.FindFirst("sub").Value);
            return Ok(transferDAO.GetTranfersForCurrentUser(userId));
        }

        [HttpGet("listpendingrequests")]
        public ActionResult<List<Models.Transfer>> ListPendingRequestsForCurrentUser()
        {
            int userId = Convert.ToInt32(User.FindFirst("sub").Value);
            return Ok(transferDAO.GetPendingRequestsForCurrentUser(userId));
        }

        [HttpPost("transfer/{toUserId}/{amount}")]
        public ActionResult<Transfer> Transfer(int toUserId, decimal amount)
        {
            int currentUserAcctId = accountDAO.GetAccountId(Convert.ToInt32(User.FindFirst("sub").Value));
            int toAcctId = accountDAO.GetAccountId(toUserId);
            Transfer createdTransfer = transferDAO.MakeTransfer(toAcctId, currentUserAcctId, amount);
            return Created($"Transfer created. Transfer ID: {createdTransfer.TransferId}", createdTransfer);
        }

        [HttpPost("requesttransfer/{fromUserId}/{amount}")]
        public ActionResult<Transfer> RequestTransfer(int fromUserId, decimal amount)
        {
            int currentUserAcctId = accountDAO.GetAccountId(Convert.ToInt32(User.FindFirst("sub").Value));
            int fromAcctId = accountDAO.GetAccountId(fromUserId);
            Transfer requestedTransfer = transferDAO.MakeTransferRequest(currentUserAcctId, fromAcctId, amount);
            return Created($"Transfer created. Transfer ID: {requestedTransfer.TransferId}", requestedTransfer);
        }

        [HttpPut("respondtransfer/{transferId}/{statusId}/{toAcctId}/{amount}")]
        public ActionResult<string> RespondToRequest(int transferId, int statusId, int toAcctId, decimal amount)
        {
            int currentUserAcctId = accountDAO.GetAccountId(Convert.ToInt32(User.FindFirst("sub").Value));
            return Ok(transferDAO.RespondToTransferRequest(transferId, statusId, toAcctId, currentUserAcctId, amount));
        }
    }
}
