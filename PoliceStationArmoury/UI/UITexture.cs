namespace PoliceStationArmoury.UI
{
    using System.Drawing;
    using Rage;

    internal class UITexture : UIElementBase
    {
        public Texture Texture { get; set; }
        public RectangleF RectangleF { get; set; }
        public Vector2 CurrentPosition { get; set; }
        //public float Rotation { get; set; }

        public UITexture(Texture texture, RectangleF rect, UIScreenBorder borderToComeIntoViewFrom, float comeIntoViewSpeed, float hideSpeed/*, float rotation*/) : base()
        {
            Texture = texture;
            RectangleF = rect;
            CurrentPosition = new Vector2(-10000f, -10000f);
            BorderToComeIntoViewFrom = borderToComeIntoViewFrom;
            ComeIntoViewSpeed = comeIntoViewSpeed;
            HideSpeed = hideSpeed;
            //Rotation = rotation;
        }

        private float _comingIntoViewAmount = 0f;
        private float _hidingAmount = 0f;
        //public void Draw(GraphicsEventArgs e)
        //{
        //    switch (State)
        //    {
        //        case UIState.Hidden:
        //            break;
        //        case UIState.ComingIntoView:
        //            CurrentPosition = Vector2.Lerp(new Vector2(
        //                                        BorderToComeIntoViewFrom == UIScreenBorder.Left ? RectangleF.X - Game.Resolution.Width : BorderToComeIntoViewFrom == UIScreenBorder.Right ? RectangleF.X + Game.Resolution.Width : RectangleF.X,
        //                                        BorderToComeIntoViewFrom == UIScreenBorder.Top ? RectangleF.Y - Game.Resolution.Height : BorderToComeIntoViewFrom == UIScreenBorder.Bottom ? RectangleF.X + Game.Resolution.Height : RectangleF.Y
        //                                        ),
        //                                        new Vector2(RectangleF.X, RectangleF.Y),
        //                                        _comingIntoViewAmount);
        //            e.Graphics.DrawTexture(Texture, new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height));
        //            _comingIntoViewAmount += ComeIntoViewSpeed;
        //            if ((BorderToComeIntoViewFrom == UIScreenBorder.Left && CurrentPosition.X >= RectangleF.X) || (BorderToComeIntoViewFrom == UIScreenBorder.Right && CurrentPosition.X <= RectangleF.X) ||
        //                (BorderToComeIntoViewFrom == UIScreenBorder.Top && CurrentPosition.Y >= RectangleF.Y) || (BorderToComeIntoViewFrom == UIScreenBorder.Bottom && CurrentPosition.Y <= RectangleF.Y))
        //            {
        //                _comingIntoViewAmount = 0f;
        //                State = UIState.Showing;
        //            }
        //            break;
        //        case UIState.Showing:
        //            CurrentPosition = new Vector2(RectangleF.X, RectangleF.Y);
        //            e.Graphics.DrawTexture(Texture, RectangleF);
        //            break;
        //        case UIState.Hiding:
        //            CurrentPosition = Vector2.Lerp(new Vector2(RectangleF.X, RectangleF.Y),
        //                                        new Vector2(
        //                                        BorderToComeIntoViewFrom == UIScreenBorder.Left ? RectangleF.X - Game.Resolution.Width : BorderToComeIntoViewFrom == UIScreenBorder.Right ? RectangleF.X + Game.Resolution.Width : RectangleF.X,
        //                                        BorderToComeIntoViewFrom == UIScreenBorder.Top ? RectangleF.Y - Game.Resolution.Height : BorderToComeIntoViewFrom == UIScreenBorder.Bottom ? RectangleF.X + Game.Resolution.Height : RectangleF.Y),
        //                                        _hidingAmount
        //                                        );
        //            e.Graphics.DrawTexture(Texture, new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height));
        //            _hidingAmount += HideSpeed;
        //            if ((BorderToComeIntoViewFrom == UIScreenBorder.Left && CurrentPosition.X <= RectangleF.X - Game.Resolution.Width) || (BorderToComeIntoViewFrom == UIScreenBorder.Right && CurrentPosition.X >= RectangleF.X + Game.Resolution.Width) ||
        //                (BorderToComeIntoViewFrom == UIScreenBorder.Top && CurrentPosition.Y <= RectangleF.Y - Game.Resolution.Height) || (BorderToComeIntoViewFrom == UIScreenBorder.Bottom && CurrentPosition.Y >= RectangleF.X + Game.Resolution.Height))
        //            {
        //                _hidingAmount = 0f;
        //                State = UIState.Hidden;
        //            }
        //            break;
        //        default:
        //            break;
        //    }
        //}


        //public void Process()
        //{
        //    if (State == UIState.Hidden)
        //        return;

        //    if (UICommon.IsMouseInBounds(RectangleF))
        //    {
        //        MouseState = UIMouseState.ElementSelected;
        //        OnHovered();
        //    }
        //}


        public override void Draw(GraphicsEventArgs e)
        {
            switch (State)
            {
                case UIState.Hidden:
                    break;
                case UIState.ComingIntoView:
                    e.Graphics.DrawTexture(Texture, new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height));
                    //e.Graphics.DrawTexture(Texture, CurrentPosition, new Vector2(RectangleF.Width, RectangleF.Height), CurrentPosition.X + 16 / 2, CurrentPosition.Y + 16 / 2, RectangleF.Width + 16 / 2, RectangleF.Height + 16 / 2, Rotation);
                    break;
                case UIState.Showing:
                    e.Graphics.DrawTexture(Texture, RectangleF);
                    //e.Graphics.DrawTexture(Texture, CurrentPosition, new Vector2(RectangleF.Width, RectangleF.Height), CurrentPosition.X + 16 / 2, CurrentPosition.Y + 16 / 2, RectangleF.Width + 16 / 2, RectangleF.Height + 16 / 2, Rotation);
                    break;
                case UIState.Hiding:
                    e.Graphics.DrawTexture(Texture, new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height));
                    //e.Graphics.DrawTexture(Texture, CurrentPosition, new Vector2(RectangleF.Width, RectangleF.Height), CurrentPosition.X + 16 / 2, CurrentPosition.Y + 16 / 2, RectangleF.Width + 16 / 2, RectangleF.Height + 16 / 2, Rotation);
                    break;
                default:
                    break;
            }
        }


        public override void Process()
        {
            if (State == UIState.Hidden)
                return;

            if (UICommon.IsCursorInBounds(new RectangleF(CurrentPosition.X, CurrentPosition.Y, RectangleF.Width, RectangleF.Height)))
            {
                MouseState = UIMouseState.ElementHovered;
                OnHovered();

                if (IsMouseLeftButtonJustPressed()/*Game.IsControlJustPressed(0, GameControl.CursorAccept)/*mouse.IsLeftButtonDown*/)
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
    }
}
