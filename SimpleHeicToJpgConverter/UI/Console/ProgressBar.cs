using System.Drawing;

namespace SimpleHeicToJpgConverter.UI.Console
{
    public class ProgressBar
    {
        private static object syncObj = new object();
        public const int ElementHeight = 2;
        public const int MinValue = 0;

        private double _max;
        private double _value;

        private Point _position;

        public double Value
        {
            get => _value;
        }

        public double Max
        {
            get => _max;
        }

        public ProgressBar(double max = 100, double value = 0)
        {
            _max = max;
            _value = value;

            _position = new Point
            {
                X = System.Console.CursorLeft,
                Y = System.Console.CursorTop
            };
        }

        private void Print()
        {
            lock(syncObj)
            {
                System.Console.SetCursorPosition(_position.X, _position.Y);

                int width = System.Console.WindowWidth;
                int progress = (int)((_value / _max) * (width - 2));

                string progressValue = $"   { _value }/{ _max }  { ((int)(_value / _max * 100)).ToString() }%";

                System.Console.WriteLine(progressValue + new string(' ', System.Console.WindowWidth - progressValue.Length));
                System.Console.Write("[" + new string('=', progress) + new string(' ', (width - progress - 2)) + "]");
            }
        }

        public void Update(double value)
        {
            _value = value;

            this.Print();
        }

        public void Update(double value, int posX, int posY)
        {
            _position.X = posX;
            _position.Y = posY;

            _value = value;

            this.Print();
        }
    }
}