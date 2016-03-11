namespace PoliceStationArmoury.UI
{
    internal enum UIState
    {
        Hidden,
        ComingIntoView,
        Showing,
        Hiding,
    }

    internal enum UIScreenBorder
    {
        Top,
        Bottom,
        Left,
        Right,
    }

    internal enum UIMouseState
    {
        None,
        ElementHovered,
    }

    internal enum UIRectangleType
    {
        Filled,
        OnlyBorders,
        FilledWithBorders,
    }
}
