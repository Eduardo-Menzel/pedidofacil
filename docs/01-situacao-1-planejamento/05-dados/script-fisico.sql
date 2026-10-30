-- =============================================================================
--  PedidoFácil – Projeto físico do banco de dados
--  SGBD: SQLite 3
--  Projeto Integrador Transdisciplinar em Engenharia de Software II
--  Aluno: Eduardo Menzel                                   Versão 1.0 – 29/10/2026
-- -----------------------------------------------------------------------------
--  Observações
--  * Este script é o projeto físico das tabelas de NEGÓCIO. Na Situação 2 ele
--    será substituído pelo script gerado automaticamente pelo Entity Framework
--    Core (dotnet ef migrations script), que também cria as tabelas AspNet*
--    do ASP.NET Core Identity (usuários, senhas com hash, perfis).
--  * Valores monetários e percentuais são gravados como TEXT, que é como o
--    Entity Framework Core guarda o tipo decimal no SQLite para não perder
--    precisão. Por isso as regras CHECK convertem o valor com CAST.
--  * As chaves estrangeiras só são verificadas no SQLite com
--    PRAGMA foreign_keys = ON (o Entity Framework Core já liga isso sozinho).
-- =============================================================================

PRAGMA foreign_keys = ON;

-- -----------------------------------------------------------------------------
-- Tabela CLIENTES
-- -----------------------------------------------------------------------------
CREATE TABLE "Clientes" (
    "Id"            INTEGER NOT NULL CONSTRAINT "PK_Clientes" PRIMARY KEY AUTOINCREMENT,
    "Nome"          TEXT    NOT NULL CHECK (length("Nome") BETWEEN 3 AND 100),
    "CnpjCpf"       TEXT    NOT NULL CHECK (length("CnpjCpf") BETWEEN 11 AND 18),   -- RN01
    "Email"         TEXT    NULL     CHECK ("Email" IS NULL OR length("Email") <= 100),
    "Telefone"      TEXT    NULL     CHECK ("Telefone" IS NULL OR length("Telefone") <= 20),
    "Cidade"        TEXT    NULL     CHECK ("Cidade" IS NULL OR length("Cidade") <= 60),
    "UF"            TEXT    NULL     CHECK ("UF" IS NULL OR length("UF") = 2),
    "DataCadastro"  TEXT    NOT NULL DEFAULT (datetime('now'))
);

CREATE UNIQUE INDEX "IX_Clientes_CnpjCpf" ON "Clientes" ("CnpjCpf");             -- RN01
CREATE INDEX "IX_Clientes_Nome" ON "Clientes" ("Nome");                           -- busca por nome (RF02)

-- -----------------------------------------------------------------------------
-- Tabela PRODUTOS
-- -----------------------------------------------------------------------------
CREATE TABLE "Produtos" (
    "Id"             INTEGER NOT NULL CONSTRAINT "PK_Produtos" PRIMARY KEY AUTOINCREMENT,
    "Codigo"         TEXT    NOT NULL CHECK (length("Codigo") BETWEEN 1 AND 30),
    "Descricao"      TEXT    NOT NULL CHECK (length("Descricao") BETWEEN 3 AND 150),
    "PrecoUnitario"  TEXT    NOT NULL CHECK (CAST("PrecoUnitario" AS REAL) > 0),  -- RN02
    "Ativo"          INTEGER NOT NULL DEFAULT 1 CHECK ("Ativo" IN (0, 1))
);

CREATE UNIQUE INDEX "IX_Produtos_Codigo" ON "Produtos" ("Codigo");

-- -----------------------------------------------------------------------------
-- Tabela PEDIDOS
-- Status: 0 = Aberto | 1 = Aprovado | 2 = Faturado | 3 = Cancelado
-- -----------------------------------------------------------------------------
CREATE TABLE "Pedidos" (
    "Id"                  INTEGER NOT NULL CONSTRAINT "PK_Pedidos" PRIMARY KEY AUTOINCREMENT,
    "ClienteId"           INTEGER NOT NULL,
    "DataPedido"          TEXT    NOT NULL DEFAULT (datetime('now')),                        -- RN10
    "Status"              INTEGER NOT NULL DEFAULT 0 CHECK ("Status" BETWEEN 0 AND 3),       -- RN06
    "DescontoPercentual"  TEXT    NOT NULL DEFAULT '0'
                          CHECK (CAST("DescontoPercentual" AS REAL) BETWEEN 0 AND 30),       -- RN03
    "Observacao"          TEXT    NULL CHECK ("Observacao" IS NULL OR length("Observacao") <= 500),
    CONSTRAINT "FK_Pedidos_Clientes_ClienteId" FOREIGN KEY ("ClienteId")
        REFERENCES "Clientes" ("Id") ON DELETE RESTRICT                                     -- RN08
);

