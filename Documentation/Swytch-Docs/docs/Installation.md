# Installing

## Make sure .Net SDK is available
Swytch a .NET C# web framework hence to use it, you'll need the .NET SDK installed on your machine. If you don't you can
download and install it from the
official [.NET website](https://dotnet.microsoft.com/en-us/download).

Once installed, verify the installation by running:

```sh
 dotnet --version
```

## Create A Project

The foundation of  swytch app is nothing but a  console application. So we shall create a simple boring console app and
then super charge it with all the amazing swytch functionalities.

First create a console application like you always do

> Swytch has starter templates which we shall later take a look at but for now let's set everything up
> from scratch to better appreciate a Swytch up in it's entirety

```sh
dotnet new console -n MyFistSwytchApp
cd MyFirstSwytchApp
```
>You can optionally rename the program.cs file to Server.cs In Swytch, the entry point of your application is Server.cs, which is simply the Program.cs file renamed after creating a console application. This is just a convention for consistency, and you can name the file however you like. Throughout this documentation, whenever you see Server.cs, it refers to the entry point of your application, assuming you started from scratch using the console template.

## Adding Nuget Package

You can easily add Swytch to your project with the following

```sh 
 dotnet add package Swytch 
```

or maybe a specific version with

```sh
 dotnet add package Swytch -Version <number>
```

alternatively you can add the package reference to your .csproj file

```sh
<PackageReference Include="Swytch" Version="<number>" />
```


 [ Next  >>](Quickstart.md)

