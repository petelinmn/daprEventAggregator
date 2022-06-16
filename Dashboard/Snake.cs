using Dapr.Client;
using Dapr.Actors.Client;
using Configuration.Actors.Contract;
using Dapr.Actors;
using Common;

namespace Dashboard
{
    public partial class Snake: UserControl
    {
        public Snake()
        {
            InitializeComponent();
            _daprClient = new DaprClientBuilder().Build();
        }

        Graphics graphics { get; set; }
        private DaprClient _daprClient { get; }

        public int XWidth { get; } = 10;
        public int YHeight { get; } = 10;

        private async void timer1_Tick(object sender, EventArgs e)
        {
            await Draw();
        }

        private async Task Draw()
        {
            var snakeActor = ActorProxy.Create<ISnakeActor>(new ActorId("SnakeActor"), "SnakeActor");
            var snake = await snakeActor.GetSnake();
            if (snake == null || snake.Body == null)
            {
                MessageBox.Show("snake is null");
                return;
            }
            var squareWidth = Width / XWidth;
            var squareHeight = Height / YHeight;
            for (var i = 0; i < XWidth; i++)
            {
                graphics.DrawLine(Pens.LightPink, i * squareWidth, 0, i * squareWidth, Height);
            }
            for (var i = 0; i < YHeight; i++)
            {
                graphics.DrawLine(Pens.LightPink, 0, i * squareHeight, Width, i * squareHeight);
            }

            for (var i = 0; i < snake.Body.Count; i++)
            {
                var point = snake.Body[i];
                var x = point.X * squareWidth;
                var y = point.Y * squareHeight;

                var brush = Brushes.YellowGreen;
                if (i == snake.Body.Count - 1)
                    brush = Brushes.Yellow;

                graphics.FillRectangle(brush, x, y, squareWidth, squareHeight);
                graphics.DrawRectangle(Pens.Red, x, y, squareWidth, squareHeight);

                graphics.DrawString($"{i + 1}", new Font("courier new", 10),
                    Brushes.Black, new PointF(x + 24, y + 22));
            }
        }

        private void Snake_Load(object sender, EventArgs e)
        {
            graphics = CreateGraphics();
        }

        async Task eventInvoke(string direction) =>
            await _daprClient.PublishEventAsync("pubsub", "move", new EventRequest
            {
                Name = "move"
            });

        private async void upButton_Click(object sender, EventArgs e) =>
            await eventInvoke("up");

        private async void downButton_Click(object sender, EventArgs e) =>
            await eventInvoke("down");

        private async void leftButton_Click(object sender, EventArgs e) =>
            await eventInvoke("left");

        private async void rightButton_Click(object sender, EventArgs e) =>
            await eventInvoke("right");

        private async void refreshButton_Click(object sender, EventArgs e)
        {
            this.CreateGraphics().Clear(this.BackColor);
            await Draw();
        }
    }
}
