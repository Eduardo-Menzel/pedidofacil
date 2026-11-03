# Plano de Testes – PedidoFácil

O plano segue o **Modelo V**: cada nível de especificação tem um tipo de teste correspondente.

| Especificação | Tipo de teste | Como é feito no PedidoFácil |
|---|---|---|
| Requisitos (RF, RN) | Teste de aceitação / validação | Avaliação por 5 pessoas com o formulário de laudo |
| Análise (casos de uso) | Teste de sistema | Casos de teste funcionais CT01 a CT27, executados pelo navegador |
| Arquitetura (MVC + banco) | Teste de integração | Testes automatizados `IntegracaoWebTests` e `BancoDeDadosTests` |
| Código (classes do Model) | Teste unitário | Testes automatizados `PedidoRegrasTests` e `ConversaoDecimalTests` |

- **Verificação** (o software está de acordo com a especificação?): testes automatizados e casos de teste funcionais.
- **Validação** (o produto atende ao que o usuário precisa?): testes feitos pelos avaliadores ("dublês de usuário").

---

## 1. Testes automatizados (xUnit) – 51 execuções

Executar: Visual Studio → **Test → Test Explorer → Run All Tests**, ou `dotnet test` na pasta do repositório.

### 1.1 Unitários – regras de negócio (`PedidoRegrasTests`, 28 execuções)

| ID | O que verifica | Regra |
|---|---|---|
| T01 | Total sem desconto = soma dos itens (936,00) | RN04 |
| T02 | Total com 10% de desconto (842,40) e valor do desconto (93,60) | RN04 |
| T03 | Desconto de −1%, 30,01% e 35% é recusado (no método e no formulário) | RN03 |
| T03b | Desconto de 0%, 12,5% e 30% é aceito | RN03 |
| T04 | Pedido faturado não aceita itens nem desconto | RN05 |
| T05 | Pedido sem itens não pode ser aprovado | RN07 |
| T06 | Pedido faturado não muda para Aberto, Aprovado ou Cancelado | RN06 |
| T06b | Pedido aberto não pode ir direto para Faturado | RN06 |
| T06c | Fluxo válido Aberto → Aprovado → Faturado | RN06 |
| T06d | Pedido aprovado pode ser cancelado | RN06 |
| T07 | Quantidade 0 e −3 é recusada | RN02 |
| T08 | Cliente sem CNPJ/CPF é inválido | RN01 |
| T08b | CPF e CNPJ com e sem pontuação são aceitos (4 formatos) | RN01 |
| T08c | Produto com preço zero é inválido | RN02 |
| T14 | Item guarda o preço do dia da venda mesmo se o produto mudar de preço | RN09 |
| T15 | Adicionar o mesmo produto soma a quantidade | RF05 |
| T16 | Produto inativo não pode ser adicionado | RF03, RF05 |
| T17 | Pedido novo nasce Aberto, com desconto 0 e data de hoje | RN10 |

### 1.2 Unitários – leitura de valores (`ConversaoDecimalTests`, 9 execuções)

| ID | O que verifica | Regra |
|---|---|---|
| T18 | "45,90", "45.90", "1.234,56", "1.234", "10", "R$ 32,90" e "2.5" são lidos corretamente | RNF05 |
| T18b | "abc" e "12,3,4" são recusados | RNF05 |

### 1.3 Integração com o banco SQLite (`BancoDeDadosTests`, 6 execuções)

| ID | O que verifica | Regra |
|---|---|---|
| T12 | Pedido gravado volta do banco com 3 itens e total 842,40 | RN04 |
| T13 | O banco impede excluir cliente que tem pedido | RN08 |
| T13b | O banco impede CNPJ/CPF repetido | RN01 |
| T13c | Excluir o pedido exclui os itens (cascata) | Integridade referencial |
| T13d | Mudança de status é gravada no banco | RN06 |
| T13e | Dados de exemplo são criados uma única vez | – |

### 1.4 Integração das telas – requisições HTTP (`IntegracaoWebTests`, 8 execuções)

| ID | O que verifica | Regra |
|---|---|---|
| T09 | /Clientes, /Produtos, /Pedidos e /Pedidos/Details/1 sem login levam ao login | RNF04 |
| T10 | Página inicial responde e mostra "PedidoFácil" | – |
| T11 | Tela de login abre com os campos E-mail e Senha | RF01 |
| T11b | Tela de criar conta abre com o campo Confirmar senha | RF01 |
| T19 | Endereço inexistente mostra a página amigável "Página não encontrada" (404) | RNF05 |

---

## 2. Casos de teste funcionais (sistema) – CT01 a CT27

Executados pelo navegador, com o sistema rodando, seguindo passos e resultados esperados definidos antes da execução.

