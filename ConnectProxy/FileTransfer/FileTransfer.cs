using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FubarDev.FtpServer;
using Microsoft.Extensions.DependencyInjection;

namespace ConnectProxy.FileTransfer
{
    class FileTransfer
    {

        //public string getFileFromRemote(string remoteFileName)
        //{
        //    // Build the service provider
        //    using (var serviceProvider = services.BuildServiceProvider())
        //    {

        //        // Setup dependency injection
        //        var services = new ServiceCollection();

        //        // use %TEMP%/TestFtpServer as root folder
        //        services.Configure<DotNetFileSystemOptions>(opt => opt
        //            .RootPath = Path.Combine(Path.GetTempPath(), "TestFtpServer"));

        //        // Add FTP server services
        //        // DotNetFileSystemProvider = Use the .NET file system functionality
        //        // AnonymousMembershipProvider = allow only anonymous logins
        //        services.AddFtpServer(builder => builder
        //            .UseDotNetFileSystem() // Use the .NET file system functionality
        //            .EnableAnonymousAuthentication()); // allow anonymous logins
        //        // Configure the FTP server
        //        services.Configure<FtpServerOptions>(opt => opt.ServerAddress = "127.0.0.1");
        //        // Initialize the FTP server
        //        var ftpServerHost = serviceProvider.GetRequiredService<IFtpServerHost>();

        //        // Start the FTP server
        //        ftpServerHost.StartAsync(CancellationToken.None).Wait();

        //        Console.WriteLine("Press ENTER/RETURN to close the test application.");
        //        Console.ReadLine();

        //        // Stop the FTP server
        //        ftpServerHost.StopAsync(CancellationToken.None).Wait();
        //    }

        //    private IServiceCollection ftpServer = new ServiceCollection();
        //}

    }
}
