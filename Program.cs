// NOTE: we shouldn'r declare Main function in v.C# > 9
// NOTE: we don't need a namespace

const int numberOfThreads = 10;
const int maxNumberOfWrites = 10;
const string filePath = "/log/out.txt";

FileReadWriter readWriter = new FileReadWriter(filePath);

bool completedFirstStep = false;
try
{
    readWriter.WriteFirstStep();
    completedFirstStep = true;
}
catch (Exception ex)
{
    Console.WriteLine($"Exception when create the file: {ex.Message}");
}

if (completedFirstStep)
{
    try
    {
        var tasks = new ParallelTasks(maxNumberOfWrites, numberOfThreads, readWriter);
        tasks.Run();
        Console.WriteLine("All tasks completed");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception in main thread: {ex.Message}");
    }

}
Console.WriteLine("Press <Enter> to exit");
Console.ReadLine();








