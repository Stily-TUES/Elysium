using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
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

    public static List<ScriptFile> ScriptFiles { get; private set; } = new List<ScriptFile>();

    public static void LoadScriptsFromDirectory(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            ScriptFiles.Clear();
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
}
