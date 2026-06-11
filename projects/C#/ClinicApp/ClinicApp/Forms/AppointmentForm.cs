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

        public AppointmentForm(int patientId, long? appointmentId = null)
        {
            this.patientId = patientId;
            this.appointmentId = appointmentId;
            SetupUI();
            if (appointmentId.HasValue) LoadAppointment();
        }

        private void SetupUI()
        {
            this.Text = appointmentId.HasValue ? "Редактировать приём" : "Новый приём";
            this.Size = new Size(600, 520);
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
            layout.RowCount = 7;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));

            dtDate = new DateTimePicker();
            dtDate.Format = DateTimePickerFormat.Short;
            dtDate.Dock = DockStyle.Fill;

            txtDiagnosis = new TextBox() { Dock = DockStyle.Fill };
            txtComplaints = new TextBox() { Dock = DockStyle.Fill, Multiline = true };
            txtExamination = new TextBox() { Dock = DockStyle.Fill, Multiline = true };
            txtConclusion = new TextBox() { Dock = DockStyle.Fill, Multiline = true };

            chkNextAppt = new CheckBox();
            chkNextAppt.Text = "Назначить следующий приём";
            chkNextAppt.Dock = DockStyle.Fill;
            chkNextAppt.CheckedChanged += (s, e) => dtNext.Enabled = chkNextAppt.Checked;

            dtNext = new DateTimePicker();
            dtNext.Format = DateTimePickerFormat.Short;
            dtNext.Dock = DockStyle.Fill;
            dtNext.Enabled = false;

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
                var lbl = new Label();
                lbl.Text = rows[i].label;
                lbl.Dock = DockStyle.Fill;
                lbl.TextAlign = ContentAlignment.MiddleLeft;
                layout.Controls.Add(lbl, 0, i);
                layout.Controls.Add(rows[i].ctrl, 1, i);
            }

            layout.Controls.Add(dtNext, 1, 5);

            var btnPanel = new FlowLayoutPanel();
            btnPanel.Dock = DockStyle.Fill;
            btnPanel.FlowDirection = FlowDirection.RightToLeft;

            var btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Size = new Size(100, 34);
            btnCancel.Click += (s, e) => this.Close();

            var btnSave = new Button();
            btnSave.Text = "Сохранить";
            btnSave.Size = new Size(100, 34);
            btnSave.Click += (s, e) => Save();

            btnPanel.Controls.Add(btnCancel);
            btnPanel.Controls.Add(btnSave);
            layout.Controls.Add(btnPanel, 1, 6);

            this.Controls.Add(layout);
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

        private void Save()
        {
            using var conn = DB.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();

            var nextDate = chkNextAppt.Checked
                ? dtNext.Value.ToString("yyyy-MM-dd")
                : "";

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
                    VALUES
                        (@pid, @dt, @dg, @cp, @ex, @cn, @na)
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
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}