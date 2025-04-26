using System;
using System.Collections.Generic;
using System.Text;

namespace BioLis_30i
{
    public class BioLis30iParser
    {
        private const char FieldSeparator = '|';
        private const char ComponentSeparator = '^';
        private const char SubcomponentSeparator = '&';
        private const char RepetitionSeparator = '~';
        private const char EscapeCharacter = '\\';

        public class ParsedMessage
        {
            public string MessageType { get; set; }
            public string MessageId { get; set; }
            public string SendingApplication { get; set; }
            public string SendingFacility { get; set; }
            public string ReceivingApplication { get; set; }
            public string ReceivingFacility { get; set; }
            public DateTime MessageDateTime { get; set; }
            public Dictionary<string, List<string>> Segments { get; set; }

            public ParsedMessage()
            {
                Segments = new Dictionary<string, List<string>>();
            }
        }

        public ParsedMessage ParseMessage(string message)
        {
            var parsedMessage = new ParsedMessage();
            var lines = message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var fields = line.Split(FieldSeparator);
                if (fields.Length < 2) continue;

                var segmentName = fields[0];
                if (!parsedMessage.Segments.ContainsKey(segmentName))
                {
                    parsedMessage.Segments[segmentName] = new List<string>();
                }
                parsedMessage.Segments[segmentName].Add(line);

                // Parse MSH segment
                if (segmentName.Contains("MSH"))
                {
                    if (fields.Length > 8)
                    {
                        parsedMessage.SendingApplication = fields[2];
                        parsedMessage.SendingFacility = fields[3];
                        parsedMessage.ReceivingApplication = fields[4];
                        parsedMessage.ReceivingFacility = fields[5];
                        
                        if (DateTime.TryParse(fields[6], out DateTime messageDateTime))
                        {
                            parsedMessage.MessageDateTime = messageDateTime;
                        }

                        var messageType = fields[8].Split(ComponentSeparator);
                        if (messageType.Length > 0)
                        {
                            parsedMessage.MessageType = messageType[0];
                        }

                        parsedMessage.MessageId = fields[9];
                    }
                }
            }

            return parsedMessage;
        }

        public string CreateAcknowledgment(string originalMessageId, string status = "AA")
        {
            var ack = new StringBuilder();
            ack.AppendLine($"MSH|^~\\&|BioLis|30i|Sender|Facility|{DateTime.Now:yyyyMMddHHmmss}||ACK^A01|{Guid.NewGuid()}|P|2.5");
            ack.AppendLine($"MSA|{status}|{originalMessageId}|Message processed successfully");
            return ack.ToString();
        }
    }
} 