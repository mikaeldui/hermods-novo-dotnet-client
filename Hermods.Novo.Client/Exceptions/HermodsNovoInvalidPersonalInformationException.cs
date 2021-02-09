using System;
using System.Runtime.Serialization;

namespace Hermods.Novo
{
    [Serializable]
    internal class HermodsNovoInvalidPersonalInformationException : Exception
    {
        public HermodsNovoPersonalInformation PersonalInformation { get; }

        public HermodsNovoInvalidPersonalInformationException(string message, HermodsNovoPersonalInformation personalInformation) : base(message)
        {
            PersonalInformation = personalInformation;
        }

        public HermodsNovoInvalidPersonalInformationException(string message, HermodsNovoPersonalInformation personalInformation, Exception innerException) : base(message, innerException)
        {
            PersonalInformation = personalInformation;
        }
    }
}