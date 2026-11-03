# Ambiente e Execução – PedidoFácil

## Ferramentas e versões

| Ferramenta | Versão | Uso |
|---|---|---|
| .NET SDK | 10.0 (LTS) | Compilar e executar |
| C# | 14 | Linguagem do back-end |
| ASP.NET Core MVC | 10.0 | Framework web (padrão MVC) |
| Entity Framework Core + SQLite | 10.0 | Acesso ao banco de dados |
| ASP.NET Core Identity | 10.0 | Login e senhas com hash |
| Bootstrap | 5.3.3 | Layout responsivo (CSS) |
| jQuery + jQuery Validation | 3.7.1 / 1.21.0 | Validação dos formulários no navegador |
| xUnit | 2.9 | Testes automatizados |
| Visual Studio Community | 2026 | IDE |
| Git + GitHub | 2.55 | Versionamento |

Bootstrap, jQuery e os ícones são carregados de CDN (cdn.jsdelivr.net). Por isso, para as telas aparecerem formatadas, é preciso estar conectado à internet.

## Executar pelo Visual Studio

1. Abra o arquivo `PedidoFacil.slnx`.
2. Aguarde o Visual Studio baixar os pacotes (aparece "Restaurando pacotes" na barra inferior).
3. Clique no botão verde ▶ **PedidoFacil**.
4. O navegador abre em `http://localhost:5080`.

Na primeira execução, o arquivo do banco `pedidofacil.db` é criado na pasta `src/PedidoFacil.Web`, já com dados de exemplo fictícios (5 clientes, 10 produtos e 3 pedidos). Para começar do zero, feche o sistema e apague esse arquivo.

## Executar pelo terminal

```
cd src/PedidoFacil.Web
dotnet run
```

## Rodar os testes automatizados

- **Visual Studio:** menu **Teste → Gerenciador de Testes → Executar Todos**.
- **Terminal** (na pasta do repositório): `dotnet test`

## Configuração

O arquivo `appsettings.json` guarda o local do banco:

```json
"ConnectionStrings": { "DefaultConnection": "Data Source=pedidofacil.db" }
```

Para usar SQL Server no lugar do SQLite, basta trocar o pacote `Microsoft.EntityFrameworkCore.Sqlite` por `Microsoft.EntityFrameworkCore.SqlServer`, `UseSqlite` por `UseSqlServer` no `Program.cs` e a conexão no `appsettings.json`.

## Publicação online

_Será preenchido quando o sistema for publicado (endereço, hospedagem e passos)._
