## The Problem

Organizations today normally have multiple deployment environments (e.g., Development, Testing, Production) and use separate API Management instances for each environment. Some of these instances are shared by multiple development teams, who are responsible for different APIs with different release cadence.

As a result, customers often come deal with the following challenges:

* How to automate deployment of APIs into API Management?
* How to migrate configurations from one environment to another?
* How to avoid interference between different development teams who share the same API Management instance?


## The Microsoft's Solution: APIM DevOps Toolkit

Microsoft has provided an [effective approach and toolkit](https://github.com/Azure/azure-api-management-devops-resource-kit) for taking care of these challenges. You can read the thorough [explanation](https://github.com/Azure/azure-api-management-devops-resource-kit) here and also there is a [video by Miao Jiang](https://www.youtube.com/watch?v=4Sp2Qvmg6j8) which demonstrates the approach through a sample. 

The core of this approach is to allow users to define the structure of an APIM instance using an easy to understand yml file and OpenApi. In the next step, the toolkit transforms the yml file to a set of [ARM templates](https://docs.microsoft.com/azure/azure-resource-manager/resource-group-authoring-templates) which are ready to be deployed into Azure. With this approach, the deployment of API changes into API Management instances can be automated and it is easy to promote changes from one environment to another. 


In one sentence this approach is going from **yml/openapi** to **ARM Templates** and then **Azure API Managenemt**.

## Deviation from Microsoft's Approach: dotnet-apim

While Microsoft's approach to the problem is solid on paper, we found that there are some issues in the implementation that need to be addressed:  


* The official toolkit is actually a C# project which you need to build/run manually, near impossible to have it in your ci/cd pipeline. 
* It has some bugs and flaws in generating the arm templates.
* The source code is not in comply with C# best practices and design patterns. This makes maintenance and contribution more difficult for the community.

To address these fundemental issues, we have re-written the source code from the ground up. Bugs are eliminated and source code is much more in line with best practices. A cross-platform dotnet global tool, **dotnet-apim** is at your hand to streamline your APIM deployment by integrating in into CI/CD pipelines. Furthermore, new functionalities have been added such as Global/Local variables.

## Install [dotnet-apim](https://www.nuget.org/packages/Apim.DevOps.Toolkit/)

dotnet-apim is a cross-platform dotnet global tool that you can easily install from the command-line of your choice. Make sure you have the awesome [.NET Core](https://dotnet.microsoft.com/download) installed on your machine and then run the following command to install the tool.

```powershell
dotnet tool install --global Apim.DevOps.Toolkit
```

## YML Structure.

dotnet-apim use the same [yml structure](https://github.com/Azure/azure-api-management-devops-resource-kit/blob/master/src/APIM_ARMTemplate/README.md#create-the-config-file) defined in the Microsoft's toolkit. In this yaml file you define the structure of your APIM and its entities such as apis and products. This YML structure tries to mirror the APIM ARM templates of [APIs](https://docs.microsoft.com/en-us/azure/templates/microsoft.apimanagement/2019-01-01/service/apis), [API Version Sets](https://docs.microsoft.com/en-us/azure/templates/microsoft.apimanagement/2019-01-01/service/apiversionsets), [Products](https://docs.microsoft.com/en-us/azure/templates/microsoft.apimanagement/2019-01-01/service/backends), [Backends](https://docs.microsoft.com/en-us/azure/templates/microsoft.apimanagement/2019-01-01/service/backends), [Authorization Servers](https://docs.microsoft.com/en-us/azure/templates/microsoft.apimanagement/2019-01-01/service/authorizationservers), [Policies](https://docs.microsoft.com/en-us/azure/templates/microsoft.apimanagement/2019-01-01/service/policies), etc. 

The yml structure supports a wide array of entities and configuration. A simple one could be like below:

### Sample Config File

The following is a full config.yml file with each property listed:

```yml
version: 0.0.1
apimServiceName: myAPIMService

apis:
    - name: myApi
      type: http
      description: myFirstApi
      serviceUrl: http://myApiBackendUrl.com
      openApiSpec: C:\apim\swaggerPetstore.json
      policy: C:\apim\apiPolicyHeaders.xml
      suffix: conf
      subscriptionRequired: true
      isCurrent: true
      apiRevision: 1
      operations:
        addPet:
          policy: C:\apim\operationRateLimit.xml
        deletePet:
          policy: C:\apim\operationRateLimit.xml

outputLocation: C:\apim\output
linkedTemplatesBaseUrl : https://mystorageaccount.blob.core.windows.net/mycontainer
```

The above yml definition has the minimum properties required for defining an API in an APIM instance. More examples are provided here. Few things to note that are:

* **_apimServiceName_**: Specifies the name of your apim. All entities inside this yml file are deployed to this instance.
* **_openApiSpec_**: Takes a local path or url which refers to the OpenApi spec of your apis. You have to have this file ready for deployment. The tool creates operations based on this file.
* **_policy_**: Takes a local path or url which refers to an XML file that contains the policy.
* **_operations_**: Under this node, operation-specific policies are defined. Takes a local path or url which refers to an XML file that contains the policy. In this sample, **addPet** and **deletePet** are OperationIds defined in the OpenApi spec file.
* **_outputLocation_**: Refers to the place that tool output the generated ARM templates.
* **_linkedTemplatesBaseUrl_**: The address of the blob storage account you want to upload the ARM templates to. You have to upload all templates to a place accessible by Azure Resource Manager. This [limitation](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-linked-templates#external-template) is imposed by Azure.

## Run dotnet-apim

After having the yml file ready, it's time to running the dotnet-apim to generate the corresponding ARM templates.

```powershell
dotnet-apim create --configFile "c:/apim/definition.yml" 
```

You can find the generated files in the location defined by **_outputLocation_**.

Among all generated templates, following two files have a fundemntal role in the whole scenario:

**Master Template**: By default named master.template.json, is the main file executed by Azure Resource Manager. Has links to all other templates. 
**Patameter Template**: By default named parameters.json, contains the parametes required for Master template.

### Uploading Generated ARM Templates.

A known limitation of the ARM Templates is that they [need to get uploaded](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-linked-templates#external-template) to a location accessible to Azure Resource Manager. 

Azure Blob Storage could be a good option for most users. For test purposes, you can use [Azure Storage Explorer](https://azure.microsoft.com/en-us/features/storage-explorer/) to manually upload files in a drag and drop manner. You can also use [Azure CLI](https://docs.microsoft.com/en-us/azure/storage/common/storage-azure-cli), [AzCopy](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azcopy-v10), or [Azure File Copy](https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/deploy/azure-file-copy?view=azure-devops) task in Azure Pipeline to upload your files according to your needs.

### Deploying ARM Templates into APIM

Having the ARM templates uploaded, we are ready to start the deployment. We can use [Azure Cli](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-deploy-cli) or [Azure Resource Group Deployment](https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/deploy/azure-resource-group-deployment?view=azure-devops) in Azure Pipeline.

```powershell
az group deployment create --resource-group your-resource-group --template-file "c:\apim\output\master.template.json" --parameters "c:\apim\output\parameters.json"
```

## Variables

In practice we need to have customization and flexibility in place to cover a wide range of deployment scenarios. Global and Local variables can be define to customize the yml file and policies according to requirements imposed by environment or other conditions.

The following yml sample shows how we can have a customizeable deployment.

```yml
version: 0.0.1
apimServiceName: $(apimInstanceName)

apis:
    - name: myApi
      type: http
      description: myFirstApi
      serviceUrl: $(serviceUrl)
      openApiSpec: $(openApiPath)
      policy: C:\apim\apiPolicyHeaders.xml
      suffix: conf
      subscriptionRequired: true
      isCurrent: true
      apiRevision: 1
      operations:
        addPet:
          policy: C:\apim\operationRateLimit.xml
        deletePet:
          policy: C:\apim\operationRateLimit.xml

outputLocation: $(apimFolder)\output
linkedTemplatesBaseUrl : $(uploadLocation)
```

```xml
<!--this is C:\apim\operationRateLimit.xml-->
<policies>
    <inbound>
        <base />
        <set-backend-service base-url="$(BackendUrl)" />
    </inbound>
    <backend>
        <base />
    </backend>
    <outbound>
        <base />
    </outbound>
    <on-error>
        <base />
    </on-error>
</policies>
```

In the yml file, global variables are defined using the **$(variableName)** syntax. You can define whatever variable you like in everywhere, even the referenced policies. There variables are defined by you in the document and their corresponding values are set when you run dotnet-apim.

### Passing Global Variables

The variable file is simply a yml file that defines the variables like below.

```yml
- "apimInstanceName=your-apim-name"
- "apimFolder=C:\\apim"
- "uploadLocation=url-to-your-blob-storage"
- "openApiPath=path-to-the-open-api-spec" 
- "BackendUrl=backend-absolute-url"
- "serviceUrl=api-service-url"
```

Assuming the above file is located at _c:/apim/replace.yml_, we pass the file to dotnet-apim using the **--replacementFile** argument.

```powershell
dotnet-apim create --configFile "c:/apim/definition.yml" --replacementFile "c:/apim/replace.yml"
```

Another way which can be more flexible in CI/CD pipeline is passing variables through a string. In this string key-values should be separated using semicolon.

```powershell
dotnet-apim create --configFile "c:/apim/definition.yml" --replacementVars "apimInstanceName=value1;apimFolder=value2;uploadLocation=value3"
```

You can pass global variables through a file and string simultaneously.

### Passing Local Variables

By Having the previous exaple in mind, let's say we want to have similar policy for both operations **add_pet** and **delete_pet** while we want to set the backend variable **$(BackendUrl)** to different values in these operaions. We can achieve this using local variables.

```yml
version: 0.0.1
apimServiceName: $(apimInstanceName)

apis:
    - name: myApi
      type: http
      description: myFirstApi
      serviceUrl: $(serviceUrl)
      openApiSpec: $(openApiPath)
      policy: C:\apim\apiPolicyHeaders.xml
      suffix: conf
      subscriptionRequired: true
      isCurrent: true
      apiRevision: 1
      operations:
        addPet:
          policy: C:\apim\operationRateLimit.xml:::BackendUrl=http://server1.com
        deletePet:
          policy: C:\apim\operationRateLimit.xml:::BackendUrl=http://server2.com

outputLocation: $(apimFolder)\output
linkedTemplatesBaseUrl : $(uploadLocation)
```

Local variables are defined using **:::variableName1=value1;variableName2=value2** syntax inside yml file. Each key value is separated using a semicolon. The values are only applied to their associated policy. 

## Customizing the Name of Generated ARM templates

Another customization that you can apply is to the name of generated ARM templates.

1. Specify a prefix for all generated ARM files using the **--prefix** argument. Uses to tag files to categorize based on APIM instance, date, etc
2. Specify the name of the **Master Template** using the **--masterFileName** argument.


```powershell
dotnet-apim create --configFile "c:/apim/definition.yml" --replacementVars "apimInstanceName=value1;apimFolder=value2;uploadLocation=value3" --prefix current-date-time" --masterFileName "master.file"
```

## License

This project is licensed under [the MIT License](LICENSE).
