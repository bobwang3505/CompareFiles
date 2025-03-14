using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ConsoleApp1_CompareFiles
{
    // 文件实体类
    public class FileInfoEntity
    {
        public Guid Id { get; set; }
        public MemoryStream Stream { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
    }

    // 文件比较工具类
    public class FileComparisonTool
    {
        public static string[] GetAllFilesInFolder(string folderPath)
        {
            try
            {
                // 获取当前文件夹下的所有文件
                string[] files = Directory.GetFiles(folderPath);

                // 获取当前文件夹下的所有子文件夹
                string[] subFolders = Directory.GetDirectories(folderPath);

                foreach (string subFolder in subFolders)
                {
                    // 递归调用获取子文件夹下的所有文件
                    string[] subFiles = GetAllFilesInFolder(subFolder);
                    files = files.Concat(subFiles).ToArray();
                }

                return files;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
                return new string[0];
            }
        }

        // 计算文件哈希值
        private static string CalculateHash(Stream stream)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        // 入参是两个文件流，比较两个文件
        public static bool CompareFiles(Stream stream1, Stream stream2)
        {
            stream1.Position = 0;
            stream2.Position = 0;
            string hash1 = CalculateHash(stream1);
            string hash2 = CalculateHash(stream2);
            return hash1 == hash2;
        }

        // 入参是两个文件路径，比较两个文件
        public static bool CompareFiles(string filePath1, string filePath2)
        {
            using (FileStream stream1 = File.OpenRead(filePath1))
            using (FileStream stream2 = File.OpenRead(filePath2))
            {
                return CompareFiles(stream1, stream2);
            }
        }

        // 入参是文件路径的数组，找出重复文件
        public static List<FileInfoEntity> FindDuplicateFiles(string[] filePaths)
        {
            var fileInfos = new List<FileInfoEntity>();
            foreach (var filePath in filePaths)
            {
                var fileInfo = new FileInfo(filePath);
                fileInfos.Add(new FileInfoEntity
                {
                    Id = Guid.NewGuid(),
                    FileName = filePath,
                    FileSize = fileInfo.Length
                });
            }

            var groups = fileInfos.GroupBy(f => f.FileSize);
            var duplicateFiles = new List<FileInfoEntity>();

            foreach (var group in groups)
            {
                if (group.Count() > 1)
                {
                    var hashes = new Dictionary<string, FileInfoEntity>();
                    foreach (var file in group)
                    {
                        using (FileStream stream = File.OpenRead(file.FileName))
                        {
                            string hash = CalculateHash(stream);
                            if (hashes.ContainsKey(hash))
                            {
                                if (!duplicateFiles.Contains(hashes[hash]))
                                {
                                    duplicateFiles.Add(hashes[hash]);
                                }
                                duplicateFiles.Add(file);
                            }
                            else
                            {
                                hashes[hash] = file;
                            }
                        }
                    }
                }
            }

            return duplicateFiles;
        }

        /*
        代码说明
        FindDuplicateFilesGrouped 方法：
        首先遍历传入的文件路径数组，将每个文件的信息（Id、FileName、FileSize）封装到 FileInfoEntity 对象中，并添加到 fileInfos 列表。
        接着按文件大小对 fileInfos 进行分组，得到 sizeGroups。
        针对每个大小组，计算组内每个文件的哈希值，把具有相同哈希值的文件存到 hashGroups 字典里。
        最后，将 hashGroups 中包含多个文件的组添加到 duplicateFileGroups 列表，该列表即为最终按组返回的重复文件结果。
        通过这种方式，就能方便地找出所有重复文件，并将它们按组分类返回。
        */

        // 入参是文件路径数组，按组返回重复文件
        public static List<List<FileInfoEntity>> FindDuplicateFilesGrouped(string[] filePaths)
        {
            var fileInfos = new List<FileInfoEntity>();
            foreach (var filePath in filePaths)
            {
                var fileInfo = new FileInfo(filePath);
                fileInfos.Add(new FileInfoEntity
                {
                    Id = Guid.NewGuid(),
                    FileName = filePath,
                    FileSize = fileInfo.Length
                });
            }

            var sizeGroups = fileInfos.GroupBy(f => f.FileSize);
            var duplicateFileGroups = new List<List<FileInfoEntity>>();

            foreach (var sizeGroup in sizeGroups)
            {
                if (sizeGroup.Count() > 1)
                {
                    var hashGroups = new Dictionary<string, List<FileInfoEntity>>();
                    foreach (var file in sizeGroup)
                    {
                        using (FileStream stream = File.OpenRead(file.FileName))
                        {
                            string hash = CalculateHash(stream);
                            if (!hashGroups.ContainsKey(hash))
                            {
                                hashGroups[hash] = new List<FileInfoEntity>();
                            }
                            hashGroups[hash].Add(file);
                        }
                    }

                    foreach (var hashGroup in hashGroups.Values)
                    {
                        if (hashGroup.Count > 1)
                        {
                            duplicateFileGroups.Add(hashGroup);
                        }
                    }
                }
            }

            return duplicateFileGroups;
        }
    }
}


