using Newtonsoft.Json;
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
        Dictionary<string, List<PointF>>? Data { get; set; } =
            new Dictionary<string, List<PointF>>()
            {
                //{ "test1", new List<Tuple<float, float>>()
                //    {
                //        new Tuple<float, float>(1, 1),
                //        new Tuple<float, float>(2, 30),
                //        new Tuple<float, float>(3, 100),
                //    }
                //}
            };

        List<PointF>? UpperBound { get; set; } =
            new List<PointF>()
            {
                new PointF(0, 70),
                new PointF(1, 80),
                new PointF(2, 100),
                new PointF(3, 140),
            };

        List<PointF>? LowerBound { get; set; } =
            new List<PointF>()
            {
                new PointF(3, 10),
                new PointF(2, 30),
                new PointF(1, 45),
                new PointF(0, 20),
            };

        List<Color> linesColors = new List<Color>()
        {
            Color.FromArgb(255, 255, 0, 0),
            Color.FromArgb(255, 0, 255, 0),
            Color.FromArgb(255, 0, 255, 50),
        };

        List<Color> pointColors = new List<Color>()
        {
            Color.FromArgb(255, 155, 0, 0),
            Color.FromArgb(255, 0, 155, 0),
            Color.FromArgb(255, 0, 155, 50),
        };

        bool IsConfirmed { get; set; } = false;

        public SimpleChart()
        {
            InitializeComponent();

            Draw();
        }

        private Brush GetBrush()
        {
            return IsConfirmed ? new SolidBrush(Color.FromArgb(80, 0, 255, 0)) : new SolidBrush(Color.FromArgb(80, 255, 0, 0));
        }
        public void SetData(Dictionary<string, List<PointF>>? data,
            List<PointF>? upperBound = null, List<PointF>? lowerBound = null, bool isConfirmed = false)
        {
            Data = data;
            UpperBound = upperBound;
            LowerBound = lowerBound;
            IsConfirmed = isConfirmed;
            Draw();
        }

        public void Draw()
        {
            var graphics = CreateGraphics();
            graphics.Clear(Color.White);

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            if (Data == null || Data.Count == 0) return;

            var upperBound = UpperBound?.Select(p => new PointF(p.X, p.Y)).ToList();
            var lowerBound = LowerBound?.Select(p => new PointF(p.X, p.Y)).ToList();
            lowerBound?.Reverse();

            var max1 = Data.Max(i => i.Value.Max(j => j.X));
            var max2 = Data.Max(i => i.Value.Max(j => j.Y));

            ChartWidth = (int)max1 + (int)(max1 / 4);
            if (ChartWidth < 3)
                ChartWidth = 3;

            ChartHeight = (int)max2 + (int)(max2 / 4);

            var yAxisLength = ChartHeight - PaddingBottom;
            var xAxisLength = ChartWidth - PaddingLeft * 2;
            graphics.DrawLine(Pens.Black, MathToDevice(new PointF(0, 0)), MathToDevice(new PointF(0, yAxisLength)));
            graphics.DrawLine(Pens.Black, MathToDevice(new PointF(0, 0)), MathToDevice(new PointF(xAxisLength, 0)));

            int minBy10 = 0;
            while (minBy10 == 0 || yAxisLength / minBy10 >= 9)
                minBy10 += 5;

            for (var j = minBy10; j < yAxisLength; j += minBy10)
            {
                var ellipseSize = new Size(4, 4);
                var devicePoint = MathToDevice(new Point(0, j));
                graphics.FillEllipse(Brushes.Black,
                    new RectangleF(EllipseDelta(devicePoint, ellipseSize), ellipseSize));

                graphics.DrawString(j.ToString(), new Font("courier", 9), Brushes.Black, new PointF(devicePoint.X - 32, devicePoint.Y - 10));
            }

            var i = 0;
            foreach (var line in Data)
            {
                PointF? prevousPoint = null;
                foreach (var point in line.Value)
                {
                    var p = new PointF(point.X, point.Y);

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
                    var p = new PointF(point.X, point.Y);

                    var ellipseSize = new Size(8, 8);
                    graphics.FillEllipse(new SolidBrush(pointColors[i % pointColors.Count]),
                        new RectangleF(EllipseDelta(MathToDevice(p), ellipseSize), ellipseSize));
                }

                i++;
            }

            //Func<GraphicsPath, PathGradientBrush> getPathBrush = path =>
            //{
            //    try
            //    {
            //        var confirmedColor1 = Color.FromArgb(125, 0, 255, 0);
            //        var confirmedColor2 = Color.FromArgb(125, 200, 255, 200);
            //        var rejectedColor1 = Color.FromArgb(125, 255, 0, 0);
            //        var rejectedColor2 = Color.FromArgb(125, 255, 200, 200);

            //        var pthGrBrush = new PathGradientBrush(new GraphicsPath());

            //        pthGrBrush.CenterColor = IsConfirmed ? confirmedColor1 : rejectedColor1;
            //        pthGrBrush.CenterPoint = new PointF((this.Width / 2), 0);
            //        Color[] colors = { IsConfirmed ? confirmedColor2 : rejectedColor2 };
            //        pthGrBrush.SurroundColors = colors;
            //        return pthGrBrush;
            //    }
            //    catch { return null; }
            //};

            bool upperMoreThanLower = upperBound?.All((u) => u.Y > lowerBound?.FirstOrDefault(l => l.X == u.X).Y) == true;

            var path = new GraphicsPath();
            PointF? previousPoint = null;
            var countPointsForCurve = 8;
            if (upperBound != null)
            {
                var drawed = upperBound.ToList();
                if (!upperMoreThanLower)
                {
                    var lastPoint = upperBound.Last();
                    var nextPoint = new PointF(lastPoint.X, 0);
                    var zeroPoint = new PointF(0, 0);

                    drawed.Add(lastPoint);
                    drawed.Add(nextPoint);
                    drawed.Add(zeroPoint);
                }

                if (upperBound.Count >= countPointsForCurve)
                    path.AddCurve(drawed.Select(p => MathToDevice(p)).ToArray());
                else
                    path.AddLines(drawed.Select(p => MathToDevice(p)).ToArray());

                if (lowerBound == null)
                {
                    //path.AddLine(MathToDevice(new PointF(upperBound.LastOrDefault().X, 0)), MathToDevice(new PointF(0, 0)));
                }

                if (!upperMoreThanLower)
                {
                    path.CloseFigure();
                    graphics.FillPath(GetBrush(), path);
                    path = new GraphicsPath();
                }
            }

            if (lowerBound != null)
            {
                var drawed = lowerBound.ToList();
                if (!upperMoreThanLower)
                {
                    var lastPoint = lowerBound.Last();

                    drawed.Add(lastPoint);
                    drawed.Add(new PointF(lastPoint.X, this.Height));
                    drawed.Add(new PointF(0, this.Height));
                }

                if (lowerBound.Count >= countPointsForCurve)
                    path.AddCurve(drawed.Select(p => MathToDevice(p)).ToArray());
                else
                    path.AddLines(drawed.Select(p => MathToDevice(p)).ToArray());

                //if (upperBound == null)
                //{
                //    path.AddLine(MathToDevice(new PointF(lowerBound.LastOrDefault().X, ChartHeight + 1)),
                //        MathToDevice(new PointF(lowerBound.FirstOrDefault().X, ChartHeight + 1)));
                //}

                if (!upperMoreThanLower)
                {
                    path.CloseFigure();
                    graphics.FillPath(GetBrush(), path);
                    path = new GraphicsPath();
                }
            }

            if ((upperBound != null || lowerBound != null) && upperMoreThanLower)
            {
                path.CloseFigure();
                graphics.FillPath(GetBrush(), path);
            }

            previousPoint = null;
            if (upperBound?.Count >= countPointsForCurve)
            {
                graphics.DrawCurve(new Pen(new SolidBrush(pointColors[1])), upperBound.Select(p => MathToDevice(p)).ToArray());
            }
            else
                upperBound?.ForEach(p => {
                    if (previousPoint != null)
                        graphics.DrawLine(new Pen(new SolidBrush(pointColors[1])),
                            MathToDevice(previousPoint.Value),
                            MathToDevice(p));

                    previousPoint = p;
                    var ellipseSize = new Size(8, 8);
                    graphics.FillEllipse(new SolidBrush(pointColors[1]),
                        new RectangleF(EllipseDelta(MathToDevice(p), ellipseSize), ellipseSize));
                });

            previousPoint = null;
            if (lowerBound?.Count >= countPointsForCurve)
            {
                graphics.DrawCurve(new Pen(new SolidBrush(pointColors[2])), lowerBound.Select(p => MathToDevice(p)).ToArray());
            }
            else
                lowerBound?.ForEach(p => {
                    if (previousPoint != null)
                        graphics.DrawLine(new Pen(new SolidBrush(pointColors[2])),
                            MathToDevice(previousPoint.Value),
                            MathToDevice(p));

                    previousPoint = p;
                    var ellipseSize = new Size(8, 8);
                    graphics.FillEllipse(new SolidBrush(pointColors[2]),
                        new RectangleF(EllipseDelta(MathToDevice(p), ellipseSize), ellipseSize));
                });
        }

        int ChartHeight = 200;
        int ChartWidth = 5;

        int ScaleLevelX { get => this.Width / ChartWidth; }
        int ScaleLevelY { get => this.Height / ChartHeight; }

        float PaddingLeft { get => (float)ChartWidth / 20; }
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
    }
}
