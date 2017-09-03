using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CubicsGamesNetworking
{
    public class Connection
    {
        public Thread ListenThread;
        public Thread SendingThread;
        public TcpClient client;
        public Stream stream;
        public StreamWriter writer;
        public StreamReader reader;
        public bool isDisconnected = false;

        public List<String> RessievesBuffer = new List<String>();
        public List<String> SendingsBuffer = new List<String>();

        public UInt64 UID = 0;

        public Connection(TcpClient clt, UInt64 uid)
        {
            UID = uid;
            client = clt;
            stream = client.GetStream();
            reader = new StreamReader(stream);
            ListenThread = new Thread(Listen);
            ListenThread.Start();
            writer = new StreamWriter(stream);
            SendingThread = new Thread(Sending);
            SendingThread.Start();
        }

        private void Listen()
        {
            try
            {
                while (!isDisconnected)
                {
                    String msg = reader.ReadLine();
                    if (msg == String.Empty)
                    {
                        break;
                    }
                    RessievesBuffer.Add(msg);
                }
            }catch (Exception e){ }
            isDisconnected = true;
        }

        private void Sending()
        {
            try
            {
                while (!isDisconnected)
                {
                    if (SendingsBuffer.Count > 0)
                    {
                        String msg = SendingsBuffer[0];
                        SendingsBuffer.RemoveAt(0);
                        writer.WriteLine(msg);
                        writer.Flush();
                    }
                    else
                    {
                        Thread.Sleep(5);
                    }
                }
            }
            catch (Exception e){ }
            isDisconnected = true;
        }

        private void Destroy()
        {
            try { ListenThread.Abort(); } finally { }
            try { SendingThread.Abort(); } finally { }
            try { reader.Close(); } finally { }
            try { writer.Close(); } finally { }
            try { reader.Dispose(); } finally { }
            try { writer.Dispose(); } finally { }
            try { stream.Close(); } finally { }
            try { stream.Dispose(); } finally { }
            try { client.Close(); } finally { }
        }

        public bool IsDisconnected()
        {
            return isDisconnected;
        }

        public void Kick()
        {
            try
            {
                Destroy();
            }catch(Exception e) { }
            isDisconnected = true;
        }

        public void SendMSG(String msg)
        {
            SendingsBuffer.Add(msg);
        }

        public void forceSendMSG(string msg)
        {
            try
            {
                writer.WriteLine(msg);
                writer.Flush();
            }catch (Exception e){ }
        }

        public UInt64 getUID()
        {
            return UID;
        }

        public String GetMSG()
        {
            String msg = String.Empty;
            if (RessievesBuffer.Count > 0)
            {
                msg = RessievesBuffer[0];
                RessievesBuffer.RemoveAt(0);
            }
            return msg;
        }

        public String getIP()
        {
            return ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
        }
    }
}
