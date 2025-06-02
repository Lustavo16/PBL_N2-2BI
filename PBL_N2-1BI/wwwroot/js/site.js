var min = 0;
var max = 0;
var media = 0;
var graficoTempoReal = null;
var graficoHistorico = null;
var valoresTemp = [];
var table;
var ipRequisicao = "35.171.156.216";
var listaPermissoes = [];
var listaTempSimulacao = []

var dashboard2 = function () {

    const carregarDados = function () {

        graficoTemperatura();
        if (min > 0)
            document.querySelector('.val-min').innerHTML = `Mínima: <span id="minTemp">${min}</span> ºC`;
        if (max > 0)
            document.querySelector('.val-max').innerHTML = `Máxima: <span id="maxTemp">${max}</span> ºC`;
        if (media > 0)
            document.querySelector('.val-mid').innerHTML = `Média: <span id="mediaTemp">${media}</span> ºC`;
    }

    const resetarZoom = function () {
        graficoHistorico.resetZoom();
    }

    const graficoTemperatura = function () {

        if (valoresTemp) {
            const labels = valoresTemp.map(function (p) {
                var data = new Date(p[0]);
                data = new Date(data.setHours(data.getHours() + 3));

                return data.toLocaleString("pt-BR");
            });

            //const labels = valoresTemp.map(p => new Date(p[0]).setHours(new Date(p[0]).getHours() + 3).toLocaleString("pt-BR"));
            const valores = valoresTemp.map(p => p[1].toFixed(2));

            if (graficoHistorico)
                graficoHistorico.destroy();

            const ctx = document.getElementById('graficoHistorico').getContext('2d');
            graficoHistorico = new Chart(ctx, {
                type: 'line',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Temperatura (°C)',
                        data: valores,
                        borderColor: 'steelblue',
                        borderWidth: 2,
                        fill: false,
                        tension: 0.2,
                        pointRadius: 0,
                    }],
                },
                options: {
                    responsive: true,
                    animation: false,
                    plugins: {
                        legend: { display: true },
                        decimation: {
                            enabled: true,
                            algorithm: 'lttb',
                            samples: 1000
                        },
                        zoom: {
                            zoom: {
                                wheel: {
                                    enabled: true,
                                },
                                pinch: {
                                    enabled: true,
                                },
                                mode: 'x',
                            },
                            pan: {
                                enabled: true,
                                mode: 'x',
                            }
                        }
                    },
                    scales: {
                        x: {
                            ticks: {
                                maxTicksLimit: 20
                            }
                        },
                        y: {
                            ticks: {
                                stepSize: 5
                            },
                            suggestedMin: 0,
                            suggestedMax: 50
                        }
                    }
                }
            });

            if (valores.length > 0) {
                min = Math.min(...valores.filter(xs => xs > 0))
                max = Math.max(...valores);
                media = (
                    valores
                        .map(v => parseFloat(v)) // converte para número
                        .reduce((a, b) => a + b, 0) / valores.length
                ).toFixed(2);
            }
        }

        if (min > 0)
            document.getElementById("minTemp").innerText = min;
        if (max > 0)
            document.getElementById("maxTemp").innerText = max;
        if (media > 0)
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
                    valoresTemp = response;

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

            registros.map(function (item) {
                var data = new Date(new Date(item[0]).setHours(new Date(item[0]).getHours() + 3)).toLocaleString("pt-BR");
                var valor = item[1];

                table.row.add([
                    data,
                    valor.toFixed(2),
                ]);
            })

            table.draw();
        }
    }

    const consultaHistorico = function () {
        $.ajax({
            url: "/Dashboard/ObterDadosAgregadosMedia",
            data: {
                "ip": ipRequisicao,
                "tipoSensor": "Temp",
                "idSensor": "urn:ngsi-ld:Temp:001",
                "atributo": "temperature",
                "dateFrom": $("#DataInicio").val(),
                "dateTo": $("#DataFim").val()
            },
            method: "GET"
        }).done(function (response) {

            if (response) {
                valoresTemp = response;
                montarTabelaRegistros(response)

                carregarDados();
            }
        })
    }

    return {
        carregarDados: carregarDados,
        consultaTemperatura: consultaTemperatura,
        graficoTemperatura: graficoTemperatura,
        montarTabelaRegistros: montarTabelaRegistros,
        resetarZoom: resetarZoom,
        consultaHistorico: consultaHistorico,
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
    };

    const ledOnOffAsync = async function (onOff) {
        await onOffLed(onOff);
    }

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

    const resetarZoom = function () {
        graficoTempoReal.resetZoom();
    }

    const graficoTemperatura = function () {

        if (valoresTemp) {

            const labels = valoresTemp.map(p =>
                new Date(p.recvTime).toLocaleTimeString("pt-BR", { hour: '2-digit', minute: '2-digit', second: '2-digit' })
            );
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
                graficoTempoReal.data.labels = labels;
                graficoTempoReal.data.datasets[0].data = valores;
                graficoTempoReal.update();
            }
            else {
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
                            tension: 0.2,
                            pointRadius: 0,
                        }],
                    },
                    options: {
                        responsive: true,
                        animation: false,
                        plugins: {
                            legend: { display: true },
                            decimation: {
                                enabled: true,
                                algorithm: 'lttb',
                                samples: 1000
                            },
                            zoom: {
                                zoom: {
                                    wheel: {
                                        enabled: true,
                                    },
                                    pinch: {
                                        enabled: true,
                                    },
                                    mode: 'x',
                                },
                                pan: {
                                    enabled: true,
                                    mode: 'x',
                                }
                            }
                        },
                        scales: {
                            x: {
                                ticks: {
                                    maxTicksLimit: 20
                                }
                            },
                            y: {
                                ticks: {
                                    stepSize: 5
                                },
                                suggestedMin: 0,
                                suggestedMax: 50
                            }
                        }
                    }
                });
            }
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

    const onOffLed = async function (onOff) {
        await new Promise(resolve =>
            $.ajax({
                url: "/Dashboard/OnOffLed",
                data: {
                    "ip": ipRequisicao,
                    "onOff": onOff
                },
                method: "GET"
            })
                .done(function (response) {

                })
                .always(resolve())
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

                    let max = $('#ValMax').val();
                    let min = $('#ValMin').val();

                    if (max && response[0].attrValue > max)
                        ledOnOffAsync(true);
                    else if (min && response[0].attrValue < min)
                        ledOnOffAsync(true);
                    else
                        ledOnOffAsync(false);

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
        resetarZoom: resetarZoom,
        onOffLed: onOffLed,
        ledOnOffAsync: ledOnOffAsync,
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

var perfilSection = function () {

    const preenchePermissoes = function () {
        let listaPermissoesCarregadas = $('#Permissoes').val();

        if (listaPermissoesCarregadas) {
            listaPermissoesCarregadas = JSON.parse(listaPermissoesCarregadas);

            listaPermissoesCarregadas.map(function (permissao) {
                let idCampo = permissao.NomeController + permissao.NomeAction;

                $('#' + idCampo).prop('checked', true);
            })

            onchangePermissoes();
        }
    }

    const onchangePermissoes = function () {
        const ids = [
            'UsuarioConsulta', 'UsuarioAdicionar', 'UsuarioEditar', 'UsuarioExcluir',
            'MotorConsulta', 'MotorAdicionar', 'MotorEditar', 'MotorExcluir',
            'PerfilConsulta', 'PerfilAdicionar', 'PerfilEditar', 'PerfilExcluir',
            'SimulacaoConsulta', 'SimulacaoAdicionar', 'SimulacaoEditar', 'SimulacaoExcluir'
        ];

        ids.forEach(id => {
            const checkbox = document.getElementById(id);
            if (checkbox) {
                let objeto = {};

                // Separar o prefixo (C) e a ação (A)
                const partes = id.match(/([A-Za-z]+)(Consulta|Adicionar|Editar|Excluir)/);
                if (partes) {
                    const modulo = partes[1];
                    const acao = partes[2];

                    // Tratamento para ação: mudar "Consultar" para "Consulta"
                    objeto.NomeController = modulo;
                    objeto.NomeAction = acao;
                }

                if (checkbox.checked) {
                    let itemExistente = listaPermissoes.some(function (item) {
                        if (item.NomeController == objeto.NomeController) {
                            if (item.NomeAction == objeto.NomeAction)
                                return true;
                            else
                                return false;
                        }
                        else
                            return false
                    })

                    if (!itemExistente)
                        listaPermissoes.push(objeto);
                }
                else {
                    let itemExistente = listaPermissoes.some(function (item) {
                        if (item.NomeController == objeto.NomeController) {
                            if (item.NomeAction == objeto.NomeAction)
                                return true;
                            else
                                return false;
                        }
                        else
                            return false
                    })

                    if (itemExistente)
                        listaPermissoes = listaPermissoes.filter(item => !(item.NomeController == objeto.NomeController && item.NomeAction == objeto.NomeAction));
                }
            }
        });

        const jsonResult = JSON.stringify(listaPermissoes);

        $('#Permissoes').val(jsonResult);

        console.log(jsonResult);
    };

    return {
        onchangePermissoes: onchangePermissoes,
        preenchePermissoes: preenchePermissoes,
    }
}();

var historicoSection = function () {

    const confirmarBusca = function () {

        Swal.fire({
            title: 'Aviso!',
            text: "Selecionar longos períodos pode causar lentidão, deseja prosseguir com a busca?",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Sim, prosseguir!',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                consultaHistorico();
            }
        });
    }

    const consultaHistorico = function () {
        $.ajax({
            url: "/Historico/ObterDadosAgregadosMedia",
            data: {
                "ip": ipRequisicao,
                "tipoSensor": "Temp",
                "idSensor": "urn:ngsi-ld:Temp:001",
                "atributo": "temperature",
                "dateFrom": $("#DataInicio").val(),
                "dateTo": $("#DataFim").val(),
                "intervalo": $('#Intervalo').val()
            },
            method: "GET"
        }).done(function (response) {

            if (response) {
                valoresTemp = response;
                montarTabelaRegistros(response)
            }
        })
    }

    const montarTabelaRegistros = function (registros) {

        if (table && registros) {
            table.clear();

            registros.map(function (item) {
                var data = new Date(new Date(item[0]).setHours(new Date(item[0]).getHours() + 3)).toLocaleString("pt-BR");
                var valor = item[1];

                table.row.add([
                    data,
                    valor.toFixed(2),
                ]);
            })

            table.draw();
        }
    }

    return {
        consultaHistorico: consultaHistorico,
        montarTabelaRegistros: montarTabelaRegistros,
        confirmarBusca: confirmarBusca
    }
}();

var simulacaoSection = function () {

    function exibirLoading() {
        $('#overlay-loading').show();
    }

    function esconderLoading() {
        $('#overlay-loading').hide();
    }

    const onchangeCalculos = async function () {
        let dataInicio = $("#DataInicio").val();
        let dataFim = $("#DataFim").val();

        let midVal = 0;
        let minVal = 0;
        let maxVal = 0;

        if (dataInicio && dataInicio != "01/01/0001" && dataFim && dataFim != "01/01/0001")
            await consultaHistorico(dataInicio, dataFim);

        if (listaTempSimulacao.length > 0) {
            let valores = listaTempSimulacao.map(p => p[1].toFixed(2));

            if (valores.length > 0) {
                minVal = Math.min(...valores.filter(xs => xs > 0)).toFixed(2)
                maxVal = Math.max(...valores).toFixed(2)
                midVal = (
                    valores
                        .map(v => parseFloat(v)) // converte para número
                        .reduce((a, b) => a + b, 0) / valores.length
                ).toFixed(2);

                if (midVal > 0)
                    $("#media").val(midVal.replace(".", ","));
                if (minVal > 0)
                    $("#min").val(minVal.replace(".", ","));
                if (maxVal > 0)
                    $("#max").val(maxVal.replace(".", ","));
            }
        }
    }

    const consultaHistorico = async function (dataInicio, dataFim) {
        exibirLoading();
        await new Promise((resolve, reject) => {
            $.ajax({
                url: "/Historico/ObterDadosAgregadosMedia",
                data: {
                    ip: ipRequisicao,
                    tipoSensor: "Temp",
                    idSensor: "urn:ngsi-ld:Temp:001",
                    atributo: "temperature",
                    dateFrom: dataInicio,
                    dateTo: dataFim
                },
                method: "GET"
            }).always(function () {
                esconderLoading();
            })
                .done(function (response) {
                    if (response) {
                        listaTempSimulacao = response;
                    }
                    resolve();
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    console.error("Erro na requisição:", textStatus, errorThrown);
                    reject(errorThrown);
                });
        });
    }

    return {
        onchangeCalculos: onchangeCalculos,
        consultaHistorico: consultaHistorico,
    }
}();