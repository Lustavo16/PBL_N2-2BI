var min = 0;
var max = 0;
var media = 0;
var graficoTempoReal = null;
var graficoHistorico = null;
var valoresTemp;
var dadosDashBoard;
var valoresLum;
var valoresUmid;
var table;
var ipRequisicao = "18.207.213.221";

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

    const init = async function () {
        await Promise.all([
            dashboard1.consultaTemperatura(),
            dashboard1.consultaUmidade(),
            dashboard1.consultaLuminosidade()
        ]);
    }

    const onChangeCombo = function () {
        const valorCombo = document.getElementById('tipoDado').value;
        document.getElementById("titulo").textContent = "Histórico de " + valorCombo

        var corStatus;

        if (valorCombo == "Temperatura") {
            let status = "Normal";
            document.getElementById("statusTemp").innerText = `Status: ${status}`;

            let valorAtual = 0;

            if (valoresTemp)
                valorAtual = valoresTemp.length > 0 ? valoresTemp[valoresTemp.length - 1].attrValue : '';

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

            document.getElementById("tipoValor").innerText = "Temperatura Cº";

            if (status == "Normal")
                corStatus = 'steelblue';
            else if (status == "Em alerta")
                corStatus = 'yellow';
            else
                corStatus = 'red';

            graficoTemperatura(corStatus);
            montarTabelaRegistros(valoresTemp);
        }
        else if (valorCombo == "Umidade") {
            let status = "Normal";
            document.getElementById("statusTemp").innerText = `Status: ${status}`;

            let valorAtual = 0;

            if (valoresUmid)
                valorAtual = valoresUmid.length > 0 ? valoresUmid[valoresUmid.length - 1].attrValue : '';

            document.getElementById("tempAtual").innerText = `${valorAtual} %`;

            if (valorAtual >= 45 || valorAtual == 41) {
                status = "Em alerta"
                document.getElementById("statusTemp").innerText = `Status: ${status}`;
            }
            else if (valorAtual >= 48 || valorAtual == 40) {
                status = "Crítico"
                document.getElementById("statusTemp").innerText = `Status: ${status}`;
            }

            document.getElementById("tipoValor").innerText = "Umidade %";

            if (status == "Normal")
                corStatus = 'steelblue';
            else if (status == "Em alerta")
                corStatus = 'yellow';
            else
                corStatus = 'red';

            graficoUmidade(corStatus);
            montarTabelaRegistros(valoresUmid);
        }
        else if (valorCombo == "Luminosidade") {
            let status = "Normal";
            document.getElementById("statusTemp").innerText = `Status: ${status}`;

            let valorAtual = 0;

            if (valoresLum)
                valorAtual = valoresLum.length > 0 ? valoresLum[valoresLum.length - 1].attrValue : '';

            document.getElementById("tempAtual").innerText = `${valorAtual} %`;

            if (valorAtual >= 45 || valorAtual == 41) {
                status = "Em alerta"
                document.getElementById("statusTemp").innerText = `Status: ${status}`;
            }
            else if (valorAtual >= 48 || valorAtual == 40) {
                status = "Crítico"
                document.getElementById("statusTemp").innerText = `Status: ${status}`;
            }

            document.getElementById("tipoValor").innerText = "Luminosidade %";

            if (status == "Normal")
                corStatus = 'steelblue';
            else if (status == "Em alerta")
                corStatus = 'yellow';
            else
                corStatus = 'red';

            graficoLuminosidade(corStatus);
            montarTabelaRegistros(valoresLum);
        }
        else if (valorCombo == "Geral") {
            graficoGeral();
        }
    }

    const graficoTemperatura = function (cor = 'steelblue') {

        if (valoresTemp) {
            const labels = valoresTemp.map(p => new Date(p.recvTime).toLocaleString("pt-BR"));
            const valores = valoresTemp.map(p => p.attrValue);

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
                        borderColor: cor,
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
    }

    const graficoUmidade = function (cor = 'steelblue') {

        if (valoresUmid) {
            const labels = valoresUmid.map(p => new Date(p.recvTime).toLocaleString("pt-BR"));
            const valores = valoresUmid.map(p => p.attrValue);

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
                        borderColor: cor,
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
    }

    const graficoLuminosidade = function (cor = 'steelblue') {

        if (valoresLum) {
            const labels = valoresLum.map(p => new Date(p.recvTime).toLocaleString("pt-BR"));
            const valores = valoresLum.map(p => p.attrValue);

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
                        borderColor: cor,
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

    const consultaLuminosidade = async function () {
        await new Promise(resolve =>
            $.ajax({
                url: "/Dashboard/ObterDadosDispositivo",
                data: {
                    "ip": ipRequisicao,
                    "tipoSensor": "Lamp",
                    "idSensor": "urn:ngsi-ld:Lamp:001",
                    "atributo": "luminosity",
                    "quantidadeValores": "15"
                },
                method: "GET"
            }).done(function (response) {
                if (response) {
                    valoresLum = response;

                    if ($('#tipoDado').val() == "Luminosidade")
                        montarTabelaRegistros(response)

                    onChangeCombo();
                }
                resolve();
            })
        );
    }

    const consultaTemperatura = async function () {
        await new Promise(resolve =>
            $.ajax({
                url: "/Dashboard/ObterDadosDispositivo",
                data: {
                    "ip": ipRequisicao,
                    "tipoSensor": "Dht",
                    "idSensor": "urn:ngsi-ld:Dht:002",
                    "atributo": "humidity",
                    "quantidadeValores": "15"
                },
                method: "GET"
            }).done(function (response) {
                if (response) {
                    valoresTemp = response.slice(-15);

                    if ($('#tipoDado').val() == "Temperatura")
                        montarTabelaRegistros(response)

                    onChangeCombo();
                }
                resolve();
            })
        );
    }

    const consultaUmidade = async function () {
        await new Promise(resolve =>
            $.ajax({
                url: "/Dashboard/ObterDadosDispositivo",
                data: {
                    "ip": ipRequisicao,
                    "tipoSensor": "Dht",
                    "idSensor": "urn:ngsi-ld:Dht:002",
                    "atributo": "temperature",
                    "quantidadeValores": "15"
                },
                method: "GET"
            }).done(function (response) {
                if (response) {
                    valoresUmid = response.slice(-15);

                    if ($('#tipoDado').val() == "Umidade")
                        montarTabelaRegistros(response)

                    onChangeCombo();
                }
                resolve();
            })
        );
    }

    const montarTabelaRegistros = function (registros) {

        if (table && registros) {
            table.clear();

            registros.forEach(registro => {
                table.row.add([
                    new Date(registro.recvTime).toLocaleString("pt-BR"),
                    registro.attrValue,
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
        consultaTemperatura: consultaTemperatura,
        consultaLuminosidade: consultaLuminosidade,
        consultaUmidade: consultaUmidade,
        init: init,
    }
}();

var loginSection = function () {
    const cadastrarSenha = function () {
        var login = $('#login').val()

        window.location.href = `/Login/CadastrarSenha?loginUsuario=${login}`
    }

    const verificaLogin = function () {
        var login = $('#login').val()

        $.ajax({
            url: `/Login/VerificarExistenciaLogin`,
            data: {
                "loginUsuario": login
            },
            method: "GET"
        }).done(function (response) {
            if (response) {
                confirmarCadastrarSenha()
            }
        });
    }

    const confirmarCadastrarSenha = function () {
        Swal.fire({
            title: 'Cadastrar Senha',
            text: "Você ainda não cadastrou uma senha, deseja cadastrá-la agora?",
            icon: 'warning',
            showCancelButton: true,
            width: '600px',
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Sim!',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                cadastrarSenha();
            }
        });
    }

    return {
        confirmarCadastrarSenha: confirmarCadastrarSenha,
        verificaLogin: verificaLogin,
        cadastrarSenha: cadastrarSenha,
    }
}();