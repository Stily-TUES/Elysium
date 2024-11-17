using Editor.GameProject;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Editor.Utils;

[DataContract]
public class TextureFile
{
    [DataMember]
    public string FileName { get; set; }
    [IgnoreDataMember]
    public byte[] Image { get; set; }
    [DataMember]
    public string ImagePath { get; set; }
    [IgnoreDataMember]
    public static List<TextureFile> TextureFiles { get; private set; }

    static TextureFile()
    {
        string texturesFolderPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../GameEngine", "Textures"));
        if (Directory.Exists(texturesFolderPath))
        {
            TextureFiles = Directory.GetFiles(texturesFolderPath)
                .Select(filePath => new TextureFile
                {
                    FileName = Path.GetFileName(filePath),
                    ImagePath = filePath,
                    Image = File.ReadAllBytes(filePath),
                }).ToList();
        }
        else
        {
            TextureFiles = new List<TextureFile>();
        }
    }
}
