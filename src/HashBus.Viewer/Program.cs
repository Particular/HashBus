namespace HashBus.Viewer
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;

    class Program
    {
        static Task Main()
        {
            var webApiBaseUrl = ConfigurationManager.AppSettings["WebApiBaseUrl"];
            var track = ConfigurationManager.AppSettings["Track"];
            var refreshInterval = int.Parse(ConfigurationManager.AppSettings["refreshInterval"]);
            var showPercentages = bool.Parse(ConfigurationManager.AppSettings["ShowPercentages"]);
            var verticalPadding = int.Parse(ConfigurationManager.AppSettings["VerticalPadding"]);
            var horizontalPadding = int.Parse(ConfigurationManager.AppSettings["HorizontalPadding"]);
            var rotateInterval = int.Parse(ConfigurationManager.AppSettings["RotateInterval"]);
            var views = ConfigurationManager.AppSettings["Views"]
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(view => view.Trim())
                .ToArray();

            Console.Title = typeof(Program).Assembly.GetName().Name;

            return App.Run(
                webApiBaseUrl,
                track,
                refreshInterval,
                showPercentages,
                verticalPadding,
                horizontalPadding,
                views,
                rotateInterval);
        }
    }
}
