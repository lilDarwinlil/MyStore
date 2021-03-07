using System.Threading.Tasks;

namespace Store
{
    public interface INotificationService
    {
        void SendConfirmationCode(string cellPhone, int code);

        Task SendConfirmationCodeAsync(string cellPhone, int code);

        void StartProcess(Order order);

        Task StartProcessAsync(Order order);
    }
}
