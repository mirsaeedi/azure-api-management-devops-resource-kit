using System.Text;

namespace Apim.DevOps.Toolkit.ArmTemplates
{
    public class LoggerProperties
    {
        public string LoggerType { get; set; }
        public string Description { get; set; }
        public LoggerCredentials Credentials { get; set; }
        public bool IsBuffered { get; set; }
        public string ResourceId { get; set; }
    }

    public class LoggerCredentials
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string InstrumentationKey { get; set; }
    }
}
