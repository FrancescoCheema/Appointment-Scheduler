using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Windows.Forms;

namespace C969
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, EventArgs e)
        {
            Form1 customerAdd = new Form1();
            customerAdd.FormClosed += (s, args) => { UpdateDataGridView(); };
            customerAdd.textBox1.Text = customerAdd.addressID_Counter + 1.ToString();
            customerAdd.ShowDialog();

        }

        private void CustomerUpdate(object sender, EventArgs e)
        {

        }

        private void CustomerUpdate_Load(object sender, EventArgs e)
        {

        }

        public void Main_Load(object sender, EventArgs e)
        {
            UpdateDataGridView();
            UpdateDataGridView2();
        }

        public void UpdateDataGridView()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["localdb"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string query = "SELECT address.Phone, address.Address, customer.CustomerName, address.AddressID " +
                                   "FROM address " +
                                   "JOIN customer ON address.AddressID = customer.AddressId";

                    MySqlCommand cmd = new MySqlCommand(query, con);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridView1.DataSource = dataTable;
                    dataGridView1.Update();
                    dataGridView1.Refresh();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        public void UpdateDataGridView2()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["localdb"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string query = "SELECT appointment.Title, appointment.Description, appointment.Start " +
                                   "FROM appointment ";

                    MySqlCommand cmd = new MySqlCommand(query, con);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridView2.DataSource = dataTable;
                    dataGridView2.Update();
                    dataGridView2.Refresh();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void CustomerAdd(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {

                string customerName = dataGridView1.SelectedRows[0].Cells["CustomerName"].Value.ToString();

                string connectionString = ConfigurationManager.ConnectionStrings["localdb"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    try
                    {
                        con.Open();

                        string query = @"
                    DELETE c, a
                    FROM customer c
                    LEFT JOIN address a ON c.AddressID = a.AddressID
                    WHERE c.customerName = @CustomerName;
                ";

                        MySqlCommand cmd = new MySqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@CustomerName", customerName);
                        cmd.ExecuteNonQuery();

                        UpdateDataGridView();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("No row is selected.");
            }
        }

        public void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                CustomerUpdate form = new CustomerUpdate(selectedRow);
                form.textBox5.Text = dataGridView1.SelectedRows[0].Cells["Phone"].Value.ToString();
                form.textBox2.Text = dataGridView1.SelectedRows[0].Cells["CustomerName"].Value.ToString();
                form.textBox4.Text = dataGridView1.SelectedRows[0].Cells["Address"].Value.ToString();
                form.textBox5.Text = dataGridView1.SelectedRows[0].Cells["Phone"].Value.ToString();
                form.ShowDialog();
                UpdateDataGridView(); ;
            }
            else
            {
                MessageBox.Show("No row has been selected");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            AddAppointment form = new AddAppointment();
            form.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
