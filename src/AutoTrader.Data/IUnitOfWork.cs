using System.Threading.Tasks;

namespace AutoTrader.Data
{
    public interface IUnitOfWork
    {
        void SaveChanges(bool force = false);
        Task SaveChangesAsync(bool force = false);
        void CancelSaving();
    }
}
