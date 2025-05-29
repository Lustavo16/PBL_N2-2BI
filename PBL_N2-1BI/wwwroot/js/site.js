var min = 0;
var max = 0;
var media = 0;
var graficoTempoReal = null;
var graficoHistorico = null;
var valoresTemp = [];
var dadosDashBoard;
var table;
var ipRequisicao = "35.171.156.216";
var valorTempAtual = 41;
var contador = 0;
var listaTemps = [];

var dashboard2 = function () {

    const carregarDados = function () {
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

        var url = `/Dashboard/Dashboard2?dataInicio=${dataIncio}&dataFim=${dataFim}`

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
        var corStatus;

        let valorAtual = 0;

        if (valoresTemp)
            valorAtual = valoresTemp.length > 0 ? valoresTemp[valoresTemp.length - 1].attrValue : '';

        graficoTemperatura();
        montarTabelaRegistros(valoresTemp);
    }

    const init = async function () {
        await consultaUltimaTemperatura();
    };init

    const graficoGauge = function (tempAtual) {
        const ctx = document.getElementById('gaugeChart').getContext('2d');

        // Gradiente do verde para o vermelho
        const gradient = ctx.createLinearGradient(0, 0, 300, 0);
        gradient.addColorStop(0, 'green');
        gradient.addColorStop(0.5, 'yellow');
        gradient.addColorStop(1, 'red');

        // Destroi gráfico anterior
        if (window.gauge) window.gauge.destroy();

        // Plugin para escrever texto no centro
        const centerText = {
            id: 'centerText',
            afterDraw(chart) {
                const { ctx, chartArea: { width, height } } = chart;

                ctx.save();
                ctx.font = 'bold 24px Arial';
                ctx.fillStyle = 'black';
                ctx.textAlign = 'center';
                ctx.textBaseline = 'middle';
                ctx.fillText(`${tempAtual}°C`, width / 2, height / 2);
            }
        };

        window.gauge = new Chart(ctx, {
            type: 'doughnut',
            data: {
                datasets: [{
                    data: [tempAtual, 100 - tempAtual],
                    backgroundColor: [gradient, '#e0e0e0'],
                    borderWidth: 0,
                    circumference: 360,
                    rotation: -90,
                    cutout: '70%'
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { display: false },
                    tooltip: { enabled: false }
                }
            },
            plugins: [centerText]
        });
    };

    const graficoTemperatura = function () {

        if (valoresTemp) {
            const labels = valoresTemp.map(p => new Date(p.recvTime).toLocaleString("pt-BR"));
            const valores = valoresTemp.map(p => p.attrValue);

            if (valoresTemp.length > 0)
                graficoGauge(valoresTemp[valoresTemp.length - 1].attrValue);
            else
                graficoGauge(0);

            let setpoint = 35.00;
            let K = 0.8144;

            const valoresErro = valores.map(p => {
                let A = setpoint - p;
                A = A < 0 ? A * -1 : A;

                return A / 1 + K;
            })

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
        }
    }

    const consultaAtualiza = function () {

        $.ajax({
            url: "/Dashboard/ConsultaRegistros",
            method: "GET"
        }).done(function (response) {
            if (response) {
                montarTabelaRegistros(response);
                valoresTemp = response;
            }
            carregarDados();
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
                    "quantidadeValores": "30"
                },
                method: "GET"
            }).done(function (response) {
                if (response) {
                    valoresTemp = response;
                    montarTabelaRegistros(response)

                    carregarDados();
                }
                resolve();
            })
        );
    }

    const consultaUltimaTemperatura = async function () {
        await new Promise(resolve =>
            $.ajax({
                url: "/Dashboard/ObterDadosDispositivo",
                data: {
                    "ip": ipRequisicao,
                    "tipoSensor": "Temp",
                    "idSensor": "urn:ngsi-ld:Temp:001",
                    "atributo": "temperature",
                    "quantidadeValores": "1"
                },
                method: "GET"
            }).done(function (response) {
                if (response) {
                    valoresTemp.push(response[0]);
                    montarTabelaRegistros(valoresTemp)

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
        carregarDados: carregarDados,
        graficoTemperatura: graficoTemperatura,
        consultaAtualiza: consultaAtualiza,
        montarTabelaRegistros: montarTabelaRegistros,
        consultaTemperatura: consultaTemperatura,
        graficoGauge: graficoGauge,
        consultaUltimaTemperatura: consultaUltimaTemperatura,
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