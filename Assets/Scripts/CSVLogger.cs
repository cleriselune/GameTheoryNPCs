using System.IO;
using UnityEngine;

public static class CSVLogger
{
    static string path = Application.dataPath + "/simulation_results.csv";

    public static void CreateFile()
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "Year,Wars,Alliances\n");
        }
    }

    public static void Log(int year, int wars, int alliances)
    {
        string line = year + "," + wars + "," + alliances + "\n";
        File.AppendAllText(path, line);
    }
}