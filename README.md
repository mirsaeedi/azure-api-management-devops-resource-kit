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

found that are dotnet-apim is a cross-platform dotnet tool that you can easily install on your dev machine or ci/cd system. Using dotnet-apim is straigtforward and consists of 4 steps.

1. [install dotnet-apim] on your machine.
2. Define a yml to the entities of your APIM instance. A wide array of resouces are supported such as API Version Sets, APIs, Policies, Products, Backend, Authorization, etc.
3. Execute dotnet-apim which generates a set of ARM templates according to yml definitions.
4. Deploy the arm templates into Azure Api Management.


**Note**: This repository is forked from the official [API Management Devops Resource Kit](). While there are similarities in the approach, dotnet-apim offers wide range of advantages not available in the official toolkit.



## CI/CD with API Management

The proposed approach is illustrated in the below picture.  

![alt](APIM-DevOps.png)

In this example, there are two deployment environments: Development and Production, each has its own API Management instance. API developers have access to the Development instance and can use it for developing and testing their APIs. The Production instance is managed by a designated team called the API publishers.

The key in this proposed approach is to keep all API Management configurations in Azure [Resource Manager templates](https://docs.microsoft.com/azure/azure-resource-manager/resource-group-authoring-templates). These templates should be kept in a source control system. We will use GIT throughout this example. As illustrated in the picture, there is a Publisher repository that contains all configurations of the Production API Management instance in a collection of templates.

* **Service template**: contains all the service-level configurations of the API Management instance (e.g., pricing tier and custom domains). 
* **Shared templates**: contain shared resources throughout an API Management instance (e.g., groups, products, loggers). 
* **API templates**: include configurations of APIs and their sub-resources (e.g., operations, policies, diagnostics settings). 
* **Master template**: ties everything together by [linking](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-linked-templates) to all templates and deploy them in order. If we want to deploy all configurations to an API Management instance, we can just deploy the master template. Meanwhile, each template can also be deployed individually.

API developers will fork the publisher repository to a developer repository and work on the changes for their APIs. In most cases, they will focus on the API templates for their APIs and do not need to change the shared or service templates.

We realize there are two challenges for API developers when working with Resource Manager templates:

* First, API developers often work with [OpenAPI Specification](https://github.com/OAI/OpenAPI-Specification) and may not be familiar with Resource Manager schemas. Authoring templates manually might be an error-prone task. Therefore, we created a utility tool called [**Creator**](./src/APIM_ARMTemplate/README.md#Creator) to automate the creation of API templates based on an Open API Specification file. Optionally, developers can supply API Management policies for an API in XML format. Basically, the tool inserts the Open API specification and policies into a Resource Manager template in the proper format. With this tool, API developers can continue focusing on the formats and artifacts they are familiar with.


We realize our customers bring a wide range of engineering cultures and existing automation solutions. The approach and tools provided here are not meant to be a one-size-fits-all solution. That's why we created this repository and open-sourced everything, so that you can extend and customize the solution.

If you have any questions or feedback, please raise issues in the repository or email us at apimgmt at microsoft. We also started an [FAQ page](./FAQ.md) to answer most common questions.

## Alternatives

* For customers who are just starting out or have simple scenarios, they may not necessarily need to use the tools we provided and may find it easier to begin with the boilerplate templates we provided in the [example](./example/) folder.
* Customers can also run [PowerShell](https://docs.microsoft.com/powershell/module/azurerm.apimanagement/?view=azurermps-6.13.0) scripts as part of their release process to deploy APIs to API Management.
* There is also a **non-official** Azure DevOps [extension](https://marketplace.visualstudio.com/items?itemName=stephane-eyskens.apim) created by [Stephane Eyskens](https://stephaneeyskens.wordpress.com/).

## Kudos

This project was inspired by Mattias Lögdberg's [API Management ARM Template Creator](http://mlogdberg.com/apimanagement/arm-template-creator) and Eldert Grootenboer's [series of blog posts](https://blog.eldert.net/api-management-ci-cd-using-arm-templates-api-management-instance/) on CI/CD with API Management. We have also received feedback from many members in our community. Thank you to all who have contributed in the project!

## License

This project is licensed under [the MIT License](LICENSE)

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information, see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Contributing

This project welcomes contributions and suggestions. Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information, see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
