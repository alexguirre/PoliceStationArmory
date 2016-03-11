//namespace PoliceStationArmory.UI
//{
//    using Rage;
//    using System.Drawing;
//    using System.Collections.Generic;
//    using System.Windows.Forms;
//    using System.Linq;

//    internal class UITextBox : UIElementBase
//    {
//        public string FontName { get; set; }
//        public float Size { get; set; }
//        public RectangleF RectangleF { get; set; }
//        public Vector2 CurrentPosition { get; set; }
//        public Color BackgroundColor { get; set; }
//        public Color TextColor { get; set; }
//        public Color BorderColor { get; set; }
//        public UIRectangleType BackgroundType { get; set; }
//        public bool ShouldDrawBackground { get; set; }
//        public bool HasFocus { get; set; }
//        public bool AdjustTextToRectangle { get; set; }
//        private string _text;
//        public string Text
//        {
//            get { return _text; }
//            set
//            {
//                if (_text == value)
//                    return;
//                _text = value;
//                OnTextChanged();
//            }
//        }

//        public event KeyPressedEventHandler KeyRightPressed;
//        public event KeyPressedEventHandler KeyLeftPressed;
//        public event KeyPressedEventHandler KeyDeletePressed;
//        public event KeyPressedEventHandler KeyBackspacePressed;
//        public event KeyPressedEventHandler KeySpacePressed;
//        public event KeyPressedEventHandler KeyEnterPressed;

//        public event TextChangedEventHandler TextChanged;

//        public UITextBox(string text, string fontName, float size, RectangleF rectF, Color backgroundColor, Color textColor, UIRectangleType backgroundType, UIScreenBorder borderToComeIntoViewFrom, float comeIntoViewSpeed, float hideSpeed) : base()
//        {
//            Text = text;
//            FontName = fontName;
//            Size = size;
//            RectangleF = rectF;
//            CurrentPosition = new Vector2(-10000f, -10000f);
//            BackgroundColor = backgroundColor;
//            TextColor = textColor;
//            BorderToComeIntoViewFrom = borderToComeIntoViewFrom;
//            ComeIntoViewSpeed = comeIntoViewSpeed;
//            HideSpeed = hideSpeed;
//            BackgroundType = backgroundType;
//            ShouldDrawBackground = true;
//            BorderColor = Color.Black;
//            KeyRightPressed = delegate { };
//            KeyLeftPressed = delegate { };
//            KeyDeletePressed = delegate { };
//            KeyBackspacePressed = delegate { };
//            KeySpacePressed = delegate { };
//            KeyEnterPressed = delegate { };
//            TextChanged = delegate { };
//            HasFocus = false;
//            //for (int i = 0; i < 9999; i++)
//            //{
//            //    if (Rage.Graphics.MeasureText(new String('A', i), FontName, Size).Width > RectangleF.Width)
//            //    {
//            //        _maxCharachterPerLine = i;
//            //        Game.DisplaySubtitle("MaxChar: " + _maxCharachterPerLine, 20000);
//            //        break;
//            //    }
//            //}
//        }

//        //private int _maxCharachterPerLine = 0;

//        private float _comingIntoViewAmount = 0f;
//        private float _hidingAmount = 0f;
//        public override void Draw(GraphicsEventArgs e)
//        {
//            switch (State)
//            {
//                case UIState.Hidden:
//                    break;
//                case UIState.ComingIntoView:
//                case UIState.Showing:
//                case UIState.Hiding:
//                    DrawBackground(e);
//                    DrawText(e);
//                    DrawBar(e);
//                    break;
//                default:
//                    break;
//            }
//        }


//        protected void DrawText(GraphicsEventArgs e)
//        {
//            e.Graphics.DrawText(Text, FontName, Size, new PointF(CurrentPosition.X, CurrentPosition.Y), TextColor);
//        }

//        protected void DrawBackground(GraphicsEventArgs e)
//        {
//            if (!ShouldDrawBackground)
//                return;

