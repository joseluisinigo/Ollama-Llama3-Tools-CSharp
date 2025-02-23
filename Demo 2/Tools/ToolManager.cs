using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace Tools
{
    public static class ToolManager
    {
        private static readonly List<ITool> tools = new List<ITool>
        {
            new WeatherTool(),
            new StockPriceTool()
        };

        public static ITool? GetToolByName(string name)
        {
            Log.Information("🔎 Buscando herramienta '{name}' en ToolManager...", name);
            var tool = tools.FirstOrDefault(t => t.Name == name);
            
            if (tool == null)
            {
                Log.Warning("⚠️ Herramienta '{name}' no encontrada en ToolManager.", name);
            }
            else
            {
                Log.Information("✅ Herramienta '{name}' encontrada en ToolManager.", name);
            }

            return tool;
        }
    }
}
