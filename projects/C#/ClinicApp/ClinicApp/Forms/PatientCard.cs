using ClinicApp.Database;
using ClinicApp.Models;
using Microsoft.Data.Sqlite;

namespace ClinicApp.Forms
{
    public class PatientCard : Form
    {
        private Patient patient;
        private TabControl tabs;
        private ListView appointmentList;

        public PatientCard(Patient p)
        {
            patient = p;
            SetupUI();
            LoadAppointments();
        }

        private void SetupUI()
        {
            this.Text = $"Карточка: {patient.FullName}";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 10);

            tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;

            // --- вкладка 1: личные данные ---
            var tabInfo = new TabPage("Личные данные");
            var info = new TableLayoutPanel();
            info.Dock = DockStyle.Fill;
            info.Padding = new Padding(12);
            info.ColumnCount = 2;
            info.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            info.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var fields = new (string label, string value)[]
            {
                ("ФИО", patient.FullName),
                ("Дата рождения", patient.BirthDate ?? ""),
                ("Пол", patient.Gender ?? ""),
                ("Телефон", patient.Phone ?? ""),
                ("Доп. телефон", patient.PhoneExtra ?? ""),
                ("Адрес", patient.Address ?? ""),
                ("Полис", patient.Policy ?? ""),
                ("СНИЛС", patient.Snils ?? ""),
                ("Примечание", patient.Notes ?? ""),
            };

            info.RowCount = fields.Length + 1;
            for (int i = 0; i < fields.Length; i++)
                info.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            info.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            for (int i = 0; i < fields.Length; i++)
            {
                var lbl = new Label();
                lbl.Text = fields[i].label + ":";
                lbl.Dock = DockStyle.Fill;
                lbl.TextAlign = ContentAlignment.MiddleLeft;
                lbl.ForeColor = Color.Gray;

                var val = new Label();
                val.Text = fields[i].value;
                val.Dock = DockStyle.Fill;
                val.TextAlign = ContentAlignment.MiddleLeft;
                val.Font = new Font("Segoe UI", 10, FontStyle.Regular);

                info.Controls.Add(lbl, 0, i);
                info.Controls.Add(val, 1, i);
            }

            tabInfo.Controls.Add(info);

            // --- вкладка 2: приёмы ---
            var tabAppt = new TabPage("Приёмы");
            var apptPanel = new Panel();
            apptPanel.Dock = DockStyle.Fill;

            appointmentList = new ListView();
            appointmentList.Dock = DockStyle.Fill;
            appointmentList.View = View.Details;
            appointmentList.FullRowSelect = true;
            appointmentList.GridLines = true;
            appointmentList.Columns.Add("Дата", 100);
            appointmentList.Columns.Add("Диагноз", 200);
            appointmentList.Columns.Add("Заключение", 300);
            appointmentList.Columns.Add("Следующий приём", 130);
            appointmentList.DoubleClick += (s, e) => OpenAppointment();

            var apptButtons = new Panel();
            apptButtons.Dock = DockStyle.Bottom;
            apptButtons.Height = 45;
            apptButtons.Padding = new Padding(8, 6, 8, 6);

            var btnAddAppt = new Button();
            btnAddAppt.Text = "Новый приём";
            btnAddAppt.Size = new Size(130, 32);
            btnAddAppt.Click += (s, e) => AddAppointment();

            var btnDelAppt = new Button();
            btnDelAppt.Text = "Удалить";
            btnDelAppt.Size = new Size(100, 32);
            btnDelAppt.Location = new Point(145, 0);
            btnDelAppt.Click += (s, e) => DeleteAppointment();

            apptButtons.Controls.Add(btnAddAppt);
            apptButtons.Controls.Add(btnDelAppt);

            apptPanel.Controls.Add(appointmentList);
            apptPanel.Controls.Add(apptButtons);
            tabAppt.Controls.Add(apptPanel);

            // --- вкладка 3: диспансеризация ---
            var tabDisp = new TabPage("Диспансеризация");
            var dispLabel = new Label();
            dispLabel.Text = "В разработке";
            dispLabel.Dock = DockStyle.Fill;
            dispLabel.TextAlign = ContentAlignment.MiddleCenter;
            dispLabel.ForeColor = Color.Gray;
            tabDisp.Controls.Add(dispLabel);

            tabs.TabPages.Add(tabInfo);
            tabs.TabPages.Add(tabAppt);
            tabs.TabPages.Add(tabDisp);
            this.Controls.Add(tabs);
        }

        private void LoadAppointments()
        {
            appointmentList.Items.Clear();

            using var conn = DB.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, Date, Diagnosis, Conclusion, NextAppointment
                FROM Appointments
                WHERE PatientId = @pid
                ORDER BY Date DESC
            ";
            cmd.Parameters.AddWithValue("@pid", patient.Id);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var item = new ListViewItem(reader["Date"]?.ToString() ?? "");
                item.SubItems.Add(reader["Diagnosis"]?.ToString() ?? "");
                item.SubItems.Add(reader["Conclusion"]?.ToString() ?? "");
                item.SubItems.Add(reader["NextAppointment"]?.ToString() ?? "");
                item.Tag = reader["Id"];
                appointmentList.Items.Add(item);
            }
        }

        private void AddAppointment()
        {
            var form = new AppointmentForm(patient.Id);
            if (form.ShowDialog() == DialogResult.OK)
                LoadAppointments();
        }

        private void OpenAppointment()
        {
            if (appointmentList.SelectedItems.Count == 0) return;
            var id = (long)appointmentList.SelectedItems[0].Tag;
            var form = new AppointmentForm(patient.Id, id);
            if (form.ShowDialog() == DialogResult.OK)
                LoadAppointments();
        }

        private void DeleteAppointment()
        {
            if (appointmentList.SelectedItems.Count == 0) return;

            var result = MessageBox.Show("Удалить приём?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            var id = (long)appointmentList.SelectedItems[0].Tag;
            using var conn = DB.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Appointments WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            LoadAppointments();
        }
    }
}