using HtmlGenerator;
using Microsoft.Extensions.DependencyInjection;

namespace HtmlGeneratorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: HtmlGeneratorApp templateFilePath dataFilePath outputFilePath");
                return;
            }

            var serviceProvider = new ServiceCollection()
                .AddHtmlGeneratorServices()
                .BuildServiceProvider();

            var templateFilePath = args[0];
            var dataFilePath = args[1];
            var outputFilePath = args[2];

            var template = File.ReadAllText(templateFilePath);
            var jsonData = File.ReadAllText(dataFilePath);

            var htmlGenerator = serviceProvider.GetRequiredService<IGenerator>();
            var resultHtml = htmlGenerator.CreateHtml(template, jsonData);

            File.WriteAllText(outputFilePath, resultHtml);

            Console.WriteLine("HTML generation completed. Output saved to: " + outputFilePath);
        }
    }
}
