using Microsoft.AspNetCore.SignalR;
namespace DdnsSharp.Core
{
    public class DdnsHub:Hub
    {
        public const string HubUrl = "/ddnschat";
        private readonly DdnsMessageContainer messageContainer;
        public DdnsHub(DdnsMessageContainer messageContainer)
        {
            this.messageContainer = messageContainer;
        }
        public async Task DdnsMessage(string message)
        {
            string msg = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss} {message}";
            await Clients.All.SendAsync("DdnsMessage", msg);
            messageContainer.AddMessage(message);
        }
        public override async Task OnConnectedAsync()
        {
             await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception e)
        {
            await base.OnDisconnectedAsync(e);
        }
    }
}