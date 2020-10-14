## The Problem

Organizations today normally have multiple deployment environments (e.g., Development, Testing, Production) and use separate API Management instances for each environment. Some of these instances are shared by multiple development teams, who are responsible for different APIs with different release cadence.

As a result, Azure API Management users often deal with the following challenges:

* How to automate the deployment of APIs into API Management?
* How to migrate configurations from one environment to another?
* How to avoid interference between different development teams who share the same API Management instance?


## Microsoft's Solution: APIM DevOps Toolkit

Microsoft has provided an [effective approach and toolkit](https://github.com/Azure/azure-api-management-devops-resource-kit) for taking care of these challenges. You can read the thorough [explanation](https://github.com/Azure/azure-api-management-devops-resource-kit) here and also there is a [video by Miao Jiang](https://www.youtube.com/watch?v=4Sp2Qvmg6j8) which demonstrates the approach through a sample. 

The core of this approach is to allow users to define the structure of an APIM instance using an easy to understand yml file and OpenApi. In the next step, the toolkit transforms the yml file to a set of [ARM templates](https://docs.microsoft.com/azure/azure-resource-manager/resource-group-authoring-templates) that are ready to be deployed into Azure. With this approach, the deployment of API changes into API Management instances can be automated and it is easy to promote changes from one environment to another. 


In one sentence this approach is going from **yml/openapi** to **ARM Templates** and then **Azure API Management**.

## Deviation from Microsoft's Approach: dotnet-apim

While Microsoft's approach to the problem is solid on paper, we found that there are some issues in the implementation that need to be addressed:  


* The official toolkit is actually a C# project which you need to build/run manually, near impossible to have it in your ci/cd pipeline. 
* It has some bugs and flaws in generating the arm templates.
* The source code is not in comliance with C# best practices and design patterns. This makes maintenance and contribution more difficult for the community.
* Support for tags, users, subscriptions, and certificates is required.

To address these fundamental issues, we have re-written the source code from the ground up. Bugs are eliminated and source code is much more in line with best practices. A cross-platform dotnet global tool, **dotnet-apim** is at your hand to streamline your APIM deployment by integrating it into CI/CD pipelines. Furthermore, new functionalities have been added such as Global/Local variables.

## Install [dotnet-apim](https://www.nuget.org/packages/Apim.DevOps.Toolkit/)

dotnet-apim is a cross-platform dotnet global tool that you can easily install from the command-line of your choice. Make sure you have the awesome [.NET Core](https://dotnet.microsoft.com/download) installed on your machine and then run the following command to install the tool.

```powershell
dotnet tool install --global Apim.DevOps.Toolkit
```

## YML Structure

dotnet-apim use the same [yml structure](https://github.com/Azure/azure-api-management-devops-resource-kit/blob/master/src/APIM_ARMTemplate/README.md#create-the-config-file) defined by Microsoft's toolkit. In this yaml file, you define the structure of your APIM and its entities such as apis, products, and backends. This YML structure tries to mirror the reference APIM ARM templates of [APIs](https://docs.microsoft.com/en-us/azure/templates/microsoft.apimanagement/2019-01-01/service/apis), [API Version Sets](https://docs.microsoft.com/en-us/azure/templates/microsoft.apimanagement/2019-01-01/service/apiversionsets), [Products](https://docs.microsoft.com/en-us/azure/templates/microsoft.apimanagement/2019-01-01/service/backends), [Backends](https://docs.microsoft.com/en-us/azure/templates/microsoft.apimanagement/2019-01-01/service/backends), [Authorization Servers](https://docs.microsoft.com/en-us/azure/templates/microsoft.apimanagement/2019-01-01/service/authorizationservers), [Policies](https://docs.microsoft.com/en-us/azure/templates/microsoft.apimanagement/2019-01-01/service/policies), etc. 

### Sample Config File

The provided yml structure supports a wide array of entities and configuration. A simple one could be like below:

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
      path: conf
      subscriptionRequired: true
      isCurrent: true
      apiRevision: 1
      operations:
        addPet:
          policy: C:\apim\operationRateLimit.xml
        deletePet:
          policy: C:\apim\operationRateLimit.xml
```

The above yml definition has the minimum properties required for defining an API in an APIM instance. More examples are provided [here](https://github.com/mirsaeedi/dotnet-apim/tree/master/src/Examples/Definitions). Few things to note here are:

* **_apimServiceName_**: Specifies the name of your apim. All entities inside this yml file are deployed to this instance.
* **_openApiSpec_**: Takes a local path or url which refers to the OpenApi spec of your apis. You have to have this file ready for deployment. The tool creates operations based on this file.
* **_policy_**: Takes a local path or url which refers to an XML file that contains the policy.
* **_operations_**: Under this node, operation-specific policies are defined. Takes a local path or url which refers to an XML file that contains the policy. In this sample, **addPet** and **deletePet** are OperationIds defined in the OpenApi spec file.

## Run dotnet-apim

After having the yml file ready, it's time for running dotnet-apim for generating the corresponding ARM templates.

```powershell
dotnet-apim --yamlConfig "c:/apim/definition.yml" --output "c:/output"
```


```powershell
dotnet-apim -c "c:/apim/definition.yml" -o "c:/output"
```

The command creates ARM templates for the defined products, tags,, APIs, etc accordingly. You can find the generated files in the location defined by **_output_**. 

the following two files play a fundamental role in the whole scenario:

**Deploy Template**: By default named `apim_deploy.template.json`, is the main file executed by Azure Resource Manager. It consists of all resources required to deploy into APIM. 
**Parameter Template**: By default named `parameters.json`, contains the parameters required for executing the Master template.

### Deploying ARM Templates into APIM

Having the ARM templates ready, we are ready to start the deployment. We can use [Azure CLI](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-deploy-cli) or [Azure Resource Group Deployment](https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/deploy/azure-resource-group-deployment?view=azure-devops) in Azure Pipeline.

```powershell
az group deployment create --resource-group your-resource-group --template-file "c:\apim\output\apim_deploy.template.json" --parameters "c:\apim\output\parameters.json"
```

## Variables

In practice, a great degree of customization and flexibility is required to cover a wide range of deployment scenarios. Global and Local variables can be defined to customize the yml file and policies according to requirements imposed by the environment or other conditions.

The following yml sample shows how we can have a customizable deployment definition by replacing raw values with a user-defined variable.

```yml
version: 0.0.1
apimServiceName: $(apimInstanceName)  # global variable

apis:
    - name: myApi
      type: http
      description: myFirstApi
      serviceUrl: $(serviceUrl)  # global variable
      openApiSpec: $(openApiPath)  # global variable
      policy: C:\apim\apiPolicyHeaders.xml
      path: api
      subscriptionRequired: true
      isCurrent: true
      apiRevision: 1
      operations:
        addPet:
          policy: C:\apim\operationRateLimit.xml
        deletePet:
          policy: C:\apim\operationRateLimit.xml
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

In the yml file, global variables are defined using the **$(variableName)** syntax. You can define any variable you like in any place, even in the referenced policies. There is no built-in variable, they are all user-defined.

The next step is to inform dotnet-apim of these variables.

### Passing Global Variables

The variable file is simply a yml file that defines the variables.

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
dotnet-apim --yamlConfig "c:/apim/definition.yml" --variableFile "c:/apim/replace.yml"
```


```powershell
dotnet-apim -c "c:/apim/definition.yml" -f "c:/apim/replace.yml"
```

By executing this command, dotnet-apim replaces all variables with their corresponding value before staring to generate the ARM templates. 

Another way which can be more flexible in CI/CD pipeline is passing variables through a string. In this string, key-values should be separated using a semicolon.

```powershell
dotnet-apim --yamlConfig "c:/apim/definition.yml" --variableString "apimInstanceName=value1;apimFolder=value2;uploadLocation=value3"
```

```powershell
dotnet-apim -c "c:/apim/definition.yml" -s "apimInstanceName=value1;apimFolder=value2;uploadLocation=value3"
```

You can pass global variables through a file and string simultaneously.

### Passing Local Variables

By Having the previous example in mind, let's say we want to have a similar policy for both operations **add_pet** and **delete_pet** while we want to set the backend variable **$(BackendUrl)** to different values in these operaions. We can achieve this using local variables.

```yml
version: 0.0.1
apimServiceName: $(apimInstanceName) # global variable

apis:
    - name: myApi
      type: http
      description: myFirstApi
      serviceUrl: $(serviceUrl)  # global variable
      openApiSpec: $(openApiPath)  # global variable
      policy: C:\apim\apiPolicyHeaders.xml
      path: conf
      subscriptionRequired: true
      isCurrent: true
      apiRevision: 1
      operations:
        addPet:
          policy: C:\apim\operationRateLimit.xml:::BackendUrl=http://server1.com         # All occurances of $(BackendUrl) inside operationRateLimit.xml will be replaced by http://server1.com 
        deletePet:
          policy: C:\apim\operationRateLimit.xml:::BackendUrl=http://server2.com         # All occurances of $(BackendUrl) inside operationRateLimit.xml will be replaced by http://server2.com 

outputLocation: $(apimFolder)\output  # global variable
linkedTemplatesBaseUrl : $(uploadLocation)  # global variable
```

Local variables are defined using **:::variableName1=value1;variableName2=value2** syntax inside yml file. Key=Value pairs are separated using a semicolon. The local variables are only applied to their associated policy and override the global variables in case of name collision.

## Conditional Statements
In some cases, we need to customize our deployments based on some conditions. dotnet-apim supports simple conditional statements. An if block starts with **#if $(booleanVar)** and ends with **#endif**. If booleanVar is _true_, then all the lines in between will be interpreted by dotnet-apim otherwise they are ignored. The tool supports nested if statements too.

The following sample shows how we can include/exclude products based on a condition imposed by our CI/CD pipeline through a variable.

```yml
version: 0.0.1
apimServiceName: $(apimInstanceName) # global variable

apis:
    - name: myApi
      type: http
      description: myFirstApi
      serviceUrl: $(serviceUrl)  # global variable
      openApiSpec: $(openApiPath)  # global variable
      policy: C:\apim\apiPolicyHeaders.xml
      path: conf
      subscriptionRequired: true
      isCurrent: true
      apiRevision: 1
      products: ProductMain, #if $(includeProductA) ProductA #endif, #if $(includeProductB) ProductB #endif     #inline if
      operations:
        addPet:
          policy: C:\apim\operationRateLimit.xml:::BackendUrl=http://server1.com         # All occurances of $(BackendUrl) inside operationRateLimit.xml will be replaced by http://server1.com 
        deletePet:
          policy: C:\apim\operationRateLimit.xml:::BackendUrl=http://server2.com         # All occurances of $(BackendUrl) inside operationRateLimit.xml will be replaced by http://server2.com 
          
products:

    - name: productMain
      displayName: productMain
      description: Product Main
      subscriptionRequired: true
      approvalRequired: true
      subscriptionsLimit: $(productMainLimit)
      state: published
      policy: $(apimBasePath)\Products\ProductMain\policy.xml

    #if $(includeProductA)

    - name: productA
      displayName: productA
      description: Product A
      subscriptionRequired: true
      approvalRequired: true
      subscriptionsLimit: $(productALimit)
      state: published
      policy: $(apimBasePath)\Products\ProductA\policy.xml
    
    #endif

    #if $(includeProductB)

    - name: productB
      displayName: productB
      description: Product B
      subscriptionRequired: true
      approvalRequired: true
      subscriptionsLimit: $(productBLimit)
      state: published
      policy: $(apimBasePath)\Products\ProductB\policy.xml
      
    #endif
```

## Customizing The Name of Generated ARM Templates

Another customization that can be applied is changing the name of generated ARM templates.

1. Specify a prefix for all generated ARM files using the **--armPrefix** argument. Used to tag files to categorize each deployment based on APIM instance, date, etc
2. Specify the name of **Deploy Template** using the **--master** argument.


```powershell
dotnet-apim --yamlConfig "c:/apim/definition.yml" --variableString "apimInstanceName=value1;apimFolder=value2;uploadLocation=value3" --armPrefix "current-date-time" --master "master.file"
```


```powershell
dotnet-apim -c "c:/apim/definition.yml" -s "apimInstanceName=value1;apimFolder=value2;uploadLocation=value3" -p "current-date-time" -m "master.file"
```

## License

This project is licensed under [the MIT License](LICENSE).
