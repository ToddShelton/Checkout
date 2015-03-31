  using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Cloudrocket.Interfaces;
    using Orleans;
    using Orleans.Providers;

  namespace Cloudrocket.Grains
{
    [StorageProvider(ProviderName = "AzureTableGrainState")]
    public class GrainPurchase : Grain<IGrainPurchaseState>, IGrainPurchase
    {
        private Purchase _purchase;

        public override Task OnActivateAsync()
        {
            if (State.Purchase == null) { State.Purchase = new Purchase(); }
            if (State.Purchase.PurchaseId == Guid.Empty) { State.Purchase.PurchaseId = this.GetPrimaryKey(); }

            return base.OnActivateAsync();
        }

        public async Task<Purchase> CreateOrderAsync(Order order)
        {
            _purchase = new Purchase(order);

            return await Task<Purchase>.FromResult(_purchase);
        }

        public async Task<Purchase> GetOrderAsync()
        {
            return await Task<Purchase>.FromResult(State.Purchase);
        }
    }

    public interface IGrainPurchaseState : IGrainState
    {
        Purchase Purchase { get; set; }
    }
}