//            switch (State)
//            {
//                case UIState.Hidden:
//                    break;
//                case UIState.ComingIntoView:
//                    if (BackgroundType == UIRectangleType.Filled)
//                    {
//                        e.Graphics.DrawRectangle(new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height), BackgroundColor);
//                    }
//                    else if (BackgroundType == UIRectangleType.OnlyBorders)
//                    {
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), BorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), BorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), BorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), BorderColor);
//                    }
//                    else if (BackgroundType == UIRectangleType.FilledWithBorders)
//                    {
//                        e.Graphics.DrawRectangle(new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height), BackgroundColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), BorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), BorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), BorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), BorderColor);
//                    }
//                    break;
//                case UIState.Showing:
//                    if (BackgroundType == UIRectangleType.Filled)
//                    {
//                        e.Graphics.DrawRectangle(RectangleF, BackgroundColor);
//                    }
//                    else if (BackgroundType == UIRectangleType.OnlyBorders)
//                    {
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), BackgroundColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), BackgroundColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), BackgroundColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), BackgroundColor);
//                    }
//                    else if (BackgroundType == UIRectangleType.FilledWithBorders)
//                    {
//                        e.Graphics.DrawRectangle(RectangleF, BackgroundColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), BorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), BorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), BorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), BorderColor);
//                    }
//                    break;
//                case UIState.Hiding:
//                    if (BackgroundType == UIRectangleType.Filled)
//                    {
//                        e.Graphics.DrawRectangle(new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height), BackgroundColor);
//                    }
//                    else if (BackgroundType == UIRectangleType.OnlyBorders)
//                    {
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), BackgroundColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), BackgroundColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), BackgroundColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), BackgroundColor);
//                    }
//                    else if (BackgroundType == UIRectangleType.FilledWithBorders)
//                    {
//                        e.Graphics.DrawRectangle(new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height), BackgroundColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), BorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), BorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X, CurrentPosition.Y + RectangleF.Height), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), BorderColor);
//                        e.Graphics.DrawLine(new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y), new Vector2(CurrentPosition.X + RectangleF.Width, CurrentPosition.Y + RectangleF.Height), BorderColor);
//                    }
//                    break;
//                default:
//                    break;
//            }
//        }

//        public int CursorPos { get; set; }
//        public int CursorEnd { get; set; }

//        private float _lastInputTime = getTimeInSeconds();
//        private float _barTime = 0;
//        private RectangleF _barRect = new RectangleF();
//        protected void DrawBar(GraphicsEventArgs e)
//        {
//            if (!HasFocus)
//                return;

//            if ((_barTime % 1.0f) <= 0.5f)
//            {
//                e.Graphics.DrawRectangle(_barRect, Color.Black);
//            }
//        }
//        protected void ProcessBar()
//        {
//            if (!HasFocus)
//                return;
//            _barTime = getTimeInSeconds() - _lastInputTime;

//            int numLines = System.Text.RegularExpressions.Regex.Matches(Text, System.Environment.NewLine).Count + 1;
//            SizeF textMeasure = Rage.Graphics.MeasureText("A", FontName, Size);
//            PointF charPos = GetCharacterPosition(CursorPos);
//            if (charPos != new PointF(0, 0))
//            {
//                _barRect.X = charPos.X + 2;
//            }
//            else
//            {
//                _barRect.X = RectangleF.X + 2;
//            }
//            _barRect.Y = RectangleF.Y + ((textMeasure.Height / 2 - 1) * numLines);
//            _barRect.Width = 1;
//            _barRect.Height = textMeasure.Height + 2;
//        }

//        public override void Process()
//        {
//            if (State == UIState.Hidden)
//                return;

//            if (HasFocus)
//            {
//                ProcessKeys();
//            }

//            MouseState mouse = Game.GetMouseState();
//            if (UICommon.IsCursorInBounds(RectangleF))
//            {
//                MouseState = UIMouseState.ElementHovered;
//                OnHovered();

//                if (IsMouseLeftButtonJustPressed()/*Game.IsControlJustPressed(0, GameControl.CursorAccept)/*mouse.IsLeftButtonDown*/)
//                {
//                    if (!HasFocus)
//                        HasFocus = true;

//                    OnClicked();
//                }
//            }
//            else
//            {
//                MouseState = UIMouseState.None;
//                if (IsMouseLeftButtonJustPressed()/*Game.IsControlJustPressed(0, GameControl.CursorAccept)/*mouse.IsLeftButtonDown*/)
//                {
//                    HasFocus = false;
//                }
//            }

//            ProcessBar();

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

//        private Keys _lastPressedKey = Keys.None;
//        private int _counter = 0;
//        protected void ProcessKeys()
//        {
//            KeyboardState keyboardState = Game.GetKeyboardState();
//            IList<Keys> keys = keyboardState.PressedKeys;
//            if (keys.Count >= 1)
//            {
//                Keys key = keys.Last();

//                if (key == _lastPressedKey)
//                {
//                    if (_counter >= 70)
//                    {
//                        if (key == Keys.Left)
//                            OnKeyLeftPressed();
//                        else if (key == Keys.Right)
//                            OnKeyRightPressed();
//                        else if (key == Keys.Back)
//                            OnKeyBackspacePressed();
//                        else if (key == Keys.Delete)
//                            OnKeyDeletePressed();
//                        else if (key == Keys.Space)
//                            OnKeySpacePressed();
//                        else if (key == Keys.Enter)
//                            OnKeyEnterPressed();
//                        else
//                        {
//                            string str = new KeysConverter().ConvertToString(key);
//                            if (str.Length == 1)
//                                InsertTextAtCursorPos(str);
//                        }
//                        _lastInputTime = getTimeInSeconds();
//                    }
//                    _counter++;
//                }
//                else
//                {
//                    if (key == Keys.Left)
//                        OnKeyLeftPressed();
//                    else if (key == Keys.Right)
//                        OnKeyRightPressed();
//                    else if (key == Keys.Back)
//                        OnKeyBackspacePressed();
//                    else if (key == Keys.Delete)
//                        OnKeyDeletePressed();
//                    else if (key == Keys.Space)
//                        OnKeySpacePressed();
//                    else if (key == Keys.Enter)
//                        OnKeyEnterPressed();
//                    else
//                    {
//                        string str = new KeysConverter().ConvertToString(key);
//                        if (str.Length == 1)
//                            InsertTextAtCursorPos(str);
//                    }
//                    _lastInputTime = getTimeInSeconds();
//                    _counter = 0;
//                    _lastPressedKey = key;
//                }
//            }
//            else
//            {
//                _lastPressedKey = Keys.None;
//            }
//        }

