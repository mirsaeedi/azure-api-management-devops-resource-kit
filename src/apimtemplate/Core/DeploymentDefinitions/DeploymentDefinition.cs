using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using Apim.DevOps.Toolkit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions
{
  public class DeploymentDefinition : EntityDeploymentDefinition
  {
    public string Version { get; set; }

    public string ApimServiceName { get; set; }

    /// <summary>
    /// local path or url to global policy
    /// </summary>
    public string Policy { get; set; }

    public ICollection<ApiVersionSetDeploymentDefinition> ApiVersionSets { get; set; } = new List<ApiVersionSetDeploymentDefinition>();

    public ICollection<ApiDeploymentDefinition> Apis { get; set; } = new List<ApiDeploymentDefinition>();

    public ICollection<CertificateDeploymentDefinition> Certificates { get; set; } = new List<CertificateDeploymentDefinition>();

    public ICollection<SubscriptionDeploymentDefinition> Subscriptions { get; set; } = new List<SubscriptionDeploymentDefinition>();

    public ICollection<UserDeploymentDefinition> Users { get; set; } = new List<UserDeploymentDefinition>();

    public ICollection<ProductDeploymentDefinition> Products { get; set; } = new List<ProductDeploymentDefinition>();

    public ICollection<TagDeploymentDefinition> Tags { get; set; } = new List<TagDeploymentDefinition>();

    public ICollection<LoggerDeploymentDefinition> Loggers { get; set; } = new List<LoggerDeploymentDefinition>();

    public ICollection<AuthorizationServerDeploymentDefinition> AuthorizationServers { get; set; } = new List<AuthorizationServerDeploymentDefinition>();

    public ICollection<BackendDeploymentDefinition> Backends { get; set; } = new List<BackendDeploymentDefinition>();

    public ICollection<NamedValueDeploymentDefinition> NamedValues { get; set; } = new List<NamedValueDeploymentDefinition>();

    public ICollection<GatewayDeploymentDefinition> Gateways { get; set; } = new List<GatewayDeploymentDefinition>();

    internal DeploymentDefinition MergeWith(DeploymentDefinition individualDefinition)
    {
      var mergedDefinition = new DeploymentDefinition
      {
        Version = this.Version ?? individualDefinition.Version,
        ApimServiceName = this.ApimServiceName ?? individualDefinition.ApimServiceName,
        Policy = this.Policy ?? individualDefinition.Policy
      };

      mergedDefinition.ApiVersionSets.AddRange(this.ApiVersionSets).AddRange(individualDefinition.ApiVersionSets);
      mergedDefinition.Apis.AddRange(this.Apis).AddRange(individualDefinition.Apis);
      mergedDefinition.Certificates.AddRange(this.Certificates).AddRange(individualDefinition.Certificates);
      mergedDefinition.Subscriptions.AddRange(this.Subscriptions).AddRange(individualDefinition.Subscriptions);
      mergedDefinition.Users.AddRange(this.Users).AddRange(individualDefinition.Users);
      mergedDefinition.Products.AddRange(this.Products).AddRange(individualDefinition.Products);
      mergedDefinition.Tags.AddRange(this.Tags).AddRange(individualDefinition.Tags);
      mergedDefinition.Loggers.AddRange(this.Loggers).AddRange(individualDefinition.Loggers);
      mergedDefinition.AuthorizationServers.AddRange(this.AuthorizationServers).AddRange(individualDefinition.AuthorizationServers);
      mergedDefinition.Backends.AddRange(this.Backends).AddRange(individualDefinition.Backends);
      mergedDefinition.NamedValues.AddRange(this.NamedValues).AddRange(individualDefinition.NamedValues);
      mergedDefinition.Gateways.AddRange(this.Gateways).AddRange(individualDefinition.Gateways);

      return mergedDefinition;
    }

    public string OutputLocation { get; set; }

    public string PrefixFileName { get; set; }

    public string MasterTemplateName { get; set; }

    public bool HasCertificates() => Certificates != null && Certificates.Count() > 0;

    public override IEnumerable<string> Dependencies() => Array.Empty<string>();
  }
}