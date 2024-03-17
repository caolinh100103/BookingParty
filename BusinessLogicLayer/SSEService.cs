using BusinessLogicLayer.Interfaces;
using Newtonsoft.Json;

namespace BusinessLogicLayer;

public class SSEService : ISSEService
{
    private readonly List<StreamWriter> Clients = new List<StreamWriter>();
    private readonly Dictionary<int, StreamWriter> _connections = new Dictionary<int, StreamWriter>();

    public async Task SendNotification(string message)
    {
        lock (Clients)
        {
            foreach (var client in Clients)
            {
                try
                {
                    client.Write("data: " + message + "\n\n");
                    client.Flush();
                }
                catch (Exception ex)
                {
                    Clients.Remove(client);
                }
            }
        }
    }
    private readonly Dictionary<string, StreamWriter> ClientConnections = new Dictionary<string, StreamWriter>();
    public async Task SendNotification(string userId, object data)
    {
        StreamWriter writer;
        lock (ClientConnections)
        {
            if (ClientConnections.TryGetValue(userId, out writer))
            {
                try
                {
                    string jsonData = JsonConvert.SerializeObject(data);
                    writer.Write("data: " + jsonData + "\n\n");
                    writer.Flush();
                }
                catch (Exception ex)
                {
                    ClientConnections.Remove(userId);
                }
            }
        }
    }
    
    public void AddConnection(int userId, StreamWriter streamWriter)
    {
        _connections[userId] = streamWriter;
    }

    public void RemoveConnection(int userId)
    {
        _connections.Remove(userId);
    }
    public async Task SendNotificationToUserAsync(int userId, string notificationMessage)
    {
        if (_connections.TryGetValue(userId, out var responseStreamWriter))
        {
            await responseStreamWriter.WriteLineAsync($"data: {notificationMessage}\n\n");
            await responseStreamWriter.FlushAsync();
        }
    }
}