using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BioLis_30i
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("BioLis-30i HL7 Listener");
            Console.WriteLine("======================");


            int port = 50001;
            var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            var parser = new BioLis30iParser();

            try
            {
                listener.Start();
                Console.WriteLine($"Listening for HL7 messages on port {port}...");
                Console.WriteLine("Press Ctrl+C to stop");

                using (var client = await listener.AcceptTcpClientAsync())
                {

                    var endpoint = client.Client.RemoteEndPoint as IPEndPoint;
                    Console.WriteLine($"\nConnection received from {endpoint}");

                    while (true)
                    {
                        var stream = client.GetStream();
                        var buffer = new byte[4096];
                        var messageBuilder = new StringBuilder();

                        int bytesRead;
                        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            var chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            messageBuilder.Append(chunk);

                            // Check for message terminator
                            if (chunk.Contains("\u001C"))
                            {
                                break;
                            }
                        }

                        var message = messageBuilder.ToString();
                        Console.WriteLine("\nReceived message:");
                        Console.WriteLine(message);

                        // Parse the message and create acknowledgment
                        var parsedMessage = parser.ParseMessage(message);
                        var ack = parser.CreateAcknowledgment(parsedMessage.MessageId);

                        // Send acknowledgment
                        var ackBytes = Encoding.UTF8.GetBytes(ack);
                        await stream.WriteAsync(ackBytes, 0, ackBytes.Length);

                        Console.WriteLine("\nSent acknowledgment:");
                        Console.WriteLine(ack);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
