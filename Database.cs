using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.IO;


namespace C969
{
    public partial class Database : Form
    {
        public static System.Globalization.CultureInfo InstalledUICulture { get; }
        public Database()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public bool button1WasClicked;

        private static string logFilePath = "Login_History.txt";

        private void button1_Click(object sender, EventArgs e)
        {
            string userName = textBox1.Text;
            string userPass = textBox2.Text;
            button1WasClicked = true;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userPass))
            {
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "it")
                {
                    MessageBox.Show("Il tuo username o la tua password é incorretta.");
                }
                else
                {

                    MessageBox.Show("You haven't entered a username or password.");
                }

                return;
            }

            if ((userName == "test" && userPass == "test") && button1WasClicked)
            {
                string constr = ConfigurationManager.ConnectionStrings["localdb"].ConnectionString;

                MySqlConnection conn = null;

                try
                {
                    conn = new MySqlConnection(constr);

                    conn.Open();

                    if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "it")
                    {
                        MessageBox.Show("Sei connesso.");

                        MessageBox.Show("Hai completato l'autenticazione con successo.");
                    }
                    else
                    {
                        MessageBox.Show("Connection is open");

                        MessageBox.Show("You've logged in successfully.");
                    }
                    string logMessage = $"{DateTime.Now.ToString()} - User test logged in.";

                    using (StreamWriter writer = new StreamWriter(logFilePath, true))
                    {
                        writer.WriteLine(logMessage);
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    conn.Close();
                }

                Main main = new Main();
                main.Show();
            }
            else
            {
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "it")
                {
                    MessageBox.Show("Il tuo username o la tua password é incorretta.");
                }
                else
                {
                    MessageBox.Show("You haven't entered a username or password.");
                }
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }
    }
}
