namespace PoliceStationArmory.UI
{
    using Rage;

    internal abstract class UIElementBase
    {
        public event HoveredEventHandler Hovered;
        public event ClickedEventHandler Clicked;
        public event StateChangedEventHandler StateChanged;

        public virtual UIScreenBorder BorderToComeIntoViewFrom { get; set; }
        public virtual UIMouseState MouseState { get; set; }
        private UIState state;
        public virtual UIState State
        {
            get
            {
                return state;
            }
            set
            {
                if (value == state)
                    return;
                state = value;
                RaiseStateChanged();
            }
        }

        public virtual float ComeIntoViewSpeed { get; set; }
        public virtual float HideSpeed { get; set; }

        public UIElementBase()
        {
            Hovered = delegate { };
            Clicked = delegate { };
            StateChanged = delegate { };
            MouseState = UIMouseState.None;
            State = UIState.Hidden;
        }

        public abstract void Draw(Rage.Graphics g);
        public abstract void Process();

        public virtual void RaiseHovered()
        {
            if (Hovered != null)
                Hovered(this);
        }

        public virtual void RaiseClicked()
        {
            if (Clicked != null)
                Clicked(this);
        }

        public virtual void RaiseStateChanged()
        {
            if (StateChanged != null)
                StateChanged(this);
        }

        bool _canClickAgainLeftButton = true;
        protected virtual bool IsMouseLeftButtonJustPressed()
        {
            MouseState mouse = Game.GetMouseState();
            if (mouse.IsLeftButtonUp)
            {
                _canClickAgainLeftButton = true;
                return false;
            }
            else if (mouse.IsLeftButtonDown && _canClickAgainLeftButton)
            {
                _canClickAgainLeftButton = false;
                return true;
            }
            return false;
        }

        bool _canClickAgainRightButton = true;
        protected virtual bool IsMouseRightButtonJustPressed()
        {
            MouseState mouse = Game.GetMouseState();
            if (mouse.IsRightButtonUp)
            {
                _canClickAgainRightButton = true;
                return false;
            }
            else if (mouse.IsRightButtonDown && _canClickAgainRightButton)
            {
                _canClickAgainRightButton = false;
                return true;
            }
            return false;
        }
    }
}
