using Twilio;

namespace Hyper.Messaging.Twilio
{
    public class TwilioPartialSendResult
    {
        #region Properties

        public int SendOrder
        {
            get;
            set;
        }

        public string SmsString
        {
            get;
            internal set;
        }

        public bool SendAttempted => SendResult != null;

        public int SendAttempts
        {
            get;
            internal set;
        }

        public bool SendSuccess => SendAttempted && SendResult.RestException == null;

        public SMSMessage SendResult
        {
            get;
            internal set;
        }

        private string _detail;
        public string Detail
        {
            get
            {
                var detail = _detail;

                if (SendAttempted)
                    detail = SendSuccess ? SendResult.Sid : SendResult.RestException.Message;

                return detail;
            }
            internal set
            {
                _detail = value;
            }
        }

        #endregion Properties

        #region Public Methods

        public TwilioPartialSendResult() { }

        public TwilioPartialSendResult(int sendOrder, string smsString)
            : this()
        {
            SendOrder = sendOrder;
            SmsString = smsString;
        }

        #endregion Public Methods
    }
}
