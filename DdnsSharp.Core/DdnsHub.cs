using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DdnsSharp.Core
{
    public class DdnsHub:Hub
    {
        public const string HubUrl = "/ddnschat";
        private readonly DdnsMessageContainer messageContainer;
        private readonly ILogger<DdnsHub> logger;
        public DdnsHub(DdnsMessageContainer messageContainer,ILogger<DdnsHub> logger)
        {
            this.messageContainer = messageContainer;
            this.logger = logger;
        }
        public async Task DdnsMessage(string message)
        {
            string msg = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss} {message}";
            await Clients.All.SendAsync("DdnsMessage", msg);
            logger.LogInformation(msg);
            messageContainer.AddMessage(msg);
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