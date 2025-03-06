using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Utils
{
    public static class CodeExporter
    {
        public static void ExportCodeToTxt(string rootDirectory)
        {
            string outputFile = Path.Combine(rootDirectory, "code.txt");

            try
            {
                StringBuilder sb = new StringBuilder();
                var files = Directory.GetFiles(rootDirectory, "*.*", SearchOption.AllDirectories)
                                     .Where(f => (f.EndsWith(".cs") || f.EndsWith(".json") || f.EndsWith(".csproj") || f.EndsWith(".sln")) &&
                                                 !f.Contains(@"\obj\") && !f.Contains(@"\bin\"))
                                     .OrderBy(f => f);

                foreach (var file in files)
                {
                    sb.AppendLine($"====================================");
                    sb.AppendLine($"📄 Ruta: {file}");
                    sb.AppendLine($"====================================");
                    sb.AppendLine(File.ReadAllText(file));
                    sb.AppendLine("\n\n");
                }

                File.WriteAllText(outputFile, sb.ToString());
                Console.WriteLine($"✅ Código exportado a: {outputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al exportar el código: {ex.Message}");
            }
        }
    }
}
