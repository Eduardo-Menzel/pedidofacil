// Faz a validação do navegador aceitar números no formato brasileiro (45,90 · 1.234,56)
// e também com ponto decimal (45.90) – mesma regra usada no servidor (DecimalBrasilModelBinder).
(function ($) {
    "use strict";
    if (!$ || !$.validator) return;

    function paraNumero(valor) {
        let texto = String(valor).replace("R$", "").replace(/\s/g, "");
        if (texto.indexOf(",") >= 0) {
            texto = texto.replace(/\./g, "").replace(",", ".");
        } else {
            const ultimoPonto = texto.lastIndexOf(".");
            if (ultimoPonto >= 0 && texto.length - ultimoPonto - 1 === 3) {
                texto = texto.replace(/\./g, ""); // 1.234 = mil duzentos e trinta e quatro
            }
        }
        return /^-?\d+(\.\d+)?$/.test(texto) ? Number(texto) : NaN;
    }

    $.validator.methods.number = function (valor, elemento) {
        return this.optional(elemento) || !isNaN(paraNumero(valor));
    };

    $.validator.methods.range = function (valor, elemento, limites) {
        if (this.optional(elemento)) return true;
        const numero = paraNumero(valor);
        return !isNaN(numero) && numero >= Number(limites[0]) && numero <= Number(limites[1]);
    };
})(window.jQuery);
