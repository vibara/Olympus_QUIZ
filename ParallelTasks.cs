using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class ParallelTasks
{
    private int _numberOfThreads;
    private int _maxNumberOfWrites;
    private FileReadWriter _fileReadWriter;
    private static readonly object _lockObject = new object();
    private ManualResetEvent[] _stopEvents; 

    public ParallelTasks(int maxNumberOfWrites, int numberOfThreads, FileReadWriter fileReadWriter)
    {
        _maxNumberOfWrites = maxNumberOfWrites;
        _numberOfThreads = numberOfThreads;
        _fileReadWriter = fileReadWriter;
        _stopEvents = new ManualResetEvent[_numberOfThreads];
        for (int iEvent = 0; iEvent < _numberOfThreads; iEvent ++)
        {
            _stopEvents[iEvent] = new ManualResetEvent(false);
        }
    }

    public void Run()
    {
        var tasks = new Task[_numberOfThreads];
        for (int iTask = 0; iTask < _numberOfThreads; iTask++)
        {
            var stopEvent = _stopEvents[iTask];
            tasks[iTask] = Task.Run(() => OneTask(_maxNumberOfWrites, _fileReadWriter, stopEvent));
        }
        // NOTE: await Task.WhenAll(tasks); - calls after first Task.Yield; so we need to use events 
        WaitHandle.WaitAll(_stopEvents);
    }

    async void OneTask(int maxNumberOfWrites, FileReadWriter fileReadWriter, ManualResetEvent stopEvent)
    {
        int threadId = Thread.CurrentThread.ManagedThreadId;
        try
        {
            Console.WriteLine($"Task {threadId} started");
            for (int numberOfWrites = 0; numberOfWrites < maxNumberOfWrites; numberOfWrites++)
            {
                lock (_lockObject)
                {
                    fileReadWriter.WriteStep(threadId);
                }
                await Task.Yield();     // NOTE: after that the system can switch to another thread/task 
            }
            Console.WriteLine($"Task {threadId} stopped");
            stopEvent.Set();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in task {threadId}: {ex.Message}");
        }
    }
}
