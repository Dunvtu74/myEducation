using ClinicApp.Database;
using ClinicApp.Forms;
using ClinicApp.Models;
using Microsoft.Data.Sqlite;

namespace ClinicApp
{
    public partial class Form1 : Form
    {
        private ListView patientList;
        private Label lblCount;

        public Form1()
        {
            InitializeComponent();
            SetupUI();
            LoadPatients();
        }

        private void SetupUI()
        {
            this.Text = "Учёт пациентов";
            this.Size = new Size(1000, 650);
            this.MinimumSize = new Size(700, 400);
            this.BackColor = Theme.Background;
            this.Font = Theme.FontMain;

            // меню сверху
            var menuStrip = new MenuStrip();
            menuStrip.BackColor = SystemColors.MenuBar;
            menuStrip.Font = Theme.FontMain;

            var menuFile = new ToolStripMenuItem("Файл");
            var menuHelp = new ToolStripMenuItem("Справка");
            var menuAbout = new ToolStripMenuItem("О программе");
            menuAbout.Click += (s, e) => MessageBox.Show(
                "Учёт пациентов v1.0\n\nРазработано Альбертом Мамбетовым",
                "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
            menuHelp.DropDownItems.Add(menuAbout);
            menuStrip.Items.Add(menuFile);
            menuStrip.Items.Add(menuHelp);
            this.MainMenuStrip = menuStrip;

            // тулбар
            var toolBar = new ToolStrip();
            toolBar.BackColor = SystemColors.ButtonFace;
            toolBar.Font = Theme.FontMain;
            toolBar.GripStyle = ToolStripGripStyle.Hidden;

            var tbAdd = new ToolStripButton("Создать");
            tbAdd.Click += (s, e) => AddPatient();

            var tbEdit = new ToolStripButton("Открыть");
            tbEdit.Click += (s, e) => EditPatient();

            var tbDelete = new ToolStripButton("Удалить");
            tbDelete.Click += (s, e) => DeletePatient();

            var sep = new ToolStripSeparator();

            var tbSearch = new ToolStripLabel("Поиск:");
            var tbSearchBox = new ToolStripTextBox();
            tbSearchBox.Size = new Size(200, 22);
            tbSearchBox.TextChanged += (s, e) => LoadPatients(tbSearchBox.Text);

            toolBar.Items.Add(tbAdd);
            toolBar.Items.Add(tbEdit);
            toolBar.Items.Add(tbDelete);
            toolBar.Items.Add(sep);
            toolBar.Items.Add(tbSearch);
            toolBar.Items.Add(tbSearchBox);

            // список
            patientList = new ListView();
            patientList.Dock = DockStyle.Fill;
            patientList.View = View.Details;
            patientList.FullRowSelect = true;
            patientList.GridLines = true;
            patientList.MultiSelect = false;
            Theme.StyleListView(patientList);
            patientList.Columns.Add("ФИО", 240);
            patientList.Columns.Add("Дата рождения", 120);
            patientList.Columns.Add("Телефон", 140);
            patientList.Columns.Add("Полис", 150);
            patientList.Columns.Add("Адрес", 220);
            patientList.DoubleClick += (s, e) => EditPatient();

            // статус бар
            var statusStrip = new StatusStrip();
            statusStrip.Font = Theme.FontSmall;
            lblCount = new Label();
            statusStrip.BackColor = SystemColors.ButtonFace;

            var statusLabel = new ToolStripStatusLabel();
            statusLabel.Spring = true;
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            statusLabel.Text = "Готово";

            var authorLabel = new ToolStripStatusLabel();
            authorLabel.Text = "Разработано Альбертом Мамбетовым  |  v1.0";
            authorLabel.ForeColor = Theme.TextGray;

            statusStrip.Items.Add(statusLabel);
            statusStrip.Items.Add(new ToolStripSeparator());
            statusStrip.Items.Add(authorLabel);

            this.Controls.Add(patientList);
            this.Controls.Add(toolBar);
            this.Controls.Add(menuStrip);
            this.Controls.Add(statusStrip);

            // сохраняем ссылку на statusLabel чтобы обновлять счётчик
            patientList.Tag = statusLabel;
        }

        private void LoadPatients(string search = "")
        {
            patientList.Items.Clear();

            using var conn = DB.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, LastName, FirstName, MiddleName, BirthDate, Phone, Policy, Address
                FROM Patients
                WHERE LOWER(LastName) LIKE LOWER(@s)
                   OR LOWER(FirstName) LIKE LOWER(@s)
                   OR LOWER(MiddleName) LIKE LOWER(@s)
                   OR Phone LIKE @s
                ORDER BY LastName, FirstName
            ";
            cmd.Parameters.AddWithValue("@s", $"%{search}%");

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var fullName = $"{reader["LastName"]} {reader["FirstName"]} {reader["MiddleName"]}".Trim();
                var item = new ListViewItem(fullName);
                item.SubItems.Add(reader["BirthDate"]?.ToString() ?? "");
                item.SubItems.Add(reader["Phone"]?.ToString() ?? "");
                item.SubItems.Add(reader["Policy"]?.ToString() ?? "");
                item.SubItems.Add(reader["Address"]?.ToString() ?? "");
                item.Tag = reader["Id"];
                patientList.Items.Add(item);
            }

            if (patientList.Tag is ToolStripStatusLabel lbl)
                lbl.Text = $"Пациентов: {patientList.Items.Count}";
        }

        private void AddPatient()
        {
            var form = new PatientForm();
            if (form.ShowDialog() == DialogResult.OK)
                LoadPatients();
        }

        private void EditPatient()
        {
            if (patientList.SelectedItems.Count == 0) return;
            var id = (long)patientList.SelectedItems[0].Tag;
            var patient = GetPatientById(id);
            if (patient == null) return;
            var card = new PatientCard(patient);
            card.ShowDialog();
            LoadPatients();
        }

        private void DeletePatient()
        {
            if (patientList.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите пациента", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var name = patientList.SelectedItems[0].Text;
            var result = MessageBox.Show($"Удалить пациента «{name}»?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            var id = patientList.SelectedItems[0].Tag;
            using var conn = DB.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Patients WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            LoadPatients();
        }

        private Patient GetPatientById(long id)
        {
            using var conn = DB.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Patients WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new Patient
            {
                Id = (int)(long)reader["Id"],
                LastName = reader["LastName"]?.ToString(),
                FirstName = reader["FirstName"]?.ToString(),
                MiddleName = reader["MiddleName"]?.ToString(),
                BirthDate = reader["BirthDate"]?.ToString(),
                Gender = reader["Gender"]?.ToString(),
                Phone = reader["Phone"]?.ToString(),
                PhoneExtra = reader["PhoneExtra"]?.ToString(),
                Address = reader["Address"]?.ToString(),
                Policy = reader["Policy"]?.ToString(),
                Snils = reader["Snils"]?.ToString(),
                Notes = reader["Notes"]?.ToString()
            };
        }
    }
}