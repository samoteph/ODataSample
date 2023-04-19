---

Projet: '[Scamark.Microservice](https://scamark.visualstudio.com/Framework/_git/Microservice)'

Langages: 
- C#

Package: '[![Scamark.Microservice package in scamark feed in Azure Artifacts](https://scamark.feeds.visualstudio.com/_apis/public/Packaging/Feeds/bdf762d7-d2b2-4ead-a686-9a3d22356824/Packages/60d9793a-5432-45e5-819a-20120409bdb4/Badge)](https://scamark.visualstudio.com/Framework/_packaging?_a=package&feed=bdf762d7-d2b2-4ead-a686-9a3d22356824&package=60d9793a-5432-45e5-819a-20120409bdb4&preferRelease=true)'

Pipeline CI: '[![Build Status](https://scamark.visualstudio.com/Framework/_apis/build/status/Microservice?branchName=master)](https://scamark.visualstudio.com/Framework/_build/latest?definitionId=9&branchName=master)'

--- 

[[_TOC_]]

# Introduction 

Bibliothèque permettant de configurer un Host ASP.NET Core avec le paramétrage par défaut d'un microservice Scamark.
Le template par défaut créé avec le templates `dotnet new scamark-api` utilise le package Nuget Scamark.Microservice.

# Utilisation

Dans le fichier `Program.c``, remplacer le contenu par :

```csharp
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Scamark.Framework.Common;
using Scamark.Microservice;

var host = ScamarkHostBuilder.CreateODataWebHost<Scamark.API.Article.BusinessModel.OData.ModelConfiguration>(args);

// bind la classe ApplicationConfiguration à la section Scamark de la configuration
var appConfig = host.Services.AddScamarkConfiguration<ApplicationConfiguration>(host.Configuration);

var app = host.Build();
app.UseScamarkOData();
app.Run();
```

# Dépendances

Le projet référence les bibliothèques suivantes, sous forme de packages Nuget :

- ASP.NET Core 6.0
- Asp.Versioning.OData 6.0
- Microsoft.AspNetCore.OData 8.0
- Scamark.Framework.Common 2.x
- Swagger
- Serilog

::: mermaid
graph TD;
    A[Scamark.Microservice] -->|Référence| D[ASP.NET Core 6.0]
    A[Scamark.Microservice] -->|Référence| B[Asp.Versioning.OData 6.0]
    A[Scamark.Microservice] -->|Référence| C[Microsoft.AspNetCore.OData 8.0]
    A[Scamark.Microservice] -->|Référence| E[Scamark.Framework.Common 2.x]
    A[Scamark.Microservice] -->|Référence| F[Swagger]
    E -->|Référence| G[Serilog]
:::



# Sample 

La solution contient un sample `test/Scamark.Samples.Microservice.WebHost`

## Test clé API $metadata

Pour vérifier que la clé d'API avec les droits de lecture seule fonctionnent sur le endpoint $metadata :

1. Vérifier que la valeur de la config `Scamark:API:Authentication:Disabled = false`
2. Exécuter la requête 

```
curl -v 'http://localhost:5000/v1/$metadata' -H 'X-Api-Key: metadata'
```

## Test clé API POST

```
curl -X 'POST' `
  'https://localhost:5001/v1/Orders' `
  -d '{}'`
  -H 'accept: */*' `
  -H 'X-Api-Key: 5c794d9c-fa40-491a-b962-f364f702d370' `
  -H 'Content-Type: application/json;odata.metadata=minimal;odata.streaming=true' 
```
