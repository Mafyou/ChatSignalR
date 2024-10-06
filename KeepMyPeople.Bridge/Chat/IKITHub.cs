using KeepMyPeople.Data;

namespace KeepMyPeople.Bridge.Chat
{
    public interface IKITHub
    {
        Task OnConnectedAsync();
        Task Initializing(ChatSettings settings);
        Task SendMessage(ChatSettings settings, SendMessage sm);
    }
}