using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Hyper.Services.HyperNodeExtensibility;

namespace HyperNet.ExtensibilityTest.Shared.CommandModules
{
    public class DataContractCommandSerializer<TRequest, TResponse> : ICommandModuleRequestSerializer, ICommandModuleResponseSerializer
        where TRequest : ICommandModuleRequest
        where TResponse : ICommandModuleResponse
    {
        public string Serialize(ICommandModuleRequest request)
        {
            return Serialize((object)request);
        }

        public string Serialize(ICommandModuleResponse response)
        {
            return Serialize((object)response);
        }

        protected string Serialize(object target)
        {
            var builder = new StringBuilder();
            using (var stringWriter = new StringWriter(builder))
            {
                using (var writer = new XmlTextWriter(stringWriter))
                {
                    new DataContractSerializer(target.GetType()).WriteObject(writer, target);
                    writer.Flush();
                }
            }

            return builder.ToString();
        }

        ICommandModuleResponse ICommandModuleResponseSerializer.Deserialize(string responseString)
        {
            using (var stringReader = new StringReader(responseString))
            {
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    return (TResponse)new DataContractSerializer(typeof(TResponse)).ReadObject(xmlReader);
                }
            }
        }

        ICommandModuleRequest ICommandModuleRequestSerializer.Deserialize(string requestString)
        {
            using (var stringReader = new StringReader(requestString))
            {
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    return (TRequest)new DataContractSerializer(typeof(TRequest)).ReadObject(xmlReader);
                }
            }
        }
    }
}
