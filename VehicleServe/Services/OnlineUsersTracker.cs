using System.Collections.Concurrent;

namespace VehicleServe.Services
{
    public class OnlineUsersTracker
    {
        private static readonly ConcurrentDictionary<string, string> _onlineProviders = new();

        public static void AddProvider(string providerId, string connectionId)
        {
            _onlineProviders[providerId] = connectionId;
        }

        public static void RemoveProvider(string providerId)
        {
            _onlineProviders.TryRemove(providerId, out _);
        }

        public static bool TryGetConnectionId(string providerId, out string connectionId)
        {
            return _onlineProviders.TryGetValue(providerId, out connectionId);
        }

        public static List<string> GetOnlineProviders()
        {
            return _onlineProviders.Keys.ToList();
        }
    }
}
