using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static We_have_doom_at_home.Technical.Common;
using We_have_doom_at_home.Entities.Interfaces;
using We_have_doom_at_home.Entities;
using We_have_doom_at_home.MVC.ServerMVC;
using We_have_doom_at_home.Technical;
using We_have_doom_at_home.World;

namespace We_have_doom_at_home.MVC.ClientMVC;

public class ClientView
{
    private static ClientView _instance;
    private int mapDrawingStartingPointX = 2;
    private int mapDrawingStartingPointY = 1;
    private int prevPosX =0;
    private int prevPosY = 0;
    private ClientView()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Clear();
    }

    public static ClientView GetInstance()
    {
        if (_instance == null)
        {
            lock (typeof(ClientView)) // thread safety for future generations??? 
            {
                if (_instance == null)
                {
                    _instance = new ClientView();
                }
            }
        }
        return _instance;
    }

    public void DisplayPlayerImage(char[][] Buffer, IEnumerable<string> gameLogs, int posX, int posY)
    {
        Console.CursorVisible = false;

        Console.OutputEncoding = Encoding.UTF8;
        Console.ForegroundColor = DefaulftForegroundColor;

        for (int y = 0; y < Buffer.Length; y++)
        {
            Console.SetCursorPosition(0, y);
            Console.Write(Buffer[y]);
        }
        RenderLog(gameLogs);
        Console.SetCursorPosition(posX + mapDrawingStartingPointX + 1, posY + mapDrawingStartingPointY + 1);
        Console.ForegroundColor = PlayerColor;
        Console.Write(Buffer[posY + 2][posX + 3]);
        Console.ForegroundColor = DefaulftForegroundColor;


    }
    public void InsertStringIntoBuffer(char[,] buffer, int x, int y, string text)
    {
        if (y >= buffer.GetLength(1)) return;
        for (int i = 0; i < text.Length; i++)
        {
            if (x + i < buffer.GetLength(0)) // Prevent overflow
            {
                buffer[x + i, y] = text[i];
            }
        }
    }
    public void RenderLog(IEnumerable<string> gameLogs)
    {
        char[,] LogBuffer = new char[MapWidth, LogConsoleHeight];
        int startX = mapDrawingStartingPointX;
        int startY = mapDrawingStartingPointY + MapHeight + 6;
        // Clear the buffer
        for (int y = 0; y < LogBuffer.GetLength(1); y++)
        {
            for (int x = 0; x < LogBuffer.GetLength(0); x++)
            {
                LogBuffer[x, y] = ' ';
            }
        }

        int line = 0; // Track which row we are on
        InsertStringIntoBuffer(LogBuffer, 0, line++, "===============================================================");
        InsertStringIntoBuffer(LogBuffer, 0, line++, "Logs: ");

        List<string> lastLogs = gameLogs.TakeLast(LogConsoleHeight - 2).ToList();

        // Insert logs into the buffer, ensuring the latest log appears at the bottom
        int logStartLine = LogConsoleHeight - lastLogs.Count; // Start displaying from the bottom
        for (int i = 0; i < lastLogs.Count; i++)
        {
            InsertStringIntoBuffer(LogBuffer, 0, logStartLine + i, lastLogs[i]);
        }
        // Print buffer to console
        Console.SetCursorPosition(startX, startY);
        for (int y = 0; y < LogBuffer.GetLength(1); y++)
        {
            for (int x = 0; x < LogBuffer.GetLength(0); x++)
            {
                Console.Write(LogBuffer[x, y]);
            }
            Console.SetCursorPosition(startX, startY + 1 + y);
        }
    }
}
