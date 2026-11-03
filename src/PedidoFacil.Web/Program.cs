using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders;
using PedidoFacil.Web.Data;
using PedidoFacil.Web.Infra;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Banco de dados: SQLite via Entity Framework Core (RNF03)
// ---------------------------------------------------------------------------
// A conexão está no appsettings.json ("DefaultConnection"). É lida quando o banco é usado,
// o que permite aos testes trocarem por um banco temporário.
builder.Services.AddDbContext<ApplicationDbContext>((servicos, options) =>
{
    var connectionString = servicos.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("A conexão 'DefaultConnection' não foi encontrada no appsettings.json.");

    // Caminho relativo do arquivo .db passa a ser relativo à pasta do sistema (funciona também na hospedagem).
    var conexao = new SqliteConnectionStringBuilder(connectionString);
    if (conexao.DataSource != ":memory:" && !Path.IsPathRooted(conexao.DataSource))
    {
        var pastaDoSistema = servicos.GetRequiredService<IWebHostEnvironment>().ContentRootPath;
        conexao.DataSource = Path.Combine(pastaDoSistema, conexao.DataSource);
    }

    options.UseSqlite(conexao.ToString());
});

// ---------------------------------------------------------------------------
// Login e cadastro de usuários: ASP.NET Core Identity (RF01, RNF04)
// ---------------------------------------------------------------------------
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false;   // avaliadores entram sem confirmar e-mail
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<IdentityMensagensPtBr>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Conta/Entrar";
    options.LogoutPath = "/Conta/Sair";
    options.AccessDeniedPath = "/Conta/Entrar";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// ---------------------------------------------------------------------------
// MVC com mensagens de validação em português (RNF05)
// ---------------------------------------------------------------------------
builder.Services.AddControllersWithViews(options =>
{
    // Aceita valores como 45,90 · 1.234,56 · 45.90
    options.ModelBinderProviders.Insert(0, new DecimalBrasilModelBinderProvider());

    var msg = options.ModelBindingMessageProvider;
    msg.SetValueMustNotBeNullAccessor(_ => "Este campo é obrigatório.");
    msg.SetMissingBindRequiredValueAccessor(campo => $"O campo {campo} é obrigatório.");
    msg.SetMissingKeyOrValueAccessor(() => "Este campo é obrigatório.");
    msg.SetAttemptedValueIsInvalidAccessor((valor, campo) => $"O valor '{valor}' não é válido para o campo {campo}.");
    msg.SetNonPropertyAttemptedValueIsInvalidAccessor(valor => $"O valor '{valor}' não é válido.");
    msg.SetUnknownValueIsInvalidAccessor(campo => $"O valor informado não é válido para o campo {campo}.");
    msg.SetNonPropertyUnknownValueIsInvalidAccessor(() => "O valor informado não é válido.");
    msg.SetValueIsInvalidAccessor(valor => $"O valor '{valor}' não é válido.");
    msg.SetValueMustBeANumberAccessor(campo => $"O campo {campo} deve ser um número.");
    msg.SetNonPropertyValueMustBeANumberAccessor(() => "Este campo deve ser um número.");
});

// Mantém acentos como texto normal no HTML (ex.: "PedidoFácil" em vez de "PedidoF&#xE1;cil").
builder.Services.Configure<WebEncoderOptions>(options =>
    options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

var app = builder.Build();

// ---------------------------------------------------------------------------
// Cria o banco (se não existir) e os dados de exemplo fictícios
// ---------------------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
    DadosIniciais.Popular(db);
}

// ---------------------------------------------------------------------------
// Tratamento de erros: o usuário nunca vê mensagem técnica (RNF05)
// ---------------------------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/Home/StatusCode/{0}");

// Idioma e formato brasileiro (datas dd/mm/aaaa, valores R$ 1.234,56)
var culturaBrasil = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = culturaBrasil;
CultureInfo.DefaultThreadCurrentUICulture = culturaBrasil;
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culturaBrasil),
    SupportedCultures = new[] { culturaBrasil },
    SupportedUICultures = new[] { culturaBrasil }
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Permite que os testes de integração (WebApplicationFactory) acessem a aplicação.
public partial class Program { }
