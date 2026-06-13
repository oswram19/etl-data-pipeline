namespace TablaPosiciones;

public sealed class TablaPosicionesService
{
    public IReadOnlyDictionary<string, IReadOnlyList<TablaPosicionItem>> CalcularPorGrupo(IEnumerable<ResultadoPartido> partidos)
    {
        ArgumentNullException.ThrowIfNull(partidos);

        var tablaPorGrupo = new Dictionary<string, Dictionary<string, TablaPosicionItem>>(StringComparer.OrdinalIgnoreCase);

        foreach (var partido in partidos)
        {
            if (!tablaPorGrupo.TryGetValue(partido.Grupo, out var tablaGrupo))
            {
                tablaGrupo = new Dictionary<string, TablaPosicionItem>(StringComparer.OrdinalIgnoreCase);
                tablaPorGrupo[partido.Grupo] = tablaGrupo;
            }

            var local = ObtenerOAgregar(tablaGrupo, partido.EquipoLocal, partido.Grupo);
            var visitante = ObtenerOAgregar(tablaGrupo, partido.EquipoVisitante, partido.Grupo);

            local.PJ++;
            visitante.PJ++;

            local.GF += partido.GolesLocal;
            local.GC += partido.GolesVisitante;
            visitante.GF += partido.GolesVisitante;
            visitante.GC += partido.GolesLocal;

            if (partido.GolesLocal > partido.GolesVisitante)
            {
                local.G++;
                local.PTS += 3;
                visitante.P++;
            }
            else if (partido.GolesLocal < partido.GolesVisitante)
            {
                visitante.G++;
                visitante.PTS += 3;
                local.P++;
            }
            else
            {
                local.E++;
                visitante.E++;
                local.PTS++;
                visitante.PTS++;
            }
        }

        return tablaPorGrupo.ToDictionary(
            kvp => kvp.Key,
            kvp => (IReadOnlyList<TablaPosicionItem>)kvp.Value.Values
                .OrderByDescending(item => item.PTS)
                .ThenByDescending(item => item.DG)
                .ThenByDescending(item => item.GF)
                .ThenBy(item => item.Equipo, StringComparer.Ordinal)
                .ToList(),
            StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyDictionary<string, IReadOnlyList<TablaPosicionItem>> CalcularTablaPorGrupo(IEnumerable<ResultadoPartido> partidos)
        => CalcularPorGrupo(partidos);

    private static TablaPosicionItem ObtenerOAgregar(IDictionary<string, TablaPosicionItem> tablaGrupo, string equipo, string grupo)
    {
        if (tablaGrupo.TryGetValue(equipo, out var tablaExistente))
        {
            return tablaExistente;
        }

        var item = new TablaPosicionItem(grupo, equipo);
        tablaGrupo[equipo] = item;
        return item;
    }
}

public sealed record ResultadoPartido(string Grupo, string EquipoLocal, string EquipoVisitante, int GolesLocal, int GolesVisitante);

public sealed class TablaPosicionItem
{
    public TablaPosicionItem(string grupo, string equipo)
    {
        Grupo = grupo;
        Equipo = equipo;
    }

    public string Grupo { get; }
    public string Equipo { get; }
    public int PJ { get; set; }
    public int G { get; set; }
    public int E { get; set; }
    public int P { get; set; }
    public int GF { get; set; }
    public int GC { get; set; }
    public int DG => GF - GC;
    public int PTS { get; set; }
}
