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

## License

This project is licensed under [the MIT License](LICENSE).
