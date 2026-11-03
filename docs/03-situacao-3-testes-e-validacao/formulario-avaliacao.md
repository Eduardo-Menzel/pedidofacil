# Formulário de Avaliação (Laudo de Teste) – PedidoFácil

Modelo usado pelos 5 avaliadores ("dublês de usuário") para testar o sistema publicado e registrar o resultado. Aplicado pelo Google Forms. Contém, no mínimo, os itens pedidos pela disciplina: nome de quem testou, data, o que funcionou, o que não funcionou e o que não foi testado.

## Texto de abertura do formulário

> **Avaliação do sistema PedidoFácil**
>
> Olá! O PedidoFácil é um sistema web de controle de pedidos de venda, feito para o Projeto Integrador II de Engenharia de Software (Cruzeiro do Sul Virtual). Obrigado por ajudar a testar!
>
> **Endereço do sistema:** http://pedidofacil.runasp.net
> **Tempo estimado:** 15 minutos. Todos os dados do sistema são fictícios; não use dados reais.
>
> **Roteiro sugerido de tarefas**
> 1. Clique em **Criar conta** e cadastre-se com um e-mail e uma senha de pelo menos 6 caracteres (pode ser um e-mail inventado).
> 2. Em **Clientes**, cadastre um cliente novo e use a busca por nome.
> 3. Em **Produtos**, cadastre um produto novo.
> 4. Em **Pedidos**, crie um pedido, adicione 2 produtos diferentes e aplique 10% de desconto. Confira se o total está certo.
> 5. Aprove o pedido e depois fature.
> 6. Tente fazer algo "errado": aprovar um pedido sem itens, dar 40% de desconto ou excluir um cliente que já tem pedidos.
> 7. Se puder, abra o sistema também no celular.
>
> Fique à vontade para testar outras coisas. Se encontrar um erro, tire um print da tela.

## Perguntas

| # | Pergunta | Tipo no Google Forms | Obrigatória |
|---|---|---|---|
| 1 | Nome completo | Resposta curta | Sim |
| 2 | Você é: | Múltipla escolha: Colega de curso · Profissional da área de TI · Outro | Sim |
| 3 | Data do teste | Data | Sim |
| 4 | Dispositivo e navegador usados (ex.: notebook com Chrome, celular Android com Chrome) | Resposta curta | Sim |
| 5 | O que você testou e **funcionou**? Descreva as funcionalidades. | Parágrafo | Sim |
| 6 | O que você testou e **não funcionou**? Descreva o problema, o que você fez antes de ele acontecer e o que deveria ser corrigido. (Se tudo funcionou, escreva "Nada".) | Parágrafo | Sim |
| 7 | Alguma funcionalidade você **não testou**? Qual e por quê? (Também vale citar algo que existe no sistema mas você não conseguiu realizar.) | Parágrafo | Sim |
| 8 | Sugestões de melhoria (aparência, facilidade de uso, textos, novas funções) | Parágrafo | Não |
| 9 | Prints das telas com erro | Upload de arquivo (imagens, até 5 arquivos) | Não |
| 10 | De 1 a 5, quão fácil foi usar o sistema? | Escala linear 1 (muito difícil) a 5 (muito fácil) | Não |
| 11 | Autorizo o uso destas respostas, com meu nome, no relatório acadêmico do projeto. | Caixa de seleção | Sim |

**Observação sobre a pergunta 9:** no Google Forms, o campo de upload exige que o avaliador entre com uma conta Google. Quem não tiver conta pode enviar os prints por WhatsApp ou e-mail, que serão anexados ao relatório.

## Mensagem-convite

> Oi, [nome]! Tudo bem? Estou fazendo o Projeto Integrador II de Engenharia de Software e preciso que 5 pessoas testem o sistema que eu desenvolvi, um controle de pedidos de venda. Leva uns 15 minutos.
> Sistema: http://pedidofacil.runasp.net
> Formulário (com o roteiro do teste): [link do formulário]
> Consegue testar até [data]? Pode ser bem sincero(a) no que achar de errado, isso me ajuda bastante. Obrigado!

## Como as respostas são usadas

1. As respostas são exportadas do Google Forms (uma página por avaliador, com os prints) para o arquivo `avaliacoes-pares.pdf`.
2. Cada problema e sugestão entra na tabela de [changelog-melhorias.md](changelog-melhorias.md), com a decisão (aceita ou recusada) e a justificativa.
3. O resumo das avaliações entra na seção 4 do [relatório de testes](relatorio-de-testes.md).
