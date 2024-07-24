using LanchesMac.Areas.Admin.Services;
using LanchesMac.Context;
using LanchesMac.Models;
using LanchesMac.Repositories;
using LanchesMac.Repositories.Interfaces;
using LanchesMac.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.WebHost.UseUrls("http://*:80");

//Configura o pacote de paginação ReflectionIT.Mvc.Paging
builder.Services.AddPaging( options => {
    options.ViewName = "Bootstrap5";
    options.PageParameterName = "pageindex";
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(
    options=> options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

//Aqui eu sobreescrevo a política de senha do identity
builder.Services.Configure<IdentityOptions>(options=>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;    
});

builder.Services.AddTransient<ILanchesRepository, LanchesRepository>();
builder.Services.AddTransient<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddTransient<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<ISeedUserRolerInitial, SeedUserRolerInitial>();
builder.Services.AddScoped<RelatorioVendasService>();
builder.Services.AddScoped<GraficoVendasService>();
builder.Services.AddScoped<RelatorioLanchesService>();

//registra o serviço que lê o arquivo de configuração para lidar com as imagens
builder.Services.Configure<ConfigurationImagens>(
    builder.Configuration.GetSection("ConfigurationPastaImagens")
);

builder.Services.AddAuthorization( options => {
    options.AddPolicy("Admin", politica =>
    {
        politica.RequireRole("Admin");
    });
});

//Injeta a classe CarrinhoCompra no conteiner DI da asp.net.
//Por isso o método é estático, para que possa ser invocado no program.cs ou na startup.cs se for o caso.
//O AddScoped indica que o tempo de vida da instância do carrinho de compras é por requisição
builder.Services.AddScoped(sp=>CarrinhoCompra.GetCarrinho(sp));
builder.Services.AddMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

CriarPerfisUsuarios(app);

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "categoriaFiltro",
    pattern: "Lanche/{action}/{categoria?}",
    defaults: new { Controller = "Lanche", action = "List"}
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


void CriarPerfisUsuarios(WebApplication app)
{    
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();   
     using (var scope = scopedFactory.CreateScope())   
      { 
         var service = scope.ServiceProvider.GetService<ISeedUserRolerInitial>();
         service.SeedRoles();   
         service.SeedUsers();     
     }
}