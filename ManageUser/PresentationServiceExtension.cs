namespace ManageUser
{
    public static class PresentationServiceExtension
    {
        public static IServiceCollection AddPresentationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add necessary service registrations here
            // Example: services.AddTransient<IMyService, MyService>();

            return services; // Ensure a value is returned to fix CS0161
        }
    }
}
