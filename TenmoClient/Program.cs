using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        private static readonly APIService apiService = new APIService();

        static void Main(string[] args)
        {
            Run();
        }

        private static void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        API_User user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
                        }
                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = consoleService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }

            MenuSelection();
        }

        private static void MenuSelection()
        {
            API_User user = new API_User();
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1)
                {
                    Console.WriteLine($"Your current account balance is: {apiService.GetMyBalance():C2}");
                }
                else if (menuSelection == 2)
                {
                    List<Transfer> transfers = apiService.GetMyTransfers();

                    Console.WriteLine("---------------------------------");
                    Console.WriteLine("Transfers\nID \t From/To \t Amount");
                    Console.WriteLine("---------------------------------");

                    foreach (Transfer transfer in transfers)
                    {
                        if (transfer.UserFrom == UserService.GetUsername())
                        {
                            Console.WriteLine($"{transfer.TransferId} \t To: {transfer.UserTo} \t {transfer.Amount:C2}");
                        }
                        else
                        {
                            Console.WriteLine($"{transfer.TransferId} \t From: {transfer.UserFrom} \t {transfer.Amount:C2}");
                        }
                    }

                    Console.WriteLine("Please enter transfer ID to view details (0 to cancel):");
                    int userResponse = int.Parse(Console.ReadLine());
                    if (userResponse != 0)
                    {
                        Transfer selectedTransfer = new Transfer();
                        foreach (Transfer transfer in transfers)
                        {
                            if (transfer.TransferId == userResponse)
                            {
                                selectedTransfer = transfer;
                            }
                        }
                        Console.WriteLine("---------------------------------");
                        Console.WriteLine("Transfer Details");
                        Console.WriteLine("---------------------------------");
                        Console.WriteLine($"ID: {selectedTransfer.TransferId} \nFrom: {selectedTransfer.UserFrom} \nTo: {selectedTransfer.UserTo} \nType: {selectedTransfer.TransferTypeDesc} \nStatus: {selectedTransfer.TransferStatusDesc} \nAmount: {selectedTransfer.Amount:C2}");
                    }

                }
                else if (menuSelection == 3)
                {
                    List<Transfer> transfers = apiService.GetMyPendingRequests();

                    Console.WriteLine("---------------------------------");
                    Console.WriteLine("Pending\nTransfer\nRequests\nID \tTo \t Amount");
                    Console.WriteLine("---------------------------------");

                    foreach (Transfer transfer in transfers)
                    {
                        Console.WriteLine($"{transfer.TransferId} \t {transfer.UserTo} \t {transfer.Amount:C2}");
                    }
                    Console.WriteLine("Please enter transfer ID to approve/reject (0 to cancel):");
                    int userResponse = int.Parse(Console.ReadLine());
                    if (userResponse != 0)
                    {
                        Transfer transferToUpdate = new Transfer();
                        foreach (Transfer t in transfers)
                        {
                            if (userResponse == t.TransferId)
                            {
                                transferToUpdate = t;
                            }
                        }

                        Console.WriteLine("1: Approve\n2: Reject\n3: Don't Approve or Reject");
                        Console.WriteLine("Please choose an option:");
                        int userSelection = int.Parse(Console.ReadLine());

                        if (userSelection == 1)
                        {
                            Console.WriteLine(apiService.UpdatePendingRequest(transferToUpdate.TransferId, 2, transferToUpdate.AccountTo, transferToUpdate.Amount));
                        }
                        else if (userSelection == 2)
                        {
                            apiService.UpdatePendingRequest(transferToUpdate.TransferId, 3, transferToUpdate.AccountTo, transferToUpdate.Amount);
                        }
                    }
                }
                else if (menuSelection == 4)
                {
                    List<User> userList = apiService.GetListUsers();
                    Console.WriteLine("---------------------------------");
                    Console.WriteLine("Users \nID\tName");
                    Console.WriteLine("---------------------------------");
                    foreach (User u in userList)
                    {
                        Console.WriteLine($"{u.UserId}\t{u.Username}");
                    }
                    Console.WriteLine();
                    Console.WriteLine("Please enter ID of user you are sending to (0 to cancel):");
                    int uResponse = int.Parse(Console.ReadLine());
                    if (uResponse != 0)
                    {
                        Console.WriteLine("Enter amount:");
                        decimal amountToTransfer = decimal.Parse(Console.ReadLine());
                        Transfer newTransfer = apiService.MakeTransferToAcct(uResponse,amountToTransfer);

                        if (newTransfer != null)
                        {
                            Console.WriteLine($"Transfer successful. Transfer ID: {newTransfer.TransferId}");
                        }
                    }

                }
                else if (menuSelection == 5)
                {
                    List<User> userList = apiService.GetListUsers();
                    Console.WriteLine("---------------------------------");
                    Console.WriteLine("Users \nID\tName");
                    Console.WriteLine("---------------------------------");
                    foreach (User u in userList)
                    {
                        Console.WriteLine($"{u.UserId}\t{u.Username}");
                    }
                    Console.WriteLine();
                    Console.WriteLine("Please enter ID of user you are requesting from (0 to cancel):");
                    int uResponse = int.Parse(Console.ReadLine());
                    if (uResponse != 0)
                    {
                        Console.WriteLine("Enter amount:");
                        decimal amountToRequest = decimal.Parse(Console.ReadLine());
                        Transfer newTransfer = apiService.RequestTransfer(uResponse, amountToRequest);

                        if (newTransfer != null)
                        {
                            Console.WriteLine($"Transfer requested. Pending approval. Transfer ID: {newTransfer.TransferId}");
                        }
                    }
                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new API_User()); //wipe out previous login info
                    Run(); //return to entry point
                }
                else
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }
    }
}
