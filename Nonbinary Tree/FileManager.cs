using ListTreesLibrary;
using Newtonsoft.Json;
using System.IO;
using System.Text;
namespace Nonbinary_Tree
{
    public class FileManager : IDeletable<FileManager>
    {
        [JsonProperty]
        private string FileName;

        public FileManager(string name)
        {
            FileName = name;
        }

        public FileManager()
        {

        }

        public override string ToString()
        {
            return FileName;
        }

        public override int GetHashCode()
        {
            return FileName.GetHashCode();
        }

        public bool FileExists()
        {
            return File.Exists(FileName + ".cs");
        }

        public void Delete()
        {
            File.Delete(FileName + ".cs");
        }

        public FileStream CreateFile()
        {

            using (FileStream fs = File.Create(FileName + ".cs"))
            {
                AddText(fs, "namespace Nonbinary_Tree\n");
                AddText(fs, "{\n");
                AddText(fs, $"\tclass {FileName}\n");
                AddText(fs, "\t{\n");
                AddText(fs, "\t\tpublic string ClassName { get; set; }\n\n");
                AddText(fs, $"\t\tpublic {FileName}()\n");
                AddText(fs, "\t\t{\n");
                AddText(fs, $"\t\t\tthis.ClassName = {"\"" + FileName + "\""};\n");
                AddText(fs, "\t\t}\n");
                AddText(fs, "\t}\n");
                AddText(fs, "}\n");
                return fs;
            }

        }

        private void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }

        public string Read()
        {
            string output = "";
            using (FileStream fs = File.OpenRead(FileName + ".cs"))
            {
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);
                while (fs.Read(b, 0, b.Length) > 0)
                {
                    output += temp.GetString(b);
                }
            }
            return output;
        }

        public void DeleteItem(FileManager item)
        {
            if (item == null)
            {
                return;
            }
            item.Delete();
        }
    }
}
