using System;
using System.Linq;
using BioLis_30i.DTOs;

namespace BioLis_30i.Services
{
    public class GenericResultMappingService
    {
        public List<GenericResult> MapToGenericResult(OULMessageDTO oluMessage)
        {
            if (oluMessage == null)
            {
                throw new ArgumentNullException(nameof(oluMessage));
            }

            var result = new List<GenericResult>();

            foreach (var test in oluMessage.ObservationResults)
            {
                result.Add(new GenericResult
                {
                    MessageId = oluMessage.MessageId,
                    TestCode = test?.ObservationIdentifier,
                    TestName = test?.ObservationIdentifierText,
                    Result = test?.ObservationValue,
                    Units = test?.Units,
                    ReferenceRange = test?.ReferencesRanges,
                    ObservationDateTime = oluMessage.ObservationDateTime,
                    ResultStatus = oluMessage.ResultStatus
                });
            }

            return result;
        }
    }
} 