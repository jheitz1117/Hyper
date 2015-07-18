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

        public string SMSString
        {
            get;
            internal set;
        }

        public bool SendAttempted
        {
            get
            {
                return (SendResult != null);
            }
        }

        public int SendAttempts
        {
            get;
            internal set;
        }

        public bool SendSuccess
        {
            get
            {
                return (SendAttempted && SendResult.RestException == null);
            }
        }

        public SMSMessage SendResult
        {
            get;
            internal set;
        }

        private string _detail = null;
        public string Detail
        {
            get
            {
                string detail = _detail;

                if (SendAttempted)
                {
                    if (SendSuccess)
                    {
                        detail = SendResult.Sid;
                    }
                    else
                    {
                        detail = SendResult.RestException.Message;
                    }
                }

                return detail;
            }
            internal set
            {
                _detail = value;
            }
        }

        #endregion Properties

        #region Public Methods

        public TwilioPartialSendResult()
        {
        }

        public TwilioPartialSendResult(int sendOrder, string smsString)
            : this()
        {
            SendOrder = sendOrder;
            SMSString = smsString;
        }

        #endregion Public Methods
    }
}