//        public SizeF MeasureText()
//        {
//            return Rage.Graphics.MeasureText(Text.Replace(' ', 'A'), FontName, Size);
//        }

//        public PointF GetCharacterPosition(int index)
//        {
//            if (Text.Length == 0 || index == 0)
//                return new PointF(0, 0);

//            string sub = Text.Substring(0, index);
//            PointF p = Rage.Graphics.MeasureText(sub, FontName, Size).ToPointF().Add(RectangleF.Location);

//            return p;
//        }

//        public void DeleteText(int startPos, int length)
//        {
//            if (startPos > Text.Length)
//                return;

//            string str = Text;
//            str = str.Remove(startPos, length);
//            Text = str;

//            if (CursorPos > startPos)
//            {
//                CursorPos = CursorPos - length;
//            }

//            CursorEnd = CursorPos;
//        }


//        void InsertTextAtCursorPos(string text)
//        {
//            //if (HasSelection)
//            //{
//            //    EraseSelection();
//            //}

//            if (CursorPos > Text.Length)
//                CursorPos = Text.Length;

//            string str = Text;
//            str = str.Insert(CursorPos, text);
//            Text = str;

//            CursorPos += text.Length;
//            CursorEnd = CursorPos;

//            if (AdjustTextToRectangle)
//            {

//                //if(Text.Length > _maxCharachterPerLine)
//                //{
//                //    string str2 = Text;
//                //    str2 = str2.Insert(_maxCharachterPerLine, "\r\n");
//                //    Text = str2;
//                //    Game.DisplayNotification("new line");
//                //}
//                //for (int i = 0; i < Text.Length; i++)
//                //{
//                //    string subStr = Text.Substring(0, i);
//                //    if(Rage.Graphics.MeasureText(subStr, FontName, Size).Width > RectangleF.Width)
//                //    {
//                //        Text.Insert(i, System.Environment.NewLine);
//                //    }
//                //}
//            }
//        }

//        protected void OnTextChanged()
//        {
//            if (TextChanged != null)
//                TextChanged(this);
//        }

//        protected void OnKeyLeftPressed()
//        {
//            if (CursorPos > 0)
//                CursorPos--;

//            _lastInputTime = getTimeInSeconds();

//            if (KeyLeftPressed != null)
//                KeyLeftPressed(this, Keys.Left);
//        }

//        protected void OnKeyRightPressed()
//        {
//            if (CursorPos < Text.Length)
//                CursorPos++;

//            _lastInputTime = getTimeInSeconds();

//            if (KeyRightPressed != null)
//                KeyRightPressed(this, Keys.Right);
//        }

//        protected void OnKeyBackspacePressed()
//        {
//            if (CursorPos > Text.Length || Text.Length <= 0)
//                return;

//            DeleteText(CursorPos - 1, 1);

//            _lastInputTime = getTimeInSeconds();

//            if (KeyBackspacePressed != null)
//                KeyBackspacePressed(this, Keys.Back);
//        }

//        protected void OnKeyDeletePressed()
//        {
//            if (Text.Length <= 0 || Text.Length <= CursorPos)
//                return;

//            DeleteText(CursorPos, 1);

//            _lastInputTime = getTimeInSeconds();

//            if (KeyDeletePressed != null)
//                KeyDeletePressed(this, Keys.Delete);
//        }

//        protected void OnKeySpacePressed()
//        {
//            InsertTextAtCursorPos(" ");

//            _lastInputTime = getTimeInSeconds();

//            if (KeySpacePressed != null)
//                KeySpacePressed(this, Keys.Space);
//        }

//        protected void OnKeyEnterPressed()
//        {
//            InsertTextAtCursorPos(System.Environment.NewLine);

//            _lastInputTime = getTimeInSeconds();

//            if (KeySpacePressed != null)
//                KeySpacePressed(this, Keys.Enter);
//        }

//        private static float getTimeInSeconds()
//        {
//            var time = System.DateTime.UtcNow;
//            var diff = time - _lastTime;
//            var seconds = diff.TotalSeconds;
//            if (seconds > 0.1)
//                seconds = 0.1;
//            _currentTime += (float)seconds;
//            _lastTime = time;
//            return _currentTime;
//        }
//        private static System.DateTime _lastTime;
//        private static float _currentTime;
//    }
//}