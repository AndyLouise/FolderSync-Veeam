using System.IO;
using System.Timers;

class FolderSynchronizer
{
    private static string sourceFolderPath = "";
    private static string replicaFolderPath = "";
    private static string logFilePath = "";
    private static int synchronizationInterval = 0;

    static void Main(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Usage: FolderSynchronizer.exe <sourceFolderPath> <replicaFolderPath> <synchronizationIntervalInSeconds> <logFilePath>");
            return;
        }

        Initialize(args);

        Console.WriteLine($"Source Folder: {sourceFolderPath}");
        Console.WriteLine($"Replica Folder: {replicaFolderPath}");
        Console.WriteLine($"Synchronization Interval: {synchronizationInterval} seconds");
        Console.WriteLine($"Log File: {logFilePath}");

        // Initial synchronization
        SynchronizeFolders();

        // Set up periodic synchronization using a timer
        System.Timers.Timer timer = new System.Timers.Timer(synchronizationInterval * 1000);
        timer.Elapsed += TimerElapsed;
        timer.AutoReset = true;
        timer.Start();

        Console.WriteLine("Press 'Q' to quit.");

        // Wait for user to quit
        while (Console.ReadKey().Key != ConsoleKey.Q) { }
    }

    private static void Initialize(string[] args)
    {
        sourceFolderPath = args[0];
        replicaFolderPath = args[1];
        synchronizationInterval = Convert.ToInt32(args[2]);
        logFilePath = args[3];
    }

    private static void TimerElapsed(object? sender, ElapsedEventArgs e)
    {
        SynchronizeFolders();
    }

    private static void SynchronizeFolders()
    {
        try
        {
            LogMessage("Synchronization started.");

            EnsureReplicaFolderExists();

            SynchronizeFiles();

            RemoveExcessFiles();

            LogMessage("Synchronization completed.");
        }
        catch (Exception ex)
        {
            LogMessage($"Error during synchronization: {ex.Message}");
        }
    }

    private static void EnsureReplicaFolderExists()
    {
        // Ensure replica folder exists
        if (!Directory.Exists(replicaFolderPath))
        {
            Directory.CreateDirectory(replicaFolderPath);
            LogMessage($"Replica folder created: {replicaFolderPath}");
        }
    }

    private static void SynchronizeFiles()
    {
        // Synchronize files
        foreach (string sourceFilePath in Directory.GetFiles(sourceFolderPath))
        {
            string fileName = Path.GetFileName(sourceFilePath);
            string replicaFilePath = Path.Combine(replicaFolderPath, fileName);

            File.Copy(sourceFilePath, replicaFilePath, true);
            LogMessage($"Copied: {fileName}");
        }
    }

    private static void RemoveExcessFiles()
    {
        // Remove excess files in replica folder
        foreach (string replicaFilePath in Directory.GetFiles(replicaFolderPath))
        {
            string fileName = Path.GetFileName(replicaFilePath);
            string sourceFilePath = Path.Combine(sourceFolderPath, fileName);

            if (!File.Exists(sourceFilePath))
            {
                File.Delete(replicaFilePath);
                LogMessage($"Removed: {fileName}");
            }
        }
    }

    private static void LogMessage(string message)
    {
        string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
        Console.WriteLine(logEntry);

        // Log to file
        File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
    }
}