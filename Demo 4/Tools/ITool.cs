using System.Threading.Tasks;

namespace Tools
{
    public interface ITool
    {
        string Name { get; }
        Task<string> ExecuteAsync(string argumentsJson);
    }
}
