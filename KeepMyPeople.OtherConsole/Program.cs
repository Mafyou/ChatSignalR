// See https://aka.ms/new-console-template for more information
using KeepMyPeople.Data;
using KeepMyPepole.Constants;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;

async Task<string> SetTokenAsync(string id)
{
    var httpClient = new HttpClient
    {
        BaseAddress = Constants.ACTION_URL
    };
    HttpResponseMessage request = null;
    try
    {
        request = await httpClient.PostAsJsonAsync("/Token", id);
        return await request.Content.ReadAsStringAsync();
    }
    catch (Exception)
    {
        await Task.Delay(TimeSpan.FromSeconds(5));
        SetTokenAsync(id);
    }
    return null;
}


var mafyouId = "21C54AD1-560C-4560-9FE1-FAB703896AD3".ToLower();
var broId = "21C54AD1-560C-4560-9FE1-FAB703896AD2".ToLower();

await Task.Delay(TimeSpan.FromSeconds(5));

var token = await SetTokenAsync(broId);
if (token is null)
{
    Console.WriteLine("Token is null");
    return;
}
var hub = new HubConnectionBuilder()
           .WithUrl($"{Constants.ACTION_URL}{Constants.Hub_Name}", o =>
           {
               o.AccessTokenProvider = () => Task.FromResult<string?>(token);
           })
           .WithAutomaticReconnect()
           .Build();
var chatSettings = new ChatSettings
{
    Me = new AirUser
    {
        Id = broId,
        Pseudo = "Bro"
    },
    MeKey = Guid.NewGuid().ToString(),
    Other = new AirUser
    {
        Id = mafyouId,
        Pseudo = "Mafyou"
    },
    OtherKey = Guid.NewGuid().ToString()
};
hub.On<ReceivingMessage>(ChatHubStatus.ReceivingMessage, (rm) =>
{
    Console.WriteLine($"{chatSettings.Me.Pseudo} : {rm.Message}");
});
hub.On<SentMessage>(ChatHubStatus.SentMessage, (sm) =>
{
    Console.WriteLine($"Message bien envoyé à {sm.To.Pseudo} !");
});
hub.On<string, string>(ChatHubStatus.Connected, async (userId, connectionId) =>
{
    Console.WriteLine("Connected : " + userId);
    Console.WriteLine("under Me : " + chatSettings.Me.Id);
    Console.WriteLine("With the connection id : " + connectionId);
    chatSettings.Me.Id = userId;
    chatSettings.ConnectionId = connectionId;
    await hub.SendAsync(ChatHubStatus.Initializing, chatSettings);
});
hub.On<Initialization>(ChatHubStatus.Initialized, async (init) =>
{
    Console.WriteLine("Initializing");
    Console.WriteLine($"ChatHubStatus.Initialized: me => {chatSettings.Me?.Pseudo} other => {chatSettings.Other?.Pseudo}");
});
async Task newPrompt()
{
    Console.Write($"{chatSettings.Me.Pseudo} : ");
    var line = Console.ReadLine();
    await hub.SendAsync(ChatHubStatus.SendMessage, chatSettings, new SendMessage(line));
}

try
{
    await hub.StartAsync();
    while (true)
    {
        await newPrompt();
    }
}
catch (Exception ex)
{
    throw;
}