using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bluscream;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
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
                var zip = QuestDir.CombineFile(song.Name + ".zip");
                if (zip.Exists) continue;
                Console.WriteLine(song.Name);
                CreateSample(zip.FullName, null, song.FullName);
            }

            Console.ReadKey();
        }

        public void CreateSample(string outPathname, string password, string folderName)
        {
            using (FileStream fsOut = File.Create(outPathname))
            using (var zipStream = new ZipOutputStream(fsOut))
            {
                //0-9, 9 being the highest level of compression
                zipStream.SetLevel(0);

                // optional. Null is the same as not setting. Required if using AES.
                zipStream.Password = password;

                // This setting will strip the leading part of the folder path in the entries,
                // to make the entries relative to the starting folder.
                // To include the full path for each entry up to the drive root, assign to 0.
                int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

                CompressFolder(folderName, zipStream, folderOffset);
            }
        }

        // Recursively compresses a folder structure
        private void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        {
            var files = Directory.GetFiles(path);

            foreach (var filename in files)
            {
                var fi = new FileInfo(filename);

                // Make the name in zip based on the folder
                var entryName = filename.Substring(folderOffset);

                // Remove drive from name and fix slash direction
                entryName = ZipEntry.CleanName(entryName);

                var newEntry = new ZipEntry(entryName);

                // Note the zip format stores 2 second granularity
                newEntry.DateTime = fi.LastWriteTime;

                // Specifying the AESKeySize triggers AES encryption.
                // Allowable values are 0 (off), 128 or 256.
                // A password on the ZipOutputStream is required if using AES.
                //   newEntry.AESKeySize = 256;

                // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003,
                // WinZip 8, Java, and other older code, you need to do one of the following:
                // Specify UseZip64.Off, or set the Size.
                // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility,
                // you do not need either, but the zip will be in Zip64 format which
                // not all utilities can understand.
                //   zipStream.UseZip64 = UseZip64.Off;
                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                var buffer = new byte[4096];
                using (FileStream fsInput = File.OpenRead(filename))
                {
                    StreamUtils.Copy(fsInput, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }

            // Recursively call CompressFolder on all folders in path
            var folders = Directory.GetDirectories(path);
            foreach (var folder in folders)
            {
                CompressFolder(folder, zipStream, folderOffset);
            }
        }
    }