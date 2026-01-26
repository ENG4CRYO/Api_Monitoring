using System.Collections.Concurrent;
using Api_Monitoring.Core;

namespace Api_Monitoring.Services
{
    public class LogBufferService
    {
        private readonly ConcurrentQueue<ApiLog> _logsQueue = new ConcurrentQueue<ApiLog>();


        public void AddLog(ApiLog log)
        {
            _logsQueue.Enqueue(log);    
        }

        public IEnumerable<ApiLog> DequeueBatch(int maxCount = 50)
        {
            var batch = new List<ApiLog>();

            while(batch.Count < maxCount && _logsQueue.TryDequeue(out var log))
            {
                batch.Add(log);
            }

            return batch;
        }

        public int PendingLogsCount => _logsQueue.Count;    
    }
}
