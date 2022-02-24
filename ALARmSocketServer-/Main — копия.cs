using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ALARmSocketServer
{
    public partial class Main : Form
    {
        int lastDeactivateTick;
        bool lastDeactivateValid;
        bool firstLoading = true;
        private IPHostEntry ipHost = Dns.GetHostEntry("localhost");
        private IPAddress ipAddr;
        private IPEndPoint ipEndPoint;
        private Socket sListener;
        public Main()
        {
            InitializeComponent();
            try
            {
                ipAddr = ipHost.AddressList[0];
                ipEndPoint = new IPEndPoint(ipAddr, 11000);
                sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);
                timer1.Start();

            }
            catch
            {
                Status.AppendText("Не удалось запустить socket server!!!");
            }
        }
        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            lastDeactivateTick = Environment.TickCount;
            lastDeactivateValid = true;
            this.Hide();
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (lastDeactivateValid && Environment.TickCount - lastDeactivateTick < 1000) return;
            this.Show();
            this.Activate();
        }

       

       

        private void Main_Paint(object sender, EventArgs e)
        {   if (firstLoading)
            {
                this.Hide();
                firstLoading = false;
            }

        }
        private void StartSocketListening()
        {
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Status.Text =($"Ожидаем соединение через порт {ipEndPoint}");
            
            // Программа приостанавливается, ожидая входящее соединение
            Socket handler = sListener.Accept();
            string data = null;

            // Мы дождались клиента, пытающегося с нами соединиться

            byte[] bytes = new byte[1024];
            int bytesRec = handler.Receive(bytes);

            data += Encoding.UTF8.GetString(bytes, 0, bytesRec);

            // Показываем данные на консоли
            Status.AppendText("Полученный текст: " + data + "\n\n");

            // Отправляем ответ клиенту\
            string reply = "Спасибо за запрос в " + data.Length.ToString()
                    + " символов";
            byte[] msg = Encoding.UTF8.GetBytes(reply);
            handler.Send(msg);

            if (data.IndexOf("<TheEnd>") > -1)
            {
                Status.AppendText("Сервер завершил соединение с клиентом.");
                //timer1.Stop();
            }

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
    }
}
