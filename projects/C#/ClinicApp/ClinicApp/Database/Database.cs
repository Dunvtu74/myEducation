using Microsoft.Data.Sqlite;

namespace ClinicApp.Database
{
    public class DB
    {
        private static string dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ClinicApp", "clinic.db"
        );

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source={dbPath}");
        }

        public static void Init()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath));

            using var conn = GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Patients (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    LastName TEXT NOT NULL,
                    FirstName TEXT NOT NULL,
                    MiddleName TEXT,
                    BirthDate TEXT,
                    Gender TEXT,
                    Phone TEXT,
                    PhoneExtra TEXT,
                    Address TEXT,
                    Policy TEXT,
                    Snils TEXT,
                    Notes TEXT
                );

                CREATE TABLE IF NOT EXISTS Appointments (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PatientId INTEGER NOT NULL,
                    Date TEXT NOT NULL,
                    Diagnosis TEXT,
                    Complaints TEXT,
                    Examination TEXT,
                    Conclusion TEXT,
                    NextAppointment TEXT,
                    FOREIGN KEY (PatientId) REFERENCES Patients(Id)
                );

                CREATE TABLE IF NOT EXISTS Prescriptions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    AppointmentId INTEGER NOT NULL,
                    Medicine TEXT,
                    Dosage TEXT,
                    Schedule TEXT,
                    DateFrom TEXT,
                    DateTo TEXT,
                    Notes TEXT,
                    FOREIGN KEY (AppointmentId) REFERENCES Appointments(Id)
                );
            ";
            cmd.ExecuteNonQuery();
        }
    }
}