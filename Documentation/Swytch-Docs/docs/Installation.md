# Installing

## Make sure .Net SDK is available
Swytch a .NET C# Web framework, hence to use it you'll need the .NET SDK installed on your machine. If you don't, you can
download and install it from the
official [.NET website](https://dotnet.microsoft.com/en-us/download).

Once installed, verify the installation by running:

```sh
 dotnet --version
```

## Create A Project

The foundation of  swytch app is nothing but a console application. So we shall create a simple boring console app and
then supercharge it with all the amazing features Swytch has to offer.

> Note that Swytch has starter templates that can be used to quickly bootstrap a project and get up and running quickly.
> We shall later take a look at that under the [Guide](#) section but  for now let's set up everything from the ground up
> to have a better perspective into what makes a Swytch application.

First create a console application like you always do:
```sh
dotnet new console -n MyFistSwytchApp
cd MyFirstSwytchApp
```
>Inside the `MyFirstSwytchApp` directory, you can optionally rename the `Program.cs` file to `Server.cs` In Swytch, this will be the entry point of your application.
> This is just a convention for consistency, and you can name the file however you like. However throughout this documentation,
> this covention will be used and whenever you see `Server.cs` you should know that this is just the entry point 
> of your application just like the `Program.cs` file you're used to.

## Adding Nuget Package

You can easily add Swytch to your project using the [Nuget package](https://www.nuget.org/packages/Swytch/#dependencies-body-tab).

```sh 
 dotnet add package Swytch 
```

or maybe a specific version with

```sh
 dotnet add package Swytch -Version <number>
```

alternatively you can add the package reference to your project's `.csproj` file

```sh
<PackageReference Include="Swytch" />
```


 [ Next  >>](Quickstart.md)

