using System;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Contracts.Extensibility.Serializers;
using Hyper.NodeServices.Extensibility.CommandModules;
using NodeModuleTest.Contracts.CommandModules;

namespace NodeModuleTest.CommandModules
{
    public class ComplexCommandModule : ICommandModule, ICommandRequestSerializerFactory, ICommandResponseSerializerFactory
    {
        private readonly object _serializer;

        public ComplexCommandModule()
        {
            _serializer = new DataContractCommandSerializer<ComplexCommandRequest, ComplexCommandResponse>();
        }

        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            context.Activity.Track("In Complex command module...");

            var request = context.Request as ComplexCommandRequest;
            if (request == null)
            {
                context.Activity.Track("Request was null. Throw tragic exception!");
                throw new InvalidOperationException("Request was null. Invalid deserialization!");
            }
            
            context.Activity.Track("Request was not null. Proceeding as planned.");
            context.Activity.Track($"{nameof(request.MyDateTime)} = '{request.MyDateTime}'");
            context.Activity.Track($"{nameof(request.MyInt32)} = '{request.MyInt32}'");
            context.Activity.Track($"{nameof(request.MyTimeSpan)} = '{request.MyTimeSpan}'");
            context.Activity.Track($"{nameof(request.MyDateTime)} = '{request.MyDateTime}'");

            return new ComplexCommandResponse
            {
                EightDaysLaterThanRequest = request.MyDateTime.AddDays(8),
                FiveHundressLessThanRequest = request.MyInt32 - 500,
                ProcessStatusFlags = MessageProcessStatusFlags.Success,
                FortyYearTimeSpan = (new DateTime(2040,1,1) - new DateTime(2000, 1,1)),
                ResponseStringNotTheSame = request.MyString + " not the same!!!"
            };
        }
        
        ICommandRequestSerializer ICommandRequestSerializerFactory.Create()
        {
            return _serializer as ICommandRequestSerializer;
        }

        ICommandResponseSerializer ICommandResponseSerializerFactory.Create()
        {
            return _serializer as ICommandResponseSerializer;
        }
    }
}
