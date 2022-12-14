namespace StereotypeRecognizer
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Controller Sample.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main for Controller Sample.
        /// </summary>
        /// <param name="args">Arguments.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates WebHost Builder.
        /// </summary>
        /// <param name="args">Arguments.</param>
        /// <returns>Returns IHostbuilder.</returns>
        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
