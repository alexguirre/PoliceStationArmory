//namespace PoliceStationArmory.UI
//{
//    using System.Windows.Forms;
//    using System.Drawing;
//    using Rage;

//    internal class UICheckBox : UIElementBase
//    {
//        public RectangleF RectangleF { get; set; }
//        public Vector2 CurrentPosition { get; set; }
//        public Color Color { get; set; }
//        public Color CurrentColor { get; set; }
//        public Color BorderColor { get; set; }
//        public Color CurrentBorderColor { get; set; }
//        public UIRectangleType Type { get; set; }

//        public bool IsChecked { get; set; }

//        public bool HighlightBorder { get; set; }

//        public UICheckBox(RectangleF rect, Color color, Color borderColor, UIRectangleType type, UIScreenBorder borderToComeIntoViewFrom, float comeIntoViewSpeed, float hideSpeed, bool highlightBorder) : base()
//        {
//            RectangleF = rect;
//            CurrentPosition = new Vector2(-10000f, -10000f);
//            Color = color;
//            CurrentColor = Color;
//            BorderColor = borderColor;
//            CurrentBorderColor = BorderColor;
//            BorderToComeIntoViewFrom = borderToComeIntoViewFrom;
//            ComeIntoViewSpeed = comeIntoViewSpeed;
//            HideSpeed = hideSpeed;
//            IsChecked = false;
//            Type = type;
//            HighlightBorder = highlightBorder;
//        }

//        public UICheckBox(RectangleF rect, Color color, Color borderColor, UIRectangleType type, UIScreenBorder borderToComeIntoViewFrom, float comeIntoViewSpeed, float hideSpeed) :
//            this(rect, color, borderColor, type, borderToComeIntoViewFrom, comeIntoViewSpeed, hideSpeed, false)
//        { }


//        public UICheckBox(RectangleF rect, Color color, UIRectangleType type, UIScreenBorder borderToComeIntoViewFrom, float comeIntoViewSpeed, float hideSpeed, bool highlightBorder) :
//            this(rect, color, Color.Empty, type, borderToComeIntoViewFrom, comeIntoViewSpeed, hideSpeed, highlightBorder)
//        { }

//        public UICheckBox(RectangleF rect, Color color, UIRectangleType type, UIScreenBorder borderToComeIntoViewFrom, float comeIntoViewSpeed, float hideSpeed) :
//            this(rect, color, Color.Empty, type, borderToComeIntoViewFrom, comeIntoViewSpeed, hideSpeed, false)
//        { }

//        private float _comingIntoViewAmount = 0f;
//        private float _hidingAmount = 0f;

//        public override void Draw(GraphicsEventArgs e)
//        {
//            switch (State)
//            {
//                case UIState.Hidden:
//                    break;
//                case UIState.ComingIntoView:
//                    if (Type == UIRectangleType.Filled)
//                    {
//                        e.Graphics.DrawRectangle(new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height), CurrentColor);
//                    }
//                    else if (Type == UIRectangleType.OnlyBorders)
//                    {
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), CurrentBorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), CurrentBorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), CurrentBorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), CurrentBorderColor);
//                    }
//                    else if (Type == UIRectangleType.FilledWithBorders)
//                    {
//                        e.Graphics.DrawRectangle(new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height), CurrentColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), CurrentBorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), CurrentBorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), CurrentBorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), CurrentBorderColor);
//                    }
//                    if (IsChecked)
//                        e.Graphics.DrawTexture(UICommon.TickTexture, new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height));
//                    break;
//                case UIState.Showing:
//                    if (Type == UIRectangleType.Filled)
//                    {
//                        e.Graphics.DrawRectangle(new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height), CurrentColor);
//                    }
//                    else if (Type == UIRectangleType.OnlyBorders)
//                    {
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), CurrentBorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), CurrentBorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), CurrentBorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), CurrentBorderColor);
//                    }
//                    else if (Type == UIRectangleType.FilledWithBorders)
//                    {
//                        e.Graphics.DrawRectangle(new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height), CurrentColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), CurrentBorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), CurrentBorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), CurrentBorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), CurrentBorderColor);
//                    }
//                    if (IsChecked)
//                        e.Graphics.DrawTexture(UICommon.TickTexture, RectangleF);
//                    break;
//                case UIState.Hiding:
//                    if (Type == UIRectangleType.Filled)
//                    {
//                        e.Graphics.DrawRectangle(new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height), CurrentColor);
//                    }
//                    else if (Type == UIRectangleType.OnlyBorders)
//                    {
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), CurrentBorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), CurrentBorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), CurrentBorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), CurrentBorderColor);
//                    }
//                    else if (Type == UIRectangleType.FilledWithBorders)
//                    {
//                        e.Graphics.DrawRectangle(new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height), CurrentColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), CurrentBorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), CurrentBorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), CurrentBorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), CurrentBorderColor);
//                    }
//                    if (IsChecked)
//                        e.Graphics.DrawTexture(UICommon.TickTexture, new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height));
//                    break;
//                default:
//                    break;
//            }
//        }


