using Microsoft.AspNetCore.SignalR;
namespace DdnsSharp.SignalR
{
    public class DdnsHub:Hub
    {
        public const string HubUrl = "/ddnschat";
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"{Context.ConnectionId} connected");
             await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception e)
        {
            Console.WriteLine($"Disconnected {e?.Message} {Context.ConnectionId}");
            await base.OnDisconnectedAsync(e);
        }
    }
}