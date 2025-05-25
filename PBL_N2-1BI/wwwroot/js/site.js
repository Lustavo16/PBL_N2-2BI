var min = 0;
var max = 0;
var media = 0;
var graficoTempoReal = null;
var graficoHistorico = null;
var dadosDashBoard;
var table;

var dashboard2 = function () {

    const onChangeCombo = function () {
        const valorCombo = document.getElementById('tipoDado').value;
        document.getElementById("titulo").textContent = "Histórico de " + valorCombo

        if (valorCombo == "Temperatura") {
            graficoTemperatura();
            document.querySelector('.val-min').innerHTML = `Mínima: <span id="minTemp">${min}</span> ºC`;
            document.querySelector('.val-max').innerHTML = `Máxima: <span id="maxTemp">${max}</span> ºC`;
            document.querySelector('.val-mid').innerHTML = `Média: <span id="mediaTemp">${media}</span> ºC`;
        }
        else if (valorCombo == "Umidade") {
            graficoUmidade();
            document.querySelector('.val-min').innerHTML = `Mínima: <span id="minTemp">${min}</span> %`;
            document.querySelector('.val-max').innerHTML = `Máxima: <span id="maxTemp">${max}</span> %`;
            document.querySelector('.val-mid').innerHTML = `Média: <span id="mediaTemp">${media}</span> %`;
        }
        else if (valorCombo == "Luminosidade") {
            graficoLuminosidade();
            document.querySelector('.val-min').innerHTML = `Mínima: <span id="minTemp">${min}</span> %`;
            document.querySelector('.val-max').innerHTML = `Máxima: <span id="maxTemp">${max}</span> %`;
            document.querySelector('.val-mid').innerHTML = `Média: <span id="mediaTemp">${media}</span> %`;
        }
        else if (valorCombo == "Geral") {
            graficoGeral();
            document.querySelector('.val-min').innerHTML = `Mínima: <span id="minTemp">${min}</span> %`;
            document.querySelector('.val-max').innerHTML = `Máxima: <span id="maxTemp">${max}</span> %`;
            document.querySelector('.val-mid').innerHTML = `Média: <span id="mediaTemp">${media}</span> %`;
        }
    }

    const filtrar = function () {
        console.log("teste");
        var dataIncio = $("#DataInicio").val();
        var dataFim = $("#DataFim").val();
        var tipoDado = $("#tipoDado").val();

        var url = `/Dashboard/Dashboard2?dataInicio=${dataIncio}&dataFim=${dataFim}&tipoDado=${tipoDado}`

        window.location.href = url;
    }

    const graficoTemperatura = function () {

        const labels = dadosDashBoard.map(p => new Date(p.dataRegistro).toLocaleString("pt-BR"));
        const valores = dadosDashBoard.map(p => p.valorTemperatura);

        if (graficoHistorico) {
            graficoHistorico.destroy();
        }

        const ctx = document.getElementById('graficoHistorico').getContext('2d');
        graficoHistorico = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Temperatura (°C)',
                    data: valores,
                    borderColor: 'red',
                    borderWidth: 2,
                    fill: false,
                    tension: 0.2
                }],
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { display: true }
                }
            }
        });

        min = Math.min(...valores);
        max = Math.max(...valores);
        media = (valores.reduce((a, b) => a + b, 0) / valores.length).toFixed(2);

        document.getElementById("minTemp").innerText = min;
        document.getElementById("maxTemp").innerText = max;
        document.getElementById("mediaTemp").innerText = media;
    }

    const graficoUmidade = function () {

        const labels = dadosDashBoard.map(p => new Date(p.dataRegistro).toLocaleString("pt-BR"));
        const valores = dadosDashBoard.map(p => p.valorUmidade);

        if (graficoHistorico) {
            graficoHistorico.destroy();
        }

        const ctx = document.getElementById('graficoHistorico').getContext('2d');
        graficoHistorico = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Umidade (%)',
                    data: valores,
                    borderColor: 'steelblue',
                    borderWidth: 2,
                    fill: false,
                    tension: 0.2
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { display: true }
                }
            }
        });

        // Calcular estatísticas
        min = Math.min(...valores);
        max = Math.max(...valores);
        media = (valores.reduce((a, b) => a + b, 0) / valores.length).toFixed(2);

        document.getElementById("minTemp").innerText = min;
        document.getElementById("maxTemp").innerText = max;
        document.getElementById("mediaTemp").innerText = media;
    }

    const graficoLuminosidade = function () {

        const labels = dadosDashBoard.map(p => new Date(p.dataRegistro).toLocaleString("pt-BR"));
        const valores = dadosDashBoard.map(p => p.valorLuminosidade);

        if (graficoHistorico) {
            graficoHistorico.destroy();
        }

        const ctx = document.getElementById('graficoHistorico').getContext('2d');
        graficoHistorico = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Luminosidade (%)',
                    data: valores,
                    borderColor: 'yellow',
                    borderWidth: 2,
                    fill: false,
                    tension: 0.2
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { display: true }
                }
            }
        });

        // Calcular estatísticas
        min = Math.min(...valores);
        max = Math.max(...valores);
        media = (valores.reduce((a, b) => a + b, 0) / valores.length).toFixed(2);

        document.getElementById("minTemp").innerText = min;
        document.getElementById("maxTemp").innerText = max;
        document.getElementById("mediaTemp").innerText = media;
    }

    function graficoGeral() {

        const labels = dadosDashBoard.map(p => new Date(p.dataRegistro).toLocaleString("pt-BR"));
        const valoresTemperatura = dadosDashBoard.map(p => p.valorTemperatura);
        const valoresUmidade = dadosDashBoard.map(p => p.valorUmidade);
        const valoresLuminosidade = dadosDashBoard.map(p => p.valorLuminosidade);

        if (graficoHistorico) {
            graficoHistorico.destroy();
        }

        const ctx = document.getElementById('graficoHistorico').getContext('2d');
        graficoHistorico = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Temperatura (Cº)',
                    data: valoresTemperatura,
                    borderColor: 'red',
                    borderWidth: 2,
                    fill: false,
                    tension: 0.2
                },
                {
                    label: 'Umidade (%)',
                    data: valoresUmidade,
                    borderColor: 'steelblue',
                    borderWidth: 2,
                    fill: false,
                    tension: 0.2
                },
                {
                    label: 'Luminosidade (%)',
                    data: valoresLuminosidade,
                    borderColor: 'yellow',
                    borderWidth: 2,
                    fill: false,
                    tension: 0.2
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { display: true }
                }
            }
        });

        // Calcular estatísticas
        min = Math.min(...valoresLuminosidade);
        max = Math.max(...valoresLuminosidade);
        media = (valoresLuminosidade.reduce((a, b) => a + b, 0) / valoresLuminosidade.length).toFixed(2);
    }

    const montarTabelaRegistros = function (registros) {

        if (table) {
            table.clear();

            registros.forEach(registro => {
                table.row.add([
                    new Date(registro.dataRegistro).toLocaleString("pt-BR"),
                    registro.valorTemperatura,
                    registro.valorUmidade,
                    registro.valorLuminosidade
                ]);
            });

            table.draw();
        }
    }

    return {
        filtrar: filtrar,
        onChangeCombo: onChangeCombo,
        graficoTemperatura: graficoTemperatura,
        graficoUmidade: graficoUmidade,
        graficoLuminosidade: graficoLuminosidade,
        graficoGeral: graficoGeral,
        montarTabelaRegistros: montarTabelaRegistros,
    }
}();

