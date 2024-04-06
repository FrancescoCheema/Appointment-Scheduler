using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace C969
{
    public class ReportGenerator
    {
        private string connectionString;

        public ReportGenerator(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Dictionary<int, List<UserReport>> GenerateUserScheduleReport()
        {
            Dictionary<int, List<UserReport>> report = new Dictionary<int, List<UserReport>>();

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT UserID, AppointmentID, CustomerID, Start, End FROM Appointment";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int userId = reader.GetInt32(0);
                        int appointmentID = reader.GetInt32(1);
                        int customerID = reader.GetInt32(2);
                        DateTime start = reader.GetDateTime(3);
                        DateTime end = reader.GetDateTime(4);

                        UserReport userReport = new UserReport(appointmentID, userId, customerID, start, end);

                        report.TryGetValue(userId, out List<UserReport> userReports);
                        report[userId] = userReports ?? new List<UserReport>();
                        report[userId].Add(userReport);
                    }
                }
            }

            return report;
        }

        public Dictionary<string, Dictionary<string, int>> GenerateAppointmentTypesByMonthReport()
        {
            {
                Dictionary<string, Dictionary<string, int>> report = new Dictionary<string, Dictionary<string, int>>();

                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT Type, MONTH(Start) AS Month, COUNT(*) AS Count FROM Appointment GROUP BY Type, MONTH(Start)";
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        report = Enumerable.Range(0, reader.FieldCount)
                            .ToDictionary(
                                i => reader.GetName(i),
                                i => new Dictionary<string, int>()
                            );

                        while (reader.Read())
                        {
                            string type = reader.GetString("Type");
                            string month = reader.GetInt32("Month").ToString("00");
                            int count = reader.GetInt32("Count");

                            report["Month"].TryGetValue(month, out int monthCount);
                            report["Month"][month] = monthCount + count;

                            report["Type"].TryGetValue(type, out int typeCount);
                            report["Type"][type] = typeCount + count;
                        }
                    }
                }

                return report;
            }
        }

        public Dictionary<int, List<Appointments>> GenerateUserScheduleReportByCustomerID()
        {

            Dictionary<int, List<Appointments>> report = new Dictionary<int, List<Appointments>>();

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                string query = @"
            SELECT CustomerID, AppointmentID, Start, End
            FROM Appointment
            ORDER BY CustomerID, Start;
        ";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int customerID = reader.GetInt32(0);
                        int appointmentID = reader.GetInt32(1);
                        DateTime start = reader.GetDateTime(2);
                        DateTime end = reader.GetDateTime(3);

                        Appointments appointment = new Appointments(appointmentID, customerID, start, end);

                        if (!report.TryGetValue(customerID, out List<Appointments> appointments))
                        {
                            appointments = new List<Appointments>();
                            report[customerID] = appointments;
                        }

                        appointments.Add(appointment);
                    }
                }
            }

            return report;
        }

        public List<Appointments> ViewAppointmentsForDay(DateTime date)
        {
            List<Appointments> appointmentsForDay = new List<Appointments>();

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                string query = @"
            SELECT AppointmentID, CustomerID, Start, End
            FROM Appointment
            WHERE DATE(Start) = @Date;
        ";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Date", date.Date);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int appointmentID = reader.GetInt32(0);
                            int customerID = reader.GetInt32(1);
                            DateTime start = reader.GetDateTime(2);
                            DateTime end = reader.GetDateTime(3);

                            Appointments appointment = new Appointments(appointmentID, customerID, start, end);
                            appointmentsForDay.Add(appointment);
                        }
                    }
                }
            }

            return appointmentsForDay;
        }

        public Dictionary<DateTime, List<Appointments>> ViewAppointmentCalendarByMonth(DateTime month)
        {
            Dictionary<DateTime, List<Appointments>> calendar = new Dictionary<DateTime, List<Appointments>>();

            DateTime firstDayOfMonth = new DateTime(month.Year, month.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                string query = @"
                SELECT AppointmentID, CustomerID, Start, End
                FROM Appointment
                WHERE Start >= @FirstDayOfMonth AND End <= @LastDayOfMonth;
            ";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@FirstDayOfMonth", firstDayOfMonth.Date);
                    cmd.Parameters.AddWithValue("@LastDayOfMonth", lastDayOfMonth.Date);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int appointmentID = reader.GetInt32(0);
                            int customerID = reader.GetInt32(1);
                            DateTime start = reader.GetDateTime(2);
                            DateTime end = reader.GetDateTime(3);

                            Appointments appointment = new Appointments(appointmentID, customerID, start, end);
                            DateTime appointmentDate = start.Date;

                            if (!calendar.ContainsKey(appointmentDate))
                            {
                                calendar[appointmentDate] = new List<Appointments>();
                            }

                            calendar[appointmentDate].Add(appointment);
                        }
                    }
                }
            }

            return calendar;
        }
    }
}
