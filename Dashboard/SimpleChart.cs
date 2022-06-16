using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dashboard
{
    
    public partial class SimpleChart : UserControl
    {
        Dictionary<string, List<Tuple<float, float>>>? Data { get; set; } =
            new Dictionary<string, List<Tuple<float, float>>>()
            {
                { "test1", new List<Tuple<float, float>>()
                    {
                        new Tuple<float, float>(1, 1),
                        new Tuple<float, float>(2, 30),
                        new Tuple<float, float>(3, 100),
                    }
                }
            };

        List<Color> linesColors = new List<Color>()
        {
            Color.FromArgb(255, 255, 0, 0),
            Color.FromArgb(255, 0, 255, 0),
            Color.FromArgb(255, 0, 0, 255),
        };

        List<Color> pointColors = new List<Color>()
        {
            Color.FromArgb(255, 155, 0, 0),
            Color.FromArgb(255, 0, 155, 0),
            Color.FromArgb(255, 0, 0, 155),
        };

        public SimpleChart()
        {
            InitializeComponent();

            Draw();
        }

        public void SetData(Dictionary<string, List<Tuple<float, float>>>? data)
        {
            Data = data;
            Draw();
        }

        public void Draw()
        {
            var graphics = CreateGraphics();
            graphics.Clear(Color.White);

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            if (Data == null || Data.Count == 0) return;

            var max1 = Data.Max(i => i.Value.Max(j => j.Item1));
            var max2 = Data.Max(i => i.Value.Max(j => j.Item2));

            graphics.DrawLine(Pens.Black, MathToDevice(new PointF(0, 0)), MathToDevice(new PointF(0, ChartHeight - ChartHeight / 10)));
            graphics.DrawLine(Pens.Black, MathToDevice(new PointF(0, 0)), MathToDevice(new PointF(ChartWidth - ChartWidth / 5, 0)));

            var i = 0;
            foreach (var line in Data)
            {
                PointF? prevousPoint = null;
                foreach (var point in line.Value)
                {
                    var p = new PointF(point.Item1, point.Item2);

                    if (prevousPoint.HasValue)
                    {
                        graphics.DrawLine(new Pen(linesColors[i % linesColors.Count], 2), MathToDevice(prevousPoint.Value), MathToDevice(p));
                    }

                    prevousPoint = p;
                }

                i++;
            }

            i = 0;
            foreach (var line in Data)
            {
                foreach (var point in line.Value)
                {
                    var p = new PointF(point.Item1, point.Item2);
                    var ellipseSize = new Size(8, 8);
                    graphics.FillEllipse(new SolidBrush(pointColors[i % pointColors.Count]),
                        new RectangleF(EllipseDelta(MathToDevice(p), ellipseSize), ellipseSize));
                }

                i++;
            }
        }

        int ChartHeight = 200;
        int ChartWidth = 5;

        int ScaleLevelX { get => this.Width / ChartWidth; }
        int ScaleLevelY { get => this.Height / ChartHeight; }

        float PaddingLeft { get => (float)ChartWidth / 10; }
        float PaddingBottom { get => (float)ChartHeight / 10; }

        Point MathToDevice(PointF point)
        {
            var x = point.X * ScaleLevelX + PaddingLeft * ScaleLevelX;
            var y = this.Height - (point.Y * ScaleLevelY + PaddingBottom * ScaleLevelY);

            return new Point((int)x, (int)y);
        }

        Point EllipseDelta(Point point, Size size) =>
            new Point(point.X - size.Width / 2, point.Y - size.Height / 2);

        private void SimpleChart_VisibleChanged(object sender, EventArgs e)
        {
            Draw();
        }

        //PointF DeviceToMath(Point point)
        //{
        //    //var x = point.X * ScaleLevelX + PaddingLeft;
        //    var x = (point.X - PaddingLeft) / ScaleLevelX;
        //    //var y = this.Height - (point.Y * ScaleLevelY + PaddingBottom);
        //    var y = (this.Height - PaddingBottom - point.Y) / ScaleLevelY;

        //    return new Point((int)x, (int)y);
        //}
    }
}
