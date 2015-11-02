using System;
using System.Configuration;

namespace HashBus.Projection.UserLeaderboard
{
    class Program
    {
        static void Main()
        {
            var dataFolder = ConfigurationManager.AppSettings["DataFolder"];
            if (dataFolder == null)
            {
                throw new ArgumentException("Please make sure you have the 'DataFolder' set in your appSettings");
            }

            App.RunAsync(dataFolder).GetAwaiter().GetResult();
        }
    }
}
