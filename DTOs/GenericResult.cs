using System;

namespace BioLis_30i.DTOs
{
    public record GenericResult
    {
        public string MessageId { get; init; }
        public string TestCode { get; init; }
        public string TestName { get; init; }
        public string Result { get; init; }
        public string Units { get; init; }
        public string ReferenceRange { get; init; }
        public DateTime? ObservationDateTime { get; init; }
        public string ResultStatus { get; init; }
    }
} 