namespace PoliceStationArmory.UI
{
    using Rage;
    using System.Drawing;

    internal class UILabel : UIElementBase
    {
        public string FontName { get; set; }
        public float Size { get; set; }
        public PointF Position { get; set; }
        public Vector2 CurrentPosition { get; set; }
        public Color Color { get; set; }
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text == value)
                    return;
                _text = value;
                OnTextChanged();
            }
        }
        public RectangleF RectangleF { get { return new RectangleF(new PointF(CurrentPosition.X, CurrentPosition.Y), Rage.Graphics.MeasureText(Text, FontName, Size)); } }

        public event TextChangedEventHandler TextChanged;

        public UILabel(string text, string fontName, float size, PointF position, Color color, UIScreenBorder borderToComeIntoViewFrom, float comeIntoViewSpeed, float hideSpeed) : base()
        {
            Text = text;
            FontName = fontName;
            Size = size;
            Position = position;
            CurrentPosition = new Vector2(-10000f, -10000f);
            Color = color;
            BorderToComeIntoViewFrom = borderToComeIntoViewFrom;
            ComeIntoViewSpeed = comeIntoViewSpeed;
            HideSpeed = hideSpeed;
            TextChanged = delegate { };
        }

        private float _comingIntoViewAmount = 0f;
        private float _hidingAmount = 0f;

        public override void Draw(GraphicsEventArgs e)
        {
            switch (State)
            {
                case UIState.Hidden:
                    break;
                case UIState.ComingIntoView:
                    e.Graphics.DrawText(Text, FontName, Size, new PointF(CurrentPosition.X, CurrentPosition.Y), Color);
                    break;
                case UIState.Showing:
                    e.Graphics.DrawText(Text, FontName, Size, Position, Color);
                    break;
                case UIState.Hiding:
                    e.Graphics.DrawText(Text, FontName, Size, new PointF(CurrentPosition.X, CurrentPosition.Y), Color);
                    break;
                default:
                    break;
            }
        }

        public override void Process()
        {
            if (State == UIState.Hidden)
                return;

            if (UICommon.IsCursorInBounds(RectangleF))
            {
                MouseState = UIMouseState.ElementHovered;
                OnHovered();

                if (IsMouseLeftButtonJustPressed())
                {
                    OnClicked();
                }
            }
            else
            {
                MouseState = UIMouseState.None;
            }

            switch (State)
            {
                case UIState.Hidden:
                    break;
                case UIState.ComingIntoView:
                    CurrentPosition = Vector2.Lerp(new Vector2(
                                                BorderToComeIntoViewFrom == UIScreenBorder.Left ? Position.X - Game.Resolution.Width : BorderToComeIntoViewFrom == UIScreenBorder.Right ? Position.X + Game.Resolution.Width : Position.X,
                                                BorderToComeIntoViewFrom == UIScreenBorder.Top ? Position.Y - Game.Resolution.Height : BorderToComeIntoViewFrom == UIScreenBorder.Bottom ? Position.X + Game.Resolution.Height : Position.Y
                                                ),
                                                new Vector2(Position.X, Position.Y),
                                                _comingIntoViewAmount);
                    _comingIntoViewAmount += ComeIntoViewSpeed;
                    if (_comingIntoViewAmount > 1f)
                        _comingIntoViewAmount = 1f;
                    if ((BorderToComeIntoViewFrom == UIScreenBorder.Left && CurrentPosition.X >= Position.X) || (BorderToComeIntoViewFrom == UIScreenBorder.Right && CurrentPosition.X <= Position.X) ||
                        (BorderToComeIntoViewFrom == UIScreenBorder.Top && CurrentPosition.Y >= Position.Y) || (BorderToComeIntoViewFrom == UIScreenBorder.Bottom && CurrentPosition.Y <= Position.Y))
                    {
                        _comingIntoViewAmount = 0f;
                        State = UIState.Showing;
                    }
                    break;
                case UIState.Showing:
                    CurrentPosition = new Vector2(Position.X, Position.Y);
                    break;
                case UIState.Hiding:
                    CurrentPosition = Vector2.Lerp(new Vector2(Position.X, Position.Y),
                                                new Vector2(
                                                BorderToComeIntoViewFrom == UIScreenBorder.Left ? Position.X - Game.Resolution.Width : BorderToComeIntoViewFrom == UIScreenBorder.Right ? Position.X + Game.Resolution.Width : Position.X,
                                                BorderToComeIntoViewFrom == UIScreenBorder.Top ? Position.Y - Game.Resolution.Height : BorderToComeIntoViewFrom == UIScreenBorder.Bottom ? Position.X + Game.Resolution.Height : Position.Y),
                                                _hidingAmount
                                                );
                    _hidingAmount += HideSpeed;
                    if (_hidingAmount > 1f)
                        _hidingAmount = 1f;
                    if ((BorderToComeIntoViewFrom == UIScreenBorder.Left && CurrentPosition.X <= Position.X - Game.Resolution.Width) || (BorderToComeIntoViewFrom == UIScreenBorder.Right && CurrentPosition.X >= Position.X + Game.Resolution.Width) ||
                        (BorderToComeIntoViewFrom == UIScreenBorder.Top && CurrentPosition.Y <= Position.Y - Game.Resolution.Height) || (BorderToComeIntoViewFrom == UIScreenBorder.Bottom && CurrentPosition.Y >= Position.X + Game.Resolution.Height))
                    {
                        _hidingAmount = 0f;
                        State = UIState.Hidden;
                    }
                    break;
                default:
                    break;
            }
        }

        public int GetNumberOfLines()
        {
            int newLineLen = System.Environment.NewLine.Length;
            int numLines = Text.Length - Text.Replace(System.Environment.NewLine, string.Empty).Length;
            if (newLineLen != 0)
            {
                numLines /= newLineLen;
                numLines++;
            }
            return numLines;
        }

        public SizeF Measure()
        {
            return Rage.Graphics.MeasureText(Text, FontName, Size);
        }

        protected void OnTextChanged()
        {
            if (TextChanged != null)
                TextChanged(this);
        }
    }
}
