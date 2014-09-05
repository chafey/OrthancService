using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace OrthancService
{
    [DataContract]
    public class OrthancServiceConfiguration
    {
        [DataMember]
        public string OrthancPath;

        [DataMember]
        public string CommandLineArguments;

        [DataMember]
        public string WorkingDirectory;


        public static OrthancServiceConfiguration Read(string configFilePath)
        {
            var text = File.ReadAllText(configFilePath);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                var serializer = new DataContractJsonSerializer(typeof(OrthancServiceConfiguration));
                var config = (OrthancServiceConfiguration)serializer.ReadObject(stream);
                return config;
            }
        }
    }
}
