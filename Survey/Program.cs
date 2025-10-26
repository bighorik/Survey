using FluentMigrator.Runner;
using Npgsql;
using Survey.Persistence;

namespace Survey
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;
            var connectionString = configuration["ConnectionString"];

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton(new NpgsqlDataSourceBuilder(connectionString).Build());
            builder.Services.AddFluentMigratorCore().ConfigureRunner(build => build.AddPostgres().WithGlobalConnectionString(connectionString).ScanIn(typeof(InitMigration).Assembly).For.Migrations());
            builder.Services.AddTransient<SurveyRepository>();

            builder.WebHost.UseUrls("http://localhost:5300");

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapControllers();

            using IServiceScope serviceScope = app.Services.CreateScope();
            serviceScope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();

            app.Run();
        }
    }
}
