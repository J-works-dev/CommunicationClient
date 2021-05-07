using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommunicationClient
{
    public partial class Form1 : Form
    {
        private Pipe pipeClient;
        public Form1()
        {
            InitializeComponent();
            CreateNewPipeClient();
        }

        void CreateNewPipeClient()
        {
            if (pipeClient != null)
            {
                pipeClient.MessageReceived -= pipeClient_MessageReceived;
                pipeClient.ServerDisconnected -= pipeClient_ServerDisconnected;
            }

            pipeClient = new Pipe();
            pipeClient.MessageReceived += pipeClient_MessageReceived;
            pipeClient.ServerDisconnected += pipeClient_ServerDisconnected;
        }

        void pipeClient_ServerDisconnected()
        {
            Invoke(new Pipe.ServerDisconnectedHandler(EnableStartButton));
        }

        void EnableStartButton()
        {
            buttonDisconnect.Enabled = true;
        }

        void pipeClient_MessageReceived(byte[] message)
        {
            Invoke(new Pipe.MessageReceivedHandler(DisplayReceivedMessage),
                new object[] { message });
        }

        void DisplayReceivedMessage(byte[] message)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            string str = encoder.GetString(message, 0, message.Length);

            if (str == "close")
            {
                pipeClient.Disconnect();

                CreateNewPipeClient();
                pipeClient.Connect(textBoxPipeName.Text);
            }
            else if (str == "logged in")
            {
                textBoxSend.Enabled = true;
                buttonSend.Enabled = true;
            }
            textBoxReceived.Text += str + "\r\n";
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            if (buttonDisconnect.Text == "Connect")
            {
                pipeClient.Connect(textBoxPipeName.Text);

                if (pipeClient.Connected)
                    buttonDisconnect.Text = "Disconnect";
            }
            else if (buttonDisconnect.Text == "Disconnect")
            {
                pipeClient.Disconnect();

                if (!pipeClient.Connected)
                    buttonDisconnect.Text = "Connect";
            }
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();

            string id = textBoxId.Text;
            string pw = textBoxPw.Text;

            pipeClient.SendMessage(encoder.GetBytes(id + "," + pw));
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();

            pipeClient.SendMessage(encoder.GetBytes(textBoxSend.Text));
        }
    }
}
