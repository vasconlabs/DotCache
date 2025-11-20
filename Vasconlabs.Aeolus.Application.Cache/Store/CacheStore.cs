using FASTER.core;
using Vasconlabs.Aeolus.Application.Cache.Serializer;
using Vasconlabs.Aeolus.Domain.Contracts.Cache;

namespace Vasconlabs.Aeolus.Application.Cache.Store;

internal class CacheStore
{
    public FasterKV<ulong, CacheModel> Store { get; }

    public CacheStore()
    {
        string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Aeolus", "logs");
        string checkpointPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Aeolus", "checkpoints");
        
        LogSettings logSettings = new LogSettings
        {
            LogDevice = Devices.CreateLogDevice(Path.Combine(logPath, "aeolus.log")),
            ObjectLogDevice = Devices.CreateLogDevice(Path.Combine(logPath, "aeolus.obj.log")),
            
        };
        
        CheckpointSettings checkpointSettings = new CheckpointSettings
        {
            CheckpointDir = checkpointPath
        };
        
        SerializerSettings<ulong, CacheModel> serializerSettings = new SerializerSettings<ulong, CacheModel>
        {
            valueSerializer = () => new CacheModelSerializer()
        };

        Store = new FasterKV<ulong, CacheModel>(1 << 20, logSettings, checkpointSettings, serializerSettings);
        
        if (Directory.EnumerateFiles(logPath, "aeolus.log*").Any() && Directory.EnumerateFiles(logPath, "aeolus.obj.log*").Any())
        {
            Store.Recover();
        }
    }
}