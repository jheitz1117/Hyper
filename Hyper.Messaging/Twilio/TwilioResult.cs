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

        public TwilioSMSSendStatus SendStatus
        {
            get
            {
                TwilioSMSSendStatus sendStatus = TwilioSMSSendStatus.SendNotAttempted;

                if (PartialSendResults.Where(x => x.SendAttempted).Count() == 0)
                {
                    sendStatus = TwilioSMSSendStatus.SendNotAttempted;
                }
                else if (PartialSendResults.Where(x => x.SendSuccess).Count() == PartialSendResults.Count)
                {
                    // All pieces sent successfully
                    sendStatus = TwilioSMSSendStatus.Success;
                }
                else if (PartialSendResults.Where(x => !x.SendSuccess).Count() == PartialSendResults.Count)
                {
                    // All pieces failed
                    sendStatus = TwilioSMSSendStatus.Failure;
                }
                else
                {
                    sendStatus = TwilioSMSSendStatus.PartialDelivery;
                }

                return sendStatus;
            }
        }

        #endregion Properties
    }

    public enum TwilioSMSSendStatus
    {
        SendNotAttempted = 0,
        Success = 1,
        Failure = 2,
        PartialDelivery = 3
    }
}