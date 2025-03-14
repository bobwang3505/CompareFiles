// See https://aka.ms/new-console-template for more information


using ConsoleApp1_CompareFiles;

Console.WriteLine("Hello, World!");




string[] filePaths = { 
    "C:\\Users\\Aa\\Desktop\\新建文件夹\\1.txt",
    "C:\\Users\\Aa\\Desktop\\新建文件夹\\2.txt",
    "C:\\Users\\Aa\\Desktop\\新建文件夹\\3.txt",

    "C:\\Users\\Aa\\Desktop\\新建文件夹\\4.xlsx",
    "C:\\Users\\Aa\\Desktop\\新建文件夹\\5.xlsx",

    "C:\\Users\\Aa\\Desktop\\新建文件夹\\6.docx",
    "C:\\Users\\Aa\\Desktop\\新建文件夹\\7.docx",

    "C:\\Users\\Aa\\Desktop\\新建文件夹\\8.jpeg",
    "C:\\Users\\Aa\\Desktop\\新建文件夹\\9.jpeg",

    "C:\\Users\\Aa\\Desktop\\新建文件夹\\捕获 - 副本.PNG",
    "C:\\Users\\Aa\\Desktop\\新建文件夹\\捕获.PNG",
    "C:\\Users\\Aa\\Desktop\\新建文件夹\\微信图片_20250221165428.png",
    
};

string folderPath = @"C:\Users\Aa\Desktop\新建文件夹";
string folderPath1 = @"C:\Users\Aa\Desktop\新建文件夹 (2)";
string folderPath2 = @"C:\Users\Aa\Desktop\新建文件夹 (4)";


//string[] allFiles = FileComparisonTool.GetAllFilesInFolder(folderPath1);
string[] allFiles = FileComparisonTool.GetAllFilesInFolder(folderPath2);


var duplicateFileGroups = FileComparisonTool.FindDuplicateFilesGrouped(allFiles);
Console.WriteLine($"找到的重复文件组数量: {duplicateFileGroups.Count}");
foreach (var group in duplicateFileGroups)
{
    Console.WriteLine($"该组重复文件数量: {group.Count}");
    foreach (var file in group)
    {
        Console.WriteLine($"  文件 ID: {file.Id}, 文件名: {file.FileName}, 文件大小: {file.FileSize}");
    }
}