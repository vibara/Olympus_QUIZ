using System.ComponentModel;
using System.Text.RegularExpressions;

internal class FileReadWriter
{
    private string _filePath;
    
    public FileReadWriter(string filePath)
    { 
        this._filePath = filePath; 
    }

    public bool WriteFirstStep()
    {
        using (FileStream fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write))
        {
            if (fileStream.CanWrite)
            {
                WriteLastLineTimeStamp(fileStream, 0, 0);
                return true;
            }
            else
            {
                Console.WriteLine("The file stream is not writable");
                return false;
            }
        }
    }

    public bool WriteStep(int threadId)
    {   
        using (FileStream fileStream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
        // using OpenOrCreate because we need access for writing, just Open doesn't provide it
        {
            if (fileStream.CanRead)
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    string? lastLine = ReadLastLine(reader);
                    // we shouldn't close StreamReader before opening StreamWriter - it gives access errror
                    if (lastLine != null)
                    {
                        if (fileStream.CanWrite)
                        {
                            int? stepNumber = FindFirstNumber(lastLine);
                            if (stepNumber != null)
                            {
                                WriteLastLineTimeStamp(fileStream, stepNumber.Value + 1, threadId);
                            }
                        }
                        else
                        {
                            Console.WriteLine("The file stream is not writable");
                            return false;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("The file stream is not readable");
                return false;
            }
        }
        return true;
    }

    private string? ReadLastLine(StreamReader reader)
    {
        // alternative method is remembering last line position in the file from last thread

        string? lastLine = null;
        string? line = null;
        while ((line = reader.ReadLine()) != null)
        {
            // TODO: Trim before?
            lastLine = line;
        }
        return lastLine;
    }

    private int? FindFirstNumber(string line)
    {
        // alternative method is reading char by char and checking if that is number

        int? firstNumber = null;

        Match match = Regex.Match(line, @"\d+");
        if (match.Success && int.TryParse(match.Value, out int result))
        {
            firstNumber = result;
        }
        return firstNumber;
    }

    private void WriteLastLineTimeStamp(FileStream fileStream, int stepNumber, int threadId)
    {
        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            fileStream.Seek(0, SeekOrigin.End);
            var currentTimeStamp = DateTime.Now;
            var currentTimeStampString = currentTimeStamp.ToString("HH:mm:ss.fff");
            writer.WriteLine($"{stepNumber}, {threadId}, {currentTimeStampString}");
        }
    }
}