//        public override void Process()
//        {
//            if (State == UIState.Hidden)
//                return;

//            if (UICommon.IsCursorInBounds(new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height)))
//            {
//                MouseState = UIMouseState.ElementHovered;

//                CurrentColor = ControlPaint.Light(Color, 1.0f);
//                if (HighlightBorder)
//                    CurrentBorderColor = ControlPaint.Light(BorderColor, 1.0f);
//                OnHovered();

//                if (IsMouseLeftButtonJustPressed())
//                {
//                    IsChecked = !IsChecked;
//                    OnClicked();
//                }
//            }
//            else
//            {
//                MouseState = UIMouseState.None;
//                CurrentColor = Color;
//                CurrentBorderColor = BorderColor;
//            }

//            switch (State)
//            {
//                case UIState.Hidden:
//                    break;
//                case UIState.ComingIntoView:
//                    CurrentPosition = Vector2.Lerp(new Vector2(
//                                                BorderToComeIntoViewFrom == UIScreenBorder.Left ? RectangleF.X - Game.Resolution.Width : BorderToComeIntoViewFrom == UIScreenBorder.Right ? RectangleF.X + Game.Resolution.Width : RectangleF.X,
//                                                BorderToComeIntoViewFrom == UIScreenBorder.Top ? RectangleF.Y - Game.Resolution.Height : BorderToComeIntoViewFrom == UIScreenBorder.Bottom ? RectangleF.X + Game.Resolution.Height : RectangleF.Y
//                                                ),
//                                                new Vector2(RectangleF.X, RectangleF.Y),
//                                                _comingIntoViewAmount);
//                    _comingIntoViewAmount += ComeIntoViewSpeed;
//                    if (_comingIntoViewAmount > 1f)
//                        _comingIntoViewAmount = 1f;
//                    if ((BorderToComeIntoViewFrom == UIScreenBorder.Left && CurrentPosition.X >= RectangleF.X) || (BorderToComeIntoViewFrom == UIScreenBorder.Right && CurrentPosition.X <= RectangleF.X) ||
//                        (BorderToComeIntoViewFrom == UIScreenBorder.Top && CurrentPosition.Y >= RectangleF.Y) || (BorderToComeIntoViewFrom == UIScreenBorder.Bottom && CurrentPosition.Y <= RectangleF.Y))
//                    {
//                        _comingIntoViewAmount = 0f;
//                        State = UIState.Showing;
//                    }
//                    break;
//                case UIState.Showing:
//                    CurrentPosition = new Vector2(RectangleF.X, RectangleF.Y);
//                    break;
//                case UIState.Hiding:
//                    CurrentPosition = Vector2.Lerp(new Vector2(RectangleF.X, RectangleF.Y),
//                                                new Vector2(
//                                                BorderToComeIntoViewFrom == UIScreenBorder.Left ? RectangleF.X - Game.Resolution.Width : BorderToComeIntoViewFrom == UIScreenBorder.Right ? RectangleF.X + Game.Resolution.Width : RectangleF.X,
//                                                BorderToComeIntoViewFrom == UIScreenBorder.Top ? RectangleF.Y - Game.Resolution.Height : BorderToComeIntoViewFrom == UIScreenBorder.Bottom ? RectangleF.X + Game.Resolution.Height : RectangleF.Y),
//                                                _hidingAmount
//                                                );
//                    _hidingAmount += HideSpeed;
//                    if (_hidingAmount > 1f)
//                        _hidingAmount = 1f;
//                    if ((BorderToComeIntoViewFrom == UIScreenBorder.Left && CurrentPosition.X <= RectangleF.X - Game.Resolution.Width) || (BorderToComeIntoViewFrom == UIScreenBorder.Right && CurrentPosition.X >= RectangleF.X + Game.Resolution.Width) ||
//                        (BorderToComeIntoViewFrom == UIScreenBorder.Top && CurrentPosition.Y <= RectangleF.Y - Game.Resolution.Height) || (BorderToComeIntoViewFrom == UIScreenBorder.Bottom && CurrentPosition.Y >= RectangleF.X + Game.Resolution.Height))
//                    {
//                        _hidingAmount = 0f;
//                        State = UIState.Hidden;
//                    }
//                    break;
//                default:
//                    break;
//            }
//        }
//    }
//}
