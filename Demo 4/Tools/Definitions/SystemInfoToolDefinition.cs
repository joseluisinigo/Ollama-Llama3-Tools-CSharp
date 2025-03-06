using System.Collections.Generic;

namespace Tools.Definitions
{
    public static class SystemInfoToolDefinition
    {
        public static object GetDefinition()
        {
            return new
            {
                name = "get_system_info",
                description = "Determina si el sistema operativo actual es Linux o Windows",
                parameters = new
                {
                    type = "object",
                    properties = new Dictionary<string, object>(),
                    required = Array.Empty<string>()
                }
            };
        }
    }
}