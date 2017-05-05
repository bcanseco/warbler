using System.Threading.Tasks;

namespace Warbler.Interfaces
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
