namespace HashBus.Viewer.MostHashtagged
{
    using System;
    using System.Configuration;

    class Program
    {
        static void Main()
        {
            var webApiBaseUrl = ConfigurationManager.AppSettings["WebApiBaseUrl"];
            var track = ConfigurationManager.AppSettings["Track"];
            var refreshInterval = int.Parse(ConfigurationManager.AppSettings["refreshInterval"]);
            var showPercentages = bool.Parse(ConfigurationManager.AppSettings["ShowPercentages"]);
            var verticalPadding = int.Parse(ConfigurationManager.AppSettings["VerticalPadding"]);
            var horizontalPadding = int.Parse(ConfigurationManager.AppSettings["HorizontalPadding"]);

            Console.Title = typeof(Program).Assembly.GetName().Name;

            App.RunAsync(webApiBaseUrl, track, refreshInterval, showPercentages, verticalPadding, horizontalPadding)
                .GetAwaiter().GetResult();
        }
    }
}
