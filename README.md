![GitHub repo size](https://img.shields.io/github/repo-size/pippinmole/Illusive)
![GitHub issues](https://img.shields.io/github/issues/pippinmole/Illusive)
![GitHub](https://img.shields.io/github/license/pippinmole/Illusive)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/pippinmole/Illusive/Build%20and%20deploy%20ASP.Net%20Core%20app%20to%20Azure%20Web%20App%20-%20illusive)
![Twitter Follow](https://img.shields.io/twitter/follow/pippinmole?style=social)

[API Documentation](https://forum.ruffles.pw/api/v1)

# Illusive

A forum website and REST API made on the [ASP.NET Core framework](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-3.1) using [Razor pages](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-5.0&tabs=visual-studio) and [.NET WebAPI](https://dotnet.microsoft.com/apps/aspnet/apis).

## Installation

Clone the repo using Git terminal
```cli
gh repo clone pippinmole/Illusive
```

*or* download [*here*](https://github.com/pippinmole/Illusive/archive/main.zip).

## Usage

You will need to set up some services to obtain connection strings so the application can interact with things like a database.

###### MongoDB
1. Create an account and cluster using [this](https://docs.mongodb.com/guides/cloud/account/) tutorial.
2. Obtain the connection string which should be formatted as such:

    ```cli
    mongodb+srv://<username>:<password>@cluster0.builj.azure.mongodb.net/<dbname>?retryWrites=true&w=majority
    ```

3. Add the app secret to the [appsettings.json](https://github.com/pippinmole/Illusive/blob/main/appsettings.json) file:
    ```json
    "ConnectionStrings": {
      "DatabaseConnectionString": "<api key>"
    }
    ```

###### Serilog

1. Add the app secret to the [appsettings.json](https://github.com/pippinmole/Illusive/blob/main/appsettings.json) file:
    ```json
    "Serilog": {
      "DataDog": {
       "ApiKey": "<api key>"
      }
    }
    ```

###### Content Delivery

###### Mail Handling
You will need access to an SMTP server for password resetting to work, since an email does get sent out to users' accounts in the scenario that they have forgotten their credentials.

To set up email services:
1. Register an SMTP server and obtain the relevant credentials
2. Enter the credentials into the appsettings.json
    ```json
    "MailSenderOptions": {
      "AddressFrom": "",
      "Password": "",
      "SmtpHostAddress": "",
      "Port": ""
    }
    ```

#### Example appsettings.json
```json
{
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "ConnectionStrings": "<api key>",
    "CdnConnectionString": "<api key>"
  },
  "RecaptchaSettings": {
    "ContentSecurityPolicy": "<policy key>",
    "SecretKey": "<api key>",
    "SiteKey": "<api key>"
  },
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.Datadog.Logs", "Serilog.Sinks.File" ],
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information",
        "System.Net.Http.HttpClient": "Warning"
      }
    },
    "Enrich": [
      "FromLogContext"
    ],
    "DataDog": {
      "OverrideLogLevel": "Information"
    },
    "WriteTo": [
      {
        "Name": "Console"
      }
    ]
  }
}
```

## WebAPI

The WebAPI allows for both anonymous and registered users to interact with the forum using GET, POST, DELETE and PUT requests. The [Illusive Public API Documentation](https://forum.ruffles.pw/api/v1/index.html) is automatically generated using [Swagger](https://swagger.io/).
To easily create requests, you can use tools such as [PostMaster](https://www.postmaster.co.uk/) or [Fiddler](https://www.telerik.com/fiddler).

The API can be seen in action here:
[![Image from Gyazo](https://i.gyazo.com/782d545c24bef29bd7dfd396ad9784f5.gif)](https://gyazo.com/782d545c24bef29bd7dfd396ad9784f5)

### Authorization

Some of the requests require account authorization. Thankfully, Illusive offers an secret JWT API token which can be used to identify a user. It can be found under the Account section of the forum.
If you are using one of the tools suggested above, you can add the authorization header as so:

 [![Image from Gyazo](https://i.gyazo.com/feb34909d5b11f4cae8b008744c3ebe1.gif)](https://gyazo.com/feb34909d5b11f4cae8b008744c3ebe1)
 
If you are using the API token elsewhere, simply use the dictionary key-value pair in your header (you may be required to explicitly state the request format is in JSON):
 
 ```
    "Authorization: Bearer <api token>",
    "Content-Type: application/json"
```

## Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D

## Dependencies

| Dependency                                                                                   	| Description                                                                                                                                  	|
|----------------------------------------------------------------------------------------------	|----------------------------------------------------------------------------------------------------------------------------------------------	|
| [Simplmds-markdown-editor](https://github.com/sparksuite/simplemde-markdown-editor)          	| A drop-in JavaScript textarea replacement for writing beautiful and understandable markdown.                                                 	|
| [MongoDB.Driver](https://www.nuget.org/packages/MongoDB.Driver/2.11.3)                       	| A driver for C# application to connect to a [MongoDB](https://www.mongodb.com/) instance                                                     	|
| [Westwind.AspNetCore.Markdown](https://www.nuget.org/packages/Westwind.AspNetCore.Markdown/) 	| A C# library that parses markdown string into HTMLStrings for rendering on HTML pages.                                                       	|
| [BCrypt.Net-Next](https://www.nuget.org/packages/BCrypt.Net-Next/)                           	| A C# encryption library.                                                                                                                     	|
| [Humanizer.Core](https://www.nuget.org/packages/Humanizer.Core)                              	| Extension methods for the DateTime and TimeSpan datatypes in C# to be parsed to human-readable/user interface strings.                       	|
| [Azure.Storage.Blobs](https://www.nuget.org/packages/Azure.Storage.Blobs)                    	| A driver for C# applications to connect to an [Azure Blob](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blobs-introduction). 	|
| [jQuery.AJAX](https://api.jquery.com/Jquery.ajax/)                                           	| A JavaScript library that allows for easy POST, GET, DELETE and PUT requests on client-side webpage.                                         	|
| [party.js](https://partyjs.yiliansource.dev/)                                                	| A JavaScript library to brighten up your user's site experience with visual effects!                                                         	|
| [Serilog.AspNetCore](https://www.nuget.org/packages/Serilog.AspNetCore)                      	| A C# driver for application logging.                                                                                                         	|
| [Serilog.Sinks.Async](https://www.nuget.org/packages/Serilog.Sinks.Async/1.4.1-dev-00073)    	| A C# driver that works with Serilog.AspNetCore to connect to external logging sinks.                                                         	|
| [Serilog.Sinks.Datadog.Logs](https://www.nuget.org/packages/Serilog.Sinks.Datadog.Logs/)     	| A C# driver that works with Serilog.Sinks.Async.                                                                                             	|

## Licence
Copyright © 2020 [pippin_mole](https://github.com/pippinmole).  
Illusive is licensed under [MIT](https://github.com/pippinmole/Illusive/blob/main/LICENSE). Do what you want.