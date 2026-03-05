using Microsoft.Maui.Controls.Shapes;

namespace MathConverter.Demo.Shapes;

// This class simulates WPF's Border's implementation of CornerRadius.
public partial class UnclampedRoundRectangle : Shape, IShape
{
    private SizeF _size;

    PathF IShape.PathForBounds(Rect bounds)
    {
        _size = bounds.Size;

        var x = (float)(bounds.X + (StrokeThickness / 2));
        var y = (float)(bounds.Y + (StrokeThickness / 2));
        var w = (float)(bounds.Width - StrokeThickness);
        var h = (float)(bounds.Height - StrokeThickness);

        var tl = (float)CornerRadius.TopLeft;
        var tr = (float)CornerRadius.TopRight;
        var bl = (float)CornerRadius.BottomLeft;
        var br = (float)CornerRadius.BottomRight;

        var path = new PathF();

        float? l = tl + bl > h ? tl * h / (tl + bl) : null;
        float? t = tl + tr > w ? tl * w / (tl + tr) : null;
        float? r = tr + br > h ? tr * h / (tr + br) : null;
        float? b = bl + br > w ? bl * w / (bl + br) : null;

        path.MoveTo(x, y + (l ?? tl));
        path.AddArc(x, y, x + (2 * (t ?? tl)), y + (2 * (l ?? tl)), 180, 90, true);
        path.AddArc(x + w - (2 * ((w - t) ?? tr)), y, x + w, y + (2 * (r ?? tr)), 90, 0, true);
        path.AddArc(x + w - (2 * ((w - b) ?? br)), y + h - (2 * ((h - r) ?? br)), x + w, y + h, 0, 270, true);
        path.AddArc(x, y + h - (2 * ((h - l) ?? bl)), x + (2 * (b ?? bl)), y + h, 270, 180, true);
        path.Close();

        return path;
    }

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius), typeof(CornerRadius), typeof(UnclampedRoundRectangle), new CornerRadius());
    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public override PathF GetPath()
    {
        var w = Width < 0 ? _size.Width : Width;
        var h = Height < 0 ? _size.Height : Height;

        return ((IShape)this).PathForBounds(new(0, 0, w, h));
    }
}
