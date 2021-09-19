using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace NullDffsGrabber
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter dffs path:");
            string FileDffsPath = Console.ReadLine().Replace("\"", "");
            byte[] data = File.ReadAllBytes(FileDffsPath);
            MemoryStream memstr = new MemoryStream(data);
            BinaryReader binread = new BinaryReader(memstr);

            int FileStart = 1078346825;
            byte[] buffer = new byte[] { 0, 0, 0, 0 };

            while (true)
            {
                buffer[0] = buffer[1];
                buffer[1] = buffer[2];
                buffer[2] = buffer[3];
                buffer[3] = binread.ReadByte();

                if (FileStart == BitConverter.ToInt32(buffer, 0))
                {
                    break;
                }

            }
            Console.WriteLine("Найден маркер начала IDF@ файла. Смещение:" + binread.BaseStream.Position);
            Console.WriteLine("Размер данных файла:" + binread.ReadInt32());
            int files = 0;
            Console.WriteLine("Количество файлов в IDF@ архиве:" + (files = binread.ReadInt32()));

            binread.ReadInt64();

            for (int i = 1; i < files; i++)
            {
                int filesize = binread.ReadInt32();
                string FileName = Encoding.UTF8.GetString(binread.ReadBytes(0x108)).Replace("/","\\");
                FileName = FileName.Remove(FileName.IndexOf("\0"));
                File.WriteAllText(".\\test.txt", FileName);
                byte[] fileadata = new byte[] { 0 };
                if (filesize > 0)
                    fileadata = binread.ReadBytes(filesize);


                Console.WriteLine("Чтение файла " + FileName + " размер данных:" + filesize + "/" + fileadata.Length);
                if (!Directory.Exists(Path.GetDirectoryName(FileName)))
                    Directory.CreateDirectory(Path.GetDirectoryName(FileName));
                File.WriteAllBytes(FileName,fileadata);
            }

            Console.ReadLine();
        }
    }
}