| ID | Caso | Requisitos |
|---|---|---|
| CT01 | Abrir /Clientes sem login leva à tela de login | RNF04 |
| CT02 | Criar conta: senhas diferentes (MSG34) e depois sucesso (MSG30) com o painel 5 · 9 · 1 · 1 | RF01 |
| CT03 | Login com senha errada (MSG10) e depois certa | RF01 |
| CT04 | Cadastrar cliente com máscaras de CNPJ e telefone (MSG01; alerta some em ~4 s) | RF02 |
| CT05 | CNPJ repetido (MSG02) | RN01 |
| CT06 | Nome vazio (MSG16) | RF02 |
| CT07 | Busca "silva" mostra só 2 clientes | RF02 |
| CT08 | Excluir cliente com pedidos é bloqueado (MSG03) | RN08 |
| CT09 | Excluir cliente sem pedidos (MSG18) | RF02 |
| CT10 | Produto com preço 0 (MSG11) | RN02 |
| CT11 | Produto com preço "15.90" é gravado como R$ 15,90 (MSG23) | RF03 |
| CT12 | Código de produto repetido (MSG12) | RF03 |
| CT13 | Excluir produto já vendido é bloqueado (MSG26) | Integridade referencial |
| CT14 | Novo pedido sem cliente (MSG36) e com desconto 35% (MSG04) | RF04, RN03 |
| CT15 | Criar pedido: nasce Aberto com a data de hoje (MSG22) | RF04, RN10 |
| CT16 | Aprovar pedido sem itens (MSG05) | RN07 |
| CT17 | Subtotal ao vivo (R$ 74,70) e item adicionado (MSG19) | RF05, RF06 |
| CT18 | Quantidade 0 é bloqueada | RN02 |
| CT19 | Mesmo produto de novo soma a quantidade (5 un., R$ 124,50) | RF05 |
| CT20 | Desconto 10% (R$ 112,05, MSG33) e 40% recusado (MSG04) | RF06, RN03 |
| CT21 | Aprovar: itens e desconto bloqueados, botões Faturar/Cancelar | RF07, RN05, RN06 |
| CT22 | Faturar: nenhum botão de ação (MSG06) | RF07, RN06 |
| CT23 | Filtro por status "Faturado" | RF08 |
| CT24 | Cálculo do pedido de exemplo: 936,00 − 10% = 842,40 | RN04 |
| CT25 | Página inexistente mostra página amigável (MSG09) | RNF05 |
| CT26 | Layout em tela de celular (390 × 844): menu ☰, sem rolagem lateral da página | RNF01 |
| CT27 | Sair e abrir /Pedidos leva ao login | RF01, RNF04 |

### Casos de teste manuais (feitos pelo desenvolvedor)

Usam janelas de confirmação do navegador, por isso não entram na execução automatizada pelo navegador.

| ID | Caso | Passos | Resultado esperado | Requisitos |
|---|---|---|---|---|
| CTM01 | Remover item com confirmação | Pedido Aberto → Remover → Cancelar na confirmação; repetir → OK | Ao cancelar nada muda; ao confirmar o item sai, o total é recalculado e aparece MSG32 | RF05, IHC "reversível" |
| CTM02 | Cancelar pedido com confirmação | Pedido Aberto ou Aprovado → Cancelar pedido → Cancelar na confirmação; repetir → OK | Ao cancelar nada muda; ao confirmar o status vira Cancelado e nenhum botão de ação aparece (MSG14, MSG20) | RF07, RN06 |
| CTM03 | Uso em celular real | Abrir o endereço publicado em um celular | Menu ☰ e telas utilizáveis com o dedo | RNF01 |

---

## 3. Matriz de rastreabilidade (requisito × teste)

| Requisito | Testes automatizados | Casos funcionais |
|---|---|---|
| RF01 Acessar o sistema | T11, T11b | CT02, CT03, CT27 |
| RF02 Manter clientes | T08, T08b, T13b | CT04, CT05, CT06, CT07, CT09 |
| RF03 Manter produtos | T08c, T16 | CT10, CT11, CT12, CT13 |
| RF04 Registrar pedido | T17 | CT14, CT15 |
| RF05 Adicionar/remover itens | T07, T14, T15, T16 | CT17, CT18, CT19, CTM01 |
| RF06 Calcular total com desconto | T01, T02, T12, T13e | CT17, CT20, CT24 |
| RF07 Alterar status | T05, T06, T06b, T06c, T06d, T13d | CT16, CT21, CT22, CTM02 |
| RF08 Listar pedidos com filtro | – | CT23 |
| RN01 CNPJ/CPF obrigatório e único | T08, T08b, T13b | CT05 |
| RN02 Preço > 0 e quantidade ≥ 1 | T07, T08c | CT10, CT18 |
| RN03 Desconto de 0% a 30% | T03, T03b | CT14, CT20 |
| RN04 Cálculo do total | T01, T02, T12 | CT17, CT19, CT20, CT24 |
| RN05 Só pedido Aberto é editável | T04 | CT21, CT22 |
| RN06 Transições de status | T06, T06b, T06c, T06d, T13d | CT21, CT22, CTM02 |
| RN07 Aprovar só com itens | T05 | CT16 |
| RN08 Cliente com pedidos não é excluído | T13 | CT08 |
| RN09 Preço histórico no item | T14 | – |
| RN10 Valores padrão do pedido novo | T17 | CT15 |
| RNF01 Responsivo | – | CT26, CTM03 |
| RNF04 Login obrigatório | T09 | CT01, CT27 |
| RNF05 Mensagens claras, sem erro técnico | T18, T18b, T19 | CT06, CT25 |
| RNF06 Desempenho (até 2 s com 1.000 registros) | – | Não testado (ver pendências no relatório) |
| RNF07 Testes automatizados | Todos (51) | – |
