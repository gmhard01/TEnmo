using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
using TenmoClient.Data;

namespace TenmoClient
{
    public class APIService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();
        private static API_User user = new API_User();

        public bool LoggedIn { get { return !string.IsNullOrWhiteSpace(user.Token); } }

        public APIService()
        {
            client = new RestClient();
        }

        public decimal GetMyBalance()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "account/balance");
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<decimal> response = client.Get<decimal>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Error occurred - unable to reach server.", response.ErrorException);
            }
            else if (!response.IsSuccessful)
            {
                throw new Exception("Error occurred - received non-success response: " + (int)response.StatusCode);
            }
            else
            {
                return response.Data;
            }
        }

        public List<Transfer> GetMyTransfers()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "account/listtransfers");
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Error occurred - unable to reach server.", response.ErrorException);
            }
            else if (!response.IsSuccessful)
            {
                throw new Exception("Error occurred - received non-success response: " + (int)response.StatusCode);
            }
            else
            {
                return response.Data;
            }
        }

        public List<User> GetListUsers()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "account/listaccountusers");
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<List<User>> response = client.Get<List<User>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Error occurred - unable to reach server.", response.ErrorException);
            }
            else if (!response.IsSuccessful)
            {
                throw new Exception("Error occurred - received non-success response: " + (int)response.StatusCode);
            }
            else
            {
                return response.Data;
            }
        }

        public Transfer MakeTransferToAcct(int userToTransferTo, decimal amtToTransfer)
        {
            RestRequest request = new RestRequest(API_BASE_URL + $"account/transfer/{userToTransferTo}/{amtToTransfer}");
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse <Transfer> response = client.Post<Transfer>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Error occurred - unable to reach server.", response.ErrorException);
            }
            else if (!response.IsSuccessful)
            {
                throw new Exception("Error occurred - received non-success response: " + (int)response.StatusCode);
            }
            else
            {
                return response.Data;
            }
        }

        public Transfer RequestTransfer(int userToTransferFrom, decimal amtToRequest)
        {
            RestRequest request = new RestRequest(API_BASE_URL + $"account/requesttransfer/{userToTransferFrom}/{amtToRequest}");
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<Transfer> response = client.Post<Transfer>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Error occurred - unable to reach server.", response.ErrorException);
            }
            else if (!response.IsSuccessful)
            {
                throw new Exception("Error occurred - received non-success response: " + (int)response.StatusCode);
            }
            else
            {
                return response.Data;
            }
        }

        public List<Transfer> GetMyPendingRequests()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "account/listpendingrequests");
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Error occurred - unable to reach server.", response.ErrorException);
            }
            else if (!response.IsSuccessful)
            {
                throw new Exception("Error occurred - received non-success response: " + (int)response.StatusCode);
            }
            else
            {
                return response.Data;
            }
        }

        public string UpdatePendingRequest(int transferId, int statusId, int toAcctId, decimal amount)
        {
            Transfer t = new Transfer()
            {
                TransferId = transferId,
                AccountTo = toAcctId,
                Amount = amount

            };

            RestRequest request = new RestRequest(API_BASE_URL + $"account/respondtransfer/{transferId}/{statusId}/{toAcctId}/{amount}");
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            request.AddJsonBody(t);
            IRestResponse<string> response = client.Put<string>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Error occurred - unable to reach server.", response.ErrorException);
            }
            else if (!response.IsSuccessful)
            {
                throw new Exception("Error occurred - received non-success response: " + (int)response.StatusCode);
            }
            else
            {
                return response.Data;
            }
        }

    }
}
