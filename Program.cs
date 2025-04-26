using System;
using System.Windows.Forms;
using BioLis_30i.Forms;
using BioLis_30i.DTOs;
using BioLis_30i.Services;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace BioLis_30i
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Check if console mode is requested
            if (args.Length > 0 && args[0].ToLower() == "--console")
            {
                // Run in console mode
                ProcessMessagesInConsoleMode(args).GetAwaiter().GetResult();
            }
            else
            {
                // Run in GUI mode
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }

        /// <summary>
        /// Command-line mode for processing HL7 messages without the UI
        /// </summary>
        static async Task ProcessMessagesInConsoleMode(string[] args)
        {
            Console.WriteLine("BioLis-30i HL7 Listener (Console Mode)");
            Console.WriteLine("=====================================");

            int port = 50001;
            var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            var parser = new BioLis30iParser();
            var genericParser = new GenericResultMappingService();

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
                        var oluMessage = parser.MapToOLUDTO(parsedMessage);
                        var genericResults = genericParser.MapToGenericResult(oluMessage);

                        // Display results in a table format
                        Console.WriteLine("\nTest Results:");
                        Console.WriteLine("----------------------------------------");
                        Console.WriteLine($"| Message ID | Test Code | Test Name | Result | Units | Reference Range |");
                        Console.WriteLine("----------------------------------------");
                        
                        foreach (var result in genericResults)
                        {
                            Console.WriteLine($"| {result.MessageId} | {result.TestCode} | {result.TestName} | {result.Result} | {result.Units} | {result.ReferenceRange} |");
                        }
                        
                        Console.WriteLine("----------------------------------------");

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
