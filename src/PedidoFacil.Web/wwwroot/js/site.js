// PedidoFácil – comportamentos da interface (front-end em JavaScript)
(function () {
    "use strict";

    const formatoMoeda = new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" });

    // (a) Confirmação antes de ações que não podem ser desfeitas (MSG07 / MSG14)
    document.querySelectorAll("form[data-confirmar]").forEach(function (form) {
        form.addEventListener("submit", function (evento) {
            if (!window.confirm(form.dataset.confirmar)) {
                evento.preventDefault();
            }
        });
    });

    // (b) Subtotal calculado na tela enquanto o usuário escolhe o produto e digita a quantidade
    const produto = document.getElementById("produtoId");
    const quantidade = document.getElementById("quantidade");
    const subtotal = document.getElementById("subtotal-previa");
    function atualizarSubtotal() {
        if (!produto || !quantidade || !subtotal) return;
        const opcao = produto.options[produto.selectedIndex];
        const preco = opcao ? parseFloat(opcao.dataset.preco || "0") : 0;
        const qtd = parseInt(quantidade.value, 10);
        subtotal.textContent = formatoMoeda.format(preco * (qtd > 0 ? qtd : 0));
    }
    if (produto && quantidade) {
        produto.addEventListener("change", atualizarSubtotal);
        quantidade.addEventListener("input", atualizarSubtotal);
        atualizarSubtotal();
    }

    // (c) Máscaras de CPF/CNPJ e telefone
    function somenteDigitos(valor) {
        return valor.replace(/\D/g, "");
    }

    function mascaraDocumento(valor) {
        const d = somenteDigitos(valor).slice(0, 14);
        if (d.length <= 11) {
            // CPF: 000.000.000-00
            return d.replace(/(\d{3})(\d)/, "$1.$2")
                    .replace(/(\d{3})(\d)/, "$1.$2")
                    .replace(/(\d{3})(\d{1,2})$/, "$1-$2");
        }
        // CNPJ: 00.000.000/0000-00
        return d.replace(/^(\d{2})(\d)/, "$1.$2")
                .replace(/^(\d{2})\.(\d{3})(\d)/, "$1.$2.$3")
                .replace(/\.(\d{3})(\d)/, ".$1/$2")
                .replace(/(\d{4})(\d)/, "$1-$2");
    }

    function mascaraTelefone(valor) {
        const d = somenteDigitos(valor).slice(0, 11);
        if (d.length <= 2) return d.length ? "(" + d : "";
        if (d.length <= 6) return "(" + d.slice(0, 2) + ") " + d.slice(2);
        if (d.length <= 10) return "(" + d.slice(0, 2) + ") " + d.slice(2, 6) + "-" + d.slice(6);
        return "(" + d.slice(0, 2) + ") " + d.slice(2, 7) + "-" + d.slice(7);
    }

    document.querySelectorAll(".mascara-documento").forEach(function (campo) {
        campo.addEventListener("input", function () { campo.value = mascaraDocumento(campo.value); });
    });
    document.querySelectorAll(".mascara-telefone").forEach(function (campo) {
        campo.addEventListener("input", function () { campo.value = mascaraTelefone(campo.value); });
    });

    // (d) Alertas de sucesso somem sozinhos após 4 segundos
    document.querySelectorAll(".alerta-temporario").forEach(function (alerta) {
        setTimeout(function () {
            if (window.bootstrap && bootstrap.Alert) {
                bootstrap.Alert.getOrCreateInstance(alerta).close();
            } else {
                alerta.remove();
            }
        }, 4000);
    });

    // Disponível para os testes do front-end (tests/front-end)
    window.PedidoFacil = { mascaraDocumento: mascaraDocumento, mascaraTelefone: mascaraTelefone };
})();
