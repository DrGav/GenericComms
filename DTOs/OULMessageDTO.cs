using System;
using System.Collections.Generic;

namespace BioLis_30i.DTOs
{
    public record OULMessageDTO
    {
        // MSH Segment
        public string MessageId { get; init; }
        public string SendingApplication { get; init; }
        public string SendingFacility { get; init; }
        public string ReceivingApplication { get; init; }
        public string ReceivingFacility { get; init; }
        public DateTime? MessageDateTime { get; init; }
        public string MessageType { get; init; }
        public string MessageTrigger { get; init; }

        // PID Segment
        public string PatientId { get; init; }
        public string PatientIdType { get; init; }
        public string PatientLastName { get; init; }
        public string PatientFirstName { get; init; }
        public string PatientMiddleName { get; init; }
        public string PatientSuffix { get; init; }
        public string PatientPrefix { get; init; }
        public DateTime? PatientDateOfBirth { get; init; }
        public string PatientSex { get; init; }
        public string PatientAddress { get; init; }
        public string PatientPhoneNumber { get; init; }

        // SPM Segment
        public string SpecimenId { get; init; }
        public string SpecimenIdType { get; init; }
        public string SpecimenType { get; init; }
        public string SpecimenTypeModifier { get; init; }
        public string SpecimenAdditives { get; init; }
        public string SpecimenCollectionMethod { get; init; }
        public string SpecimenSourceSite { get; init; }
        public string SpecimenSourceSiteModifier { get; init; }
        public string SpecimenCollectionSite { get; init; }
        public string SpecimenCollectionSiteModifier { get; init; }
        public string SpecimenRole { get; init; }
        public string SpecimenCollectionDateTime { get; init; }
        public string SpecimenReceivedDateTime { get; init; }
        public string SpecimenExpirationDateTime { get; init; }
        public string SpecimenAvailability { get; init; }
        public string SpecimenRejectReason { get; init; }
        public string SpecimenQuality { get; init; }
        public string SpecimenAppropriateness { get; init; }
        public string SpecimenCondition { get; init; }
        public string SpecimenQuantity { get; init; }
        public string SpecimenQuantityUnits { get; init; }
        public string SpecimenCollectionDevice { get; init; }
        public string SpecimenCollectionDeviceType { get; init; }
        public string SpecimenCollectionDeviceIdentifier { get; init; }
        public string SpecimenCollectionDeviceVolume { get; init; }
        public string SpecimenCollectionDeviceVolumeUnits { get; init; }
        public string SpecimenCollectionDeviceContainer { get; init; }
        public string SpecimenCollectionDeviceContainerType { get; init; }
        public string SpecimenCollectionDeviceContainerIdentifier { get; init; }
        public string SpecimenCollectionDeviceContainerVolume { get; init; }
        public string SpecimenCollectionDeviceContainerVolumeUnits { get; init; }
        public string SpecimenCollectionDeviceContainerColor { get; init; }
        public string SpecimenCollectionDeviceContainerCapColor { get; init; }
        public string SpecimenCollectionDeviceContainerDescription { get; init; }
        public string SpecimenCollectionDeviceContainerCapDescription { get; init; }
        public string SpecimenCollectionDeviceContainerAdditive { get; init; }
        public string SpecimenCollectionDeviceContainerPreparation { get; init; }
        public string SpecimenCollectionDeviceContainerSpecialHandling { get; init; }
        public string SpecimenCollectionDeviceContainerOtherSpecifications { get; init; }

        // OBR Segment
        public string PlacerOrderNumber { get; init; }
        public string FillerOrderNumber { get; init; }
        public string UniversalServiceId { get; init; }
        public string UniversalServiceIdType { get; init; }
        public DateTime? ObservationDateTime { get; init; }
        public string ObservationEndDateTime { get; init; }
        public string CollectionVolume { get; init; }
        public string CollectorIdentifier { get; init; }
        public string SpecimenActionCode { get; init; }
        public string DangerCode { get; init; }
        public string RelevantClinicalInfo { get; init; }
        public string SpecimenSource { get; init; }
        public string OrderingProvider { get; init; }
        public string OrderCallbackPhoneNumber { get; init; }
        public string ResultStatus { get; init; }

        // OBX Segment
        public List<ObservationResult> ObservationResults { get; init; } = new();

        public record ObservationResult
        {
            public string SetId { get; init; }
            public string ValueType { get; init; }
            public string ObservationIdentifier { get; init; }
            public string ObservationIdentifierText { get; init; }
            public string ObservationSubId { get; init; }
            public string ObservationValue { get; init; }
            public string Units { get; init; }
            public string ReferencesRanges { get; init; }
            public string AbnormalFlags { get; init; }
            public string Probability { get; init; }
            public string NatureOfAbnormalTest { get; init; }
            public string ObservationResultStatus { get; init; }
            public string DateTimeOfTheObservation { get; init; }
        }
    }
} 