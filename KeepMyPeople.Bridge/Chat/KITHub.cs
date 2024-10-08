using KeepMyPeople.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace KeepMyPeople.Bridge.Chat;

[Authorize]
public class KITHub(IHubContext<KITHub> hubContext) : Hub<IKITHub>, IKITHub
{
    [HubMethodName("SendMessage")]
    public async Task SendMessage(ChatSettings settings, SendMessage sm)
    {
        Console.WriteLine("SendMessage de " + settings.Me.Pseudo + " to other " + settings.Other.Pseudo);
        await hubContext.Clients.User(settings.Other.Id.ToString()).
            SendAsync(ChatHubStatus.ReceivingMessage, new ReceivingMessage(sm.Message));
        Console.WriteLine("Sendessage ACK " + settings.Me.Pseudo);
        await hubContext.Clients.User(settings.Me.Id.ToString()).
            SendAsync(ChatHubStatus.SentMessage, new SentMessage(settings.Other));
    }
    public async override Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        Console.WriteLine("Connected " + userId);

        await hubContext.Clients.User(userId).SendAsync(ChatHubStatus.Connected, userId, Context.ConnectionId);
    }
    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        Console.WriteLine("Disconnected " + userId);
    }
    public async Task Initializing(ChatSettings settings)
    {
        await hubContext.Clients.User(settings.Other.Id.ToString()).
            SendAsync(ChatHubStatus.Initialized, new Initialization());
        
        await hubContext.Clients.User(settings.Me.Id.ToString()).
            SendAsync(ChatHubStatus.Initialized, new Initialization());
    }
    
}