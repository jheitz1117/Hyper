using System;
using System.Collections.Generic;
using System.Threading;
using Twilio;

namespace Hyper.Messaging.Twilio
{
    public class TwilioAdapter
    {
        #region Public Properties

        public string AccountID { get; set; }
        public string AuthenticationToken { get; set; }

        /// <summary>
        /// If the first piece of a text message fails to send, the whole text is considered a failure. However, if any of the subsequent
        /// pieces fail after the first piece sends successfully, we want to retry those subsequent pieces in an attempt to avoid a partial
        /// delivery. MaxPartialSendAttempts defines the maximum number of times to retry the subsequent pieces before giving up. Be aware
        /// that if any piece fails to send, the API will not try to send any of the remaining pieces.
        /// </summary>
        public int MaxPartialSendAttempts
        {
            get;
            set;
        }

        /// <summary>
        /// If we are in the middle of a partial delivery, and one of the inner messages fails to send, this property defines how long to
        /// wait before attempting to resend the message.
        /// </summary>
        public TimeSpan PartialSendRetryInterval
        {
            get;
            set;
        }

        #endregion Public Properties

        #region Private Properties

        /// <summary>
        /// Maximum length for a single text message. If the string being sent is longer than this value, it is broken into multiple text messages.
        /// </summary>
        private int MaxTextLength
        {
            get
            {
                return 160;
            }
        }

        private TwilioRestClient _twilioService = null;
        private TwilioRestClient TwilioService
        {
            get
            {
                if (_twilioService == null)
                {
                    _twilioService = new TwilioRestClient(AccountID, AuthenticationToken);
                }

                return _twilioService;
            }
        }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Creates an instance of TwilioAdapter with the specified AccountID and AuthenticationToken.
        /// </summary>
        /// <param name="accountID">Twilio Account ID</param>
        /// <param name="authenticationToken">Twilio Authentication Token</param>
        public TwilioAdapter(string accountID, string authenticationToken)
            : this(accountID, authenticationToken, new TimeSpan(0, 0, 60))
        {
        }

        /// <summary>
        /// Creates an instance of TwilioAdapter with the specified AccountID, AuthenticationToken, and PartialSendRetryInterval.
        /// </summary>
        /// <param name="accountID">Twilio Account ID</param>
        /// <param name="authenticationToken">Twilio Authentication Token</param>
        /// <param name="partialSendRetryInterval">Time to wait before resending a failed message (only used for messages long enough to be split into multiple texts). Defaults to 60 seconds.</param>
        public TwilioAdapter(string accountID, string authenticationToken, TimeSpan partialSendRetryInterval)
            : this(accountID, authenticationToken, partialSendRetryInterval, 10)
        {
        }

        /// <summary>
        /// Creates an instance of TwilioAdapter with the specified settings.
        /// </summary>
        /// <param name="accountID">Twilio Account ID</param>
        /// <param name="authenticationToken">Twilio Authentication Token</param>
        /// <param name="partialSendRetryInterval">Time to wait before resending a failed message (only used for messages long enough to be split into multiple texts). Defaults to 60 seconds.</param>
        /// <param name="maxPartialSendAttempts">Max number of times to attempt later texts during a partial delivery. Defaults to 10.</param>
        public TwilioAdapter(string accountID, string authenticationToken, TimeSpan partialSendRetryInterval, int maxPartialSendAttempts)
        {
            AccountID = accountID;
            AuthenticationToken = authenticationToken;
            PartialSendRetryInterval = partialSendRetryInterval;
            MaxPartialSendAttempts = maxPartialSendAttempts;
        }

        public TwilioResult SendTextMessage(string from, string to, string body)
        {
            TwilioResult result = new TwilioResult();

            // Break the body into pieces if necessary
            List<string> smsBodyPieces = new List<string>();
            for (int i = 0; i < body.Length; i += MaxTextLength)
            {
                smsBodyPieces.Add(body.Substring(i, Math.Min(body.Length - i, MaxTextLength)));
            }

            // Send all pieces in order. We want to record a TwilioPartialSendResult object for each piece, but we only want to send a particular
            // piece if all previous pieces were sent successfully.
            bool allPreviousPiecesDelivered = true;
            for (int smsIndex = 0; smsIndex < smsBodyPieces.Count; smsIndex++)
            {
                string smsPiece = smsBodyPieces[smsIndex];

                TwilioPartialSendResult partialResult = new TwilioPartialSendResult(smsIndex, smsPiece);

                // Only attempt to send this piece if all previous pieces were sent successfully
                if (allPreviousPiecesDelivered)
                {
                    // Try MaxPartialSendAttempts times to send this piece
                    for (int i = 0; i < MaxPartialSendAttempts; i++)
                    {
                        if (string.IsNullOrWhiteSpace(body))
                        {
                            partialResult.Detail = "The text message body must consist of at least one non-whitespace character.";
                        }
                        else
                        {
                            partialResult.SendResult = TwilioService.SendSmsMessage(from, to, smsPiece);
                            partialResult.SendAttempts++;
                        }

                        if (partialResult.SendSuccess)
                        {
                            break;
                        }
                        else if (smsIndex == 0)
                        {
                            // If our first piece fails, we consider the whole message to be a failure.
                            allPreviousPiecesDelivered = false;
                            break;
                        }
                        else
                        {
                            // In this case, we want to retry, but let's wait a while before our next attempt
                            Thread.Sleep(PartialSendRetryInterval);
                        }
                    }
                }

                allPreviousPiecesDelivered &= partialResult.SendSuccess;

                result.PartialSendResults.Add(partialResult);
            }

            return result;
        }

        #endregion Public Methods
    }
}
