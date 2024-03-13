using BusinessLogicLayer.Interfaces;
using Newtonsoft.Json;

namespace BusinessLogicLayer;

public class SSEService : ISSEService
{
    private readonly List<StreamWriter> Clients = new List<StreamWriter>();
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
    private static readonly Dictionary<string, StreamWriter> ClientConnections = new Dictionary<string, StreamWriter>();
    public static async Task SendNotification(string userId, object data)
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
                    // Handle exception or client disconnect
                    // For simplicity, you can remove the client from the dictionary
                    ClientConnections.Remove(userId);
                }
            }
        }
    }
}