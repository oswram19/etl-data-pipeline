namespace TablaPosiciones.Tests;

public class TablaPosicionesServiceTests
{
    [Fact]
    public void CalcularPorGrupo_CalculaEstadisticasYPuntosSegunReglas()
    {
        var service = new TablaPosicionesService();
        var partidos = new[]
        {
            new ResultadoPartido("A", "Equipo 1", "Equipo 2", 2, 0),
            new ResultadoPartido("A", "Equipo 1", "Equipo 3", 1, 1),
            new ResultadoPartido("A", "Equipo 2", "Equipo 3", 3, 2)
        };

        var tabla = service.CalcularPorGrupo(partidos);
        var grupoA = tabla["A"];
        var equipo1 = grupoA.Single(t => t.Equipo == "Equipo 1");
        var equipo2 = grupoA.Single(t => t.Equipo == "Equipo 2");
        var equipo3 = grupoA.Single(t => t.Equipo == "Equipo 3");

        Assert.Equal(2, equipo1.PJ);
        Assert.Equal(1, equipo1.G);
        Assert.Equal(1, equipo1.E);
        Assert.Equal(0, equipo1.P);
        Assert.Equal(3, equipo1.GF);
        Assert.Equal(1, equipo1.GC);
        Assert.Equal(2, equipo1.DG);
        Assert.Equal(4, equipo1.PTS);

        Assert.Equal(2, equipo2.PJ);
        Assert.Equal(1, equipo2.G);
        Assert.Equal(0, equipo2.E);
        Assert.Equal(1, equipo2.P);
        Assert.Equal(3, equipo2.GF);
        Assert.Equal(4, equipo2.GC);
        Assert.Equal(-1, equipo2.DG);
        Assert.Equal(3, equipo2.PTS);

        Assert.Equal(2, equipo3.PJ);
        Assert.Equal(0, equipo3.G);
        Assert.Equal(1, equipo3.E);
        Assert.Equal(1, equipo3.P);
        Assert.Equal(3, equipo3.GF);
        Assert.Equal(4, equipo3.GC);
        Assert.Equal(-1, equipo3.DG);
        Assert.Equal(1, equipo3.PTS);
    }

    [Fact]
    public void CalcularPorGrupo_SeparaResultadosPorGrupo()
    {
        var service = new TablaPosicionesService();
        var partidos = new[]
        {
            new ResultadoPartido("A", "A1", "A2", 0, 0),
            new ResultadoPartido("B", "B1", "B2", 0, 1)
        };

        var tabla = service.CalcularPorGrupo(partidos);

        Assert.Equal(2, tabla.Count);
        Assert.All(tabla["A"], item => Assert.Equal("A", item.Grupo));
        Assert.All(tabla["B"], item => Assert.Equal("B", item.Grupo));
        Assert.Equal(3, tabla["B"].Single(t => t.Equipo == "B2").PTS);
    }
}
