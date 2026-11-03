# Registro de Correções e Melhorias (Changelog)

## Versão 1.0.1 – 03/11/2026

**Correção**
- D01 – O campo UF impedia salvar o cliente na validação do navegador. A regra de tamanho foi trocada por "duas letras maiúsculas" (`Models/Cliente.cs`). Encontrado no caso de teste CT04.

## Versão 1.0 – 03/11/2026

Primeira versão completa: login, clientes, produtos, pedidos com itens, desconto e status, 51 testes automatizados e documentação.

---

## Sugestões recebidas

| # | Sugestão | Origem | Decisão | Justificativa | Versão |
|---|---|---|---|---|---|
| S01 | Nas tabelas, deixar as ações (Editar, Detalhes, Excluir) acessíveis no celular sem rolar para o lado | Casos de teste (CT26) | _a decidir_ | | |
| S02 | Mostrar a mensagem do sistema ("A quantidade deve ser maior que zero.") em vez do aviso padrão do navegador | Casos de teste (CT18) | _a decidir_ | | |
| S03 | Avaliar se a cidade deve ser obrigatória no cadastro de cliente | Casos de teste (CT04) | _a decidir_ | | |

As sugestões dos 5 avaliadores serão acrescentadas nesta tabela.
