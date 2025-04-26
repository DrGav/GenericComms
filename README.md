# BioLis-30i HL7 Listener

A .NET Core application that implements an HL7 message listener for the BioLis-30i system. This application listens for incoming HL7 messages on a specified port, parses them, and sends acknowledgments back to the sender.

## Features

- HL7 message listening on configurable port (default: 50001)
- HL7 message parsing with support for:
  - Message header (MSH) segment parsing
  - Message type identification
  - Sending/Receiving application details
  - Message timestamps
- Automatic acknowledgment generation
- Console-based logging of received messages and sent acknowledgments

## Requirements

- .NET Core 6.0 or later
- Windows operating system

## Configuration

The application is configured to listen on:
- IP Address: 127.0.0.1 (localhost)
- Port: 50001

## Usage

1. Build the application:
```bash
dotnet build
```

2. Run the application:
```bash
dotnet run
```

3. The application will start listening for HL7 messages on the configured port.

4. To stop the application, press Ctrl+C.

## Message Format

The application expects HL7 messages in the standard format with segments separated by carriage returns. Example:

```
MSH|^~\\&|SENDING_APP|SENDING_FAC|RECEIVING_APP|RECEIVING_FAC|20230424120000||ORM^O01|MSG00001|P|2.5
PID|1||12345^^^MRN||DOE^JOHN||19700101|M
...
```

## Acknowledgment Format

The application generates acknowledgments in the following format:

```
MSH|^~\\&|BioLis|30i|Sender|Facility|{timestamp}||ACK^A01|{guid}|P|2.5
MSA|AA|{original_message_id}|Message processed successfully
```

## License

This project is licensed under the MIT License - see the LICENSE file for details. 