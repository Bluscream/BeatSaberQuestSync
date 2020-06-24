using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bluscream;
using Syroot.Windows.IO;

namespace BeatSaberQuestSync
{
    internal class Program
    {
        private static DirectoryInfo PCDir = new DirectoryInfo(Environment.CurrentDirectory).Combine("Beat Saber_Data", "CustomLevels");

        private static DirectoryInfo QuestDir = new DirectoryInfo(new KnownFolder(KnownFolderType.Downloads).Path);

        private static void Main(string[] args)
        {
            Console.WriteLine(PCDir);
            Console.WriteLine(QuestDir);
            Console.ReadKey();
        }
    }
}