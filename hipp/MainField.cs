using System.Net.Sockets;
using System.Net;
using SimpleUdp;
using NAudio.Wave;
using System.Collections;
using System.Text;
using NAudio.Dsp;
using System.Net.Http.Json;
using Newtonsoft.Json;

namespace hipp
{
    public partial class MainField : Form
    {
        public string name;
        const int BIT_COUNT = 2048;
        const string SERVER = "http://localhost:5256/users/";
        bool[] KEY = new bool[8] { false, true, false, true, false, false, true, true };
        private List<TUser> usersList;
        private TUser userEndp;

        public int port_in;
        public string ip;

        List<byte> buffer = new List<byte>();
        int cI = 0;
        WaveInEvent waveIn = new WaveInEvent();

        string Message;

        UdpEndpoint udpEndpoint;
        public MainField()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            usersList = await GetRequest(SERVER);
            //remove this user from list
            var user = usersList.Find(u => u.Nickname == name);
            usersList.Remove(user);
            //put nicknames to listbox
            listBox1.Items.Clear();
            foreach (var u in usersList)
            {
                listBox1.Items.Add(u.Nickname);
            }
            label9.ForeColor = Color.Red;
            label9.Text = "Не під'єднано";
            waveIn.BufferMilliseconds = 300;
            waveIn.DeviceNumber = 0;
            waveIn.WaveFormat = new WaveFormat(48000, 16, 1);
            waveIn.DataAvailable += (sender, e) => waveIn_DataAvailable(sender, e, udpEndpoint, userEndp);
        }
        void waveIn_DataAvailable(object sender, WaveInEventArgs e, UdpEndpoint udp, TUser user)
        {
            buffer.AddRange(e.Buffer);
            var ht = HideTextLSB(buffer, Message);
            udp.Send(user.IP, user.Port, ht.ToArray());
            cI++;
            if (cI >= GetBitsCount(Message) / 8)
                cI = 0;
            buffer.Clear();
            ht.Clear();
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

        private async void button1_Click(object sender, EventArgs e)
        {
            if(listBox1.Text == "")
            {
                MessageBox.Show("Будь ласка, виберіть користувача!");
                return;
            }
            if(await GetStatusRequest(SERVER, listBox1.Text)) {
                usersList = await GetRequest(SERVER);
                //remove this user from list
                var user = usersList.Find(u => u.Nickname == name);
                usersList.Remove(user);
                //put nicknames to listbox
                listBox1.Items.Clear();
                foreach (var u in usersList)
                {
                    listBox1.Items.Add(u.Nickname);
                }
                MessageBox.Show("Користувач зайнятий або сервер не відповідає!");
                return;
            }
            userEndp = usersList.Find(u => u.Nickname == listBox1.Text);
            if(textBox1.Text.Length > 0)
                Message = textBox1.Text + "\n";
            else
            {
                Message = "null\n";
            }
            textBox1.Enabled = false;
            listBox1.Enabled = false;
            button1.Enabled = false;
            button3.Enabled = false;
            button2.Enabled = true;
            udpEndpoint = new UdpEndpoint(ip, port_in);
            udpEndpoint.EndpointDetected += async (s, e) =>
            {
                if (!await PutStatusRequest(SERVER, name, true))
                {
                    MessageBox.Show("Помилка з'єднання з сервером!");
                    return;
                }
                if (label9.InvokeRequired)
                {
                    label9.Invoke(new Action(() =>
                    {
                        label9.ForeColor = Color.Green;
                        label9.Text = $"З'єднання з {userEndp.Nickname}";
                    }));
                }
            };
            udpEndpoint.DatagramReceived += (s, e) =>
            {
                var waveOut = new WaveOutEvent();
                if(richTextBox1.InvokeRequired)
                {
                    richTextBox1.Invoke(new Action(() =>
                    {
                        richTextBox1.Text += ExtractTextLSB(new List<byte>(e.Data));
                    }));
                }
                //low pass filter for received audio
                var lowPassFilter = BiQuadFilter.LowPassFilter(48000, 1000, 1f);
                var lowPassBuffer = new List<byte>();
                for (int i = 0; i < e.Data.Length; i += 2)
                {
                    var sample = BitConverter.ToInt16(e.Data, i);
                    var sample32 = (float)sample / 32768f;
                    var filtered = lowPassFilter.Transform(sample32);
                    var filtered16 = (short)(filtered * 32768f);
                    lowPassBuffer.AddRange(BitConverter.GetBytes(filtered16));
                }
                waveOut.Init(new RawSourceWaveStream(new MemoryStream(lowPassBuffer.ToArray()), new WaveFormat(48000, 16, 1)));
                waveOut.Play();
            };
            waveIn.StartRecording();
        }
        private async void button3_Click(object sender, EventArgs e)
        {
            usersList = await GetRequest(SERVER);
            //remove this user from list
            var user = usersList.Find(u => u.Nickname == name);
            usersList.Remove(user);
            //put nicknames to listbox
            listBox1.Items.Clear();
            foreach (var u in usersList)
            {
                listBox1.Items.Add(u.Nickname);
            }
        }
        int GetBitsCount(string text)
        {
            var textBytes = Encoding.UTF8.GetBytes(text);
            var textBits = textBytes.SelectMany(b => new BitArray(new byte[] { b }).Cast<bool>()).ToList();
            return textBits.Count;
        }
        List<List<bool>> PrepareMessage(string text)
        {
            var messages = new List<List<bool>>();
            foreach (var c in text)
            {
                messages.Add(Encoding.UTF8.GetBytes(c.ToString()).SelectMany(b => new BitArray(new byte[] { b }).Cast<bool>()).ToList());
            }
            return messages;
        }
        List<byte> HideTextLSB(List<byte> _audio, string text)
        {
            var preparedText = PrepareMessage(text);
            var currentChar = XOR(preparedText[cI], KEY);
            for (int i = 1; i < _audio.Count / BIT_COUNT; i++)
            {
                if (i - 1 >= currentChar.Count)
                    return _audio;
                if (currentChar[i - 1])
                    _audio[i * BIT_COUNT - 1] = 1;
                else
                    _audio[i * BIT_COUNT - 1] = 0;
            }
            return _audio;
        }
        string ExtractTextLSB(List<byte> _audio)
        {
            var textBits = new List<bool>();
            for (int i = 1; i < _audio.Count / BIT_COUNT; i++)
            {
                if (_audio[i * BIT_COUNT - 1] == 1)
                    textBits.Add(true);
                else
                    textBits.Add(false);
            }
            textBits = XOR(textBits, KEY);
            var textBytes = new List<byte>();
            for (int i = 0; i < textBits.Count / 8; i++)
            {
                var b = new byte[1];
                new BitArray(textBits.Skip(i * 8).Take(8).ToArray()).CopyTo(b, 0);
                textBytes.Add(b[0]);
            }
            var text = Encoding.UTF8.GetString(textBytes.ToArray());
            return text[0].ToString();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            textBox1.Enabled = true;
            listBox1.Enabled = true;
            button1.Enabled = true;
            button3.Enabled = true;
            button2.Enabled = false;
            waveIn.StopRecording();
            waveIn.Dispose();
            udpEndpoint.Dispose();
            if (!await PutStatusRequest(SERVER, name, false))
            {
                MessageBox.Show("Помилка з'єднання з сервером!");
                return;
            }
            userEndp = null;
            label9.ForeColor = Color.Red;
            label9.Text = "Не під'єднано";
            
        }
        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // when user close the application, send a delete request to http://localhost:5256/users/ with query "nickname"
            await DeleteRequest(SERVER, this.name);
            Application.Exit();
        }
        private List<bool> XOR(List<bool> message, bool[] key)
        {
            var result = new List<bool>();
            for (int i = 0; i < message.Count; i++)
            {
                result.Add(message[i] ^ key[i % key.Length]);
            }
            return result;
        }
        private async Task DeleteRequest(string url, string nickname)
        {
            using (HttpClient client = new HttpClient())
            {
                await client.DeleteAsync(url + "?nickname=" + nickname);
            }
        }
        private async Task<List<TUser>> GetRequest(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<TUser>>(jsonString);
                }
                else
                {
                    MessageBox.Show("Сервер недоступний!");
                    return null;
                }
            }
        }
        private async Task<bool> GetStatusRequest(string url, string nickname)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url + nickname);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(jsonString);
                }
                else
                {
                    MessageBox.Show("Сервер недоступний!");
                    return true;
                }
            }
        }
        private async Task<bool> PutStatusRequest(string url, string nickname, bool status)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PutAsync(url + "?nickname=" + nickname + "&status=" + status.ToString().ToLower(), null);
                if (response.IsSuccessStatusCode)
                {
                    return true;
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
