using ClinicApp.Database;
using Microsoft.Data.Sqlite;

namespace ClinicApp.Forms
{
    public class AppointmentForm : Form
    {
        private int patientId;
        private long? appointmentId;

        private DateTimePicker dtDate;
        private TextBox txtDiagnosis, txtComplaints, txtExamination, txtConclusion;
        private DateTimePicker dtNext;
        private CheckBox chkNextAppt;
        private ListView prescriptionList;

        public AppointmentForm(int patientId, long? appointmentId = null)
        {
            this.patientId = patientId;
            this.appointmentId = appointmentId;
            SetupUI();
            if (appointmentId.HasValue)
            {
                LoadAppointment();
                LoadPrescriptions();
            }
        }

        private void SetupUI()
        {
            this.Text = appointmentId.HasValue ? "Редактировать приём" : "Новый приём";
            this.Size = new Size(650, 560);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 10);

            var tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;

            // --- вкладка 1: приём ---
            var tabAppt = new TabPage("Приём");
            var layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.Padding = new Padding(12);
            layout.ColumnCount = 2;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowCount = 7;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));

            dtDate = new DateTimePicker() { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };
            txtDiagnosis = new TextBox() { Dock = DockStyle.Fill };
            txtComplaints = new TextBox() { Dock = DockStyle.Fill, Multiline = true };
            txtExamination = new TextBox() { Dock = DockStyle.Fill, Multiline = true };
            txtConclusion = new TextBox() { Dock = DockStyle.Fill, Multiline = true };

            chkNextAppt = new CheckBox() { Text = "Следующий приём", Dock = DockStyle.Fill };
            dtNext = new DateTimePicker() { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short, Enabled = false };
            chkNextAppt.CheckedChanged += (s, e) => dtNext.Enabled = chkNextAppt.Checked;

            var rows = new (string label, Control ctrl)[]
            {
                ("Дата приёма", dtDate),
                ("Диагноз", txtDiagnosis),
                ("Жалобы", txtComplaints),
                ("Осмотр", txtExamination),
                ("Заключение", txtConclusion),
                ("", chkNextAppt),
            };

            for (int i = 0; i < rows.Length; i++)
            {
                var lbl = new Label() { Text = rows[i].label, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
                layout.Controls.Add(lbl, 0, i);
                layout.Controls.Add(rows[i].ctrl, 1, i);
            }

            layout.Controls.Add(dtNext, 1, 5);

            var btnPanel = new FlowLayoutPanel() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            var btnCancel = new Button() { Text = "Отмена", Size = new Size(100, 34) };
            btnCancel.Click += (s, e) => this.Close();
            var btnSave = new Button() { Text = "Сохранить", Size = new Size(100, 34) };
            btnSave.Click += (s, e) => Save();
            btnPanel.Controls.Add(btnCancel);
            btnPanel.Controls.Add(btnSave);
            layout.Controls.Add(btnPanel, 1, 6);

            tabAppt.Controls.Add(layout);

            // --- вкладка 2: препараты ---
            var tabPresc = new TabPage("Препараты");
            var prescPanel = new Panel();
            prescPanel.Dock = DockStyle.Fill;

            prescriptionList = new ListView();
            prescriptionList.Dock = DockStyle.Fill;
            prescriptionList.View = View.Details;
            prescriptionList.FullRowSelect = true;
            prescriptionList.GridLines = true;
            prescriptionList.Columns.Add("Препарат", 150);
            prescriptionList.Columns.Add("Дозировка", 100);
            prescriptionList.Columns.Add("Схема приёма", 150);
            prescriptionList.Columns.Add("С", 90);
            prescriptionList.Columns.Add("По", 90);
            prescriptionList.Columns.Add("Примечание", 120);

            var prescButtons = new Panel();
            prescButtons.Dock = DockStyle.Bottom;
            prescButtons.Height = 45;
            prescButtons.Padding = new Padding(8, 6, 8, 6);

            var btnAddPresc = new Button() { Text = "Добавить", Size = new Size(110, 32) };
            btnAddPresc.Click += (s, e) => AddPrescription();

            var btnDelPresc = new Button() { Text = "Удалить", Size = new Size(100, 32) };
            btnDelPresc.Location = new Point(120, 0);
            btnDelPresc.Click += (s, e) => DeletePrescription();

            prescButtons.Controls.Add(btnAddPresc);
            prescButtons.Controls.Add(btnDelPresc);
            prescPanel.Controls.Add(prescriptionList);
            prescPanel.Controls.Add(prescButtons);
            tabPresc.Controls.Add(prescPanel);

            tabs.TabPages.Add(tabAppt);
            tabs.TabPages.Add(tabPresc);
            this.Controls.Add(tabs);
        }

        private void LoadAppointment()
        {
            using var conn = DB.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Appointments WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", appointmentId);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return;

            if (DateTime.TryParse(reader["Date"]?.ToString(), out var dt))
                dtDate.Value = dt;
            txtDiagnosis.Text = reader["Diagnosis"]?.ToString() ?? "";
            txtComplaints.Text = reader["Complaints"]?.ToString() ?? "";
            txtExamination.Text = reader["Examination"]?.ToString() ?? "";
            txtConclusion.Text = reader["Conclusion"]?.ToString() ?? "";

            var next = reader["NextAppointment"]?.ToString();
            if (!string.IsNullOrEmpty(next) && DateTime.TryParse(next, out var dtN))
            {
                chkNextAppt.Checked = true;
                dtNext.Value = dtN;
                dtNext.Enabled = true;
            }
        }

        private void LoadPrescriptions()
        {
            if (appointmentId == null) return;
            prescriptionList.Items.Clear();

            using var conn = DB.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Prescriptions WHERE AppointmentId = @id";
            cmd.Parameters.AddWithValue("@id", appointmentId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var item = new ListViewItem(reader["Medicine"]?.ToString() ?? "");
                item.SubItems.Add(reader["Dosage"]?.ToString() ?? "");
                item.SubItems.Add(reader["Schedule"]?.ToString() ?? "");
                item.SubItems.Add(reader["DateFrom"]?.ToString() ?? "");
                item.SubItems.Add(reader["DateTo"]?.ToString() ?? "");
                item.SubItems.Add(reader["Notes"]?.ToString() ?? "");
                item.Tag = reader["Id"];
                prescriptionList.Items.Add(item);
            }
        }

        private void AddPrescription()
        {
            if (!appointmentId.HasValue)
            {
                MessageBox.Show("Сначала сохраните приём, потом добавляйте препараты.",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var form = new PrescriptionForm(appointmentId.Value);
            if (form.ShowDialog() == DialogResult.OK)
                LoadPrescriptions();
        }

        private void DeletePrescription()
        {
            if (prescriptionList.SelectedItems.Count == 0) return;
            var result = MessageBox.Show("Удалить препарат?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            var id = (long)prescriptionList.SelectedItems[0].Tag;
            using var conn = DB.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Prescriptions WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            LoadPrescriptions();
        }

        private void Save()
        {
            using var conn = DB.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();

            var nextDate = chkNextAppt.Checked ? dtNext.Value.ToString("yyyy-MM-dd") : "";

            if (appointmentId.HasValue)
            {
                cmd.CommandText = @"
                    UPDATE Appointments SET
                        Date=@dt, Diagnosis=@dg, Complaints=@cp,
                        Examination=@ex, Conclusion=@cn, NextAppointment=@na
                    WHERE Id=@id
                ";
                cmd.Parameters.AddWithValue("@id", appointmentId);
            }
            else
            {
                cmd.CommandText = @"
                    INSERT INTO Appointments
                        (PatientId, Date, Diagnosis, Complaints, Examination, Conclusion, NextAppointment)
                    VALUES (@pid, @dt, @dg, @cp, @ex, @cn, @na)
                ";
                cmd.Parameters.AddWithValue("@pid", patientId);
            }

            cmd.Parameters.AddWithValue("@dt", dtDate.Value.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@dg", txtDiagnosis.Text.Trim());
            cmd.Parameters.AddWithValue("@cp", txtComplaints.Text.Trim());
            cmd.Parameters.AddWithValue("@ex", txtExamination.Text.Trim());
            cmd.Parameters.AddWithValue("@cn", txtConclusion.Text.Trim());
            cmd.Parameters.AddWithValue("@na", nextDate);

            cmd.ExecuteNonQuery();

            // получаем id если новый приём
            if (!appointmentId.HasValue)
            {
                var idCmd = conn.CreateCommand();
                idCmd.CommandText = "SELECT last_insert_rowid()";
                appointmentId = (long)idCmd.ExecuteScalar();
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}