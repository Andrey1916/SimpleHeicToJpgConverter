using System;
using System.Text;

namespace SimpleHeicToJpgConverter.UI.Console
{
    public class ProgressWithLogsScreen
    {
        private readonly ProgressBar _consoleProgressBar;
        private int _logPosition;
        private uint _intervalBetweenLogsAndProgressBar = 1;
        private static object syncObj = new object();

        public double CurrentProgress
            => _consoleProgressBar.Value;

        public double MaxValue
            => _consoleProgressBar.Max;


        public ProgressWithLogsScreen(double maxValue)
        {
            System.Console.CursorVisible = false;
            System.Console.WriteLine(System.Environment.NewLine);

            _consoleProgressBar = new ProgressBar(maxValue);
        }

        public void Log(string log)
            => Log(log, CurrentProgress);

        public void Log(string log, double value)
        {
            lock(syncObj)
            {
                _logPosition = System.Console.WindowHeight - ProgressBar.ElementHeight - 2 + 1;

                // Print logs
                System.Console.SetCursorPosition(0, _logPosition);

                log = PrepareLogMessage(log, System.Console.WindowWidth);

                System.Console.WriteLine(log);

                // Print interval
                for (int i = 0; i < _intervalBetweenLogsAndProgressBar; ++i)
                {
                    System.Console.WriteLine(
                        new string(' ', System.Console.WindowWidth)
                        );
                }

                // Print progress bar
                int pbPosY = System.Console.CursorTop;
                _consoleProgressBar.Update(value, 0, pbPosY);
            }
        }

        private static string PrepareLogMessage(string log, int consoleWidth)
        {
            var builder = new StringBuilder();
            string[] lines = log.Split(Environment.NewLine);

            foreach(var line in lines)
            {
                int diff = consoleWidth - line.Length;

                if (diff == 0)
                {
                    builder.AppendLine(line);
                    break;
                }

                if (diff > 0)
                {
                    builder.Append(line);
                    builder.AppendLine(
                        new string(' ', diff)
                        );
                }
                else // < 0
                {
                    diff = -diff;

                    while (diff > consoleWidth)
                    {
                        diff = Math.Abs(consoleWidth - diff);
                    }

                    builder.Append(line);
                    builder.AppendLine(
                        new string(' ', diff)
                        );
                }
            }

            return builder.ToString();
        }
    }
}