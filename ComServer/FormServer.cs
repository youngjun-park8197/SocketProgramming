using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComunicateTest
{
    public partial class FormServer : Form
    {
        public FormServer()
        {
            InitializeComponent();
        }

        // 통신 메세지 단위 : 1024Byte

        private void btnServStart_Click(object sender, EventArgs e)
        {
            // 1. 무한 수신 대기(Waiting)
            // 2. 기존 이벤트 처리
            // 3. 외부 접속 요청 수신 시 해당 요청 처리
            if (thread == null)
            {
                thread = new Thread(ServerProcess); // Thread로 묶여 들어갈 프로세스를 넣어줌(메소드명으로)
                thread.Start();  // Thread의 시작을 알려줘야 함
            }
            else
            {
                thread.Resume();
            }
            sbServMessage1.Text = "Thread is on Running.";
            timer1.Enabled = true;
        }

        private void btnServStop_Click(object sender, EventArgs e)
        {
            thread.Suspend();
            
            if (thread.IsAlive) sbServMessage1.Text = "Thread is Alive.";
            else sbServMessage1.Text = "Thread is Dead.";
        }

        private void FormServer_FormClosed(object sender, FormClosedEventArgs e)
        {
            thread.Abort();
        }


        // ################# 공통 사용 외부 변수 선언부 #################
        Thread thread = null;
        string MainMsg; // tbServer의 Text 내용
        TcpListener listener = null;
        byte[] bArr = new byte[200];
        // ##############################################################


        private void ServerProcess() // 별도의 프로세스 형태로 구현(쓰레드로)
        {
            while(true)
            {
                if(listener == null) listener = new TcpListener(int.Parse(tbServerPort.Text)); // 서버에서는 자신의 IP 필요 X, 서버의 포트번호만 수신
                listener.Start(); // 서버 측에서 listen을 하며 대기 (stop 명령까지 계속 수행)
                // sbServMessage1.Text = "Listening ... ";
                // Invalidate(); // 폼 전체에 대해 invalidate 수행
                if(listener.Pending()) // 현재 대기중인 요청이 있는지
                {
                    TcpClient tcp = listener.AcceptTcpClient(); // 외부로부터의 접속 요청 수신 : Blocking / Non-Blocking
                    // sbServMessage1.Text = "Connected";
                    // Socket sock = listener.AcceptSocket(); // 동일한 listner로 소켓을 받는 것도 가능 
                    NetworkStream ns = tcp.GetStream(); // 네트워크스트림을 이용해 데이터를 가져옴

                    while (ns.DataAvailable) // 읽고 있는 순간에도 데이터가 계속 들어올 수 있음 (데이터가 몇개인지 알수가 없을 수 있음)
                    {
                        ns.Read(bArr, 0, 200);
                        MainMsg += Encoding.Default.GetString(bArr); // invoke, delegate : 메세지 갱신
                    }
                    //tbServer.Text += Encoding.Default.GetString(bArr);
                }
                listener.Stop();
            }
        }

        private void btnSend_Click(object sender, EventArgs e) // 하나의 패킷을 보내는 버튼
        {
            // 1. 소켓 생성 : 주소가 없음
            // 2. 소켓 열기 -> Connection 수립 -> 주소 부여
            // 3. 메세지 전송 : Message - 문자열 외 이미지 및 동영상도 가능(단, 양측이 서로 약속된 규약을 전제로 함) => 프로토콜

            // TCP : 수신 확인, UDP : 수신 확인 없음
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(tbIPAddr.Text, int.Parse(tbPort.Text)); // tbPort : 클라이언트 측면에서 어디에 있는지 모르는 서버의 포트
            string str = tbClient.Text;

            // char[] == string : C#에서는 char가 기본적으로 2bytes 
            byte[] bArr = Encoding.Default.GetBytes(str); // 문자열을 byte로 받는 메소드(GetBytes)

            sock.Send(bArr);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tbServer.Text = MainMsg; // 100ms 마다 tbServer 텍스트 내용이 갱신됨
        }
    }
}
