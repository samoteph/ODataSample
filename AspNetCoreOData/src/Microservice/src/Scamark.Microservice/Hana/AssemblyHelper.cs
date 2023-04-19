using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Scamark.Microservice.Hana;

public class AssemblyHelper
{
    /// <summary>
    /// Charge Sap.Data.Hana.Core.v2.1.dll depuis le répertoire lib/linux ou lib/windows
    /// sous le répertoire <see cref="AppDomain.CurrentDomain.BaseDirectory"/>
    /// </summary>
    /// <returns></returns>
    public static void LoadFromBaseDirectory()
    {
        var appPath = AppDomain.CurrentDomain.BaseDirectory;
        var libPath = System.IO.Path.Combine(appPath, "lib", RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" : "windows");
        InitHanaLibs(libPath);
        SetEnvVariable(libPath);
    }

    private static void InitHanaLibs(string libPath)
    {
        var hanaDll = "Sap.Data.Hana.Core.v2.1.dll";
        //var hanaDll = $"{LinqToDB.DataProvider.SapHana.SapHanaProviderAdapter.AssemblyName}.dll";
        var hanaDllPath = System.IO.Path.Combine(libPath, hanaDll);
        if (File.Exists(hanaDllPath))
        {
            Serilog.Log.Information("Assembly {AssemblyName} loaded", hanaDllPath);
            Assembly.LoadFrom(hanaDllPath);
        }
    }

    private static void SetEnvVariable(string libPath)
    {
        // cette variable est attendue par le composant pour chercher libadonetHDB.dll / libadonetHDB.so
        var envVariable = Environment.GetEnvironmentVariable("HDBDOTNETCORE");
        if (string.IsNullOrEmpty(envVariable) == true)
        {
            Environment.SetEnvironmentVariable("HDBDOTNETCORE", libPath);
            envVariable = libPath;
        }

        Serilog.Log.Debug($"ENV variable HDBDOTNETCORE set to {envVariable} on OS {RuntimeInformation.OSDescription}");
    }
}
