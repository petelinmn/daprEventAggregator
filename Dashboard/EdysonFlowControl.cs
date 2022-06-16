using Common;
using EventAggregator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorkerManager.Actors.Contract;

namespace Dashboard
{
    public partial class EdysonFlowControl : UserControl
    {
        public EdysonFlowControl()
        {
            InitializeComponent();
        }

        public int Counter { get; set; } = 0;
        private Guid SelectedObject = Guid.Empty;

        public delegate void OnSelectObjectEventHandler(object? sender, OnSelectObjectEventArgs e);
        public event OnSelectObjectEventHandler OnSelectObject;

        private Point GetCenterOfRectangle(Rectangle rect)
        {
            return new Point(rect.X + (int)(rect.Width/2), rect.Y + (int)(rect.Height / 2));
        }

        public void SelectObject(Guid selectedObjectGuid)
        {
            if (selectedObjectGuid != SelectedObject)
            {
                SelectedObject = selectedObjectGuid;
                Draw();
            }
        }

        List<Event> Events { get; set; } = new List<Event>();
        List<WorkerInfo> WorkerInfos { get; set; }

        Dictionary<Guid, Rectangle> PaintedEvents = new Dictionary<Guid, Rectangle>();
        Dictionary<Guid, Rectangle> PaintedWorkers = new Dictionary<Guid, Rectangle>();

        public void SetData(List<Event> events, List<WorkerInfo> workersInfo)
        {
            Events = events;
            WorkerInfos = workersInfo;
            Draw();
        }

        private void Draw()
        {
            var list = new ArrayList();
            list.AddRange(Events);
            list.AddRange(WorkerInfos);
            list.Sort(new EventAndWorkerInfoComparer());

            var graphics = CreateGraphics();
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            DrawCounter(graphics);

            var previousEvents = new List<Event>();
            var previousWorkers = new List<WorkerInfo>();
            var currentColumnX = 80;
            var currentRowY = 20;
            PaintedEvents = new Dictionary<Guid, Rectangle>();
            PaintedWorkers = new Dictionary<Guid, Rectangle>();
            var workerEventRelations = new Dictionary<Guid, Guid>();
            var currentColumnWidth = 0;
            foreach (var obj in list)
            {
                var isEvent = obj is Event;
                Event? @event = isEvent ? obj as Event : null;
                WorkerInfo? workerInfo = !isEvent ? obj as WorkerInfo : null;

                if (@event != null)
                {
                    //Is event complex?
                    var isEventComplex = @event.Parents.Length > 0;
                    if (isEventComplex)
                    {
                        var parentRects = PaintedEvents.Where(i => @event.Parents.Contains(i.Key))
                            .Select(i => i.Value).TakeLast(2).ToList();

                        var maxWidth = -1;
                        var sumY = 0;
                        foreach (var parentRect in parentRects)
                        {
                            if (parentRect.Width > maxWidth)
                                maxWidth = parentRect.Width;

                            sumY += parentRect.Y;
                        }

                        var avgHeight = sumY / parentRects.Count;
                        currentColumnX += currentColumnWidth + 5;
                        currentColumnWidth = 0;

                        if (avgHeight * 1.5 > this.Height)
                            currentRowY = 30;
                        else
                            currentRowY = avgHeight + 5;
                    }

                    int width = @event.Name.Length * 12 - (@event.Name.Length > 10 ? @event.Name.Length : -1);
                    var rect = new Rectangle(currentColumnX, currentRowY, width, 32);

                    PaintedEvents.Add(@event.Id, rect);

                    if (rect.Width > currentColumnWidth)
                        currentColumnWidth = rect.Width + 20;
                }
                else if (workerInfo != null)
                {
                    var parent = Events.FirstOrDefault(e => e.Name == workerInfo.SourceName);
                    var paintedEvent = PaintedEvents.FirstOrDefault(i => i.Key == parent?.Id);
                    currentColumnX = paintedEvent.Value.X + paintedEvent.Value.Width + 10;

                    if (currentRowY + 30 > this.Height)
                        currentRowY = 0;

                    currentRowY += 25;

                    string title = GetDisplayTitleForWorker(workerInfo);
                    int width = title.Length * 12 - (title.Length > 10 ? title.Length : -1);
                    var rect = new Rectangle(currentColumnX, currentRowY, width, 32);

                    PaintedWorkers.Add(workerInfo.Id, rect);
                    workerEventRelations.Add(workerInfo.Id, parent.Id);
                }

                currentRowY += 50;
            }

            graphics.Clear(this.BackColor);

            //Draw relation lines
            foreach (var paintedEvent in PaintedEvents)
            {
                var @event = Events.FirstOrDefault(i => i.Id == paintedEvent.Key);
                if (@event?.Parents?.Any() != true)
                    continue;

                foreach (var parentId in @event.Parents)
                {
                    if (PaintedEvents.ContainsKey(parentId))
                    {
                        var paintedParent = PaintedEvents[parentId];
                        var point1 = GetCenterOfRectangle(paintedParent);
                        var point2 = GetCenterOfRectangle(paintedEvent.Value);
                        graphics.DrawLine(new Pen(Color.DarkGreen, 2), point1, point2);
                    }
                }

                foreach (var relation in workerEventRelations)
                {
                    var parentRect = PaintedEvents.FirstOrDefault(i => i.Key == relation.Value).Value;
                    var workerRect = PaintedWorkers[relation.Key];
                    var point1 = GetCenterOfRectangle(parentRect);
                    var point2 = GetCenterOfRectangle(workerRect);
                    graphics.DrawLine(new Pen(Color.Red, 2), point1, point2);
                }
            }

            //Draw events
            foreach (var paintedEvent in PaintedEvents)
            {
                DrawEvent(graphics, Events.FirstOrDefault(i => i.Id == paintedEvent.Key), paintedEvent.Value);
            }

            //Draw workers
            foreach (var paintedWorker in PaintedWorkers)
            {
                DrawWorker(graphics, WorkerInfos.FirstOrDefault(i => i.Id == paintedWorker.Key), paintedWorker.Value);
            }
        }

