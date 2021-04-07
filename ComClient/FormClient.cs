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
            int x1, y1, sizeX, sizeY;
            StringBuilder sb = new StringBuilder(20);
            GetPrivateProfileString("Comm", "IP", "127.0.0.1", sb, 512, "E:\\SW기술진흥협회 교육\\C++ 프로젝트 파일\\ComunicateTest\\ComClient\bin\\Debug\\ComClient.ini"); init_IP = sb.ToString(); // Section [Comm],   Key [IP : Port],    ...    , FileName : ComClient.ini
            GetPrivateProfileString("Comm", "Port", "9001", sb, 512, "E:\\SW기술진흥협회 교육\\C++ 프로젝트 파일\\ComunicateTest\\ComClient\bin\\Debug\\ComClient.ini");    init_Port = int.Parse(sb.ToString());
            GetPrivateProfileString("Comm", "LocX", $"0", sb, 512, "E:\\SW기술진흥협회 교육\\C++ 프로젝트 파일\\ComunicateTest\\ComClient\bin\\Debug\\ComClient.ini"); x1 = int.Parse(sb.ToString());
            GetPrivateProfileString("Comm", "LocY", $"0", sb, 512, "E:\\SW기술진흥협회 교육\\C++ 프로젝트 파일\\ComunicateTest\\ComClient\bin\\Debug\\ComClient.ini"); y1 = int.Parse(sb.ToString());
            GetPrivateProfileString("Comm", "SizeX", $"500", sb, 512, "E:\\SW기술진흥협회 교육\\C++ 프로젝트 파일\\ComunicateTest\\ComClient\bin\\Debug\\ComClient.ini"); sizeX = int.Parse(sb.ToString()); 
            GetPrivateProfileString("Comm", "SizeY", $"500", sb, 512, "E:\\SW기술진흥협회 교육\\C++ 프로젝트 파일\\ComunicateTest\\ComClient\bin\\Debug\\ComClient.ini"); sizeY = int.Parse(sb.ToString());
            GetPrivateProfileString("Comm", "Splitter", $"300", sb, 512, "E:\\SW기술진흥협회 교육\\C++ 프로젝트 파일\\ComunicateTest\\ComClient\bin\\Debug\\ComClient.ini"); SplitContainer1 = int.Parse(sb.ToString());

            Location = new Point()
            tbIP.Text = init_IP;
            tbPort.Text = $"{init_Port}";
        }

        private void FormClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            WritePrivateProfileString("Comm", "IP", tbIP.Text, "E:\\SW기술진흥협회 교육\\C++ 프로젝트 파일\\ComunicateTest\\ComClient\bin\\Debug\\ComClient.ini");
            WritePrivateProfileString("Comm", "Port", tbPort.Text, "E:\\SW기술진흥협회 교육\\C++ 프로젝트 파일\\ComunicateTest\\ComClient\bin\\Debug\\ComClient.ini"); // init_Port = int.Parse(sb.ToString());
            WritePrivateProfileString("Form", "LocX", $"{Location.X}", "E:\\SW기술진흥협회 교육\\C++ 프로젝트 파일\\ComunicateTest\\ComClient\bin\\Debug\\ComClient.ini");
            WritePrivateProfileString("Form", "LocY", $"{Location.Y}", "E:\\SW기술진흥협회 교육\\C++ 프로젝트 파일\\ComunicateTest\\ComClient\bin\\Debug\\ComClient.ini");
            WritePrivateProfileString("Form", "SizeX", $"{Size.Width}", "E:\\SW기술진흥협회 교육\\C++ 프로젝트 파일\\ComunicateTest\\ComClient\bin\\Debug\\ComClient.ini");
            WritePrivateProfileString("Form", "SizeY", $"{Size.Height}", "E:\\SW기술진흥협회 교육\\C++ 프로젝트 파일\\ComunicateTest\\ComClient\bin\\Debug\\ComClient.ini");
            
        }
    }
}
