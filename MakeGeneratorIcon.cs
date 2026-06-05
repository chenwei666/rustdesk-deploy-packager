using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

public static class MakeGeneratorIcon
{
    public static void Main()
    {
        using (Bitmap bitmap = new Bitmap(256, 256))
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (LinearGradientBrush bg = new LinearGradientBrush(new Rectangle(0, 0, 256, 256), Color.FromArgb(20, 88, 143), Color.FromArgb(39, 174, 229), 45f))
            {
                g.FillRectangle(bg, 0, 0, 256, 256);
            }

            using (Pen white = new Pen(Color.White, 18))
            {
                white.StartCap = LineCap.Round;
                white.EndCap = LineCap.Round;
                g.DrawArc(white, 48, 56, 112, 112, 35, 285);
                g.DrawLine(white, 142, 150, 192, 200);
            }

            using (SolidBrush card = new SolidBrush(Color.FromArgb(245, 250, 255)))
            using (GraphicsPath path = RoundedRect(new Rectangle(78, 82, 108, 82), 16))
            {
                g.FillPath(card, path);
            }

            using (SolidBrush blue = new SolidBrush(Color.FromArgb(20, 126, 214)))
            {
                g.FillEllipse(blue, 98, 105, 20, 20);
                g.FillRectangle(blue, 128, 108, 44, 8);
                g.FillRectangle(blue, 98, 134, 74, 8);
            }

            SaveIcon(bitmap, "RustDeskConfigGenerator.ico");
        }
    }

    private static GraphicsPath RoundedRect(Rectangle rect, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        int d = radius * 2;
        path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    private static void SaveIcon(Bitmap bitmap, string path)
    {
        using (MemoryStream png = new MemoryStream())
        {
            bitmap.Save(png, ImageFormat.Png);
            byte[] pngBytes = png.ToArray();
            using (FileStream fs = File.Create(path))
            {
                WriteUInt16(fs, 0);
                WriteUInt16(fs, 1);
                WriteUInt16(fs, 1);
                fs.WriteByte(0);
                fs.WriteByte(0);
                fs.WriteByte(0);
                fs.WriteByte(0);
                WriteUInt16(fs, 1);
                WriteUInt16(fs, 32);
                WriteUInt32(fs, (uint)pngBytes.Length);
                WriteUInt32(fs, 22);
                fs.Write(pngBytes, 0, pngBytes.Length);
            }
        }
    }

    private static void WriteUInt16(Stream stream, ushort value)
    {
        stream.WriteByte((byte)(value & 0xff));
        stream.WriteByte((byte)(value >> 8));
    }

    private static void WriteUInt32(Stream stream, uint value)
    {
        stream.WriteByte((byte)(value & 0xff));
        stream.WriteByte((byte)((value >> 8) & 0xff));
        stream.WriteByte((byte)((value >> 16) & 0xff));
        stream.WriteByte((byte)((value >> 24) & 0xff));
    }
}
