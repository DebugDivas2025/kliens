using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raktar_Szinkron
{
    public class RoundedTextBox : TextBox
    {
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            int radius = 10;
            Rectangle bounds = this.ClientRectangle;
            GraphicsPath path = new GraphicsPath();

            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(bounds.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(bounds.Width - radius, bounds.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, bounds.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();

            this.Region = new Region(path);
        }
    }
}
