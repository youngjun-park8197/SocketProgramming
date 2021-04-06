using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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
    }
}
