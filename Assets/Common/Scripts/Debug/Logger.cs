using System;
using System.IO;
using UnityEngine;

namespace Shooter.Debug
{
    public class Logger : MonoBehaviour
    {
        private StreamWriter writer;

        private void Awake()
        {
            string dir = Path.Combine(Application.persistentDataPath, "Logs");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string path = Path.Combine(dir, "Game.log");
            int i = 1;
            while (File.Exists(path))
            {
                i++;
                path = Path.Combine(dir, $"Game{i}.log");
            }
            writer = new StreamWriter(path);
            Application.logMessageReceived += LogMessageReceived;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= LogMessageReceived;
            writer.Close();
        }

        private void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Log)
                return;
            writer.WriteLine($"[{DateTime.Now}] {condition}");
        }
    }
}
