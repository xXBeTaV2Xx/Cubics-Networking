using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace CubicsGamesNetworking
{
    public class CGClient
    {
        private TcpClient Client;
        private NetworkStream Stream;
        private StreamWriter Writer;
        private StreamReader Reader;

        public string ServerIP = "127.0.0.1";
        public int ServerPort = 1337;

        private List<string> SendBuffer = new List<string>();
        private List<string> RessBuffer = new List<string>();

        private Thread ListenThread;
        private Thread SendingThread;
        private Thread WorkingThread;
        private Thread ConnectingThread;

        public bool IsConnected = false;
        public bool IsConnecting = false;
        public bool IsConnectionLost = false;

        private bool CallWorkExternal = false;

        public delegate void OnConnectedEvent();
        public delegate void OnConnectionLostEvent();
        public delegate void OnMSGEvent(string msg);

        public OnConnectedEvent OnConnected;
        public OnConnectionLostEvent OnConnectionLost;
        public OnMSGEvent OnDefaultMSG;
        public Dictionary<string, OnMSGEvent> OnMSGFilters = new Dictionary<string, OnMSGEvent>();

        public bool IsUseExternalWorkCall()
        {
            return CallWorkExternal;
        }

        public void SetExternalWorkCallUsage(bool state)
        {
            if (!IsConnected)
            {
                CallWorkExternal = state;
            }
        }

        public void Connect()
        {
            if (IsConnecting)
            {
                return;
            }
            if (IsConnected)
            {
                return;
            }
            IsConnected = false;
            IsConnectionLost = false;
            IsConnecting = true;
            SendBuffer.Clear();
            RessBuffer.Clear();
            Client = new TcpClient();
            ConnectingThread = new Thread(Connecting);
            ConnectingThread.Start();
        }

        public void Disconnect()
        {
            try { if (ConnectingThread != null) { if (ConnectingThread.IsAlive) { ConnectingThread.Abort(); } ConnectingThread = null; } } catch (Exception e) { }
            try { if (WorkingThread != null) { if (WorkingThread.IsAlive) { WorkingThread.Abort(); } WorkingThread = null; } } catch (Exception e) { }
            try { if (ListenThread != null) { if (ListenThread.IsAlive) { ListenThread.Abort(); } ListenThread = null; } } catch (Exception e) { }
            try { if (SendingThread != null) { if (SendingThread.IsAlive) { SendingThread.Abort(); } SendingThread = null; } } catch (Exception e) { }
            try { if (Reader != null) { Reader.Dispose(); Reader = null; } } catch (Exception e) { }
            try { if (Writer != null) { Writer.Dispose(); Writer = null; } } catch (Exception e) { }
            try { if (Stream != null) { Stream.Close(); Stream.Dispose(); Stream = null; } } catch (Exception e) { }
            try { if (Client != null) { Client.Close(); Client = null; } } catch (Exception e) { }
            SendBuffer.Clear();
            RessBuffer.Clear();
            IsConnected = false;
            IsConnectionLost = false;
            if (OnConnectionLost != null)
            {
                OnConnectionLost();
            }
        }

        private void Connecting()
        {
            try
            {
                Client.Connect(ServerIP, ServerPort);
                if (Client.Connected)
                {
                    IsConnected = true;
                    Stream = Client.GetStream();
                    Reader = new StreamReader(Stream);
                    ListenThread = new Thread(Listen);
                    ListenThread.Start();
                    Writer = new StreamWriter(Stream);
                    SendingThread = new Thread(Sending);
                    SendingThread.Start();
                    if (!IsUseExternalWorkCall())
                    {
                        WorkingThread = new Thread(Working);
                        WorkingThread.Start();
                    }
                    if(OnConnected != null)
                    {
                        OnConnected();
                    }
                }
            }
            catch (Exception e) { }
            IsConnecting = false;
        }

        void Working()
        {
            try
            {
                while (IsConnected)
                {
                    try
                    {
                        DoWork(true);
                    }
                    catch (Exception e) { }
                }
            }catch(Exception ee)
            {

            }
        }

        public void DoWork(bool WaitIfNothingIsToDo)
        {

            string msg = "";
            while (IsConnected)
            {
                msg = getMSG();
                if(msg == string.Empty)
                {
                    if (WaitIfNothingIsToDo)
                    {
                        Thread.Sleep(5);
                    }
                    break;
                }
                else
                {
                    ProcessMSG(msg);
                }
            }
        }

        private void ProcessMSG(string msg)
        {
            string PacketName = "";
            string PacketContent = "";

            if (msg != string.Empty)
            {
                string[] vals = msg.Split(' ');
                if (vals.Length != 2)
                {
                    return;
                }
                PacketName = vals[0].Replace("!", "");
                PacketContent = vals[1];
                if (OnMSGFilters.ContainsKey(PacketName))
                {
                   OnMSGFilters[PacketName](PacketContent);
                }
                else
                {
                    if (OnDefaultMSG != null)
                    {
                        OnDefaultMSG(msg);
                    }
                }
            }
        }

        private void Listen()
        {
            try
            {
                while (IsConnected)
                {
                    string tmp = Reader.ReadLine();
                    if (tmp != string.Empty && tmp != null && tmp != "")
                    {
                        RessBuffer.Add(tmp);
                    }
                    else
                    {
                        IsConnected = false;
                        IsConnectionLost = true;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                IsConnected = false;
                IsConnectionLost = true;
            }
            if (OnConnectionLost != null)
            {
                OnConnectionLost();
            }
        }

        public void SendMSG(string msg)
        {
            SendBuffer.Add(msg);
        }

        public string getMSG()
        {
            string msg = string.Empty;
            if (RessBuffer.Count > 0)
            {
                msg = RessBuffer[0];
                RessBuffer.RemoveAt(0);
            }
            return msg;
        }

        private void Sending()
        {
            try
            {
                while (IsConnected)
                {
                    if (SendBuffer.Count > 0)
                    {
                        string tmp = SendBuffer[0];
                        SendBuffer.RemoveAt(0);
                        Writer.WriteLine(tmp);
                        Writer.Flush();
                    }
                    else
                    {
                        Thread.Sleep(5);
                    }
                }
            }
            catch (Exception e)
            {
                IsConnected = false;
                IsConnectionLost = true;
            }
        }
    }
}
