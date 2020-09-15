using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Demo.Services
{
    public class StatusBarService
    {
        internal static void ShowStatusBar(Label lblStatus, string message, bool type)
        {
            if (message.Trim() == string.Empty)
            {
                lblStatus.Visible = false;
                return;
            }

            lblStatus.Visible = true;
            lblStatus.Text = message;
            lblStatus.ForeColor = Color.White;

            if (type)
                lblStatus.BackColor = Color.FromArgb(79, 208, 154);
            else
                lblStatus.BackColor = Color.Tomato;
        }
    }
}
