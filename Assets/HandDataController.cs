using System.IO;
using Meta.Net.NativeWebSocket;
using UnityEngine;

public class GripDataSender : MonoBehaviour
{
    private string _jsonFilePath;  // Path to the JSON file saved by GripDataCollector
    private WebSocket websocket;

    async void Start()
    {
        // Set the JSON file path to match where GripDataCollector saves it
        _jsonFilePath = Path.Combine(Application.persistentDataPath, "wrong_gesture.json");
        Debug.Log("JSON file path: " + _jsonFilePath);

        // Initialize the WebSocket connection to the server
        websocket = new WebSocket("ws://192.168.3.4:8080");  // Replace with your computer’s IP

        // Subscribe to events with expected delegate signatures
        websocket.OnOpen += OnWebSocketOpen;
        websocket.OnClose += OnWebSocketClose;
        websocket.OnError += OnWebSocketError;
        websocket.OnMessage += OnWebSocketMessage;

        await websocket.Connect();
    }

    public async void SendJsonFileOverWebSocket()
    {
        // Check if the JSON file exists
        if (File.Exists(_jsonFilePath))
        {
            try
            {
                // Read the JSON data from the file
                string jsonData = File.ReadAllText(_jsonFilePath);

                // Send JSON data over WebSocket
                if (websocket.State == WebSocketState.Open)
                {
                    await websocket.SendText(jsonData);
                    Debug.Log("Sent grip data from JSON file: " + _jsonFilePath);

                    // Delete the JSON file after sending
                    DeleteFileAfterSend();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to read or send JSON file: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("JSON file not found: " + _jsonFilePath);
        }
    }

    private void DeleteFileAfterSend()
    {
        if (File.Exists(_jsonFilePath))
        {
            try
            {
                File.Delete(_jsonFilePath);
                Debug.Log("File deleted successfully after sending.");
            }
            catch (IOException e)
            {
                Debug.LogError("Failed to delete file: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("File not found for deletion: " + _jsonFilePath);
        }
    }

    private void OnWebSocketOpen()
    {
        Debug.Log("WebSocket连接成功!");
    }

    private void OnWebSocketError(string error)
    {
        Debug.Log("WebSocket报错: " + error);
    }

    private void OnWebSocketClose(WebSocketCloseCode closeCode)
    {
        Debug.Log("WebSocket连接关闭! Close Code: " + closeCode);
    }

    private void OnWebSocketMessage(byte[] data, int offset, int length)
    {
        Debug.Log("收到消息，线程ID为: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
        string receivedMessage = System.Text.Encoding.UTF8.GetString(data, offset, length);
        Debug.Log("收到来自websocket的消息: " + receivedMessage);
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}