CREATE INDEX "IX_Pedidos_ClienteId" ON "Pedidos" ("ClienteId");
CREATE INDEX "IX_Pedidos_Status" ON "Pedidos" ("Status");                                   -- filtro (RF08)

-- -----------------------------------------------------------------------------
-- Tabela ITENSPEDIDO
-- PrecoUnitario = preço do produto no momento da venda (preço histórico – RN09)
-- -----------------------------------------------------------------------------
CREATE TABLE "ItensPedido" (
    "Id"             INTEGER NOT NULL CONSTRAINT "PK_ItensPedido" PRIMARY KEY AUTOINCREMENT,
    "PedidoId"       INTEGER NOT NULL,
    "ProdutoId"      INTEGER NOT NULL,
    "Quantidade"     INTEGER NOT NULL CHECK ("Quantidade" >= 1),                  -- RN02
    "PrecoUnitario"  TEXT    NOT NULL CHECK (CAST("PrecoUnitario" AS REAL) > 0),
    CONSTRAINT "FK_ItensPedido_Pedidos_PedidoId" FOREIGN KEY ("PedidoId")
        REFERENCES "Pedidos" ("Id") ON DELETE CASCADE,                            -- composição
    CONSTRAINT "FK_ItensPedido_Produtos_ProdutoId" FOREIGN KEY ("ProdutoId")
        REFERENCES "Produtos" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_ItensPedido_PedidoId" ON "ItensPedido" ("PedidoId");
CREATE INDEX "IX_ItensPedido_ProdutoId" ON "ItensPedido" ("ProdutoId");

-- -----------------------------------------------------------------------------
-- Visão (consulta) com o total de cada pedido – RN04
-- O total NÃO é gravado em tabela (é calculado), para manter a 3ª forma normal.
-- -----------------------------------------------------------------------------
CREATE VIEW "vw_TotalPedidos" AS
SELECT  p."Id"                                                     AS "PedidoId",
        c."Nome"                                                   AS "Cliente",
        p."DataPedido",
        p."Status",
        ROUND(COALESCE(SUM(i."Quantidade" * CAST(i."PrecoUnitario" AS REAL)), 0), 2) AS "Subtotal",
        CAST(p."DescontoPercentual" AS REAL)                       AS "DescontoPercentual",
        ROUND(COALESCE(SUM(i."Quantidade" * CAST(i."PrecoUnitario" AS REAL)), 0)
              * (1 - CAST(p."DescontoPercentual" AS REAL) / 100), 2) AS "Total"
FROM    "Pedidos" p
JOIN    "Clientes" c       ON c."Id" = p."ClienteId"
LEFT JOIN "ItensPedido" i  ON i."PedidoId" = p."Id"
GROUP BY p."Id", c."Nome", p."DataPedido", p."Status", p."DescontoPercentual";

-- -----------------------------------------------------------------------------
-- Dados de exemplo (FICTÍCIOS) – usados para conferir o script
-- -----------------------------------------------------------------------------
INSERT INTO "Clientes" ("Nome", "CnpjCpf", "Email", "Telefone", "Cidade", "UF") VALUES
    ('Mercado Bom Preço Ltda', '12.345.678/0001-90', 'compras@bompreco.exemplo', '(49) 99999-0001', 'Chapecó', 'SC'),
    ('Padaria Pão Quente ME',  '98.765.432/0001-10', NULL,                       '(49) 99999-0002', 'Xaxim',   'SC');

INSERT INTO "Produtos" ("Codigo", "Descricao", "PrecoUnitario") VALUES
    ('PRD-001', 'Caneta esferográfica azul (cx 50)', '45.00'),
    ('PRD-004', 'Papel A4 75g (resma)',              '32.90'),
    ('PRD-007', 'Grampeador de mesa',                '49.00');

INSERT INTO "Pedidos" ("ClienteId", "DescontoPercentual") VALUES (1, '10');

INSERT INTO "ItensPedido" ("PedidoId", "ProdutoId", "Quantidade", "PrecoUnitario") VALUES
    (1, 1, 4,  '45.00'),
    (1, 2, 20, '32.90'),
    (1, 3, 2,  '49.00');

-- Resultado esperado: Subtotal = 936.00 | Desconto = 10% | Total = 842.40
-- SELECT * FROM "vw_TotalPedidos";
