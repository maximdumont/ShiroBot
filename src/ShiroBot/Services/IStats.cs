using System.Threading.Tasks;

namespace ShiroBot.Services
{
    public interface IStats
    {
        Task<string> Print();
        Task Reset();
    }
}
