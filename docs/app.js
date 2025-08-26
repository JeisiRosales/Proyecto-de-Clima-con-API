async function cargarDatos() {
  // Cargar JSONs
  const actualRes = await fetch("data/clima_actual.json");
  const historicoRes = await fetch("data/clima_historico.json");

  const climaActual = await actualRes.json();
  const climaHistorico = await historicoRes.json();

  // ----- Tarjetas de datos actuales -----
  const cards = document.getElementById("cards");
  climaActual.forEach(ciudad => {
    const card = document.createElement("div");
    card.classList.add("card");
    card.innerHTML = `
      <h3>${ciudad.Ciudad}</h3>
      <p>🌡️ ${ciudad.Temperatura} °C</p>
      <p>💧 ${ciudad.Humedad} %</p>
      <p>💨 ${ciudad.Viento} km/h</p>
      <p>🔽 ${ciudad.Presion} hPa</p>
      <small>${new Date(ciudad.FechaHora).toLocaleString()}</small>
    `;
    cards.appendChild(card);
  });

  // ----- Gráfico Temperaturas -----
  const ctxTemp = document.getElementById("tempChart").getContext("2d");

  // Agrupar por ciudad
  const ciudades = [...new Set(climaHistorico.map(d => d.Ciudad))];
  const datasetsTemp = ciudades.map(c => {
    const dataCiudad = climaHistorico.filter(d => d.Ciudad === c);
    return {
      label: c,
      data: dataCiudad.map(d => d.Temperatura),
      borderWidth: 1,
      fill: false,
      tension: 0.5
    };
  });

  new Chart(ctxTemp, {
    type: "line",
    data: {
      labels: climaHistorico
        .filter(d => d.Ciudad === ciudades[0])
        .map(d => new Date(d.FechaHora).toLocaleString()),
      datasets: datasetsTemp
    },
    options: { 
      responsive: true, plugins: { 
        title: { 
          display: true, 
          text: "Temperaturas últimos 7 días",
          font: {
            size: 30,
            weight: 'bold', 
            family: 'Arial' 
          },
          color: '#333',   
          padding: {
            top: 10,
            bottom: 30
          }
        }
      } 
    }
  });

  // ----- Gráfico Humedad -----
  const ctxHum = document.getElementById("humChart").getContext("2d");
  new Chart(ctxHum, {
    type: "bar",
    data: {
      labels: climaActual.map(d => d.Ciudad),
      datasets: [{
        label: "Humedad (%)",
        data: climaActual.map(d => d.Humedad)
      }]
    },
    options: { 
      responsive: true, 
      plugins: { 
        title: { 
          display: true, 
          text: "Humedad Actual por Ciudad",
          font: {
            size: 30,
            weight: 'bold', 
            family: 'Arial' 
          },
          color: '#333',   
          padding: {
            top: 40,
            bottom: 20
          }
        } 
      } 
    }
  });
}

cargarDatos();
