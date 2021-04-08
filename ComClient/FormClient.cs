using myLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComClient
{
    public partial class FormClient : Form
    {
        public FormClient()
        {
            InitializeComponent();
        }

        /*
            [ Client ]
            1. 서버 IP : Port 설정 < From DB
            2. Connect 요청 - Loop / Thread
            3. ClientProcess Thread 실행 
              - 연결 상태 점검(에러 시 재접속 시도 : 오류 표시 후 무한 루프)
              - 서버로부터의 문자열을 지속적으로 화면에 표출
              - 사용자 요구 시 (Button / Menu) 문자열 서버 전송
            
            [ Server ]
            4. 서버에서는 문자열 수신 시 Socker 생성(서버에서는 문자열 수신 시 즉시 "OK" 문자열 회답하는 것으로 결정)
              - Client로부터의 문자열을 지속적으로 화면에 표출
              - 사용자 요구 시 (Button / Menu) 문자열 Client 전송
        */

        string init_IP = "";
        int    init_Port = 0;
        Socket sock = null;
        Thread thread = null;

        private void btnConnect_Click(object sender, EventArgs e)
        {
            // 서버와의 채널(세션)을 유지하도록 설정
            if(sock == null)
            {
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            sock.Connect(tbIP.Text, int.Parse(tbPort.Text));

            if(thread == null)
            {
                thread = new Thread(ClientProcess);
                thread.Start();
            }
        }


        delegate void cbAddText(string str);

        void AddText(string str)
        {
            if (tbClient.InvokeRequired)
            {
                cbAddText at = new cbAddText(AddText);
                object[] bArr = { str };
                Invoke(at, bArr); // Invoke([Delegate method] / [params object[] args])
                // 다르게 표현도 가능 => Invoke(at, new object[] { str });
            }
            else
            {
                tbClient.Text += str;
            }
        }

        void ClientProcess()
        {
            if (sock != null && sock.Connected) // 서버와의 연결이 되어있다면,
            {
                int n = sock.Available; // 데이터가 읽어와야할 개수 만큼
                byte[] bArr = new byte[n]; // 동적으로 byte array 할당
                sock.Receive(bArr); // 읽은 데이터를 기준으로
                AddText(Encoding.Default.GetString(bArr));
                // tbClient.Text += Encoding.Default.GetString(bArr); // 그냥 bArr만 추가해주는 것은 불가능
            }
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(tbIP.Text, int.Parse(tbPort.Text));

            int ret = sock.Send(Encoding.Default.GetBytes(tbClient.Text)); // 데이터를 전달하고

            byte[] bArr = new byte[200];

            int n = sock.Receive(bArr); // 서버로부터 날아온 메세지를 receive하여 즉시 수행
            tbClient.Text += $"{Encoding.Default.GetString(bArr, 0, n)}"; // bArr에 대한 변환 작업 필요(시작과 끝도 포함)
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

        

        iniFile inif;

        private void FormClient_Load(object sender, EventArgs e)
        {
            int x1, y1, sizeX, sizeY;

            inif = new iniFile(".\\ComClient.ini");

            init_IP     = inif.GetString("Comm", "IP", "127.0.0.1"); // Section [Comm],   Key [IP : Port],    ...    , FileName : ComClient.ini
            init_Port   = int.Parse(inif.GetString("Comm", "Port", "9001"));  
            x1          = int.Parse(inif.GetString("Form", "LocX", $"0"));
            y1          = int.Parse(inif.GetString("Form", "LocY", $"0")); 
            sizeX       = int.Parse(inif.GetString("Form", "SizeX", $"500")); 
            sizeY       = int.Parse(inif.GetString("Form", "SizeY", $"500")); 
            splitContainer1.SplitterDistance = int.Parse(inif.GetString("Form", "Splitter", $"300"));

            Location = new Point(x1, y1);
            Size = new Size(sizeX, sizeY);
            tbIP.Text = init_IP;
            tbPort.Text = $"{init_Port}";
        }

        private void FormClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            inif.SetString("Comm", "IP",        tbIP.Text);
            inif.SetString("Comm", "Port",      tbPort.Text); // init_Port = int.Parse(sb.ToString());
            inif.SetString("Form", "LocX",      $"{Location.X}");
            inif.SetString("Form", "LocY",      $"{Location.Y}");
            inif.SetString("Form", "SizeX",     $"{Size.Width}");
            inif.SetString("Form", "SizeY",     $"{Size.Height}");
            inif.SetString("Form", "Splitter",  $"{splitContainer1.SplitterDistance}");
        }

    }
}
