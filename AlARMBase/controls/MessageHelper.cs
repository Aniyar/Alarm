using System.Windows.Forms;
using MetroFramework;

namespace ALARm.controls
{
    public static class MessageHelper
    {
        internal static void ShowError(IWin32Window sender, string error)
        {
            MetroMessageBox.Show(sender, error, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        internal static void ShowWarning(IWin32Window sender, string error)
        {
            MetroMessageBox.Show(sender, error, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
