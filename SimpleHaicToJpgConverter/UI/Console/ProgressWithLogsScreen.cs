namespace SimpleHaicToJpgConverter.UI.Console
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
            System.Console.WriteLine('\n');

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
    }
}