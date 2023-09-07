using Microsoft.AspNetCore.SignalR;

namespace ConveyorBlazorServerNet7.Hubs
{
    public class ConveyorHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
