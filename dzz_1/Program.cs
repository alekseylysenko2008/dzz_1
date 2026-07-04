using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.WriteLine("1 - Закрывать браузеры");
        Console.WriteLine("2 - Найти браузеры и открыть Google");
        Console.Write("Выберите действие: ");

        string option = Console.ReadLine();

        if (option == "1")
        {
            CloseBrowsers();
        }
        else if (option == "2")
        {
            RunBrowserSearch();
        }
    }

    // ---------- Задание 1 ----------

    static void CloseBrowsers()
    {
        string[] browsers = { "chrome", "msedge", "firefox" };

        while (true)
        {
            foreach (string browser in browsers)
            {
                Process[] list = Process.GetProcessesByName(browser);

                foreach (Process p in list)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {
                        // если процесс нельзя закрыть
                    }
                }
            }

            Thread.Sleep(1000);
        }
    }

    // ---------- Задание 2 ----------

    static void RunBrowserSearch()
    {
        List<string> folders = new List<string>()
        {
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
        };

        Dictionary<string, string> foundBrowsers = FindBrowsers(folders);

        if (foundBrowsers.Count == 0)
        {
            Console.WriteLine("Браузеры не найдены.");
            return;
        }

        Console.WriteLine("\nНайденные браузеры:");

        foreach (var item in foundBrowsers)
        {
            Console.WriteLine($"{item.Key} -> {item.Value}");
        }

        Console.Write("\nВведите название браузера: ");
        string browserName = Console.ReadLine().ToLower();

        if (foundBrowsers.TryGetValue(browserName, out string path))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                Arguments = "https://google.com",
                UseShellExecute = true
            });
        }
        else
        {
            Console.WriteLine("Такого браузера нет.");
        }
    }

    static Dictionary<string, string> FindBrowsers(List<string> startFolders)
    {
        Dictionary<string, string> browsers = new Dictionary<string, string>();
        Stack<string> stack = new Stack<string>();

        foreach (string folder in startFolders)
        {
            stack.Push(folder);
        }

        while (stack.Count > 0)
        {
            string current = stack.Pop();

            try
            {
                string[] files = Directory.GetFiles(current);

                foreach (string file in files)
                {
                    string exe = Path.GetFileName(file).ToLower();

                    if (exe == "chrome.exe" ||
                        exe == "msedge.exe" ||
                        exe == "firefox.exe")
                    {
                        browsers[Path.GetFileNameWithoutExtension(file).ToLower()] = file;
                    }
                }

                string[] dirs = Directory.GetDirectories(current);

                foreach (string dir in dirs)
                {
                    if (!dir.Contains("Windows"))
                    {
                        stack.Push(dir);
                    }
                }
            }
            catch
            {
                // нет доступа к папке
            }
        }

        return browsers;
    }
}