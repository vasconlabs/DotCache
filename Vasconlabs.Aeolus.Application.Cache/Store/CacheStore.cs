using FASTER.core;

namespace Vasconlabs.Aeolus.Application.Cache.Store;

internal class CacheStore
{
    public FasterKV<ulong, byte[]> Store { get; }

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
        
        // SerializerSettings<long, SpanByte> serializerSettings = new SerializerSettings<long, SpanByte>
        // {
        //     valueSerializer = () => new CacheModelSerializer()
        // };

        Store = new FasterKV<ulong, byte[]>(1 << 20, logSettings, checkpointSettings);
        
        if (Directory.EnumerateFiles(logPath, "aeolus.log*").Any() && Directory.EnumerateFiles(logPath, "aeolus.obj.log*").Any())
        {
            Store.Recover();
        }
    }
}