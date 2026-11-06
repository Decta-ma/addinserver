using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ServerWordAddIn
{
    public class FormDialog : Form
    {
        private readonly Dictionary<string, Func<object>> _getters = new Dictionary<string, Func<object>>();
        private readonly List<KeyValuePair<string, TextBox>> _requiredTexts = new List<KeyValuePair<string, TextBox>>();

        public FormDialog(FormSchema schema)
        {
            Text = schema.Title;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MinimizeBox = false;
            MaximizeBox = false;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(8);

            if (schema.Rtl)
            {
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
            }

            var table = new TableLayoutPanel { AutoSize = true, ColumnCount = 2 };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            foreach (var field in schema.Fields)
                AddField(table, field);

            var ok = new Button { Text = "Create", DialogResult = DialogResult.OK, AutoSize = true };
            var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, AutoSize = true };
            var buttons = new FlowLayoutPanel { AutoSize = true, Anchor = AnchorStyles.Right };
            buttons.Controls.Add(ok);
            buttons.Controls.Add(cancel);
            table.Controls.Add(buttons);
            table.SetColumnSpan(buttons, 2);

            AcceptButton = ok;
            CancelButton = cancel;
            Controls.Add(table);
        }

        private void AddField(TableLayoutPanel table, FieldSchema field)
        {
            if (field.Type == "checkbox")
            {
                var check = new CheckBox { Text = field.Label, AutoSize = true };
                table.Controls.Add(check);
                table.SetColumnSpan(check, 2);
                _getters[field.Name] = () => check.Checked;
                return;
            }

            table.Controls.Add(new Label { Text = field.Label, AutoSize = true, Anchor = AnchorStyles.Left });

            if (field.Type == "select")
            {
                var combo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 260 };
                combo.DisplayMember = "Label";
                combo.ValueMember = "Value";
                combo.DataSource = field.Options;
                table.Controls.Add(combo);
                _getters[field.Name] = () => combo.SelectedValue;
            }
            else // text
            {
                var box = new TextBox { Width = 260 };
                table.Controls.Add(box);
                _getters[field.Name] = () => box.Text;
                if (field.Required)
                    _requiredTexts.Add(new KeyValuePair<string, TextBox>(field.Label, box));
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                foreach (var item in _requiredTexts)
                {
                    if (string.IsNullOrWhiteSpace(item.Value.Text))
                    {
                        MessageBox.Show("\"" + item.Key + "\" is required.", Text,
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        item.Value.Focus();
                        e.Cancel = true;
                        return;
                    }
                }
            }
            base.OnFormClosing(e);
        }

        public Dictionary<string, object> GetValues()
        {
            var values = new Dictionary<string, object>();
            foreach (var getter in _getters)
                values[getter.Key] = getter.Value();
            return values;
        }
    }
}