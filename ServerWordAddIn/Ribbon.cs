using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Office = Microsoft.Office.Core;

namespace ServerWordAddIn
{
    [ComVisible(true)]
    public class Ribbon : Office.IRibbonExtensibility
    {
        public string GetCustomUI(string ribbonID)
        {
            try
            {
                // Task.Run avoids a UI-thread deadlock when blocking on async code.
                return Task.Run(() => ApiClient.GetRibbonXmlAsync()).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show("The server-driven ribbon could not be loaded.\n\n" + ex.Message,
                                "Server Add-in", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public async void OnAction(Office.IRibbonControl control)
        {
            try
            {
                var schema = await ApiClient.GetFormAsync(control.Tag);

                using (var dialog = new FormDialog(schema))
                {
                    if (dialog.ShowDialog() != DialogResult.OK)
                        return;

                    var actions = await ApiClient.RunCommandAsync(control.Tag, dialog.GetValues());
                    ActionInterpreter.Run(Globals.ThisAddIn.Application, actions);
                }
            }
            catch (ApiException ex)
            {
                MessageBox.Show(ex.Message, "Server Add-in", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message, "Server Add-in",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}