using System.Threading.Tasks;

namespace Warbler.Identity.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
