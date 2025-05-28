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
var ipRequisicao = "35.171.156.216";

var dashboard2 = function () {

    const carregarDados = function () {
        const valorCombo = document.getElementById('tipoDado').value;
        document.getElementById("titulo").textContent = "Histórico de " + valorCombo

        graficoTemperatura();
        document.querySelector('.val-min').innerHTML = `Mínima: <span id="minTemp">${min}</span> ºC`;
        document.querySelector('.val-max').innerHTML = `Máxima: <span id="maxTemp">${max}</span> ºC`;
        document.querySelector('.val-mid').innerHTML = `Média: <span id="mediaTemp">${media}</span> ºC`;
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

        if (valoresTemp) {
            const labels = valoresTemp.map(p => new Date(p.recvTime).toLocaleString("pt-BR"));
            const valores = valoresTemp.map(p => p.attrValue);

            if (graficoTempoReal) {
                graficoTempoReal.destroy();
            }

            const ctx = document.getElementById('graficoHistorico').getContext('2d');
            graficoTempoReal = new Chart(ctx, {
                type: 'line',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Temperatura (°C)',
                        data: valores,
                        borderColor: 'steelblue',
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
        }


        document.getElementById("minTemp").innerText = min;
        document.getElementById("maxTemp").innerText = max;
        document.getElementById("mediaTemp").innerText = media;
    }

    const consultaTemperatura = async function () {
        await new Promise(resolve =>
            $.ajax({
                url: "/Dashboard/ObterDadosDispositivo",
                data: {
                    "ip": ipRequisicao,
                    "tipoSensor": "Temp",
                    "idSensor": "urn:ngsi-ld:Temp:001",
                    "atributo": "temperature",
                    "quantidadeValores": "100"
                },
                method: "GET"
            }).done(function (response) {
                if (response) {
                    valoresTemp = response.slice(-15);

                    montarTabelaRegistros(response)

                    carregarDados();
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

    return {
        filtrar: filtrar,
        carregarDados: carregarDados,
        consultaTemperatura: consultaTemperatura,
        graficoTemperatura: graficoTemperatura,
        montarTabelaRegistros: montarTabelaRegistros,
    }
}();

var dashboard1 = function () {

    const carregarDados = function () {
        const valorCombo = document.getElementById('tipoDado').value;
        document.getElementById("titulo").textContent = "Histórico de " + valorCombo

        var corStatus;

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

    const consultaTemperatura = async function () {
        await new Promise(resolve =>
            $.ajax({
                url: "/Dashboard/ObterDadosDispositivo",
                data: {
                    "ip": ipRequisicao,
                    "tipoSensor": "Temp",
                    "idSensor": "urn:ngsi-ld:Temp:001",
                    "atributo": "temperature",
                    "quantidadeValores": "15"
                },
                method: "GET"
            }).done(function (response) {
                if (response) {
                    valoresTemp = response.slice(-15);

                    if ($('#tipoDado').val() == "Temperatura")
                        montarTabelaRegistros(response)

                    carregarDados();
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
        carregarDados: carregarDados,
        graficoTemperatura: graficoTemperatura,
        consultaAtualiza: consultaAtualiza,
        recarregarTabela: recarregarTabela,
        montarTabelaRegistros: montarTabelaRegistros,
        consultaTemperatura: consultaTemperatura,
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