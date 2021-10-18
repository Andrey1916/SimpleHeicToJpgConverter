using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SimpleHaicToJpgConverter.UI.Console;

namespace SimpleHaicToJpgConverter
{
    static class Program
    {
        private static void Main(string[] args)
        {
            // If have any arguments
            // If set flag -h or --help
            if (args.Length == 0 || args.Contains("--help", "-h"))
            {
                PrintUsing();
                return;
            }

            string source = null;
            string destination = null;
            bool copyOtherFiles = false;

            ushort counter = 0;

            for (ushort i = 0; i < args.Length; ++i)
            {
                // If set only one argument and it is -c or --copy-other
                if (!copyOtherFiles && (args[i] == "-c" || args[i] == "--copy-other"))
                {
                    copyOtherFiles = true;
                }
                else
                {
                    if (counter == 0)
                    {
                        source = args[i];
                        ++counter;
                        continue;
                    }

                    if (counter == 1)
                    {
                        destination = args[i];
                        ++counter;
                        continue;
                    }
                }
            }

            if (source == null)
            {
                PrintUsing();
                return;
            }

            Convert(source, destination, copyOtherFiles);
        }

        static uint counter = 0;
        static CancellationToken _cancellationToken;

        private static void Convert(string source, string destination, bool copyOtherFiles)
        {
            _cancellationToken = new CancellationToken();

            bool isSourceFolder = false;

            if (Directory.Exists(source))
            {
                isSourceFolder = true;
            }
            else
            {
                if (!File.Exists(source))
                {
                    throw new ArgumentException(nameof(source));
                }
            }

            _cancellationToken.ThrowIfCancellationRequested();

            if (isSourceFolder)
            {
                var files = Directory.GetFiles(source);
                var screen = new ProgressWithLogsScreen(files.Length);

                screen.Log($"Found { files.Length } files.");

                var batchs = files.Split((int)(files.Length / 2));

                var tasks = new List<Task>(3);

                foreach(var batch in batchs)
                {
                    var task = ConvertBatch(batch, destination, copyOtherFiles, screen);

                    tasks.Add(task);
                }

                Task.WaitAll(tasks.ToArray());

                screen.Log("Done", files.Length);
            }
            else
            {
                Converter.HeicToJpg(source, destination, copyOtherFiles);
                Console.WriteLine("Done");
            }
        }

        private async static Task ConvertBatch(
            IEnumerable<string> files,
            string destination,
            bool copyOtherFiles,
            ProgressWithLogsScreen screen)
        {
            foreach (string file in files)
            {
                try
                {
                    _cancellationToken.ThrowIfCancellationRequested();

                    var result = await Converter.HeicToJpgAsync(file, destination, copyOtherFiles);

                    screen.Log($"\n--- { file }\n[{ result.ToString() }]", ++counter);
                }
                catch (OperationCanceledException)
                {
                    screen.Log("Operation canceled", ++counter);
                    return;
                }
                catch (FileNotFoundException fnfex)
                {
                    screen.Log($"Error: [{ fnfex.FileName }] " + fnfex.Message, ++counter);
                }
                catch (UnauthorizedAccessException uaex)
                {
                    screen.Log($"Error: " + uaex.Message, ++counter);
                }
                catch (Exception ex)
                {
                    screen.Log("Error: " + ex.Message, ++counter);
                    return;
                }
            }
        } 

        private static void PrintUsing()
        {
            const string instruction = "Converter heic to jpg.\n\n"
                + "Using:\n"
                + "\timconv <source>\n"
                + "\timconv <source> <destination>\n"
                + "\n\n"
                + "-c\t--copy-other\tCopy other files in folder.\n"
                + "-h\t--help\tShow documentation.\n";

            Console.WriteLine(instruction);
        }
    }
}
