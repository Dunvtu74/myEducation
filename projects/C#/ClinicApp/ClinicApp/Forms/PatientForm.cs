using ClinicApp.Database;
using ClinicApp.Models;
using Microsoft.Data.Sqlite;

namespace ClinicApp.Forms
{
    public class PatientForm : Form
    {
        private Patient patient;
        private bool isEdit;

        private TextBox txtLastName, txtFirstName, txtMiddleName;
        private TextBox txtPhone, txtPhoneExtra, txtAddress;
        private TextBox txtPolicy, txtSnils, txtNotes;
        private DateTimePicker dtBirth;
        private ComboBox cmbGender;
        private Button btnSave, btnCancel;

        public PatientForm(Patient p = null)
        {
            patient = p ?? new Patient();
            isEdit = p != null;
            SetupUI();
            if (isEdit) FillFields();
        }

        private void SetupUI()
        {
            this.Text = isEdit ? "Редактировать пациента" : "Новый пациент";
            this.Size = new Size(500, 580);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            var layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.Padding = new Padding(12);
            layout.ColumnCount = 2;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowCount = 12;
            for (int i = 0; i < 12; i++)
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

            txtLastName = new TextBox();
            txtFirstName = new TextBox();
            txtMiddleName = new TextBox();
            dtBirth = new DateTimePicker();
            dtBirth.Format = DateTimePickerFormat.Short;
            cmbGender = new ComboBox();
            cmbGender.Items.AddRange(new[] { "Женский", "Мужской" });
            cmbGender.DropDownStyle = ComboBoxStyle.DropDownList;
            txtPhone = new TextBox();
            txtPhoneExtra = new TextBox();
            txtAddress = new TextBox();
            txtPolicy = new TextBox();
            txtSnils = new TextBox();
            txtNotes = new TextBox();

            var fields = new (string label, Control ctrl)[]
            {
                ("Фамилия *", txtLastName),
                ("Имя *", txtFirstName),
                ("Отчество", txtMiddleName),
                ("Дата рождения", dtBirth),
                ("Пол", cmbGender),
                ("Телефон", txtPhone),
                ("Доп. телефон", txtPhoneExtra),
                ("Адрес", txtAddress),
                ("Полис", txtPolicy),
                ("СНИЛС", txtSnils),
                ("Примечание", txtNotes),
            };

            for (int i = 0; i < fields.Length; i++)
            {
                var lbl = new Label();
                lbl.Text = fields[i].label;
                lbl.Dock = DockStyle.Fill;
                lbl.TextAlign = ContentAlignment.MiddleLeft;
                lbl.Font = new Font("Segoe UI", 10);

                fields[i].ctrl.Dock = DockStyle.Fill;
                if (fields[i].ctrl is TextBox tb)
                    tb.Font = new Font("Segoe UI", 10);

                layout.Controls.Add(lbl, 0, i);
                layout.Controls.Add(fields[i].ctrl, 1, i);
            }

            // кнопки
            var btnPanel = new FlowLayoutPanel();
            btnPanel.Dock = DockStyle.Fill;
            btnPanel.FlowDirection = FlowDirection.RightToLeft;

            btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Size = new Size(100, 34);
            btnCancel.Click += (s, e) => this.Close();

            btnSave = new Button();
            btnSave.Text = "Сохранить";
            btnSave.Size = new Size(100, 34);
            btnSave.Click += (s, e) => Save();

            btnPanel.Controls.Add(btnCancel);
            btnPanel.Controls.Add(btnSave);

            layout.Controls.Add(btnPanel, 1, 11);

            this.Controls.Add(layout);
        }

        private void FillFields()
        {
            txtLastName.Text = patient.LastName;
            txtFirstName.Text = patient.FirstName;
            txtMiddleName.Text = patient.MiddleName ?? "";
            if (DateTime.TryParse(patient.BirthDate, out var dt))
                dtBirth.Value = dt;
            cmbGender.SelectedItem = patient.Gender;
            txtPhone.Text = patient.Phone ?? "";
            txtPhoneExtra.Text = patient.PhoneExtra ?? "";
            txtAddress.Text = patient.Address ?? "";
            txtPolicy.Text = patient.Policy ?? "";
            txtSnils.Text = patient.Snils ?? "";
            txtNotes.Text = patient.Notes ?? "";
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Фамилия и имя обязательны", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var conn = DB.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();

            if (isEdit)
            {
                cmd.CommandText = @"
                    UPDATE Patients SET
                        LastName=@ln, FirstName=@fn, MiddleName=@mn,
                        BirthDate=@bd, Gender=@g, Phone=@ph,
                        PhoneExtra=@pe, Address=@addr, Policy=@pol,
                        Snils=@sn, Notes=@nt
                    WHERE Id=@id
                ";
                cmd.Parameters.AddWithValue("@id", patient.Id);
            }
            else
            {
                cmd.CommandText = @"
                    INSERT INTO Patients
                        (LastName, FirstName, MiddleName, BirthDate, Gender,
                         Phone, PhoneExtra, Address, Policy, Snils, Notes)
                    VALUES
                        (@ln, @fn, @mn, @bd, @g, @ph, @pe, @addr, @pol, @sn, @nt)
                ";
            }

            cmd.Parameters.AddWithValue("@ln", txtLastName.Text.Trim());
            cmd.Parameters.AddWithValue("@fn", txtFirstName.Text.Trim());
            cmd.Parameters.AddWithValue("@mn", txtMiddleName.Text.Trim());
            cmd.Parameters.AddWithValue("@bd", dtBirth.Value.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@g", cmbGender.SelectedItem?.ToString() ?? "");
            cmd.Parameters.AddWithValue("@ph", txtPhone.Text.Trim());
            cmd.Parameters.AddWithValue("@pe", txtPhoneExtra.Text.Trim());
            cmd.Parameters.AddWithValue("@addr", txtAddress.Text.Trim());
            cmd.Parameters.AddWithValue("@pol", txtPolicy.Text.Trim());
            cmd.Parameters.AddWithValue("@sn", txtSnils.Text.Trim());
            cmd.Parameters.AddWithValue("@nt", txtNotes.Text.Trim());

            cmd.ExecuteNonQuery();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}