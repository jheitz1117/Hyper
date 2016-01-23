using System.Collections.Generic;
using System.Linq;

namespace Hyper.Messaging.Twilio
{
    public class TwilioResult
    {
        #region Properties

        private List<TwilioPartialSendResult> _partialSendResults = new List<TwilioPartialSendResult>();
        public List<TwilioPartialSendResult> PartialSendResults
        {
            get
            {
                return _partialSendResults;
            }
            internal set
            {
                _partialSendResults = value ?? new List<TwilioPartialSendResult>();
            }
        }

        public TwilioSmsSendStatus SendStatus
        {
            get
            {
                TwilioSmsSendStatus sendStatus;

                if (!PartialSendResults.Any(x => x.SendAttempted))
                    sendStatus = TwilioSmsSendStatus.SendNotAttempted;
                else if (PartialSendResults.Count(x => x.SendSuccess) == PartialSendResults.Count)
                    sendStatus = TwilioSmsSendStatus.Success; // All pieces sent successfully
                else if (PartialSendResults.Count(x => !x.SendSuccess) == PartialSendResults.Count)
                    sendStatus = TwilioSmsSendStatus.Failure; // All pieces failed
                else
                    sendStatus = TwilioSmsSendStatus.PartialDelivery;

                return sendStatus;
            }
        }

        #endregion Properties
    }

    public enum TwilioSmsSendStatus
    {
        SendNotAttempted = 0,
        Success = 1,
        Failure = 2,
        PartialDelivery = 3
    }
}