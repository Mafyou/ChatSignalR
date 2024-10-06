# ChatSignalR
How to make a chat with SignalR

# The Problem

Then try to run API then MeConsole and OtherConsole. Multi configuration as startup works as well.

The problem is this code:
```csharp
hub.On<ChatSettings, ReceivingMessage>(ChatHubStatus.ReceivingMessage, (settings, rm) =>
{
    Console.WriteLine($"{chatSettings.Me.Pseudo} : {rm.Message}");
});
hub.On<ChatSettings, SentMessage>(ChatHubStatus.SentMessage, (settings, sm) =>
{
    Console.WriteLine($"Message bien envoyé à {sm.To.Pseudo} !");
});
```

This code works  when a message is received and confirm that a message has been sent.
The problem? They aren't called.

When you ask as this :
```csharp
await hub.SendAsync(ChatHubStatus.SendMessage, chatSettings, new SendMessage("Yo from Mafyou")); // Test direcly in sending
```
It goes to the API and then :
```csharp
[HubMethodName("ReceivingMessage")]
public async Task ReceivingMessage(ChatSettings settings, SendMessage sm)
{
    Console.WriteLine("ReceivingMessage de " + settings.Me.Pseudo + " to other " + settings.Other.Pseudo);
    await hubContext.Clients.User(settings.Other.Id.ToString()).SendAsync(ChatHubStatus.ReceivingMessage, settings, new ReceivingMessage(sm.Message));
    Console.WriteLine("Sendessage ACK " + settings.Me.Pseudo);
    await hubContext.Clients.User(settings.Me.Id.ToString()).SendAsync(ChatHubStatus.ReceivingMessage, settings, new ReceivingMessage(sm.Message));
}
```

But then, nothing. The RevievingMessage for Mafyou or Bro isn't called.