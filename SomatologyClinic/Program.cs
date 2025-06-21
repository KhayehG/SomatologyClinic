using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SomatologyClinic.Data;
using SomatologyClinic.Services;
using SomatologyClinic.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpClient<IPaymentService, PaymentService>();

// Updated Identity configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// Uncomment and update these lines when you're ready to use these services
// builder.Services.AddScoped<AppointmentService>();
// builder.Services.AddScoped<TreatmentService>();
// builder.Services.AddScoped<PaymentService>();

builder.Services.AddScoped<IPaymentService, UnifiedPaymentGateway>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add email configuration
//builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
//builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var context = services.GetRequiredService<ApplicationDbContext>();
        var dbInitializer = new DbInitializer(userManager, roleManager, context);

        await dbInitializer.SeedRolesAsync();
        await dbInitializer.SeedAdminUserAsync();
        await dbInitializer.SeedTreatmentsAsync();
        await dbInitializer.SeedStaffMemberAsync();
        await dbInitializer.SeedManagerMemberAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
