using System.Net;
using System.Net.Sockets;
using System.Text;

class Server {
  static TcpListener? listener;
  static readonly string addr = "127.0.0.1";
  static readonly int port = 8080;

  static void Main(string[] args) {
    listener = new TcpListener(IPAddress.Parse(addr), port);
    listener.Start();
    Console.WriteLine($"[SERVER STARTED ON PORT]: {port}");

    while (true) {
      TcpClient client = listener.AcceptTcpClient();
      Console.WriteLine($"[CLIENT CONNECTED]: {client.Client.RemoteEndPoint}");
      Clients.ClientsList.Add(client);

      // Thread to handle an messages from a client
      Thread clientThread = new Thread(() => HandleClient(client));
      clientThread.Start();
    }
  }

  static void HandleClient(TcpClient client) {

    NetworkStream stream = client.GetStream();
    byte[] buffer = new byte[1024];
    int bytesRead;

    while (true) {
      try {
        bytesRead = stream.Read(buffer, 0, buffer.Length);
        if (bytesRead == 0) break; 

        string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        Console.WriteLine($"[RECEIVED FROM CLIENT]: {message}");

        // Sends message to all connected clients
        Broadcast(message, client);
      }
      catch (Exception err) {
        Console.WriteLine($"[ERROR]: {err.Message}");
        break;
      }
    }

    Console.WriteLine($"[CLIENT DISCONNECTED]: {client.Client.RemoteEndPoint}");
    client.Close();
  }

  static void Broadcast(string message, TcpClient sender) {
    foreach (var client in Clients.ClientsList) {
      if (client != sender) {
        NetworkStream stream = client.GetStream();
        byte[] data = Encoding.ASCII.GetBytes($"{client.Client.RemoteEndPoint}: {message}");
        stream.Write(data, 0, data.Length);
      }
    }
  }
}

class Clients {
  public static List<TcpClient> ClientsList = new List<TcpClient>();
}
