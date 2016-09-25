using AutoTrader.DomainModel;

namespace AutoTrader.Service
{
    public interface ICurrentUserProvider
    {
        User User { get; }
    }
}
