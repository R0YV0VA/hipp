using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hipp
{
    public partial class Login : Form
    {
        private string _name;
        private int _port = new Random().Next(1000, 10000);
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            //max textboxe length
            textBox1.MaxLength = 20;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            _name = textBox1.Text;
            if (_name == "")
            {
                MessageBox.Show("Введіть ім'я");
            }
            else if (_name.Length < 3)
            {
                MessageBox.Show("Ім'я повинно містити не менше 3 символів");
            }
            else if (_name.Length > 20)
            {
                MessageBox.Show("Ім'я повинно містити не більше 20 символів");
            }
            else
            {
                //make a post request with json {"nickname","ip", "port", "isBusy"} to http://localhost:5256/users
                //if response is 200 then show main form
                //else show message box with text "Сервер недоступний"

                string url = "http://localhost:5256/users";
                string json = "{\n\"nickname\": \"" + _name + "\",\n\"ip\": \"" + GetIP() + "\",\n\"port\": " + _port + ",\n\"isBusy\": false\n}";
                if (await AddRequest(url, json))
                {
                    this.Hide();
                    MainField main = new MainField();
                    main.name = _name;
                    main.port_in = _port;
                    main.ip = GetIP();
                    main.Show();
                }
            }
        }
        private string GetIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private async Task<bool> AddRequest(string url, string json)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    MessageBox.Show("Таке ім'я вже існує!");
                    return false;
                }
                else
                {
                    MessageBox.Show("Сервер недоступний!");
                    return false;
                }
            }
        }
    }
}
