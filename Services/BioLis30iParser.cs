using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BioLis_30i.DTOs;

namespace BioLis_30i.Services
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
                if (segmentName == "\vMSH")
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

        public OULMessageDTO MapToOLUDTO(ParsedMessage parsedMessage)
        {
            if (parsedMessage.MessageType != "OUL")
            {
                throw new ArgumentException("Message is not an OUL message type");
            }

            var oluDto = new OULMessageDTO();

            // Map MSH segment
            if (parsedMessage.Segments.TryGetValue("\vMSH", out var mshSegments) && mshSegments.Any())
            {
                var mshFields = mshSegments[0].Split(FieldSeparator);
                if (mshFields.Length > 9)
                {
                    var messageType = mshFields[8].Split(ComponentSeparator);
                    oluDto = oluDto with
                    {
                        MessageId = mshFields[9],
                        SendingApplication = mshFields[2],
                        SendingFacility = mshFields[3],
                        ReceivingApplication = mshFields[4],
                        ReceivingFacility = mshFields[5],
                        MessageDateTime = parsedMessage.MessageDateTime,
                        MessageType = messageType[0],
                        MessageTrigger = messageType.Length > 1 ? messageType[1] : null
                    };
                }
            }

            // Map PID segment
            if (parsedMessage.Segments.TryGetValue("PID", out var pidSegments) && pidSegments.Any())
            {
                var pidFields = pidSegments[0].Split(FieldSeparator);
                if (pidFields.Length > 13)
                {
                    var patientId = pidFields[3].Split(ComponentSeparator);
                    var patientName = pidFields[5].Split(ComponentSeparator);

                    oluDto = oluDto with
                    {
                        PatientId = patientId[0],
                        PatientIdType = patientId.Length > 3 ? patientId[3] : null,
                        PatientLastName = patientName[0],
                        PatientFirstName = patientName.Length > 1 ? patientName[1] : null,
                        PatientMiddleName = patientName.Length > 2 ? patientName[2] : null,
                        PatientSuffix = patientName.Length > 3 ? patientName[3] : null,
                        PatientPrefix = patientName.Length > 4 ? patientName[4] : null,
                        PatientDateOfBirth = DateTime.TryParse(pidFields[7], out var dob) ? dob : null,
                        PatientSex = pidFields[8],
                        PatientAddress = pidFields.Length > 11 ? pidFields[11] : null,
                        PatientPhoneNumber = pidFields.Length > 13 ? pidFields[13] : null
                    };
                }
            }

            // Map SPM segment
            if (parsedMessage.Segments.TryGetValue("SPM", out var spmSegments) && spmSegments.Any())
            {
                var spmFields = spmSegments[0].Split(FieldSeparator);
                if (spmFields.Length > 4)
                {
                    var specimenId = spmFields[2].Split(ComponentSeparator);
                    var specimenType = spmFields.Length > 4 ? spmFields[4].Split(ComponentSeparator) : null;
                    var specimenSourceSite = spmFields.Length > 6 ? spmFields[6].Split(ComponentSeparator) : null;
                    var specimenCollectionSite = spmFields.Length > 8 ? spmFields[8].Split(ComponentSeparator) : null;
                    var specimenCollectionDevice = spmFields.Length > 20 ? spmFields[20].Split(ComponentSeparator) : null;
                    var specimenCollectionDeviceContainer = spmFields.Length > 24 ? spmFields[24].Split(ComponentSeparator) : null;

                    oluDto = oluDto with
                    {
                        SpecimenId = specimenId[0],
                        SpecimenIdType = specimenId.Length > 3 ? specimenId[3] : null,
                        SpecimenType = specimenType?[0],
                        SpecimenTypeModifier = specimenType?.Length > 1 ? specimenType[1] : null,
                        SpecimenAdditives = spmFields.Length > 5 ? spmFields[5] : null,
                        SpecimenCollectionMethod = spmFields.Length > 7 ? spmFields[7] : null,
                        SpecimenSourceSite = specimenSourceSite?[0],
                        SpecimenSourceSiteModifier = specimenSourceSite?.Length > 1 ? specimenSourceSite[1] : null,
                        SpecimenCollectionSite = specimenCollectionSite?[0],
                        SpecimenCollectionSiteModifier = specimenCollectionSite?.Length > 1 ? specimenCollectionSite[1] : null,
                        SpecimenRole = spmFields.Length > 10 ? spmFields[10] : null,
                        SpecimenCollectionDateTime = spmFields.Length > 11 ? spmFields[11] : null,
                        SpecimenReceivedDateTime = spmFields.Length > 12 ? spmFields[12] : null,
                        SpecimenExpirationDateTime = spmFields.Length > 13 ? spmFields[13] : null,
                        SpecimenAvailability = spmFields.Length > 14 ? spmFields[14] : null,
                        SpecimenRejectReason = spmFields.Length > 15 ? spmFields[15] : null,
                        SpecimenQuality = spmFields.Length > 16 ? spmFields[16] : null,
                        SpecimenAppropriateness = spmFields.Length > 17 ? spmFields[17] : null,
                        SpecimenCondition = spmFields.Length > 18 ? spmFields[18] : null,
                        SpecimenQuantity = spmFields.Length > 19 ? spmFields[19] : null,
                        SpecimenQuantityUnits = spmFields.Length > 20 ? spmFields[20] : null,
                        SpecimenCollectionDevice = specimenCollectionDevice?[0],
                        SpecimenCollectionDeviceType = specimenCollectionDevice?.Length > 1 ? specimenCollectionDevice[1] : null,
                        SpecimenCollectionDeviceIdentifier = specimenCollectionDevice?.Length > 2 ? specimenCollectionDevice[2] : null,
                        SpecimenCollectionDeviceVolume = spmFields.Length > 22 ? spmFields[22] : null,
                        SpecimenCollectionDeviceVolumeUnits = spmFields.Length > 23 ? spmFields[23] : null,
                        SpecimenCollectionDeviceContainer = specimenCollectionDeviceContainer?[0],
                        SpecimenCollectionDeviceContainerType = specimenCollectionDeviceContainer?.Length > 1 ? specimenCollectionDeviceContainer[1] : null,
                        SpecimenCollectionDeviceContainerIdentifier = specimenCollectionDeviceContainer?.Length > 2 ? specimenCollectionDeviceContainer[2] : null,
                        SpecimenCollectionDeviceContainerVolume = spmFields.Length > 26 ? spmFields[26] : null,
                        SpecimenCollectionDeviceContainerVolumeUnits = spmFields.Length > 27 ? spmFields[27] : null,
                        SpecimenCollectionDeviceContainerColor = spmFields.Length > 28 ? spmFields[28] : null,
                        SpecimenCollectionDeviceContainerCapColor = spmFields.Length > 29 ? spmFields[29] : null,
                        SpecimenCollectionDeviceContainerDescription = spmFields.Length > 30 ? spmFields[30] : null,
                        SpecimenCollectionDeviceContainerCapDescription = spmFields.Length > 31 ? spmFields[31] : null,
                        SpecimenCollectionDeviceContainerAdditive = spmFields.Length > 32 ? spmFields[32] : null,
                        SpecimenCollectionDeviceContainerPreparation = spmFields.Length > 33 ? spmFields[33] : null,
                        SpecimenCollectionDeviceContainerSpecialHandling = spmFields.Length > 34 ? spmFields[34] : null,
                        SpecimenCollectionDeviceContainerOtherSpecifications = spmFields.Length > 35 ? spmFields[35] : null
                    };
                }
            }

            // Map OBR segment
            if (parsedMessage.Segments.TryGetValue("OBR", out var obrSegments) && obrSegments.Any())
            {
                var obrFields = obrSegments[0].Split(FieldSeparator);
                if (obrFields.Length > 4)
                {
                    var universalServiceId = obrFields[4].Split(ComponentSeparator);
                    oluDto = oluDto with
                    {
                        PlacerOrderNumber = obrFields[2],
                        FillerOrderNumber = obrFields[3],
                        UniversalServiceId = universalServiceId[0],
                        UniversalServiceIdType = universalServiceId.Length > 1 ? universalServiceId[1] : null,
                        ObservationDateTime = DateTime.TryParse(obrFields[7], out var obsDateTime) ? obsDateTime : null,
                        ObservationEndDateTime = obrFields.Length > 8 ? obrFields[8] : null,
                        CollectionVolume = obrFields.Length > 9 ? obrFields[9] : null,
                        CollectorIdentifier = obrFields.Length > 10 ? obrFields[10] : null,
                        SpecimenActionCode = obrFields.Length > 11 ? obrFields[11] : null,
                        DangerCode = obrFields.Length > 12 ? obrFields[12] : null,
                        RelevantClinicalInfo = obrFields.Length > 13 ? obrFields[13] : null,
                        SpecimenSource = obrFields.Length > 15 ? obrFields[15] : null,
                        OrderingProvider = obrFields.Length > 16 ? obrFields[16] : null,
                        OrderCallbackPhoneNumber = obrFields.Length > 17 ? obrFields[17] : null,
                        ResultStatus = obrFields.Length > 25 ? obrFields[25] : null
                    };
                }
            }

            // Map OBX segments
            if (parsedMessage.Segments.TryGetValue("OBX", out var obxSegments))
            {
                var observationResults = new List<OULMessageDTO.ObservationResult>();
                foreach (var obxSegment in obxSegments)
                {
                    var obxFields = obxSegment.Split(FieldSeparator);
                    if (obxFields.Length > 5)
                    {
                        var observationId = obxFields[3].Split(ComponentSeparator);
                        var observationResult = new OULMessageDTO.ObservationResult
                        {
                            SetId = obxFields[1],
                            ValueType = obxFields[2],
                            ObservationIdentifier = observationId[0],
                            ObservationIdentifierText = observationId.Length > 1 ? observationId[1] : null,
                            ObservationSubId = obxFields.Length > 4 ? obxFields[4] : null,
                            ObservationValue = obxFields.Length > 5 ? obxFields[5] : null,
                            Units = obxFields.Length > 6 ? obxFields[6] : null,
                            ReferencesRanges = obxFields.Length > 7 ? obxFields[7] : null,
                            AbnormalFlags = obxFields.Length > 8 ? obxFields[8] : null,
                            Probability = obxFields.Length > 9 ? obxFields[9] : null,
                            NatureOfAbnormalTest = obxFields.Length > 10 ? obxFields[10] : null,
                            ObservationResultStatus = obxFields.Length > 11 ? obxFields[11] : null,
                            DateTimeOfTheObservation = obxFields.Length > 14 ? obxFields[14] : null
                        };
                        observationResults.Add(observationResult);
                    }
                }
                oluDto = oluDto with { ObservationResults = observationResults };
            }

            return oluDto;
        }
    }
}