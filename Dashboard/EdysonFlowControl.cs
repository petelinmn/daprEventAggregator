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
        private enum ObjType
        {
            AtomicEvent,
            ComplexEvent,
            Stereotype,
            Worker
        }

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

        private void SelectObject(Guid selectedObjectGuid)
        {
            if (selectedObjectGuid != SelectedObject)
            {
                SelectedObject = selectedObjectGuid;

                Draw();
            }
        }

        List<Event> Events { get; set; } = new List<Event>();
        List<Stereotype> Stereotypes { get; set; } = new List<Stereotype>();
        List<WorkerInfo> WorkerInfos { get; set; }

        Dictionary<Guid, Rectangle> PaintedEvents = new Dictionary<Guid, Rectangle>();
        Dictionary<Guid, Rectangle> PaintedStereotypes = new Dictionary<Guid, Rectangle>();
        Dictionary<Guid, Rectangle> PaintedWorkers = new Dictionary<Guid, Rectangle>();


        public Event? SelectedEvent { get => Events?.FirstOrDefault(i => i.Id == SelectedObject); }
        public Stereotype? SelectedStereotype { get => Stereotypes?.FirstOrDefault(i => i.Id == SelectedObject); }
        public WorkerInfo? SelectedWorker { get => WorkerInfos?.FirstOrDefault(i => i.Id == SelectedObject); }

        public void SetData(List<Event> events, List<Stereotype> stereotypes, List<WorkerInfo> workersInfo)
        {
            Events = events;
            WorkerInfos = workersInfo;
            Stereotypes = stereotypes;
            Draw();
        }

        public void Draw()
        {
            if (!(Events?.Count > 0))
                return;

            var list = new List<BaseEntity>();
            list.AddRange(Events);
            list.AddRange(Stereotypes);
            list.AddRange(WorkerInfos);

            list.Sort((x, y) =>
            {
                if (x.DateTime > y.DateTime)
                    return 1;
                if (x.DateTime < y.DateTime)
                    return -1;

                return 0;
            });

            var graphics = CreateGraphics();
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            DrawCounter(graphics);

            var currentColumnX = 20;
            var currentRowY = 20;
            PaintedEvents = new Dictionary<Guid, Rectangle>();
            PaintedStereotypes = new Dictionary<Guid, Rectangle>();
            PaintedWorkers = new Dictionary<Guid, Rectangle>();
            var workerEventRelations = new Dictionary<Guid, Guid>();
            var currentColumnWidth = 0;
            var lastObjType = ObjType.AtomicEvent;
            foreach (var obj in list)
            {
                var @event = obj as Event;
                var stereotype = obj as Stereotype;
                var workerInfo = obj as WorkerInfo;

                var isStereotype = obj is Stereotype;
                var isEvent = !isStereotype && obj is Event;
                var isWorker = !isStereotype && !isEvent && obj is WorkerInfo;

                if (isEvent)
                {
                    //Is event complex?
                    var isEventComplex = @event.Parents.Length > 0;
                    if (isEventComplex)
                    {
                        //var parentRects = PaintedEvents.Where(i => @event.Parents.Contains(i.Key))
                        //    .Select(i => i.Value).TakeLast(2).ToList();

                        currentColumnX += currentColumnWidth / 6;
                        currentRowY = 20;
                        lastObjType = ObjType.ComplexEvent;
                    }
                    else
                    {
                        if (currentRowY < 150)
                            currentRowY = 150;

                        if (lastObjType != ObjType.AtomicEvent)
                        {
                            currentRowY = 150;
                            currentColumnX += currentColumnWidth;
                        }
                        else if (currentRowY > this.Height - 50)
                        {
                            currentRowY = 150;
                            currentColumnX += currentColumnWidth;
                        }
                        lastObjType = ObjType.AtomicEvent;
                    }

                    int width = @event.Name.Length * 12 - (@event.Name.Length > 10 ? @event.Name.Length : -1);
                    var rect = new Rectangle(currentColumnX, currentRowY, width, 32);

                    PaintedEvents.Add(@event.Id, rect);

                    if (rect.Width > currentColumnWidth)
                        currentColumnWidth = rect.Width;
                }
                if (isStereotype)
                {
                    currentColumnX += currentColumnWidth;

                    int width = stereotype.Name.Length * 16 - (stereotype.Name.Length > 10 ? stereotype.Name.Length : -1);
                    int height = width / 2;
                    currentRowY = this.Height / 2 - height;

                    var rect = new Rectangle(currentColumnX + 50, currentRowY + 50, width, height);
                    PaintedStereotypes.Add(stereotype.Id, rect);

                    currentColumnWidth = rect.Width;

                    lastObjType = ObjType.Stereotype;
                }
                else if (isWorker)
                {
                    var parent = Stereotypes.FirstOrDefault(e => e.Id == workerInfo?.Parents.FirstOrDefault());
                    var paintedStereotype = PaintedStereotypes.FirstOrDefault(i => i.Key == parent?.Id);

                    var brothersCount = parent != null ? list.Where(i => i.Parents.Contains(parent.Id)).ToList().Count() : 3;
                    
                    if (currentRowY + 30 > this.Height)
                        currentRowY = 0;

                    if (lastObjType == ObjType.Worker)
                    {
                        currentRowY += this.Height / brothersCount;
                    }
                    else
                    {
                        currentRowY = 50;
                        currentColumnX += paintedStereotype.Value.Width + 20;
                    }

                    lastObjType = ObjType.Worker;
                    string title = GetDisplayTitleForWorker(workerInfo);
                    int width = title.Length * 16 - (title.Length > 10 ? title.Length : -1);
                    var rect = new Rectangle(currentColumnX, currentRowY, width, 38);

                    PaintedWorkers.Add(workerInfo.Id, rect);

                    if (parent != null)
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
            }

            foreach (var paintedStereotype in PaintedStereotypes)
            {
                var stereotype = Stereotypes.FirstOrDefault(i => i.Id == paintedStereotype.Key);
                if (stereotype?.Parents?.Any() != true)
                    continue;

                foreach (var parentId in stereotype.Parents)
                {
                    if (PaintedEvents.ContainsKey(parentId))
                    {
                        var paintedParent = PaintedEvents[parentId];
                        var point1 = GetCenterOfRectangle(paintedParent);
                        var point2 = GetCenterOfRectangle(paintedStereotype.Value);
                        graphics.DrawLine(new Pen(Color.DarkGreen, 2), point1, point2);
                    }
                }

                foreach (var relation in workerEventRelations)
                {
                    var parentRect = PaintedStereotypes.FirstOrDefault(i => i.Key == relation.Value).Value;
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

            foreach (var paintedStereotype in PaintedStereotypes)
            {
                DrawStereotype(graphics, Stereotypes.FirstOrDefault(i => i.Id == paintedStereotype.Key), paintedStereotype.Value);
            }

            //Draw workers
            foreach (var paintedWorker in PaintedWorkers)
            {
                DrawWorker(graphics, WorkerInfos?.FirstOrDefault(i => i.Id == paintedWorker.Key), paintedWorker.Value);
            }
        }

        private static string GetDisplayTitleForWorker(WorkerInfo? workerInfo)
        {
            var title = workerInfo?.Name;
            if (workerInfo?.Data?.Result != null)
            {
                if (int.TryParse(workerInfo.Data.Result, out var progress))
                    title = $"{workerInfo?.Name}  {progress * 10}%";
            }

            return title;
        }

        private void DrawEvent(Graphics graphics, Event? @event, Rectangle rect)
        {
            if (@event == null)
                return;

            var brush = @event.Parents.Length > 0 ? Brushes.Yellow : Brushes.Bisque;
            var pen = @event.Id == SelectedObject ? new Pen(Brushes.Black, 4) : new Pen(Brushes.Gray);
            graphics.FillPath(brush, RoundedRect(rect, 10));
            graphics.DrawPath(pen, RoundedRect(rect, 10));
            graphics.DrawString(@event.Name, new Font("courier", 12), Brushes.Black, new PointF(rect.X + 5, rect.Y + 5));
        }

        private void DrawStereotype(Graphics graphics, Stereotype? stereotype, Rectangle rect)
        {
            if (stereotype == null)
                return;

            var brush = stereotype.IsConfirmed ? Brushes.LightGreen : Brushes.LightPink;
            var pen = stereotype.Id == SelectedObject ? new Pen(Brushes.Black, 4) : new Pen(Brushes.Gray);
            graphics.FillPath(brush, RoundedRect(rect, 10));
            graphics.DrawPath(pen, RoundedRect(rect, 10));
            graphics.DrawString(stereotype.Name, new Font("courier", 14), Brushes.Black, new PointF(rect.X + 16, rect.Y + 20));
            graphics.DrawString(stereotype.StereotypeName, new Font("courier", 14), Brushes.Black, new PointF(rect.X + 16, rect.Y + 32));
        }

        private void DrawWorker(Graphics graphics, WorkerInfo workerInfo, Rectangle rect)
        {
            var brush = workerInfo.Status == WorkerStatus.Init
                ? Brushes.LightYellow
                : workerInfo.Status == WorkerStatus.Work ? Brushes.Yellow
                : Brushes.YellowGreen;

            var pen = workerInfo.Id == SelectedObject ? new Pen(Brushes.Black, 4) : new Pen(Brushes.Gray);
            graphics.FillPath(brush, RoundedRect(rect, 20));
            graphics.DrawPath(pen, RoundedRect(rect, 20));

            //graphics.FillEllipse(brush, rect);
            //graphics.DrawEllipse(pen, rect);

            string title = GetDisplayTitleForWorker(workerInfo);
            graphics.DrawString(title, new Font("courier", 12), Brushes.Black, new PointF(rect.X + rect.Width / 2 - title.Length * 5, rect.Y + (int)(rect.Height / 2.4)));
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
                if (CheckIfShouldSelectObject(item))
                    return;

            foreach (var item in PaintedStereotypes)
                if (CheckIfShouldSelectObject(item))
                    return;

            foreach (var item in PaintedWorkers)
                if (CheckIfShouldSelectObject(item))
                    return;

            Draw();
        }

        private bool CheckIfShouldSelectObject(KeyValuePair<Guid, Rectangle> item)
        {
            if (IsPointInsideRect(item.Value, new Point(MousePositionRelativeToThis.X, MousePositionRelativeToThis.Y)))
            {
                SelectObject(item.Key);
                OnSelectObject?.Invoke(this, new OnSelectObjectEventArgs()
                {
                    SelectedGuid = item.Key
                });
                return true;
            }

            return false;
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

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            //e.
        }
    }
}
