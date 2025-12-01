using movesys_backend_.Deliveries.Domain.Model.Aggregates;
using movesys_backend_.Deliveries.Domain.Repositories;
using movesys_backend_.Shared.Infrastructure.Persistence.EFC;
using movesys_backend_.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace movesys_backend_.Deliveries.Infrastructure.Persistence.EFC.Repositories;

public class DeliveryRepository : BaseRepository<Delivery>, IDeliveryRepository
{
    public DeliveryRepository(AppDbContext context) : base(context)
    {
    }
}


