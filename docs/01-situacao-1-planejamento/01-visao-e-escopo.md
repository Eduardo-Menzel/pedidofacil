# Documento de Visão e Escopo – PedidoFácil

| Item | Informação |
|---|---|
| Projeto | PedidoFácil – Sistema de Controle de Pedidos de Venda |
| Disciplina | Projeto Integrador Transdisciplinar em Engenharia de Software II |
| Instituição | Cruzeiro do Sul Virtual |
| Aluno | Eduardo Menzel |
| Versão do documento | 1.0 |
| Data | 29/10/2026 |

---

## 1. Problema

Pequenas empresas e representantes comerciais costumam controlar seus pedidos de venda em planilhas, cadernos ou mensagens de WhatsApp. Isso gera:

- pedidos perdidos ou duplicados;
- erros no cálculo do valor total e do desconto;
- dificuldade para saber em que situação está cada pedido (aberto, aprovado, faturado ou cancelado);
- cadastros de clientes repetidos ou incompletos.

## 2. Objetivo

Oferecer um sistema web simples, acessível pelo navegador do computador ou do celular, para **cadastrar clientes e produtos, registrar pedidos de venda com itens, calcular o total automaticamente e acompanhar o status de cada pedido**.

## 3. Usuários

| Perfil | Descrição | O que faz no sistema |
|---|---|---|
| Vendedor | Funcionário de uma pequena empresa ou representante comercial | Cadastra clientes e produtos, registra pedidos e acompanha o status |

O sistema tem **um único perfil de usuário**. Cada pessoa cria a própria conta (e-mail e senha) para acessar.

## 4. Escopo (o que o sistema faz)

1. Criar conta, entrar (login) e sair (logout).
2. Cadastro de **clientes**: incluir, listar, buscar por nome, editar e excluir.
3. Cadastro de **produtos**: incluir, listar, editar e excluir.
4. **Pedidos**:
   - criar um pedido para um cliente;
   - adicionar e remover itens (produto + quantidade);
   - informar desconto de 0% a 30%;
   - ver o total calculado automaticamente;
   - mudar o status: Aberto → Aprovado → Faturado, ou Cancelado.
5. **Lista de pedidos** com filtro por status.

## 5. Fora do escopo

- Emissão de nota fiscal;
- Controle de estoque;
- Pagamentos e financeiro;
- Relatórios avançados e gráficos;
- Aplicativo mobile nativo (o sistema web funciona no celular pelo navegador);
- Vários perfis de acesso (administrador, gerente etc.).

## 6. Tecnologias

| Camada | Tecnologia |
|---|---|
| Linguagem | C# com .NET 10 (LTS) |
| Back-end | ASP.NET Core MVC (padrão Model–View–Controller) |
| Front-end | HTML (Razor Views), CSS (Bootstrap 5) e JavaScript |
| Banco de dados | SQLite, acessado pelo Entity Framework Core |
| Autenticação | ASP.NET Core Identity (senhas com hash) |
| Testes | xUnit |
| Versionamento | Git e GitHub |
| Hospedagem | MonsterASP.NET (plano gratuito) |

**Por que SQLite?** É o banco de dados relacional mais usado do mundo, não precisa de servidor instalado, guarda tudo em um único arquivo e qualquer pessoa consegue rodar o projeto. Se for necessário um servidor, a troca para SQL Server exige mudar apenas uma linha de configuração.

## 7. Premissas e restrições

- Todos os dados usados no sistema e nos testes são **fictícios**.
- O sistema deve funcionar nos navegadores atuais (Chrome, Edge, Firefox) e no celular.
- Prazo: o desenvolvimento cabe no período da disciplina, por isso o escopo é pequeno e completo.

## 8. Critérios de sucesso

- Todas as funcionalidades do escopo funcionando e publicadas online;
- Testes automatizados passando;
- Avaliação do sistema por 5 pessoas, com correções registradas;
- Documentação completa no GitHub.

---

## 9. Nota de revisão (Kaizen)

A Situação-Problema 1 pede um ciclo de melhoria contínua sobre o planejamento feito no PIT I.

**Situação encontrada:** os documentos produzidos no PIT I não estavam mais disponíveis para revisão.

**Decisão tomada:** o planejamento foi **reconstruído** e o tema foi **redefinido** para um sistema menor e de domínio conhecido pelo aluno (controle de pedidos de venda). Essa escolha permite entregar um sistema completo, testado e validado dentro do prazo da disciplina.

**Melhorias aplicadas nesta versão:**

| # | Melhoria | Motivo |
|---|---|---|
| 1 | Escopo reduzido a 4 entidades de negócio (Cliente, Produto, Pedido, Item do Pedido) | Escopo pequeno e completo vale mais que grande e incompleto |
| 2 | Regras de negócio numeradas (RN01 a RN08) | Permite ligar cada regra a um teste (rastreabilidade) |
| 3 | Requisitos ligados a histórias de usuário com critérios de aceitação | Prática ágil exigida pela disciplina |
| 4 | Banco de dados popular e de fácil manutenção (SQLite) | Orientação do "Problema em Foco": nada de SGBD obscuro |
| 5 | Documentação em Markdown dentro do GitHub | Documentação e código no mesmo lugar, com histórico de versões |

**Críticas de colegas sobre o planejamento:**

| Data | Colega | Crítica / sugestão | O que mudou |
|---|---|---|---|
| _a preencher_ | _a preencher_ | _a preencher_ | _a preencher_ |

## 10. Histórico de versões

| Versão | Data | Autor | Alteração |
|---|---|---|---|
| 1.0 | 29/10/2026 | Eduardo Menzel | Versão inicial (planejamento reconstruído) |
