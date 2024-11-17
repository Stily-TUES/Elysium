using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Editor.Utils;

[DataContract]
public class TextureFile
{
    public string FileName { get; set; }
    [XmlIgnore]
    public byte[] ImagePath { get; set; }
    public static List<TextureFile> TextureFiles { get; private set; }

    static TextureFile()
    {
        string texturesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../GameEngine", "Textures");
        if (Directory.Exists(texturesFolderPath))
        {
            TextureFiles = Directory.GetFiles(texturesFolderPath)
                .Select(filePath => new TextureFile
                {
                    FileName = Path.GetFileName(filePath),
                    ImagePath = File.ReadAllBytes(filePath)
                }).ToList();
        }
        else
        {
            TextureFiles = new List<TextureFile>();
        }
    }
}