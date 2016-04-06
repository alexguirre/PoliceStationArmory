namespace PoliceStationArmory.UI
{
    using Rage;
    using System.Drawing;

    internal class UIRectangle : UIElementBase
    {
        public RectangleF RectangleF { get; set; }
        public Vector2 CurrentPosition { get; set; }
        public Color Color { get; set; }
        public Color BorderColor { get; set; }
        public UIRectangleType Type { get; set; }

        public UIRectangle(RectangleF rect, Color color, Color borderColor, UIRectangleType type, UIScreenBorder borderToComeIntoViewFrom, float comeIntoViewSpeed, float hideSpeed) : base()
        {
            RectangleF = rect;
            CurrentPosition = new Vector2(-10000f, -10000f);
            Color = color;
            BorderToComeIntoViewFrom = borderToComeIntoViewFrom;
            ComeIntoViewSpeed = comeIntoViewSpeed;
            HideSpeed = hideSpeed;
            Type = type;
            BorderColor = borderColor;
        }
        public UIRectangle(RectangleF rect, Color color, UIRectangleType type, UIScreenBorder borderToComeIntoViewFrom, float comeIntoViewSpeed, float hideSpeed) :
            this(rect, color, Color.Empty, type, borderToComeIntoViewFrom, comeIntoViewSpeed, hideSpeed)
        { }

        private float _comingIntoViewAmount = 0f;
        private float _hidingAmount = 0f;

        public override void Process()
        {
            if (State == UIState.Hidden)
                return;
            
            if (UICommon.IsCursorInBounds(new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height)))
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
                                                BorderToComeIntoViewFrom == UIScreenBorder.Left ? RectangleF.X - Game.Resolution.Width : BorderToComeIntoViewFrom == UIScreenBorder.Right ? RectangleF.X + Game.Resolution.Width : RectangleF.X,
                                                BorderToComeIntoViewFrom == UIScreenBorder.Top ? RectangleF.Y - Game.Resolution.Height : BorderToComeIntoViewFrom == UIScreenBorder.Bottom ? RectangleF.X + Game.Resolution.Height : RectangleF.Y
                                                ),
                                                new Vector2(RectangleF.X, RectangleF.Y),
                                                _comingIntoViewAmount);
                    _comingIntoViewAmount += ComeIntoViewSpeed;
                    if (_comingIntoViewAmount > 1f)
                        _comingIntoViewAmount = 1f;
                    if ((BorderToComeIntoViewFrom == UIScreenBorder.Left && CurrentPosition.X >= RectangleF.X) || (BorderToComeIntoViewFrom == UIScreenBorder.Right && CurrentPosition.X <= RectangleF.X) ||
                        (BorderToComeIntoViewFrom == UIScreenBorder.Top && CurrentPosition.Y >= RectangleF.Y) || (BorderToComeIntoViewFrom == UIScreenBorder.Bottom && CurrentPosition.Y <= RectangleF.Y))
                    {
                        _comingIntoViewAmount = 0f;
                        State = UIState.Showing;
                    }
                    break;
                case UIState.Showing:
                    CurrentPosition = new Vector2(RectangleF.X, RectangleF.Y);
                    break;
                case UIState.Hiding:
                    CurrentPosition = Vector2.Lerp(new Vector2(RectangleF.X, RectangleF.Y),
                                                new Vector2(
                                                BorderToComeIntoViewFrom == UIScreenBorder.Left ? RectangleF.X - Game.Resolution.Width : BorderToComeIntoViewFrom == UIScreenBorder.Right ? RectangleF.X + Game.Resolution.Width : RectangleF.X,
                                                BorderToComeIntoViewFrom == UIScreenBorder.Top ? RectangleF.Y - Game.Resolution.Height : BorderToComeIntoViewFrom == UIScreenBorder.Bottom ? RectangleF.X + Game.Resolution.Height : RectangleF.Y),
                                                _hidingAmount
                                                );
                    _hidingAmount += HideSpeed;
                    if (_hidingAmount > 1f)
                        _hidingAmount = 1f;
                    if ((BorderToComeIntoViewFrom == UIScreenBorder.Left && CurrentPosition.X <= RectangleF.X - Game.Resolution.Width) || (BorderToComeIntoViewFrom == UIScreenBorder.Right && CurrentPosition.X >= RectangleF.X + Game.Resolution.Width) ||
                        (BorderToComeIntoViewFrom == UIScreenBorder.Top && CurrentPosition.Y <= RectangleF.Y - Game.Resolution.Height) || (BorderToComeIntoViewFrom == UIScreenBorder.Bottom && CurrentPosition.Y >= RectangleF.X + Game.Resolution.Height))
                    {
                        _hidingAmount = 0f;
                        State = UIState.Hidden;
                    }
                    break;
                default:
                    break;
            }
        }

        public override void Draw(GraphicsEventArgs e)
        {
            switch (State)
            {
                case UIState.Hidden:
                    break;
                case UIState.ComingIntoView:
                    if (Type == UIRectangleType.Filled)
                    {
                        e.Graphics.DrawRectangle(new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height), Color);
                    }
                    else if (Type == UIRectangleType.OnlyBorders)
                    {
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), BorderColor);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), BorderColor);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), BorderColor);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), BorderColor);
                    }
                    else if (Type == UIRectangleType.FilledWithBorders)
                    {
                        e.Graphics.DrawRectangle(new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height), Color);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), BorderColor);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), BorderColor);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), BorderColor);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), BorderColor);
                    }
                    break;
                case UIState.Showing:
                    if (Type == UIRectangleType.Filled)
                    {
                        e.Graphics.DrawRectangle(RectangleF, Color);
                    }
                    else if (Type == UIRectangleType.OnlyBorders)
                    {
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), Color);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), Color);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), Color);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), Color);
                    }
                    else if (Type == UIRectangleType.FilledWithBorders)
                    {
                        e.Graphics.DrawRectangle(RectangleF, Color);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), BorderColor);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), BorderColor);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), BorderColor);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), BorderColor);
                    }
                    break;
                case UIState.Hiding:
                    if (Type == UIRectangleType.Filled)
                    {
                        e.Graphics.DrawRectangle(new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height), Color);
                    }
                    else if (Type == UIRectangleType.OnlyBorders)
                    {
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), Color);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), Color);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), Color);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), Color);
                    }
                    else if (Type == UIRectangleType.FilledWithBorders)
                    {
                        e.Graphics.DrawRectangle(new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height), Color);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), BorderColor);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), BorderColor);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), BorderColor);
                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), BorderColor);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
