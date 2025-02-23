using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tools
{
    public static class ToolManager
    {
        private static readonly List<ITool> tools = new List<ITool>
        {
            new WeatherTool() // Aquí podemos agregar más herramientas
        };

        public static ITool GetToolByName(string name)
        {
            return tools.FirstOrDefault(tool => tool.Name == name);
        }
    }
}
