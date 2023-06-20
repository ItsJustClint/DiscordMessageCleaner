
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace DMCBot;

public class Program
{
    // Link to use in order to add this bot to a server with admin privileges 
    // https://discord.com/api/oauth2/authorize?client_id=1120133051746353223&permissions=8&scope=bot
    
    private readonly DiscordSocketClient _discordSocketClient = new();
    
    private const ulong DiabloWorldTrackerChannelId = 1116911382013673522;
    private const string DeletedMessageText = "[Original Message Deleted]";
    
    public static Task Main() => new Program().MainAsync();

    private async Task MainAsync()
    {
        var token = "";

        await _discordSocketClient.LoginAsync(TokenType.Bot, token);
        await _discordSocketClient.StartAsync();

        _discordSocketClient.MessageUpdated += MessageUpdated;
        
        await CleanUpDeletedMessagesByChannelId(DiabloWorldTrackerChannelId);
        
        // Block this task until the program is closed.
        await Task.Delay(-1);
    }

    private async Task MessageUpdated(Cacheable<IMessage, ulong> message, SocketMessage arg2, ISocketMessageChannel channel)
    {
        if (channel.Id == DiabloWorldTrackerChannelId)
        {
            var messageAfter = await message.GetOrDownloadAsync();
            
            if (messageAfter.Content == DeletedMessageText)
            {
                await channel.DeleteMessageAsync(messageAfter.Id);                
            }
        }
    }

    private async Task CleanUpDeletedMessagesByChannelId(ulong channelId)
    {
        var channel = await _discordSocketClient.GetChannelAsync(channelId);

        await CleanUpDeletedMessages((IMessageChannel)channel);
        
        if (channel is ISocketMessageChannel msgChannel)
        {
            await CleanUpDeletedMessages(msgChannel);
        }
    }

    private async Task CleanUpDeletedMessages(IMessageChannel msgChannel)
    {
        var pages = msgChannel.GetMessagesAsync(999).ToListAsync().Result;

        if (pages.Any())
        {
            foreach (var messagePage in pages)
            {
                foreach (var message in messagePage)
                {
                    var messageToCheck = (RestUserMessage)msgChannel.GetMessageAsync(message.Id).Result;

                    if (messageToCheck.Content == DeletedMessageText)
                    {
                        await msgChannel.DeleteMessageAsync(message.Id);
                    }
                }
            }
        }
    }
}