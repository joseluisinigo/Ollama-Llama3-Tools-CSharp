using System.Collections.Generic;
using Tools.Definitions;

namespace Tools
{
    public static class ToolDefinitionManager
    {
        public static object[] GetAllToolDefinitions()
        {
            return new object[]
            {
                WeatherToolDefinition.GetDefinition(),
                StockPriceToolDefinition.GetDefinition()
            };
        }
    }
}
