using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
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
        // ################# 공통 사용 외부 변수 선언부 #################
        Thread thread = null;
        string MainMsg; // tbServer의 Text 내용
        TcpListener listener = null;
        byte[] bArr = new byte[200];
        // ##############################################################

        delegate void cbAddText(string str); // str을 인수로 받아 tbServer 텍스트박스에 출력하는 콜백 함수

        void AddText(string str) // Thread 내부에서 호출될 함수
        {
            if(tbServer.InvokeRequired) // tbServer에 어떤 값이 써지게되면
            {
                cbAddText at = new cbAddText(AddText); // 콜백
                object[] bArr = { str }; // 배열인 이유 : 여래 인자값에 대해 처리해야할 수도 있기 때문
                Invoke(at, bArr);
            }
            else
            {
                tbServer.Text += str; // Thread에서 수행해야 할 본래 작업
            }
        }


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
            timer1.Start();
            sbServMessage1.Text = "Listener Started.";
            AddText($"ServerProcess noew started .. Open port is {tbServerPort}.");
        }


        private void btnServStop_Click(object sender, EventArgs e)
        {
            thread.Abort();
            // thread.Suspend(); // 메모리에는 남아있지만 스레드가 중단되어있는 상태 (생략)
            // thread.Resume(); // suspend 되어있는 스레드를 다시 실행 (생략)
            thread = null;
            timer1.Stop();
            sbServMessage1.Text = "Listener Stopped.";
        }


        private void ServerProcess() // 별도의 프로세스 형태로 구현(쓰레드로)
        {
            while(true)
            {
                if (listener == null) listener = new TcpListener(Dns.Resolve("localhost").AddressList[0], int.Parse(tbServerPort.Text)); // 서버에서는 자신의 IP 필요 X, 서버의 포트번호만 수신
                listener.Start(); // 서버 측에서 listen을 하며 대기 (stop 명령까지 계속 수행)
                // sbServMessage1.Text = "Listening ... "; (생략)
                // Invalidate(); // 폼 전체에 대해 invalidate 수행 (생략)

                if (listener.Pending()) // 현재 대기중인 요청이 있는지
                {
                    /*
                    TcpClient tcp = listener.AcceptTcpClient(); // 외부로부터의 접속 요청 수신 : Blocking / Non-Blocking

                    Socket sock = listener.AcceptSocket(); // Tcpclient 가 아닌 Socker을 이용해 Accept 및 입력 stream 처리
                    
                    EndPoint ep = tcp.Client.RemoteEndPoint; // 엔드포닝트에 대한 정보
                    sbServMessage1.Text = ep.ToString();

                    // sbServMessage1.Text = "Connected"; // (생략)
                    // Socket sock = listener.AcceptSocket(); // 동일한 listner로 소켓을 받는 것도 가능 (생략) 
                    NetworkStream ns = tcp.GetStream(); // 네트워크스트림을 이용해 데이터를 가져옴

                    while (ns.DataAvailable) // 읽고 있는 순간에도 데이터가 계속 들어올 수 있음 (데이터가 몇개인지 알수가 없을 수 있음)
                    {
                        int n = ns.Read(bArr, 0, 200);
                        MainMsg = Encoding.Default.GetString(bArr, 0, n); // invoke, delegate : 메세지 갱신 (byte[] ==> string 변환)
                        AddText(Encoding.Default.);
                        // MainMsg += Encoding.Default.GetString(bArr, 0, n); (생략)
                    }
                    */

                    Socket sock = listener.AcceptSocket(); // Tcpclient 가 아닌 Socker을 이용해 Accept 및 입력 stream 처리
                    byte[] bArr = new byte[sock.Available];
                    int n = sock.Receive(bArr);
                    
                    AddText(Encoding.Default.GetString(bArr, 0, n));

                    IPEndPoint ep = (IPEndPoint)sock.RemoteEndPoint; // 127.0.0.1:76543 가상의 포트 번호
                    sbIpPortMessage.Text = $"{ep.Address} : {ep.Port}"; // xxx.xxx.xxx.xxx : xxxxx
                }
                // Thread.Sleep(100); // 10 ~ 1000ms 범위 내에서 설정
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
            tbServer.Text += MainMsg; // 100ms 마다 tbServer 텍스트 내용이 갱신됨
            MainMsg = "";
        }

        private void FormServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            thread.Abort();
        }


        /*
        TcpListener listener = null;
        string mainMsg = "";

        ------- btnStart_Click 부분 -------
            Thread th = new Thread(ServerProcess);
            th.Start();
            timer1.Start();
        -----------------------------------


        --------- ServerProcess() ---------
        private void ServerProcess() {
            while(true) {
                if(listener == null) {
                    TcpListener listener = new TcpListener(int.Parse(tbServerPort.Text));
                    listener.Start();
                }

                if(listener.Pending()) {
                    TcpClient tcp = listener.AcceptTcpClient();
                    NetworkStream ns = tcp.GetStream();
                    byte[] bArr = new byte[200];

                    while(ns.DataAvailable) {
                        int n = ns.Read(bArr, 0, 200);
                        tbServer.Text += Encoding.Default.GetString(bArr, 0, n);
                    }
                }
            }
        }
        -----------------------------------


        ---------- timer1_Tick() ----------
            tbServer.Text += mainMsg;
            mainMsg = "";
        -----------------------------------
        */
    }
}
