﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C969
{
    public partial class AddAppointment : Form
    {
        public AddAppointment()
        {
            InitializeComponent();
            PopulateTimeZones();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private int appointmentId_Counter;

        private int userId_Counter;

        private int GetCustomerIdByName(string customerName)
        {
            string query = "SELECT CustomerID FROM customer WHERE CustomerName = @CustomerName";
            string connectionString = ConfigurationManager.ConnectionStrings["localdb"].ConnectionString;

            using (MySqlConnection con = new MySqlConnection(connectionString))
            using (MySqlCommand cmd = new MySqlCommand(query, con))
            {
                try
                {
                    cmd.Parameters.AddWithValue("@CustomerName", customerName);
                    con.Open();

                    object result = cmd.ExecuteScalar();
                    return result != null && result != DBNull.Value ? Convert.ToInt32(result) : -1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return -1;
                }
            }
        }

        private int GetUserIdByName(string userName)
        {
            string query = "SELECT UserID FROM user WHERE UserName = @UserName";
            string connectionString = ConfigurationManager.ConnectionStrings["localdb"].ConnectionString;

            using (MySqlConnection con = new MySqlConnection(connectionString))
            using (MySqlCommand cmd = new MySqlCommand(query, con))
            {
                try
                {
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    con.Open();

                    object result = cmd.ExecuteScalar();
                    return result != null && result != DBNull.Value ? Convert.ToInt32(result) : -1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return -1;
                }
            }
        }

        private int InsertNewUser(string userName, MySqlConnection con)
        {
            string userQuery = @"
        INSERT INTO user (UserName, Password, Active, CreateDate, CreatedBy, LastUpdate, LastUpdateBy)
        VALUES (@UserName, @Password, @Active, @CreateDate, @CreatedBy, @LastUpdate, @LastUpdateBy);
        SELECT LAST_INSERT_ID();
    ";

            MySqlCommand userCmd = new MySqlCommand(userQuery, con);
            userCmd.Parameters.AddWithValue("@UserName", userName);
            userCmd.Parameters.AddWithValue("@Password", "");
            userCmd.Parameters.AddWithValue("@Active", "Yes");
            userCmd.Parameters.AddWithValue("@CreateDate", DateTime.Now);
            userCmd.Parameters.AddWithValue("@CreatedBy", "sqlUser");
            userCmd.Parameters.AddWithValue("@LastUpdate", DateTime.Now);
            userCmd.Parameters.AddWithValue("@LastUpdateBy", "sqlUser");

            int userId = Convert.ToInt32(userCmd.ExecuteScalar());
            return userId;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["localdb"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    int appointmentId = appointmentId_Counter++;
                    string title = textBox4.Text;
                    string customerName = "";
                    string userName = textBox3.Text;
                    int customerId = GetCustomerIdByName(customerName);
                    int userId = GetUserIdByName(userName);

                    if (userId == 0)
                    {
                        userId = InsertNewUser(userName, con);
                    }

                    if (customerId == 0)
                    {
                        MessageBox.Show("Invalid customer. Please check the customer information.");
                        return;
                    }

                    string description = textBox5.Text;
                    string location = "Main Office";
                    string type = "Appointment";
                    string contact = "";
                    string url = ".";
                    DateTime selectedDate = monthCalendar1.SelectionStart.Date;
                    DateTime selectedTime = DateTime.ParseExact(comboBox1.Text, "hh:mm tt", CultureInfo.InvariantCulture);
                    DateTime start = selectedDate.Add(selectedTime.TimeOfDay);
                    DateTime end = start.AddHours(1);
                    DateTime createDate = DateTime.Now;
                    string createdBy = "sqlUser";
                    string lastUpdate = createDate.ToString("yyyy-MM-dd HH:mm:ss");
                    string lastUpdateBy = "sqlUser";

                    string appointmentQuery = @"
                INSERT INTO appointment (CustomerId, userId, title, description, location, contact, type, url, start, end, CreateDate, CreatedBy, LastUpdate, LastUpdateBy) 
                VALUES (@CustomerId, @UserId, @Title, @Description, @Location, @Contact, @Type, @Url, @Start, @End, @CreateDate, @CreatedBy, @LastUpdate, @LastUpdateBy)
            ";

                    MySqlCommand appointmentCmd = new MySqlCommand(appointmentQuery, con);
                    appointmentCmd.Parameters.AddWithValue("@CustomerId", customerId);
                    appointmentCmd.Parameters.AddWithValue("@UserId", userId);
                    appointmentCmd.Parameters.AddWithValue("@Title", title);
                    appointmentCmd.Parameters.AddWithValue("@Description", description);
                    appointmentCmd.Parameters.AddWithValue("@Location", location);
                    appointmentCmd.Parameters.AddWithValue("@Contact", contact);
                    appointmentCmd.Parameters.AddWithValue("@Type", type);
                    appointmentCmd.Parameters.AddWithValue("@Url", url);
                    appointmentCmd.Parameters.AddWithValue("@Start", start);
                    appointmentCmd.Parameters.AddWithValue("@End", end);
                    appointmentCmd.Parameters.AddWithValue("@CreateDate", createDate);
                    appointmentCmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                    appointmentCmd.Parameters.AddWithValue("@LastUpdate", lastUpdate);
                    appointmentCmd.Parameters.AddWithValue("@LastUpdateBy", lastUpdateBy);
                    appointmentCmd.ExecuteNonQuery();

                    

                    Main form = new Main();
                    form.UpdateDataGridView2();

                    MessageBox.Show("Appointment added successfully.");
                    this.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }


        private async void monthCalendar1_Load(object sender, DateRangeEventArgs e)
        {

        }

        private void PopulateTimeZones()
        {
            comboBox2.Items.Clear();
            foreach (var timeZone in TimeZoneInfo.GetSystemTimeZones())
            {
                comboBox2.Items.Add(timeZone.Id);
            }
        }

        private void Form_Load(object sender, EventArgs e)
        {
            TextBox.CheckForIllegalCrossThreadCalls = false;
            Label.CheckForIllegalCrossThreadCalls = false;
        }

        private async void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    string selectedTimeZoneId = string.Empty;
                    comboBox2.Invoke((Action)(() =>
                    {
                        selectedTimeZoneId = comboBox2.SelectedItem?.ToString();
                    }));

                    TimeZoneInfo selectedTimeZone = TimeZoneInfo.FindSystemTimeZoneById(selectedTimeZoneId);

                    DateTime selectedDate = monthCalendar1.SelectionStart;
                    DateTimeOffset selectedDateTimeOffset = new DateTimeOffset(selectedDate, selectedTimeZone.GetUtcOffset(selectedDate));

                    DateTime startTime = TimeZoneInfo.ConvertTime(selectedDateTimeOffset, selectedTimeZone).DateTime.AddHours(9);
                    DateTime endTime = TimeZoneInfo.ConvertTime(selectedDateTimeOffset, selectedTimeZone).DateTime.AddHours(17);

                    textBox1.Invoke((Action)(() =>
                    {
                        textBox1.Text = selectedDate.ToShortDateString().ToString();

                    }));

                    List<string> itemsToAdd = new List<string>();

                    while (startTime < endTime)
                    {
                        itemsToAdd.Add(startTime.ToString("hh:mm tt"));
                        startTime = startTime.AddMinutes(15);
                    }

                    comboBox1.Invoke((Action)(() =>
                    {
                        comboBox1.Items.Clear();
                        comboBox1.Items.AddRange(itemsToAdd.ToArray());
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            });
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        private void AddAppointment_Load(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
