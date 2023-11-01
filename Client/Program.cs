using System.Net.Sockets;
using System.Text;

class Client {
  static readonly string addr = "127.0.0.1";
  static readonly int port = 8080;

  static void Main(string[] args) {
    try {
      TcpClient client = new TcpClient(addr, port);
      Console.WriteLine($"[CONNECTED ON {addr}:{port}]. Start Typing:");

      NetworkStream stream = client.GetStream();

      // Thread to receive messages from the server
      Thread receiveThread = new Thread(() => ReceiveMessages(stream));
      receiveThread.Start();

      while (true) {
        string? message = Console.ReadLine();
        if (message != null) {
          byte[] data = Encoding.ASCII.GetBytes(message);
          stream.Write(data, 0, data.Length);
        }
      }
    } catch (Exception err) {
      Console.WriteLine($"[ERROR]: {err.Message}");
    }
  }

  static void ReceiveMessages(NetworkStream stream) {
    byte[] buffer = new byte[1024];
    int bytesRead;
    while (true) {
      try {
        bytesRead = stream.Read(buffer, 0, buffer.Length);
        if (bytesRead > 0) {
          string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
          Console.WriteLine(message);
        }
      } catch (Exception err) {
        Console.WriteLine($"[ERROR]: {err.Message}");
        break;
      }
    }
  }
}
