using FASTER.core;
using Vasconlabs.Aeolus.Domain.Contracts.Cache;

namespace Vasconlabs.Aeolus.Application.Cache.Store;

internal class CacheStore
{
    public FasterKV<string, CacheModel> Store { get; }

    public CacheStore()
    {
        string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Aeolus", "logs");
        string checkpointPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Aeolus", "checkpoints");
        
        LogSettings logSettings = new LogSettings
        {
            LogDevice = Devices.CreateLogDevice(Path.Combine(logPath, "aeolus.log")),
            ObjectLogDevice = Devices.CreateLogDevice(Path.Combine(logPath, "aeolus.obj.log"))
        };
        
        CheckpointSettings checkpointSettings = new CheckpointSettings
        {
            CheckpointDir = checkpointPath
        };

        Store = new FasterKV<string, CacheModel>(1 << 20, logSettings, checkpointSettings);
        
        if (Directory.EnumerateFiles(logPath, "aeolus.log*").Any() && Directory.EnumerateFiles(logPath, "aeolus.obj.log*").Any())
        {
            Store.Recover();
        }
    }
}