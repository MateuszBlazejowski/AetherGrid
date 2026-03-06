using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using We_have_doom_at_home.MVC.ClientMVC;
using We_have_doom_at_home.MVC.ServerMVC;

namespace We_have_doom_at_home.Connection;
using static We_have_doom_at_home.Technical.Log;
using static We_have_doom_at_home.Technical.Common;
public class ClientConnection
{
    private TcpClient _client;
    private StreamReader _reader;
    private StreamWriter _writer;
    private Thread _receiveThread;
    private ClientControler _controler;

    private bool _shouldReconnect = true;
    private readonly int _reconnectDelayMs = 3000; 
    private readonly int _maxReconnectAttempts = 15;

    public bool IsConnected => _client?.Connected ?? false;

    public void Connect(ClientControler controler, string ip = "127.0.0.1", int port = 5555)
    {
        _controler = controler;
        Task.Run(() => ConnectWithRetry(ip, port));
    }

    private void ConnectWithRetry(string ip, int port)
    {
        int attempt = 0;
        while (_shouldReconnect && (attempt < _maxReconnectAttempts || _maxReconnectAttempts == 0))
        {
            try
            {
                attempt++;
                Logs.Add($"Attempting to connect to server at {ip}:{port}, try #{attempt}...");
                _client = new TcpClient();
                var connectTask = _client.ConnectAsync(ip, port);
                if (!connectTask.Wait(5000)) // timeout after 5 seconds
                {
                    throw new TimeoutException("Connect timed out");
                }
                if (!_client.Connected)
                {
                    throw new Exception("Failed to connect");
                }

                var stream = _client.GetStream();
                _reader = new StreamReader(stream, Encoding.UTF8);
                _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                _receiveThread = new Thread(ReceiveLoop) { IsBackground = true };
                _receiveThread.Start();

                Logs.Add($"Connected to server at {ip}:{port}");
                return; // success
            }
            catch (Exception ex)
            {
                Logs.Add($"Connection attempt #{attempt} failed: {ex.Message}");
                Thread.Sleep(_reconnectDelayMs);
            }
        }

        Logs.Add("Failed to connect after multiple attempts.");
    }

    private void ReceiveLoop()
    {
        try
        {
            while (IsConnected)
            {
                var line = _reader.ReadLine();
                if (line == null)
                    break; // server closed connection

                try
                {
                    var frame = JsonSerializer.Deserialize<FrameMessage>(line);
                    if (frame?.Rows == null)
                        continue;

                    char[][] buffer = new char[frame.Rows.Length][];
                    for (int y = 0; y < frame.Rows.Length; y++)
                    {
                        var rowString = frame.Rows[y] ?? "";
                        buffer[y] = rowString.ToCharArray();
                    }

                    _controler.TriggerDisplay(buffer, frame.PlayerX, frame.PlayerY);
                }
                catch (JsonException je)
                {
                    Logs.Add($"[Client] JSON Deserialization error: {je.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Logs.Add("Disconnected: " + ex.Message);
        }

        // If disconnected, try to reconnect
        if (_shouldReconnect)
        {
            Logs.Add("Connection lost. Attempting to reconnect...");
            ConnectWithRetry("127.0.0.1", 5555); // You might want to save IP/port as fields for reuse
        }
    }

    public void Send(PlayerAction action)
    {
        if (IsConnected)
        {
            string json = JsonSerializer.Serialize(action);
            _writer.WriteLine(json);
        }
        else
        {
            Logs.Add("Cannot send, not connected.");
        }
    }

    public void Disconnect()
    {
        _shouldReconnect = false;
        _client?.Close();
        Logs.Add("Disconnected from server by client request.");
    }
}
