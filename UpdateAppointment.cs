using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace C969
{
    public partial class UpdateAppointment : Form
    {
        private DataGridViewRow _selectedRow;
        public UpdateAppointment(DataGridViewRow selectedRow)
        {
            InitializeComponent();
            _selectedRow = selectedRow;
        }

        private void UpdateAppointment_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _selectedRow.Cells["Start"].Value = textBox1.Text;
            _selectedRow.Cells["Title"].Value = textBox4.Text;
            _selectedRow.Cells["Description"].Value = textBox5.Text;
            _selectedRow.Cells["CustomerID"].Value = textBox3.Text;

            string query = "UPDATE appointment SET Start = @Start, Title = @Title, Description = @Description;" +
                           "UPDATE appointment SET CustomerID = @CustomerID WHERE CustomerID = @CustomerID;";

            string connectionString = ConfigurationManager.ConnectionStrings["localdb"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    MySqlCommand updateCMD = new MySqlCommand(query, con);
                    updateCMD.Parameters.AddWithValue("@Start", textBox1.Text);
                    updateCMD.Parameters.AddWithValue("@Title", textBox4.Text);
                    updateCMD.Parameters.AddWithValue("@Description", textBox5.Text);
                    updateCMD.Parameters.AddWithValue("@CustomerID", textBox3.Text);
                    updateCMD.ExecuteNonQuery();

                    Main form = new Main();
                    form.UpdateDataGridView();

                    MessageBox.Show("Customer updated successfully");
                    this.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error" + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }
    }
}
