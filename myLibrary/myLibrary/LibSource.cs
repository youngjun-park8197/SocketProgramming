using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace myLibrary
{
    public partial class LibSource: Form // 클래스명: 상속  (Form에 관련된 모든 클래스를 가져오겠다는 의미)
    {
        public LibSource()
        {
            InitializeComponent();
        }
    }


    public class iniFile // ini 파일 사용을 위한 클래스 : Read / Write
    {
        [DllImport("kernel32")] // 사용하게 되는 클래스 내부에서 선언되어야 함
        static extern int GetPrivateProfileString(string sec, string key, string defStr, StringBuilder sb, int sbSize, string path);

        [DllImport("kernel32")]
        static extern int WritePrivateProfileString(string sec, string key, string str, string path);

        public iniFile(string path) // 클래스 생성자
        {
            iniPath = path;
        }

        string iniPath = "";

        public string getString(string sect, string key, string def = "") // null과 ""(빈 문자열) 둘은 다름, 여기서는 빈 문자열 형태
        {
            StringBuilder sb = new StringBuilder(512);
            GetPrivateProfileString(sect, key, def, sb, 512, iniPath); 

            return sb.ToString();
        }

        public int setString(string sect, string key, string val)
        {
             return WritePrivateProfileString(sect, key, val, iniPath);
        }
    }

}
