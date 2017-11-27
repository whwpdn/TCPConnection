using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPConnection
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string line;
            char delimiter = ':';
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            System.IO.StreamReader file = new System.IO.StreamReader(".\\config");

            while((line= file.ReadLine())!=null){
                String[] substrings = line.Split(delimiter);
                if(substrings[1] == "server"){
                     Application.Run(new ServerForm());
                }
                else if(substrings[1] == "client"){
                    Application.Run(new ClientForm());
                }
            }
           
            //Application.Run(new ClientForm());
        }
    }
}
