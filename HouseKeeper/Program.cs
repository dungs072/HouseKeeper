
using HouseKeeper.Configs;
using HouseKeeper;
using HouseKeeper.DBContext;
using HouseKeeper.IServices;
using HouseKeeper.Respositories;
using HouseKeeper.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var provider = builder.Services.BuildServiceProvider();
var configuration = provider.GetRequiredService<IConfiguration>();
builder.Services.AddDbContext<HouseKeeperDBContext>(item => item.UseSqlServer(configuration.GetConnectionString("myconn")));


builder.Services.AddScoped<IAccountTypeRespository, AccountTypeRespository>();
builder.Services.AddScoped<IEmployerRespository, EmployerRespository>();
builder.Services.AddScoped<IEmployeeRespository, EmployeeRespository>();
builder.Services.AddScoped<IAdminRespository, AdminRespository>();
builder.Services.AddScoped<IStaffRespository, StaffRespository>();
builder.Services.AddScoped<IFirebaseService, FirebaseService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.Configure<FirebaseConfig>(configuration.GetSection("AppConfigs").GetSection("FirebaseConfigs"));
builder.Services.Configure<StripeSetting>(configuration.GetSection("StripeSettings"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
