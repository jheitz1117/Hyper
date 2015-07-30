﻿using System;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;

namespace HyperNet.ExtensibilityTest.Shared.CommandModules
{
    public class ComplexCommandResponse : ICommandResponse
    {
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }
        public DateTime EightDaysLaterThanRequest { get; set; }
        public TimeSpan FortyYearTimeSpan { get; set; }
        public int FiveHundressLessThanRequest { get; set; }
        public string ResponseStringNotTheSame { get; set; }
    }
}
