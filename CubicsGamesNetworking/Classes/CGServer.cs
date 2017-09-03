using CubicsGamesNetworking.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CubicsGamesNetworking
{
    public class CGServer
    {
        private TcpListener ClientsServer;
        private Thread WorkThread;
        private Thread ClientsListenThread;
        public delegate void OnConnectEvent(Connection con);
        public delegate void OnDefaultMSGEvent(Connection con,string msg);
        public delegate void OnDisconnectEvent(Connection con);
        public delegate void OnMSGEvent(Connection con,string msgcontent);
        private Dictionary<ulong, Connection> Clients = new Dictionary<ulong, Connection>();
        public Dictionary<string, OnMSGEvent> OnMSGFilters = new Dictionary<string, OnMSGEvent>();
        public OnDisconnectEvent OnClientDisconnected;
        public OnConnectEvent OnClientConnected;
        public OnDefaultMSGEvent OnDefaultMSG;
        private UInt64 LastID = 0;

        private bool CallWorkExternal = false;

        private bool Running = false;

        public bool IsUseExternalWorkCall()
        {
            return CallWorkExternal;
        }

        public void SetExternalWorkCallUsage(bool state)
        {
            if (!IsRunning())
            {
                CallWorkExternal = state;
            }
        }

        public bool IsRunning()
        {
            return Running;
        }

        public void StartServer(int Port)
        {
            if (Running)
            {
                return;
            }
            try
            {
                ClientsServer = new TcpListener(IPAddress.Any, Port);
                ClientsServer.Start();
                Running = true;
                ClientsListenThread = new Thread(ClientsListen);
                ClientsListenThread.Start();
                if (!IsUseExternalWorkCall())
                {
                    WorkThread = new Thread(Work);
                    WorkThread.Start();
                }
            }
            catch (Exception e){ StopServer(); }
        }

        public void StopServer()
        {
            try { if (ClientsListenThread != null) { if (ClientsListenThread.IsAlive) { ClientsListenThread.Abort(); } ClientsListenThread = null; } } catch (Exception e) { }
            try { if (WorkThread != null) { if (WorkThread.IsAlive) { WorkThread.Abort(); } WorkThread = null; } } catch (Exception e) { }
            try { if (ClientsServer != null) { ClientsServer.Stop(); ClientsServer = null; } } catch (Exception e) { }
            KickAll();
            Running = false;
        }

        private void Work()
        {
            try
            {
                while (IsRunning())
                {
                    try
                    {
                        DoWork(true);
                    }
                    catch (Exception e) { }
                }
            }
            catch (Exception ee)
            {

            }
        }

        public void DoWork(bool WaitIfNothingIsToDo)
        {
            for (int i = 0; i < Clients.Keys.Count; i++)
            {
                ulong ClientKey = Clients.ElementAt(i).Key;
                if (Clients[ClientKey].IsDisconnected())
                {
                    if (OnClientDisconnected != null)
                    {
                        OnClientDisconnected(Clients[ClientKey]);
                    }
                    Clients[ClientKey].Kick();
                    Clients.Remove(ClientKey);
                    i--;
                }
                else
                {
                    try
                    {
                        while (IsRunning())
                        {
                            string msg = Clients[ClientKey].GetMSG();
                            if (msg != "" && msg != string.Empty)
                            {
                                ProcessMSG(Clients[ClientKey], msg);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception e) { }
                }
            }
            if (Clients.Keys.Count == 0 && WaitIfNothingIsToDo)
            {
                Thread.Sleep(5);
            }
        }

        private void ProcessMSG(Connection con,string msg)
        {
            string PacketName = "";
            string PacketContent = "";

            if(msg != string.Empty)
            {
                string[] vals = msg.Split(' ');
                if(vals.Length != 2)
                {
                    return;
                }
                PacketName = vals[0].Replace("!","");
                PacketContent = vals[1];
                if (OnMSGFilters.ContainsKey(PacketName))
                {
                    OnMSGFilters[PacketName](con, PacketContent);
                }
                else
                {
                    if(OnDefaultMSG != null)
                    {
                        OnDefaultMSG(con, msg);
                    }
                }
            }
        }

        private void ClientsListen()
        {
            try
            {
                while (IsRunning())
                {
                    try
                    {
                        TcpClient client = ClientsServer.AcceptTcpClient();
                        Connection con = new Connection(client, LastID);
                        LastID++;
                        if (OnClientConnected != null)
                        {
                            OnClientConnected(con);
                        }
                        Clients.Add(con.getUID(), con);
                    }
                    catch (Exception e) { }
                }
            }
            catch (Exception exception)
            {
                /* Clean up. */
            }
        }

        public void KickAll()
        {
            try
            {
                for (int i = 0; i < Clients.Keys.Count; i++)
                {
                    ulong ClientKey = Clients.ElementAt(i).Key;
                    Clients[ClientKey].Kick();
                    Clients.Remove(ClientKey);
                    i--;
                }
            } catch (Exception e){ }
        }

        public void Kick(UInt64 id)
        {
            try
            {
                for (int i = 0; i < Clients.Keys.Count; i++)
                {
                    ulong ClientKey = Clients.ElementAt(i).Key;
                    if (Clients[ClientKey].getUID() == id)
                    {
                        Clients[ClientKey].Kick();
                        i = Clients.Count;
                    }
                }
            }
            catch (Exception e) { }
        }

        public  void Kick(Connection client)
        {
            try
            {
                client.Kick();
            }
            catch (Exception e) { }
        }

        public Connection GetConnection(ulong id)
        {
            if (Clients.ContainsKey(id))
            {
                return Clients[id];
            }
            return null;
        }

        public List<Connection> GetAllConnections()
        {
            return Clients.Values.ToList<Connection>();
        }
    }
}
