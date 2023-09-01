// NOTE: we shouldn'r declare Main function in v.C# > 9
// NOTE: we don't need a namespace
// NOTE: using Thread, not Task - as in the description, it should be multiple threads, but multiple tasks can run on a single thread


using System.Diagnostics.Metrics;
using System.Threading;
const int numberOfThreads = 10;
const int maxNumberOfWrites = 10;
const string filePath = "/log/out.txt";
object lockObject = new object();
FileReadWriter readWriter = new FileReadWriter(filePath);

bool completedFirstStep = false;
try
{
    completedFirstStep = readWriter.WriteFirstStep();
}
catch (Exception ex)
{
    Console.WriteLine($"Exception when create the file: {ex.ToString()}");
}

if (completedFirstStep)
{
    try
    {
        ThreadWork[] works = new ThreadWork[numberOfThreads];
        ManualResetEvent[] stopEvents = new ManualResetEvent[numberOfThreads];

        for (int i = 0; i < numberOfThreads; i++)
        {
            stopEvents[i] = new ManualResetEvent(false);
            works[i] = new ThreadWork(maxNumberOfWrites, readWriter, stopEvents[i], lockObject);
        }

        for (int i = 0; i < numberOfThreads; i++)
        {
            works[i].Start();
        }

        WaitHandle.WaitAll(stopEvents);
        Console.WriteLine("All threads stopped");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception in main thread: {ex.Message}");
    }
}
Console.WriteLine("Press <Enter> to exit");
Console.Read();







