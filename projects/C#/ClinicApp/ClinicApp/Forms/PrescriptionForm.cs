using ClinicApp.Database;
using Microsoft.Data.Sqlite;

namespace ClinicApp.Forms
{
    public class PrescriptionForm : Form
    {
        private long appointmentId;

        private TextBox txtMedicine, txtDosage, txtSchedule, txtNotes;
        private DateTimePicker dtFrom, dtTo;
        private CheckBox chkDates;

        public PrescriptionForm(long appointmentId)
        {
            this.appointmentId = appointmentId;
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Добавить препарат";
            this.Size = new Size(500, 360);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 10);

            var layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.Padding = new Padding(12);
            layout.ColumnCount = 2;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowCount = 6;
            for (int i = 0; i < 5; i++)
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));

            txtMedicine = new TextBox() { Dock = DockStyle.Fill };
            txtDosage = new TextBox() { Dock = DockStyle.Fill };
            txtSchedule = new TextBox() { Dock = DockStyle.Fill };
            txtNotes = new TextBox() { Dock = DockStyle.Fill };

            chkDates = new CheckBox() { Text = "Указать даты", Dock = DockStyle.Fill };
            dtFrom = new DateTimePicker() { Format = DateTimePickerFormat.Short, Enabled = false, Width = 130 };
            dtTo = new DateTimePicker() { Format = DateTimePickerFormat.Short, Enabled = false, Width = 130 };
            chkDates.CheckedChanged += (s, e) => { dtFrom.Enabled = chkDates.Checked; dtTo.Enabled = chkDates.Checked; };

            var datesPanel = new FlowLayoutPanel() { Dock = DockStyle.Fill };
            datesPanel.Controls.Add(new Label() { Text = "с", TextAlign = ContentAlignment.MiddleLeft, AutoSize = true, Margin = new Padding(0, 10, 4, 0) });
            datesPanel.Controls.Add(dtFrom);
            datesPanel.Controls.Add(new Label() { Text = "по", TextAlign = ContentAlignment.MiddleLeft, AutoSize = true, Margin = new Padding(4, 10, 4, 0) });
            datesPanel.Controls.Add(dtTo);

            var rows = new (string label, Control ctrl)[]
            {
                ("Препарат *", txtMedicine),
                ("Дозировка", txtDosage),
                ("Схема приёма", txtSchedule),
                ("Примечание", txtNotes),
                ("", chkDates),
            };

            for (int i = 0; i < rows.Length; i++)
            {
                var lbl = new Label() { Text = rows[i].label, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
                layout.Controls.Add(lbl, 0, i);
                layout.Controls.Add(rows[i].ctrl, 1, i);
            }

            layout.Controls.Add(datesPanel, 1, 4);

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
            if (string.IsNullOrWhiteSpace(txtMedicine.Text))
            {
                MessageBox.Show("Укажите название препарата", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var conn = DB.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Prescriptions (AppointmentId, Medicine, Dosage, Schedule, DateFrom, DateTo, Notes)
                VALUES (@aid, @md, @ds, @sc, @df, @dt, @nt)
            ";
            cmd.Parameters.AddWithValue("@aid", appointmentId);
            cmd.Parameters.AddWithValue("@md", txtMedicine.Text.Trim());
            cmd.Parameters.AddWithValue("@ds", txtDosage.Text.Trim());
            cmd.Parameters.AddWithValue("@sc", txtSchedule.Text.Trim());
            cmd.Parameters.AddWithValue("@df", chkDates.Checked ? dtFrom.Value.ToString("yyyy-MM-dd") : "");
            cmd.Parameters.AddWithValue("@dt", chkDates.Checked ? dtTo.Value.ToString("yyyy-MM-dd") : "");
            cmd.Parameters.AddWithValue("@nt", txtNotes.Text.Trim());
            cmd.ExecuteNonQuery();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}