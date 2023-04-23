using System;
using System.IO;
using System.Text;

namespace ShowcaseThreadLogger
{
    /// <summary>
    /// Класс FileAppender предоставляет безопасный для многопоточности
    /// способ добавления текста в файл. 
    /// </summary>
    public class FileAppender
    {
        // обьект заглушка -
        // значение будет установлено только один раз и не может быть изменено после этого.
        private readonly object _lockObject = new(); 
        
        public string FilePath { get; }

        public FileAppender(string filePath)
        {
            FilePath = filePath;
        }

        public bool Append(string content)
        {
            try
            {
                // заблокировать доступ других потоков пока не выполнит текущий поток код ниже
                lock (_lockObject) 
                {
                    using (StreamWriter sw = new StreamWriter(File.Open(FilePath, FileMode.Append, FileAccess.Write, FileShare.Read)))
                    {
                        sw.Write(content);
                    }

                }
                
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}