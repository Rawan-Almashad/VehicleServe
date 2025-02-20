using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using VehicleServe.Data;
using VehicleServe.Services;
using Microsoft.EntityFrameworkCore;


namespace VehicleServe.Hubs

{
    public class ProviderHub: Hub
    {
        private readonly AppDbContext _dbContext; // Inject database context

        public ProviderHub(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task OnConnectedAsync()
        {
            var providerId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (providerId != null)
            {
                // Check if this user is a provider
                bool isProvider = await _dbContext.Providers.AnyAsync(p => p.Id == providerId);

                if (isProvider)
                {
                    OnlineUsersTracker.AddProvider(providerId, Context.ConnectionId);
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var providerId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (providerId != null)
            {
                // Check if the user is a provider before removing
                bool isProvider = await _dbContext.Providers.AnyAsync(p => p.Id == providerId);

                if (isProvider)
                {
                    OnlineUsersTracker.RemoveProvider(providerId);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotificationToProvider(string providerId, string message)
        {
            if (OnlineUsersTracker.TryGetConnectionId(providerId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
            }
        }


    }
}
