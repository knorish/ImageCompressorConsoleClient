using System;
using System.Linq;

namespace ImageCompressorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter folder path: ");
            string path = Console.ReadLine();
            Console.WriteLine("\n");

            var files = System.IO.Directory.EnumerateFiles(path, "*.*", System.IO.SearchOption.AllDirectories).Where(s => s.EndsWith(".jpg") || s.EndsWith(".jpeg") || s.EndsWith(".png") || s.EndsWith(".gif"));

            foreach (var file in files)
            {
                ImageCompressor imageCompressor = new ImageCompressor();
                var result = imageCompressor.CompressFile(file, true);
                Console.WriteLine(result.ToString());
            }

            Console.WriteLine("Images compressed successfully with best possible algorithm.");
            Console.WriteLine("Thanks for using - Abhimanyu K Vatsa");
            Console.ReadKey();
        }
    }
}
