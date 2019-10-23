
namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class SchemaTemplateResource : TemplateResource<SchemaTemplateProperties>
    {
        public override string Type => ResourceType.ApiSchema;
    }

    public class SchemaTemplateProperties
    {
        public string ContentType { get; set; }
        public SchemaTemplateDocument Document { get; set; }
    }

    public class SchemaTemplateDocument
    {
        public string Value { get; set; }
    }

    public class RESTReturnedSchemaTemplate : TemplateResource<RESTReturnedSchemaTemplateProperties>
    {
        public override string Type => ResourceType.ApiSchema;
    }

    public class RESTReturnedSchemaTemplateProperties
    {
        public string ContentType { get; set; }
        public object Document { get; set; }
    }
}
