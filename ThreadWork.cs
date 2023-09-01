
internal class ThreadWork
{
    private Thread _thread;
    private int _numberOfWrites;
    private int _maxNumberOfWrites;
    private ManualResetEvent _stopEvent;
    private FileReadWriter _fileReadWriter;
    private object _lock;

    public ThreadWork(int maxNumberOfWrites, FileReadWriter fileReadWriter, ManualResetEvent stopEvent, object lockObject)
    {
        _maxNumberOfWrites = maxNumberOfWrites;
        _fileReadWriter = fileReadWriter;
        _stopEvent = stopEvent;
        _lock = lockObject;
        _thread = new Thread(new ThreadStart(ThreadProc));
    }

    public void Start()
    {
        _thread.Start();
    }

    private void ThreadProc()
    {
        Console.WriteLine("Thread started, ID = " + _thread.ManagedThreadId);
        try
        {
            for (; _numberOfWrites < _maxNumberOfWrites; _numberOfWrites++)
            {
                lock (_lock)
                {
                    _fileReadWriter.WriteStep(_thread.ManagedThreadId);
                }
            }
                
            // if we call:
            // Thread.Sleep(10);
            // we had multiple thread switching
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in thread {_thread.ManagedThreadId}: {ex.Message}");
        }
        finally
        {
            _stopEvent.Set();
            Console.WriteLine("Thread stops, ID = " + _thread.ManagedThreadId);
        }
    }
}
