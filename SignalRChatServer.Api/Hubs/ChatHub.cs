using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR;
using SignalRChatServer.Api.Data;
using SignalRChatServer.Api.Models;
using System.Linq;

namespace SignalRChatServer.Api.Hubs
{
    public class ChatHub : Hub
    {
        public async Task GetNickName(string nickName)
        {
            Client client = new Client
            {
                ConnectionId = Context.ConnectionId,
                NickName = nickName,
            };
            ClientSource.Clients.Add(client);

            await Clients.All.SendAsync("clientJoined", client.NickName);

            await Clients.All.SendAsync("clients", ClientSource.Clients);
        }

        public async Task SendMessageAsync(string message, string clientName)
        {
            clientName = clientName.Trim();
            Client? senderClient = ClientSource.Clients.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
            if (clientName == "Tümü")
            {
                await Clients.Others.SendAsync("ReceiveMessage", message, senderClient.NickName);
            }

            else
            {
                var client = ClientSource.Clients.FirstOrDefault(c => c.NickName == clientName);
                await Clients.Client(client!.ConnectionId).SendAsync("ReceiveMessage", message, senderClient!.NickName);
            }
        }

        public async Task AddGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            Group group = new Group { GroupName = groupName };
            group.Clients.Add(ClientSource.Clients.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId));
            GroupSouce.Groups.Add(group);

            await Clients.All.SendAsync("Groups", GroupSouce.Groups);
        }



        public async Task AddClientToGroup(string[] groupNames)
        {
            Client? client = ClientSource.Clients.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);

            foreach (var group in groupNames)
            {
                var _group = GroupSouce.Groups.FirstOrDefault(x => x.GroupName == group);

                var result = _group.Clients.Any(x => x.ConnectionId == client.ConnectionId);
                if (!result)
                {
                    _group.Clients.Add(client);
                    await Groups.AddToGroupAsync(Context.ConnectionId, group);
                }

            }
        }


        public async Task GetClientToGroup(string groupName)
        {
            Group group = GroupSouce.Groups.FirstOrDefault(x => x.GroupName == groupName);
            await Clients.Caller.SendAsync("clients", groupName == "Tümü" ? ClientSource.Clients : group.Clients);
        }



        public async Task SendMessageToGroupAsync(string groupName, string message)
        {
            var client = ClientSource.Clients.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", message, client.NickName);
        }
    }
}
