using System;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public class ValidationEvent
    {
        public string ValidationModalMessage
        {
            get; set;
        }

        public string ValidationAlertMessage
        {
            get; set;
        }

        internal List<ValidationMessage> _validationMessages = new List<ValidationMessage>();

        public List<ValidationMessage> validationMessages
        {
            get
            {
                if (_validationMessages != null)
                {
                    return _validationMessages;
                }
                else
                {
                    _validationMessages = new List<ValidationMessage>();

                    return _validationMessages;
                }
            }
        }

        public void Add(ValidationMessage v)
        {
            if (this.validationStatus == ValidationStatus.Success && v.validationStatus == ValidationStatus.Error)
            {
                this._validationStatus = ValidationStatus.Error;
            }

            validationMessages.Add(v);
        }

        internal ValidationStatus _validationStatus;

        public ValidationStatus validationStatus
        {
            get
            {
                return _validationStatus;
            }
            set
            {
                _validationStatus = value;
            }
        }

        public ValidationEvent(string modalMessage, string alertMessage)
        {
            this.ValidationModalMessage = modalMessage;
            this.ValidationAlertMessage = alertMessage;
        }
    }

    public class ValidationMessage
    {
        public string HTMLID
        {
            get; set;
        }

        public string Message
        {
            get; set;
        }

        public ValidationStatus validationStatus
        {
            get; set;
        }

        public ValidationMessage(string html, string message, ValidationStatus status)
        {
            this.HTMLID = html;
            this.Message = message;
            this.validationStatus = status;
        }
    }

    public enum ValidationStatus
    {
        Success = 0,
        Error = 1
    }
}
