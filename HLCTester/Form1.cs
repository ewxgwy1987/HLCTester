using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Collections;
using System.Xml;
using System.IO;
using System.Threading;

using BHS;
using BHS.PLCSimulator;
using BHS.PLCSimulator.Messages.Telegram;
using PALS.Configure;
using PALS.Utilities;
using PALS.Telegrams;

namespace HLCTester
{
    public partial class Form1 : Form
    {
        private const string SOURCE_PLC2GW = "PLC2GW";

        private BHS.PLCSimulator.Initializer _init;
#if DEBUG
        private const string Path_XMLFileTelegram = @"../../cfg/CFG_Telegrams.xml";
        private const string Path_XMLFileSetting = @"../../cfg/CFG_PLCSimulator.xml";
        private const string Path_XMLFileTestCase = @"../../cfg/CFG_TestCase.xml";
#else
        private const string Path_XMLFileTelegram = @"./cfg/CFG_Telegrams.xml";
        private const string Path_XMLFileSetting = @"./cfg/CFG_Simulator.xml";
        private const string Path_XMLFileTestCase = @"./cfg/CFG_TestCase.xml";
#endif


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lbCommGW.BackColor = System.Drawing.Color.Red;

            try
            {
                FileInfo[] cfgFiles = new FileInfo[3];
                cfgFiles[0] = new FileInfo(Path_XMLFileSetting); // CFG_Simulator.xml
                cfgFiles[1] = new FileInfo(Path_XMLFileTelegram); // CFG_Telegram.xml
                cfgFiles[2] = new FileInfo(Path_XMLFileTestCase); // CFG_TestCase.xml

                _init = new BHS.PLCSimulator.Initializer(cfgFiles);

                if (_init.Init())
                {
                    _init.OnConnected += new EventHandler<BHS.MessageEventArgs>(Initializer_OnConnected);
                    _init.OnDisconnected += new EventHandler<BHS.MessageEventArgs>(Initializer_OnDisconnected);
                    _init.OnReceived += new EventHandler<BHS.MessageEventArgs>(Initializer_OnReceived);

                    tbxMsgLog.Text = "[" + DateTime.Now + "] " + "Application has been successfully initialized.\r\n\r\n" + tbxMsgLog.Text;
                }
                else
                {
                    tbxMsgLog.Text = "[" + DateTime.Now + "] " + "Application initialization failed.\r\n\r\n" + tbxMsgLog.Text;
                    throw new Exception("Application initialization failure!");
                }

                
                this.cbxTelegramList.Items.Clear();
                foreach (DictionaryEntry temptele in _init.HT_TelegramTestcase)
                {
                    string msg = temptele.Key.ToString()+"."+ ((SAC2PLCTelegram)temptele.Value).TelegramAlias;
                    this.cbxTelegramList.Items.Add(msg);
                }
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Form1_FormClosed(Object sender, FormClosedEventArgs e)
        {
            if (_init != null)
            {
                _init.Dispose();
                _init = null;
            }
        }

        private void ThrdFun_SendTelegramToGW(object mykeys)
        {
            int i;
            int[] keys = (int[])mykeys;
            DlgtFun_SendTelegram dlgt_WriteMsgLog = new DlgtFun_SendTelegram(WriteMsgLog);
            for (i = 0; i < keys.Length; i++)
            {
                SAC2PLCTelegram temp_tele = (SAC2PLCTelegram)_init.HT_TelegramTestcase[keys[i]];
                _init.MsgHandler.SentToGW(temp_tele.RawData);
                this.Invoke(dlgt_WriteMsgLog, temp_tele.ShowAllData());
                Thread.Sleep(1000);
            }
        }

        public delegate void DlgtFun_SendTelegram(string telegramData);
        private void WriteMsgLog(string telegramData)
        {
            tbxMsgLog.Text = "[" + DateTime.Now + "] " + "[Gateway] <= PLC MSG: <<<<<<<<<<<< \r\n" + telegramData + ">\r\n\r\n" + tbxMsgLog.Text;
        }
        
        private void btSentGW_Click(object sender, EventArgs e)
        {
            if (cbxTelegramList.CheckedItems.Count > 0)
            {
                int i;
                int[] keys = new int[cbxTelegramList.CheckedItems.Count];
                for (i = 0; i < cbxTelegramList.CheckedItems.Count; i++)
                {
                    string str_SelectedItem = cbxTelegramList.CheckedItems[i].ToString();
                    keys[i] = Convert.ToInt32(str_SelectedItem.Substring(0, str_SelectedItem.IndexOf('.')));
                }
                Thread thrd_SendTelegramToGW = new Thread(ThrdFun_SendTelegramToGW);
                thrd_SendTelegramToGW.IsBackground = true;
                thrd_SendTelegramToGW.Start(keys);

            }
            else
            {
                MessageBox.Show("No outgoing message is selected yet!",
                            "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }    
        }

        private void Initializer_OnConnected(object sender, MessageEventArgs e)
        {
            DisplayCommStatus(lbCommGW, e);
        }

        private void Initializer_OnDisconnected(object sender, MessageEventArgs e)
        {
            DisplayCommStatus(lbCommGW, e);
        }

        private void Initializer_OnReceived(object sender, MessageEventArgs e)
        {
            DisplayIncomingMessage(tbxMsgLog, e);
        }

        public delegate void DisplayCommStatusDelegate(Label lbCommGW, MessageEventArgs e);
        private void DisplayCommStatus(Label lbCommGW, MessageEventArgs e)
        {
            if (lbCommGW.InvokeRequired)
            {
                DisplayCommStatusDelegate deleg = new DisplayCommStatusDelegate(DisplayCommStatus);

                this.Invoke(deleg, new object[] { lbCommGW, e });
            }
            else
            {
                if (e.ChainName.CompareTo(SOURCE_PLC2GW) == 0)
                {
                    lbCommGW.Text = "CommGW<" + e.OpenedChannelCount + ">";

                    if (e.OpenedChannelCount == 0)
                        lbCommGW.BackColor = System.Drawing.Color.Red;
                    else
                        lbCommGW.BackColor = System.Drawing.Color.Lime;
                }
                else
                {
                    MessageBox.Show("Unknown Chain:" + e.ChainName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public delegate void DisplayIncomingMessageDelegate(TextBox txtBox, MessageEventArgs e);
        private void DisplayIncomingMessage(TextBox txtBox, MessageEventArgs e)
        {
            if (txtBox.InvokeRequired)
            {
                DisplayIncomingMessageDelegate deleg = new DisplayIncomingMessageDelegate(DisplayIncomingMessage);

                this.Invoke(deleg, new object[] { txtBox, e });
            }
            else
            {
                txtBox.Text = "[" + DateTime.Now + "] " + "<- [" + e.ChannelName + "] => PLC MSG:<" +
                        PALS.Utilities.Functions.ConvertByteArrayToString(
                            e.Message.RawData, -1, PALS.Utilities.HexToStrMode.ToAscPaddedHexString) + ">\r\n\r\n" + txtBox.Text;

                byte[] b_type=new byte[4];
                Array.Copy(e.Message.RawData, 0, b_type, 0, 4);
                string type = new string(Util.ByteArrayToCharArray(b_type));
                if (TelegramTypeName.HasType(type))
                {
                    string tel_alias = TelegramTypeName.GetAliasByType(type);
                    General_Telegram new_tlgm = new General_Telegram(e.Message.RawData, tel_alias);
                    txtBox.Text = "[" + DateTime.Now + "] " + "<- [" + e.ChannelName + "] => PLC >>>>>>>>>>>> \r\n "
                                + new_tlgm.ShowAllData() + "\r\n\r\n"
                                + txtBox.Text;
                }
                else
                {
                    txtBox.Text = "[" + DateTime.Now + "] " + "<- [" + e.ChannelName + "]. Telegram type is unknown.\r\n\r\n" + txtBox.Text;
                }
                 
            }
        }

        private void cbxTelegramList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str_SelectedItem = this.cbxTelegramList.SelectedItem.ToString();
            int key = Convert.ToInt32(str_SelectedItem.Substring(0, str_SelectedItem.IndexOf('.')));
            SAC2PLCTelegram temp_tele = (SAC2PLCTelegram)_init.HT_TelegramTestcase[key];
            this.tbxMsgContent.Text = temp_tele.ShowAllData();
        }


       
      
    }
}