        private static string GetDisplayTitleForWorker(WorkerInfo? workerInfo)
        {
            var title = workerInfo?.Args[0];
            if (workerInfo?.Data?.Result != null)
            {
                if (int.TryParse(workerInfo.Data.Result, out var progress))
                    title = $"{workerInfo?.Args[0]}  {progress * 10}%";
            }

            return title;
        }

        private void DrawEvent(Graphics graphics, Event? @event, Rectangle rect)
        {
            var brush = @event.Id == SelectedObject ? Brushes.Red : Brushes.Bisque;
            graphics.FillPath(brush, RoundedRect(rect, 10));
            graphics.DrawPath(Pens.Black, RoundedRect(rect, 10));
            graphics.DrawString(@event.Name, new Font("courier", 12), Brushes.Black, new PointF(rect.X + 5, rect.Y + 5));
        }

        private void DrawWorker(Graphics graphics, WorkerInfo workerInfo, Rectangle rect)
        {
            var brush = workerInfo.Status == WorkerStatus.Init
                ? Brushes.LightYellow
                : workerInfo.Status == WorkerStatus.Work ? Brushes.Yellow
                : Brushes.YellowGreen;

            if (workerInfo.Id == SelectedObject)
                brush = Brushes.Red;
            graphics.FillPath(brush, RoundedRect(rect, 10));
            graphics.DrawPath(Pens.Black, RoundedRect(rect, 10));

            string title = GetDisplayTitleForWorker(workerInfo);
            graphics.DrawString(title, new Font("courier", 12), Brushes.Black, new PointF(rect.X + 5, rect.Y + 5));
        }

        GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        private void DrawCounter(Graphics graphics)
        {
            int width = 20;
            if (Counter > 9)
                width = 30;
            if (Counter > 99)
                width = 40;

            graphics.FillRectangle(Brushes.LightCoral, new Rectangle(20, 20, width, 20));
            graphics.DrawString((++Counter).ToString(), new Font("Courier new", 12), Brushes.Black, new PointF(20, 20));
        }

        private void EdysonFlowControl_Click(object sender, EventArgs e)
        {
            foreach (var item in PaintedEvents)
            {
                if (IsPointInsideRect(item.Value, new Point(MousePositionRelativeToThis.X, MousePositionRelativeToThis.Y)))
                {
                    OnSelectObject?.Invoke(this, new OnSelectObjectEventArgs()
                    {
                        SelectedGuid = item.Key
                    });
                    return;
                }
            }

            foreach (var item in PaintedWorkers)
            {
                if (IsPointInsideRect(item.Value, new Point(MousePositionRelativeToThis.X, MousePositionRelativeToThis.Y)))
                {
                    OnSelectObject?.Invoke(this, new OnSelectObjectEventArgs()
                    {
                        SelectedGuid = item.Key
                    });
                    return;
                }
            }
        }

        private bool IsPointInsideRect(Rectangle rect, Point point)
        {
            if (point.X <= rect.X || point.Y <= rect.Y)
                return false;

            if (point.X >= rect.X + rect.Width || point.Y >= rect.Y + rect.Height)
                return false;

            return true;
        }

        private void EdysonFlowControl_DoubleClick(object sender, EventArgs e)
        {

        }

        private Point MousePositionRelativeToThis { get; set; } = new Point(0, 0);
        private void EdysonFlowControl_MouseMove(object sender, MouseEventArgs e)
        {
            MousePositionRelativeToThis = new Point(e.X, e.Y);
        }
    }

    class EventAndWorkerInfoComparer : IComparer
    {
        int IComparer.Compare(object a, object b)
        {
            var dt1 = a is Event ? ((Event)a).DateTime : ((WorkerInfo)a).StartTime;
            var dt2 = b is Event ? ((Event)b).DateTime : ((WorkerInfo)b).StartTime;
            if (dt1 > dt2)
                return 1;
            if (dt1 < dt2)
                return -1;
            else
                return 0;
        }
    }
}
