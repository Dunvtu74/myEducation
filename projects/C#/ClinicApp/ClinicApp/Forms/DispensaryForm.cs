using ClinicApp.Database;
using Microsoft.Data.Sqlite;

namespace ClinicApp.Forms
{
    public class DispensaryForm : Form
    {
        private int patientId;

        private DateTimePicker dtDate;
        private ComboBox cmbType;
        private TextBox txtResult, txtNotes;
        private DateTimePicker dtNext;
        private CheckBox chkNext;

        public DispensaryForm(int patientId)
        {
            this.patientId = patientId;
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Диспансеризация";
            this.Size = new Size(520, 380);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 10);

            var layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.Padding = new Padding(12);
            layout.ColumnCount = 2;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowCount = 6;
            for (int i = 0; i < 5; i++)
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));

            dtDate = new DateTimePicker() { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };

            cmbType = new ComboBox() { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbType.Items.AddRange(new[] {
                "Профосмотр",
                "Диспансеризация 1 раз в 3 года",
                "Углублённая диспансеризация",
                "Диспансерное наблюдение"
            });

            txtResult = new TextBox() { Dock = DockStyle.Fill, Multiline = true };
            txtNotes = new TextBox() { Dock = DockStyle.Fill };

            chkNext = new CheckBox() { Text = "Следующая дата", Dock = DockStyle.Fill };
            dtNext = new DateTimePicker() { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short, Enabled = false };
            chkNext.CheckedChanged += (s, e) => dtNext.Enabled = chkNext.Checked;

            var rows = new (string label, Control ctrl)[]
            {
                ("Дата", dtDate),
                ("Вид", cmbType),
                ("Результат", txtResult),
                ("Примечание", txtNotes),
                ("", chkNext),
            };

            for (int i = 0; i < rows.Length; i++)
            {
                var lbl = new Label() { Text = rows[i].label, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
                layout.Controls.Add(lbl, 0, i);
                layout.Controls.Add(rows[i].ctrl, 1, i);
            }

            layout.Controls.Add(dtNext, 1, 4);

            var btnPanel = new FlowLayoutPanel() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            var btnCancel = new Button() { Text = "Отмена", Size = new Size(100, 34) };
            btnCancel.Click += (s, e) => this.Close();
            var btnSave = new Button() { Text = "Сохранить", Size = new Size(100, 34) };
            btnSave.Click += (s, e) => Save();
            btnPanel.Controls.Add(btnCancel);
            btnPanel.Controls.Add(btnSave);
            layout.Controls.Add(btnPanel, 1, 5);

            this.Controls.Add(layout);
        }

        private void Save()
        {
            using var conn = DB.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Dispensary (PatientId, Date, Type, Result, NextDate, Notes)
                VALUES (@pid, @dt, @tp, @rs, @nd, @nt)
            ";
            cmd.Parameters.AddWithValue("@pid", patientId);
            cmd.Parameters.AddWithValue("@dt", dtDate.Value.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@tp", cmbType.SelectedItem?.ToString() ?? "");
            cmd.Parameters.AddWithValue("@rs", txtResult.Text.Trim());
            cmd.Parameters.AddWithValue("@nd", chkNext.Checked ? dtNext.Value.ToString("yyyy-MM-dd") : "");
            cmd.Parameters.AddWithValue("@nt", txtNotes.Text.Trim());
            cmd.ExecuteNonQuery();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}