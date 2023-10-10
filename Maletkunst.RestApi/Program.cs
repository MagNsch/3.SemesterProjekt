using Maletkunst.DAL.Interfaces;
using Maletkunst.DAL.SQL;

namespace Maletkunst.RestApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<IPaintingsDao, PaintingsSqlDao>();
        builder.Services.AddScoped<IPaintingsDataAccess, PaintingsSqlDataAccess>();
        builder.Services.AddScoped<IOrdersDataAccess, OrdersSqlDao>();

		//builder.Services.AddScoped<IOrderLineMvcDataAccess, OrderLineMvcSqlDao>();



		var app = builder.Build();

        // Configure the HTTP request pipeline.
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
}