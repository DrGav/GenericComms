# BioLis-30i HL7 Listener

A Windows application for receiving and processing HL7 messages from BioLis 30i analyzers. The application provides a user-friendly interface for monitoring and displaying test results in real-time.

![Application Screenshot](screenshots/main-window.png)

## Features

- Real-time HL7 message reception
- Support for OUL (Observation Result) message types
- Modern dark-themed user interface
- Live console logging
- Results display in a data grid
- Configurable IP and port settings
- Clear console and results functionality
- Status monitoring
- CSV export functionality

## Customizing Result Mapping

The application uses a generic result mapping system that allows you to customize which HL7 OUL properties are mapped to the final results. This is done through the `GenericResultMappingService` class.

### Available OUL Properties

The following OUL properties can be mapped to the generic results:

```csharp
public class OLUDTO
{
    public string MessageId { get; set; }
    public string PatientId { get; set; }
    public string PatientName { get; set; }
    public string OrderNumber { get; set; }
    public string SpecimenId { get; set; }
    public string SpecimenType { get; set; }
    public string CollectionDateTime { get; set; }
    public string TestCode { get; set; }
    public string TestName { get; set; }
    public string Result { get; set; }
    public string Units { get; set; }
    public string ReferenceRange { get; set; }
    public string ObservationDateTime { get; set; }
    public string ResultStatus { get; set; }
    public string Comments { get; set; }
}
```

### Customizing the Mapping

To customize which properties are mapped:

1. Open `Services/GenericResultMappingService.cs`
2. Locate the `MapToGenericResult` method
3. Modify the property assignments to include or exclude desired fields
4. Update the DataGridView columns in `MainForm.cs` to match your changes

Example of customizing the mapping:

```csharp
public GenericResult MapToGenericResult(OLUDTO oluMessage)
{
    return new GenericResult
    {
        // Include only the properties you want
        MessageId = oluMessage.MessageId,
        TestCode = oluMessage.TestCode,
        TestName = oluMessage.TestName,
        Result = oluMessage.Result,
        Units = oluMessage.Units,
        ReferenceRange = oluMessage.ReferenceRange,
        ObservationDateTime = ParseDateTime(oluMessage.ObservationDateTime),
        ResultStatus = oluMessage.ResultStatus
    };
}
```

![Mapping Configuration](screenshots/mapping-config.png)

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

![Application Layout](screenshots/layout.png)

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
5. Click "Export CSV" to save the current results to a CSV file
6. Click "Stop Listening" to stop receiving messages

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

![Results Display](screenshots/results.png)

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
  - CSV export functionality
  - Customizable result mapping 