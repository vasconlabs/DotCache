using FASTER.core;

namespace Vasconlabs.Aeolus.Application.Cache.Store;

internal class CacheStore
{
    internal required FasterKV<ulong, byte[]> Store { get; set; }

    private const string logFileName = "aeolus.log";
    private const string objLogFileName = "aeolus.obj.log";

    private readonly string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Aeolus", "logs");
    private readonly string checkpointPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Aeolus", "checkpoints");

    public void InitializeStore()
    {
        LogSettings logSettings = new LogSettings
        {
            LogDevice = Devices.CreateLogDevice(Path.Combine(logPath, logFileName)),
            ObjectLogDevice = Devices.CreateLogDevice(Path.Combine(logPath, objLogFileName)),

        };

        CheckpointSettings checkpointSettings = new CheckpointSettings
        {
            CheckpointDir = checkpointPath 
        };

        Store = new FasterKV<ulong, byte[]>(1 << 20, logSettings, checkpointSettings);

        if (Directory.EnumerateFiles(logPath, $"{logFileName}*").Any() && Directory.EnumerateFiles(logPath, $"{objLogFileName}*").Any())
        {
            Store.Recover();
        }
    }

    public void FlushAllStore()
    {
        Store.Dispose();

        foreach (var file in Directory.EnumerateFiles(logPath, $"{logFileName}*"))
        {
            File.Delete(file);
        }

        foreach (var file in Directory.EnumerateFiles(logPath, $"{objLogFileName}*"))
        {
            File.Delete(file);
        }

        InitializeStore();
    }
}