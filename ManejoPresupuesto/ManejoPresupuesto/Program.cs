using ManejoPresupuesto.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IRepositorioTiposCuentas, RepositorioTiposCuentas>();
//Configurando el servicio de Usuarios
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>(); //Ahora ir hacia Tipos Cuentas Controller a usar el servicio
builder.Services.AddTransient<IRepositorioCuentas, RepositorioCuentas>();//Ahora configurar el servicio en el controlador de Cuentas
builder.Services.AddTransient<IRepositorioCategorias, RepositorioCategorias>();//Ahora configurar el servicio en el controlador de Categorías
builder.Services.AddAutoMapper(typeof(Program)); //Configurando AutoMapper


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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
