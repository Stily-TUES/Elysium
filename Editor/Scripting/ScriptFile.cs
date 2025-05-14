using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Scripting;

[DataContract]
public class ScriptFile
{
    [DataMember]
    public string FileName { get; set; }
    [DataMember]
    public string FilePath { get; set; }

    [IgnoreDataMember]
    public byte[] ScriptIcon
    {
        get
        {
            using (Icon icon = Icon.ExtractAssociatedIcon(FilePath))
            {
                if (icon != null)
                {
                    return ConvertIconToByteArray(icon);
                }
            }
            return null;
        }
    }

    public static List<ScriptFile> ScriptFiles { get; private set; } = new List<ScriptFile>();


    public static void LoadScriptsFromDirectory(string directoryPath)
    {
        LoadDefaultScripts();
        if (Directory.Exists(directoryPath))
        {
            var scripts = Directory.GetFiles(directoryPath, "*.lua");
            foreach (var script in scripts)
            {
                ScriptFiles.Add(new ScriptFile
                {
                    FileName = Path.GetFileName(script),
                    FilePath = script
                });
            }
        }
    }

    public static void LoadDefaultScripts()
    {
        string defaultScriptsFolderPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../GameEngine", "Resourses/Scripts"));
        if (Directory.Exists(defaultScriptsFolderPath))
        {
            ScriptFiles.Clear();
            var scripts = Directory.GetFiles(defaultScriptsFolderPath, "*.lua");
            foreach (var script in scripts)
            {
                ScriptFiles.Add(new ScriptFile
                {
                    FileName = Path.GetFileName(script),
                    FilePath = script
                });
            }
        }
    }

    private byte[] ConvertIconToByteArray(Icon icon)
    {
        using (var stream = new MemoryStream())
        {
            icon.ToBitmap().Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }
    }
}
