using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.Extensions.Caching.Distributed;
using Ticketing.BL.Services;
using Ticketing.Caching.Services;
using Ticketing.Db.DAL;
using Ticketing.Db.Models;
using Ticketing.Db.Providers;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureStorage(builder.Configuration, builder.Services);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static void ConfigureStorage(IConfiguration configuration, IServiceCollection services)
    {

        services.AddSingleton<IDistributedCache, MemoryDistributedCache>();
        services.AddSingleton<IResponseCacheService, InMemoryCacheService>();

        services.AddSingleton<IConnectionStringProvider, ConnectionStringProvider>(x => new ConnectionStringProvider("Server=(localdb);Database=Ticketing;Trusted_Connection=True;MultipleActiveResultSets=true"));
        services.AddTransient<Repository<Event>, EventRepository>();
        services.AddTransient<Repository<Offer>, OfferRepository>();
        services.AddTransient<Repository<Manager>, ManagerRepository>();
        services.AddTransient<Repository<Order>, OrderRepository>();
        services.AddTransient<Repository<Payment>, PaymentRepository>();
        services.AddTransient<Repository<TicketPriceLevel>, TicketPriceLevelRepository>();
        services.AddTransient<Repository<Section>, SectionRepository>();
        services.AddTransient<Repository<Ticket>, TicketRepository>();
        services.AddTransient<Repository<Venue>, VenueRepository>();
        services.AddTransient<Repository<Customer>, CustomerRepository>();
        services.AddTransient<Repository<Seat>, SeatRepository>();
    }
}