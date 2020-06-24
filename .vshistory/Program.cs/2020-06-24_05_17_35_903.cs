using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bluscream;
using Syroot.Windows.IO;

namespace BeatSaberQuestSync
{
    internal class Program
    {
        private static Regex pattern = new Regex(@"^\w{1,4} \(");
        private static DirectoryInfo PCDir = new DirectoryInfo(Environment.CurrentDirectory).Combine("Beat Saber_Data", "CustomLevels");
        private static DirectoryInfo QuestDir = new DirectoryInfo(new KnownFolder(KnownFolderType.Downloads).Path).Combine("Quest", "Beat Saber", "Songs");

        private static void Main(string[] args)
        {
            Console.WriteLine(PCDir);
            Console.WriteLine(QuestDir);

            var PCSongs = new List<DirectoryInfo>();

            foreach (var song in PCDir.GetDirectories())
            {
                if (pattern.IsMatch(song.Name)) continue;
                if (song.IsEmpty()) continue;
                if (song.Name == "Jaroslav Beck - Beat Saber (Built in)") continue;
                if (QuestDir.CombineFile(song.Name + ".zip").Exists) continue;
                Console.WriteLine(song.Name);
            }

            Console.ReadKey();
        }
    }
}