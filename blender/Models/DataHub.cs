using Microsoft.AspNetCore.SignalR;

public class DataHub : Hub
    {
        public async Task SendData(string Pulse,string So,string Oxigen, string Flow)
        {
            //write data to serial port
            System.Console.WriteLine("Listening to serial port");

    
            await Clients.All.SendAsync("ReceiveData", Pulse,So,Oxigen,Flow);
        }


        
    }