using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Email;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<LanguageDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<TestUserHandler>();
builder.Services.AddScoped<QuestionHandler>();
builder.Services.AddScoped<AnswerHandler>();
builder.Services.AddScoped<PermissionsHandler>();
builder.Services.AddTransient<IEmailSender, EmailSender>(e => new EmailSender(builder.Configuration.GetValue<string>("SocketLabsApiKey")));

builder.Services.AddRazorPages(options => {
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToFolder("/Account");
});
builder.Services.AddControllers();
builder.Services.Configure<FormOptions>(form => {
    form.ValueLengthLimit = int.MaxValue;
    form.MultipartBodyLengthLimit = int.MaxValue;
    form.MemoryBufferThreshold = int.MaxValue;
});
builder.Services.Configure<IISServerOptions>(form => {
    form.MaxRequestBodyBufferSize = int.MaxValue;
    form.MaxRequestBodySize = int.MaxValue;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseMigrationsEndPoint();
} else {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.Run();