var dashboard1 = function () {

    const onChangeCombo = function () {
        const valorCombo = document.getElementById('tipoDado').value;
        document.getElementById("titulo").textContent = "Histórico de " + valorCombo
        var status = "Normal";

        if (valorCombo == "Temperatura") {
            const valorAtual = dadosDashBoard.length > 0 ? dadosDashBoard[dadosDashBoard.length - 1].valorTemperatura : '';

            document.getElementById("tempAtual").innerText = `${valorAtual} °C`;

            if ((valorAtual >= 45 && valorAtual < 48) || valorAtual == 41) {
                status = "Em alerta"
                document.getElementById("statusTemp").innerText = `Status: ${status}`;
            }
            else if (valorAtual < 45 && valorAtual > 41) {
                status = "Normal"
                document.getElementById("statusTemp").innerText = `Status: ${status}`;
            }
            else if (valorAtual >= 48 || valorAtual == 40) {
                status = "Crítico"
                document.getElementById("statusTemp").innerText = `Status: ${status}`;
            }

            graficoTemperatura();
        }
        else if (valorCombo == "Umidade") {
            const valorAtual = dadosDashBoard.length > 0 ? dadosDashBoard[dadosDashBoard.length - 1].valorUmidade : '';

            document.getElementById("tempAtual").innerText = `${valorAtual} %`;

            if (valorAtual >= 45 || valorAtual == 41) {
                status = "Em alerta"
                document.getElementById("statusTemp").innerText = `Status: ${status}`;
            }
            else if (valorAtual >= 48 || valorAtual == 40) {
                status = "Crítico"
                document.getElementById("statusTemp").innerText = `Status: ${status}`;
            }

            graficoUmidade();
        }
        else if (valorCombo == "Luminosidade") {
            const valorAtual = dadosDashBoard.length > 0 ? dadosDashBoard[dadosDashBoard.length - 1].valorLuminosidade : '';

            document.getElementById("tempAtual").innerText = `${valorAtual} %`;

            if (valorAtual >= 45 || valorAtual == 41) {
                status = "Em alerta"
                document.getElementById("statusTemp").innerText = `Status: ${status}`;
            }
            else if (valorAtual >= 48 || valorAtual == 40) {
                status = "Crítico"
                document.getElementById("statusTemp").innerText = `Status: ${status}`;
            }

            graficoLuminosidade();
        }
        else if (valorCombo == "Geral") {
            graficoGeral();
        }
    }

    const graficoTemperatura = function () {

        const labels = dadosDashBoard.map(p => new Date(p.dataRegistro).toLocaleString("pt-BR"));
        const valores = dadosDashBoard.map(p => p.valorTemperatura);

        if (graficoTempoReal) {
            graficoTempoReal.destroy();
        }

        const ctx = document.getElementById('graficoTempoReal').getContext('2d');
        graficoTempoReal = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Temperatura (°C)',
                    data: valores,
                    borderColor: 'red',
                    borderWidth: 2,
                    fill: false,
                    tension: 0.2
                }],
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { display: true }
                }
            }
        });
    }

    const graficoUmidade = function () {

        const labels = dadosDashBoard.map(p => new Date(p.dataRegistro).toLocaleString("pt-BR"));
        const valores = dadosDashBoard.map(p => p.valorUmidade);

        if (graficoTempoReal) {
            graficoTempoReal.destroy();
        }

        const ctx = document.getElementById('graficoTempoReal').getContext('2d');
        graficoTempoReal = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Umidade (%)',
                    data: valores,
                    borderColor: 'steelblue',
                    borderWidth: 2,
                    fill: false,
                    tension: 0.2
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { display: true }
                }
            }
        });
    }

    const graficoLuminosidade = function () {

        const labels = dadosDashBoard.map(p => new Date(p.dataRegistro).toLocaleString("pt-BR"));
        const valores = dadosDashBoard.map(p => p.valorLuminosidade);

        if (graficoTempoReal) {
            graficoTempoReal.destroy();
        }

        const ctx = document.getElementById('graficoTempoReal').getContext('2d');
        graficoTempoReal = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Luminosidade (%)',
                    data: valores,
                    borderColor: 'yellow',
                    borderWidth: 2,
                    fill: false,
                    tension: 0.2
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { display: true }
                }
            }
        });

        // Calcular estatísticas
        min = Math.min(...valores);
        max = Math.max(...valores);
        media = (valores.reduce((a, b) => a + b, 0) / valores.length).toFixed(2);
    }

    const graficoGeral = function () {

        const labels = dadosDashBoard.map(p => new Date(p.dataRegistro).toLocaleString("pt-BR"));
        const valoresTemperatura = dadosDashBoard.map(p => p.valorTemperatura);
        const valoresUmidade = dadosDashBoard.map(p => p.valorUmidade);
        const valoresLuminosidade = dadosDashBoard.map(p => p.valorLuminosidade);

        if (graficoTempoReal) {
            graficoTempoReal.destroy();
        }

        const ctx = document.getElementById('graficoTempoReal').getContext('2d');
        graficoTempoReal = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Temperatura (Cº)',
                    data: valoresTemperatura,
                    borderColor: 'red',
                    borderWidth: 2,
                    fill: false,
                    tension: 0.2
                },
                {
                    label: 'Umidade (%)',
                    data: valoresUmidade,
                    borderColor: 'steelblue',
                    borderWidth: 2,
                    fill: false,
                    tension: 0.2
                },
                {
                    label: 'Luminosidade (%)',
                    data: valoresLuminosidade,
                    borderColor: 'yellow',
                    borderWidth: 2,
                    fill: false,
                    tension: 0.2
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { display: true }
                }
            }
        });
    }

    const consultaAtualiza = function () {

        $.ajax({
            url: "/Dashboard/ConsultaRegistros",
            method: "GET"
        }).done(function (response) {
            if (response) {
                montarTabelaRegistros(response);
                dadosDashBoard = response.slice(-15);
            }
            onChangeCombo();
        });
    }

    const montarTabelaRegistros = function (registros) {

        if (table) {
            table.clear();

            registros.forEach(registro => {
                table.row.add([
                    new Date(registro.dataRegistro).toLocaleString("pt-BR"),
                    registro.valorTemperatura,
                    registro.valorUmidade,
                    registro.valorLuminosidade
                ]);
            });

            table.draw();
        }
    }

    const recarregarTabela = function () {

        $('#tabelaRegistros').ajax.reload();
    }

    return {
        onChangeCombo: onChangeCombo,
        graficoTemperatura: graficoTemperatura,
        graficoUmidade: graficoUmidade,
        graficoLuminosidade: graficoLuminosidade,
        graficoGeral: graficoGeral,
        consultaAtualiza: consultaAtualiza,
        recarregarTabela: recarregarTabela,
        montarTabelaRegistros: montarTabelaRegistros,
    }
}();