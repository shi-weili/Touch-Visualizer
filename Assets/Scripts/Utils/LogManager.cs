using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Keiwando.NFSO;

public class LogManager : MonoBehaviour
{
    // Data and states
    string filename;
    List<string> contents = new List<string>();

    string FullPath
    {
        get => Path.Combine(Application.persistentDataPath, filename);
    }

    // Public functions
    public void StartNewLog()
    {
        contents.Clear();
    }

    public void AddToLog(string newContent)
    {
        contents.Add(newContent);
    }

    public void SaveToPersistentDataPath(string filename)
    {
        this.filename = filename;
        File.WriteAllText(FullPath, String.Join("\n", contents));
    }

    /// <summary>
    /// Share the log file previously saved to persistent data path.
    /// Must be called after calling SaveToPersistentDataPath().
    /// </summary>
    public void ShareLogFile()
    {
        NativeFileSO.shared.SaveFile(new FileToSave(FullPath, filename, SupportedFileType.PlainText));
    }
}
