using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JJangdoImageUtil
{
    public class MsgBox
    {
        public static DialogResult ShowTopMost(string msg, string caption = "")
        {
            return MessageBox.Show(new Form() { TopMost = true }, msg, caption);
        }


        public static DialogResult Show(string msg, string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icons = MessageBoxIcon.Question )
        {
            return MessageBox.Show(msg, caption, buttons, icons);
        }
    }
}
