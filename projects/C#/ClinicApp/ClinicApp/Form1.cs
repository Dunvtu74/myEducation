using ClinicApp.Database;
using ClinicApp.Models;
using Microsoft.Data.Sqlite;
using ClinicApp.Forms;

namespace ClinicApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SetupUI();
            LoadPatients();
        }

        private ListView patientList;
        private TextBox searchBox;
        private Button btnAdd, btnEdit, btnDelete;

        private void SetupUI()
        {
            this.Text = "Учёт пациентов";
            this.Size = new Size(900, 600);
            this.MinimumSize = new Size(700, 400);

            // поиск сверху
            searchBox = new TextBox();
            searchBox.PlaceholderText = "Поиск по ФИО или телефону...";
            searchBox.Dock = DockStyle.Top;
            searchBox.Height = 30;
            searchBox.Font = new Font("Segoe UI", 10);
            searchBox.TextChanged += (s, e) => LoadPatients(searchBox.Text);

            // список пациентов
            patientList = new ListView();
            patientList.Dock = DockStyle.Fill;
            patientList.View = View.Details;
            patientList.FullRowSelect = true;
            patientList.GridLines = true;
            patientList.Font = new Font("Segoe UI", 10);
            patientList.Columns.Add("ФИО", 250);
            patientList.Columns.Add("Дата рождения", 120);
            patientList.Columns.Add("Телефон", 130);
            patientList.Columns.Add("Полис", 130);
            patientList.Columns.Add("Адрес", 200);
            patientList.DoubleClick += (s, e) => EditPatient();

            // панель кнопок справа
            var panel = new Panel();
            panel.Dock = DockStyle.Right;
            panel.Width = 130;
            panel.Padding = new Padding(8);

            btnAdd = new Button();
            btnAdd.Text = "Добавить";
            btnAdd.Dock = DockStyle.Top;
            btnAdd.Height = 40;
            btnAdd.Margin = new Padding(0, 0, 0, 8);
            btnAdd.Click += (s, e) => AddPatient();

            btnEdit = new Button();
            btnEdit.Text = "Редактировать";
            btnEdit.Dock = DockStyle.Top;
            btnEdit.Height = 40;
            btnEdit.Margin = new Padding(0, 0, 0, 8);
            btnEdit.Click += (s, e) => EditPatient();

            btnDelete = new Button();
            btnDelete.Text = "Удалить";
            btnDelete.Dock = DockStyle.Top;
            btnDelete.Height = 40;
            btnDelete.Click += (s, e) => DeletePatient();

            panel.Controls.Add(btnDelete);
            panel.Controls.Add(btnEdit);
            panel.Controls.Add(btnAdd);

            this.Controls.Add(patientList);
            this.Controls.Add(panel);
            this.Controls.Add(searchBox);
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
                WHERE LastName LIKE @search OR FirstName LIKE @search 
                   OR MiddleName LIKE @search OR Phone LIKE @search
                ORDER BY LastName, FirstName
            ";
            cmd.Parameters.AddWithValue("@search", $"%{search}%");

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
        }

        private void AddPatient()
        {
            var form = new PatientForm();
            if (form.ShowDialog() == DialogResult.OK)
                LoadPatients(searchBox.Text);
        }

        private void EditPatient()
        {
            if (patientList.SelectedItems.Count == 0) return;
            var id = (long)patientList.SelectedItems[0].Tag;
            var patient = GetPatientById(id);
            if (patient == null) return;

            var form = new PatientForm(patient);
            if (form.ShowDialog() == DialogResult.OK)
                LoadPatients(searchBox.Text);
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

        private void DeletePatient()
        {
            if (patientList.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите пациента");
                return;
            }

            var result = MessageBox.Show("Удалить пациента?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            var id = patientList.SelectedItems[0].Tag;

            using var conn = DB.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Patients WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            LoadPatients(searchBox.Text);
        }
    }
}