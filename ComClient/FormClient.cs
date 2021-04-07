using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComClient
{
    public partial class FormClient : Form
    {
        // 파일 형태로 ip 주소와 port에 대한 정보를 가져오려는 작업
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string sec, string key, string defStr, StringBuilder sb, int sbSize, string p);

        [DllImport("kernel32")]
        static extern int WritePrivateProfileString(string sec, string key, string defStr, string p);

        public FormClient()
        {
            InitializeComponent();
        }

        string  init_IP = "127.0.0.1";
        int     init_Port = 9001;

        private void btnSend_Click(object sender, EventArgs e)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(tbIP.Text, int.Parse(tbPort.Text));
            
            int ret = sock.Send(Encoding.Default.GetBytes(tbClient.Text));
            if (ret > 0) sbMessage.Text = $"{ret} byte(s) send success.";

            /*
             sock.Send(Encoding.Default.GetBytes(tbClient.Text)); 를 아래와 같이 표현 가능
            ----------------------------------------------
             string str = tbClient.Text;
             byte[] bArr = Encoding.Default.GetBytes(str);
             sock.Send(bArr);
            ----------------------------------------------
            */
        }

        private void FormClient_Load(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder(20);
            GetPrivateProfileString("Comm", "IP", "127.0.0.1", sb, 512, "\\ComClient.ini"); init_IP = sb.ToString(); // Section [Comm],   Key [IP : Port],    ...    , FileName : ComClient.ini
            GetPrivateProfileString("Comm", "Port", "9001", sb, 512, "\\ComClient.ini");    init_Port = int.Parse(sb.ToString());

            tbIP.Text = init_IP;
            tbPort.Text = $"{init_Port}";
        }
    }
}
