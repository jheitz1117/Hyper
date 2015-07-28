using System;
using Hyper.Services.HyperNodeContracts;
using Hyper.Services.HyperNodeExtensibility;

namespace HyperNet.ExtensibilityTest.Shared.CommandModules
{
    public class ComplexCommandResponse : ICommandModuleResponse
    {
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }
        public DateTime EightDaysLaterThanRequest { get; set; }
        public TimeSpan FortyYearTimeSpan { get; set; }
        public int FiveHundressLessThanRequest { get; set; }
        public string ResponseStringNotTheSame { get; set; }
    }
}
