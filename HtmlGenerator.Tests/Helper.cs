using System.Reflection;

namespace HtmlGenerator.Tests;

public class Helper
{
    public T InvokePrivateMethod<T>(object instance, string methodName, params object[] parameters)
    {
        var methodInfo = instance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
        if (methodInfo == null)
        {
            throw new InvalidOperationException($"Private method '{methodName}' not found.");
        }

        return (T)methodInfo.Invoke(instance, parameters);
    }

    public string LoadTemplateFromFile(string fileName, string path)
    {
        var assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var dataFolderPath = Path.Combine(Path.GetDirectoryName(assemblyLocation), path);
        var filePath = Path.Combine(dataFolderPath, fileName);
        return File.ReadAllText(filePath);
    }
}
