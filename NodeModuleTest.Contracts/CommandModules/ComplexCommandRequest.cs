﻿using System;
using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace NodeModuleTest.Contracts.CommandModules
{
    [DataContract]
    public class ComplexCommandRequest : ICommandRequest
    {
        [DataMember]
        public string MyString { get; set; }
        
        [DataMember]
        public DateTime MyDateTime { get; set; }

        public int MyInt32 { get; set; }
        
        [DataMember]
        public TimeSpan MyTimeSpan { get; set; }
    }
}
