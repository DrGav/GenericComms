# BioLis-30i HL7 Listener

A Windows application for receiving and processing HL7 messages from BioLis 30i analyzers. The application provides a user-friendly interface for monitoring and displaying test results in real-time.

## Features

- Real-time HL7 message reception
- Support for OUL (Observation Result) message types
- Modern dark-themed user interface
- Live console logging
- Results display in a data grid
- Configurable IP and port settings
- Clear console and results functionality
- Status monitoring

## Requirements

- Windows operating system
- .NET 6.0 or later
- Network connectivity for HL7 message reception

## Installation

1. Download the latest release from the releases page
2. Extract the files to your desired location
3. Run `BioLis-30i.exe`

## Usage

### Starting the Application

1. Launch `BioLis-30i.exe`
2. The application will open with the main interface showing:
   - Control panel at the top
   - Console log in the middle
   - Results grid at the bottom
   - Status bar at the very bottom

### Configuration

1. The default settings are:
   - IP Address: 127.0.0.1 (localhost)
   - Port: 50001
2. You can modify these settings in the top panel before starting the listener

### Operating the Application

1. Click "Start Listening" to begin receiving HL7 messages
2. The status bar will show the current listening state
3. Received messages will be:
   - Displayed in the console log
   - Parsed and shown in the results grid
   - Automatically acknowledged
4. Use "Clear All" to reset the console and results grid
5. Click "Stop Listening" to stop receiving messages

### Message Processing

The application processes OUL (Observation Result) messages and displays:
- Message ID
- Test Code
- Test Name
- Result
- Units
- Reference Range
- Observation Date/Time
- Result Status

## Console Mode

The application can also run in console mode for automated processing:

```bash
BioLis-30i.exe --console
```

Console mode provides the same functionality without the graphical interface, suitable for:
- Server environments
- Automated processing
- Integration with other systems

## Troubleshooting

Common issues and solutions:

1. **Connection Issues**
   - Verify the IP and port settings
   - Ensure no firewall is blocking the connection
   - Check if the analyzer is properly configured to send messages

2. **Message Processing Errors**
   - Check the console log for error messages
   - Verify the message format is correct
   - Ensure the analyzer is sending OUL message type

3. **Application Not Responding**
   - Check if the port is already in use
   - Verify network connectivity
   - Restart the application if needed

## Support

For technical support or questions, please contact:
- Email: support@example.com
- Phone: (555) 123-4567

## License

This software is proprietary and confidential. Unauthorized copying, distribution, or use is strictly prohibited.

## Version History

- 1.0.0
  - Initial release
  - Basic HL7 message processing
  - GUI interface
  - Console mode support 