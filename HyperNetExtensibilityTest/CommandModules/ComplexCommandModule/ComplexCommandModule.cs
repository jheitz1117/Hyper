using System;
using Hyper.Services.HyperNodeContracts;
using Hyper.Services.HyperNodeExtensibility;

namespace HyperNetExtensibilityTest.CommandModules
{
    public class ComplexCommandModule : ComplexCommandModuleBase<ComplexCommandRequest, ComplexCommandResponse>
    {
        public override ICommandModuleResponse Execute(ICommandExecutionContext context)
        {
            context.Activity.Track("In Complex command module...");

            var request = context.Request as ComplexCommandRequest;
            if (request == null)
            {
                context.Activity.Track("Request was null. Throw tragic exception!");
                throw new InvalidOperationException("Request was null. Invalid deserialization!");
            }

            context.Activity.Track("Request was not null. Proceeding as planned.");

            return new ComplexCommandResponse
            {
                EightDaysLaterThanRequest = request.MyDateTime.AddDays(8),
                FiveHundressLessThanRequest = request.MyInt32 - 500,
                ProcessStatusFlags = MessageProcessStatusFlags.Success,
                FortyYearTimeSpan = (new DateTime(2040,1,1) - new DateTime(2000, 1,1)),
                ResponseStringNotTheSame = request.MyString + " not the same!!!"
            };
        }
    }
}
