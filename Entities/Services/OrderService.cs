namespace Entities
{
    /// <inheritdoc cref="IOrderService"/>
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBicycleRepository _bicycleRepository;
        private readonly IOrderPricingService _pricingService;

        public OrderService(IOrderRepository orderRepository, IBicycleRepository bicycleRepository, IOrderPricingService pricingService)
        {
            _orderRepository = orderRepository;
            _bicycleRepository = bicycleRepository;
            _pricingService = pricingService;
        }

        public Task<PaginatedList<Order>> GetPagedAsync(int page, int pageSize, string? sellerId) =>
            _orderRepository.GetPagedAsync(page, pageSize, sellerId);

        public Task<Order?> GetByIdAsync(long id) => _orderRepository.GetByIdAsync(id);

        public Task<Order?> GetDetailsByIdAsync(long id) => _orderRepository.GetDetailsByIdAsync(id);

        public Task<Order?> GetWithBicyclesAndSellerAsync(long id) => _orderRepository.GetWithBicyclesAndSellerAsync(id);

        public async Task CreateAsync(Order order)
        {
            _orderRepository.Add(order);
            await _orderRepository.SaveChangesAsync();
        }

        public Task SaveChangesAsync() => _orderRepository.SaveChangesAsync();

        public async Task<bool> DeleteAsync(long id)
        {
            var order = await _orderRepository.GetWithBicyclesAndSellerAsync(id);
            if (order == null) return false;

            foreach (var bicycle in order.Bicycles)
                bicycle.Order = null;

            _orderRepository.Remove(order);
            await _orderRepository.SaveChangesAsync();
            return true;
        }

        public float CalculateTotal(Order order) => _pricingService.CalculateTotal(order);

        public async Task<OrderOperationResult> AddBicycleAsync(long orderId, long bicycleId)
        {
            var order = await _orderRepository.GetWithBicyclesAndSellerAsync(orderId);
            if (order == null)
                return new OrderOperationResult { Status = OrderOperationStatus.NotFound, Error = "Commande introuvable." };
            if (order.IsValidated)
                return new OrderOperationResult { Status = OrderOperationStatus.InvalidState, Error = "Impossible de modifier une commande validée." };

            var bicycle = await _bicycleRepository.GetByIdWithOrderAsync(bicycleId);
            if (bicycle == null)
                return new OrderOperationResult { Status = OrderOperationStatus.NotFound, Error = "Vélo introuvable." };
            if (bicycle.Quantity <= 0)
                return new OrderOperationResult { Status = OrderOperationStatus.InvalidState, Error = "Stock insuffisant pour ce vélo." };
            if (bicycle.Order != null && bicycle.Order.IdOrder != orderId)
                return new OrderOperationResult { Status = OrderOperationStatus.InvalidState, Error = "Ce vélo est déjà associé à une autre commande." };

            bicycle.Order = order;
            await _orderRepository.SaveChangesAsync();

            return new OrderOperationResult { Status = OrderOperationStatus.Success, Total = _pricingService.CalculateTotal(order) };
        }

        public async Task<OrderOperationResult> RemoveBicycleAsync(long orderId, long bicycleId)
        {
            var order = await _orderRepository.GetWithBicyclesAndSellerAsync(orderId);
            if (order == null)
                return new OrderOperationResult { Status = OrderOperationStatus.NotFound, Error = "Commande introuvable." };
            if (order.IsValidated)
                return new OrderOperationResult { Status = OrderOperationStatus.InvalidState, Error = "Impossible de modifier une commande validée." };

            var bicycle = await _bicycleRepository.GetByIdInOrderAsync(bicycleId, orderId);
            if (bicycle == null)
                return new OrderOperationResult { Status = OrderOperationStatus.NotFound, Error = "Vélo introuvable dans cette commande." };

            bicycle.Order = null;
            await _orderRepository.SaveChangesAsync();

            return new OrderOperationResult { Status = OrderOperationStatus.Success, Total = _pricingService.CalculateTotal(order) };
        }

        public async Task<OrderOperationResult> ValidateAsync(long id)
        {
            var order = await _orderRepository.GetWithBicyclesAndSellerAsync(id);
            if (order == null)
                return new OrderOperationResult { Status = OrderOperationStatus.NotFound, Error = "Commande introuvable." };
            if (order.IsValidated)
                return new OrderOperationResult { Status = OrderOperationStatus.InvalidState, Error = "La commande est déjà validée." };
            if (!order.Bicycles.Any())
                return new OrderOperationResult { Status = OrderOperationStatus.InvalidState, Error = "Impossible de valider une commande sans produits." };

            var outOfStock = order.Bicycles.FirstOrDefault(b => b.Quantity <= 0);
            if (outOfStock != null)
                return new OrderOperationResult { Status = OrderOperationStatus.InvalidState, Error = $"Stock insuffisant pour le vélo #{outOfStock.Id}." };

            foreach (var bicycle in order.Bicycles)
                bicycle.Quantity -= 1;

            order.IsValidated = true;
            await _orderRepository.SaveChangesAsync();

            return new OrderOperationResult
            {
                Status = OrderOperationStatus.Success,
                Message = "Commande validée avec succès.",
                Total = _pricingService.CalculateTotal(order)
            };
        }
    }
}
