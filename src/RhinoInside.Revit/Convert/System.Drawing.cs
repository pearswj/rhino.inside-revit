using DB = Autodesk.Revit.DB;

namespace RhinoInside.Revit.Convert.System.Drawing
{
  using global::System.Drawing;

  public static class ColorConverter
  {
    public static Color ToColor(this DB.Color c)
    {
      return c.IsValid ?
             Color.FromArgb(0xFF, c.Red, c.Green, c.Blue) :
             Color.FromArgb(0, 0, 0, 0);
    }

    public static DB.Color ToColor(this Color c)
    {
      return c.ToArgb() == 0 ?
             DB.Color.InvalidColorValue :
             new DB.Color(c.R, c.G, c.B);
    }
  }

  public static class ColorWithTransparencyConverter
  {
    public static Color ToColor(this DB.ColorWithTransparency c)
    {
      return c.IsValidObject ?
             Color.FromArgb(0xFF - (int) c.GetTransparency(), (int) c.GetRed(), (int) c.GetGreen(), (int) c.GetBlue()) :
             Color.FromArgb(0, 0, 0, 0);
    }

    public static DB.ColorWithTransparency ToColorWithTransparency(this Color c)
    {
      return new DB.ColorWithTransparency(c.R, c.G, c.B, 0xFFu - c.A);
    }
  }

  public static class RectangleConverter
  {
    public static Rectangle ToRectangle(this DB.Rectangle value)
    {
      return new Rectangle(value.Left, value.Top, value.Right - value.Left, value.Bottom - value.Top);
    }

    public static DB.Rectangle ToRectangle(this Rectangle value)
    {
      return new DB.Rectangle(value.Left, value.Top, value.Right, value.Bottom);
    }
  }
}
