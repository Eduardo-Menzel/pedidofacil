# PedidoFácil – Controle de Pedidos de Venda

Projeto Integrador Transdisciplinar em Engenharia de Software II – Cruzeiro do Sul Virtual
Aluno: **Eduardo Menzel**

## Sobre o sistema

Sistema web para pequenas empresas e representantes comerciais cadastrarem clientes e produtos, registrarem pedidos de venda com itens e desconto, e acompanharem o status de cada pedido (Aberto → Aprovado → Faturado, ou Cancelado).

**Funcionalidades**
- Criar conta, login e logout
- Cadastro de clientes (com busca por nome) e de produtos
- Pedidos com itens, desconto de até 30% e total calculado automaticamente
- Mudança de status com regras de negócio
- Lista de pedidos com filtro por status

## Tecnologias

C# · .NET 10 · ASP.NET Core MVC · Entity Framework Core · SQLite · Bootstrap 5 · JavaScript · xUnit

## Acesse online

_Link será publicado na Situação 2._

## Vídeo de apresentação

_Link será publicado na Situação 3._

## Como rodar localmente

Pré-requisito: [.NET 10 SDK](https://dotnet.microsoft.com/download) (ou Visual Studio 2026 com a carga "ASP.NET e desenvolvimento Web").

**Visual Studio:** abra `PedidoFacil.slnx` e clique no botão ▶ **PedidoFacil**.

**Terminal:**
```
git clone https://github.com/Eduardo-Menzel/pedidofacil.git
cd pedidofacil/src/PedidoFacil.Web
dotnet run
```
Acesse `http://localhost:5080`, clique em **Criar conta** e use o sistema. O banco SQLite (`pedidofacil.db`) é criado sozinho na primeira execução, com dados de exemplo fictícios.

## Como rodar os testes

**Visual Studio:** menu **Teste → Gerenciador de Testes → Executar Todos**.
**Terminal**, na pasta do repositório:
```
dotnet test
```
São testes unitários (regras de negócio), de integração com o banco SQLite e de integração das telas (requisições HTTP).

## Documentação

### Situação-Problema 1 – Planejamento (revisão Kaizen)
| Documento | Arquivo |
|---|---|
| Visão e escopo + nota de revisão | [01-visao-e-escopo.md](docs/01-situacao-1-planejamento/01-visao-e-escopo.md) |
| Requisitos, regras de negócio e histórias de usuário | [02-requisitos-e-historias.md](docs/01-situacao-1-planejamento/02-requisitos-e-historias.md) |
| Diagrama de casos de uso | [casos-de-uso.md](docs/01-situacao-1-planejamento/03-uml/casos-de-uso.md) |
| Diagrama de classes | [classes.md](docs/01-situacao-1-planejamento/03-uml/classes.md) |
| Diagramas de sequência | [sequencia.md](docs/01-situacao-1-planejamento/03-uml/sequencia.md) |
| Diagrama de atividades | [atividades.md](docs/01-situacao-1-planejamento/03-uml/atividades.md) |
| Diagrama de estados | [estados.md](docs/01-situacao-1-planejamento/03-uml/estados.md) |
| Protótipo de telas e princípios de IHC | [prototipo.md](docs/01-situacao-1-planejamento/04-ihc/prototipo.md) |
| Mapa de navegação | [mapa-de-navegacao.md](docs/01-situacao-1-planejamento/04-ihc/mapa-de-navegacao.md) |
| Catálogo de mensagens | [mensagens.md](docs/01-situacao-1-planejamento/04-ihc/mensagens.md) |
| Modelo conceitual (DER) | [der.md](docs/01-situacao-1-planejamento/05-dados/der.md) |
| Modelo lógico normalizado | [modelo-logico.md](docs/01-situacao-1-planejamento/05-dados/modelo-logico.md) |
| Projeto físico (script SQL) | [script-fisico.sql](docs/01-situacao-1-planejamento/05-dados/script-fisico.sql) |
| Dicionário de dados | [dicionario-de-dados.md](docs/01-situacao-1-planejamento/05-dados/dicionario-de-dados.md) |

### Situação-Problema 2 – Desenvolvimento
| Documento | Arquivo |
|---|---|
| Arquitetura MVC e estrutura do código | [arquitetura-mvc.md](docs/02-situacao-2-desenvolvimento/arquitetura-mvc.md) |
| Ambiente, execução e testes | [ambiente-e-execucao.md](docs/02-situacao-2-desenvolvimento/ambiente-e-execucao.md) |
| Manual de uso | [manual-de-uso.md](docs/02-situacao-2-desenvolvimento/manual-de-uso.md) |

### Situação-Problema 3 – Testes e validação
[docs/03-situacao-3-testes-e-validacao](docs/03-situacao-3-testes-e-validacao/) – plano e relatório de testes, formulário de avaliação, avaliações dos pares e melhorias.

## Estrutura de pastas

```
pedidofacil/
├── README.md
├── docs/
│   ├── 01-situacao-1-planejamento/    visão, requisitos, UML, IHC e banco de dados
│   ├── 02-situacao-2-desenvolvimento/ arquitetura, ambiente e manual de uso
│   └── 03-situacao-3-testes-e-validacao/ testes, avaliações e melhorias
├── src/PedidoFacil.Web/               aplicação ASP.NET Core MVC
└── tests/PedidoFacil.Tests/           testes automatizados (xUnit)
```

> Todos os dados usados no sistema e na documentação são fictícios.
