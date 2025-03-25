using System;
using System.IO;
using System.Linq;
using TextCopy; // 🔹 Требуется NuGet-пакет TextCopy

class Program
{
    static void Main(string[] args)
    {
        string directoryPath = args.Length > 0 ? args[0] : GetDirectoryFromUser();

        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine("❌ Ошибка: Указанная папка не существует.");
            return;
        }

        string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.txt");
        string[] extensions = { ".cs", ".csproj", ".cpp", ".h", ".java", ".py" };

        try
        {
            using (StreamWriter writer = new StreamWriter(outputPath, false))
            {
                foreach (string file in Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories)
                                                 .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()))
                                                 .Where(f => !IsInExcludedFolder(f)))
                {
                    writer.WriteLine($"это {Path.GetFileName(file)} файл:\n");
                    writer.WriteLine(File.ReadAllText(file));
                    writer.WriteLine("\n" + new string('-', 50) + "\n"); // Разделитель между файлами
                }
            }

            // 🔹 Копируем содержимое файла в буфер
            string outputText = File.ReadAllText(outputPath);
            ClipboardService.SetText(outputText);

            Console.WriteLine($"✅ Файлы записаны в {outputPath} и скопированы в буфер обмена! (Ctrl + V)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка: {ex.Message}");
        }
    }

    static string GetDirectoryFromUser()
    {
        Console.WriteLine("📂 Перетащите папку с проектом в окно программы и нажмите Enter:");
        string? path = Console.ReadLine()?.Trim('"');
        return path ?? string.Empty;
    }

    static bool IsInExcludedFolder(string filePath)
    {
        string[] excludedFolders = { "\\bin\\", "\\obj\\" };
        return excludedFolders.Any(folder => filePath.Contains(folder));
    }
}
