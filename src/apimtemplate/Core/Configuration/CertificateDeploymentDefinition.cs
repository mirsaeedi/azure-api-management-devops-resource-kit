using Apim.DevOps.Toolkit.ApimEntities.Tag;
using Apim.DevOps.Toolkit.ArmTemplates;
using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
	public class CertificateDeploymentDefinition : CertificateProperties
	{
		/// <summary>
		/// The Id of the Certificate
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The path to the pfx certificate
		/// </summary>
		public string FilePath { get; set; }
	}

}