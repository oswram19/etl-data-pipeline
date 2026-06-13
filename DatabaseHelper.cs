using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace EtlDataPipeline;

public sealed class Partido
{
    public int Id { get; set; }
    public string Fecha { get; set; } = string.Empty;
    public string HoraEste { get; set; } = string.Empty;
    public string HoraElSalvador { get; set; } = string.Empty;
    public string Equipo1 { get; set; } = string.Empty;
    public string Equipo2 { get; set; } = string.Empty;
    public string Grupo { get; set; } = string.Empty;
    public string Estadio { get; set; } = string.Empty;
    public int GolesEquipo1 { get; set; }
    public int GolesEquipo2 { get; set; }
    public string Estado { get; set; } = string.Empty;
}

public sealed class DatabaseHelper : IDisposable
{
    private readonly SQLiteConnection _connection;

    public DatabaseHelper(string connectionString)
    {
        _connection = new SQLiteConnection(connectionString);
        _connection.Open();
        CrearTablaPartidos();
    }

    private void CrearTablaPartidos()
    {
        const string query = @"
            CREATE TABLE IF NOT EXISTS Partidos (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Fecha TEXT NOT NULL,
                HoraEste TEXT NOT NULL,
                HoraElSalvador TEXT NOT NULL,
                Equipo1 TEXT NOT NULL,
                Equipo2 TEXT NOT NULL,
                Grupo TEXT NOT NULL,
                Estadio TEXT NOT NULL,
                GolesEquipo1 INTEGER NOT NULL,
                GolesEquipo2 INTEGER NOT NULL,
                Estado TEXT NOT NULL
            );";

        using var command = new SQLiteCommand(query, _connection);
        command.ExecuteNonQuery();
    }

    public void InsertarPartido(Partido partido)
    {
        const string query = @"
            INSERT INTO Partidos
            (Fecha, HoraEste, HoraElSalvador, Equipo1, Equipo2, Grupo, Estadio, GolesEquipo1, GolesEquipo2, Estado)
            VALUES
            (@Fecha, @HoraEste, @HoraElSalvador, @Equipo1, @Equipo2, @Grupo, @Estadio, @GolesEquipo1, @GolesEquipo2, @Estado);";

        using var command = new SQLiteCommand(query, _connection);
        command.Parameters.AddWithValue("@Fecha", partido.Fecha);
        command.Parameters.AddWithValue("@HoraEste", partido.HoraEste);
        command.Parameters.AddWithValue("@HoraElSalvador", partido.HoraElSalvador);
        command.Parameters.AddWithValue("@Equipo1", partido.Equipo1);
        command.Parameters.AddWithValue("@Equipo2", partido.Equipo2);
        command.Parameters.AddWithValue("@Grupo", partido.Grupo);
        command.Parameters.AddWithValue("@Estadio", partido.Estadio);
        command.Parameters.AddWithValue("@GolesEquipo1", partido.GolesEquipo1);
        command.Parameters.AddWithValue("@GolesEquipo2", partido.GolesEquipo2);
        command.Parameters.AddWithValue("@Estado", partido.Estado);
        command.ExecuteNonQuery();
    }

    public List<Partido> ListarPartidos()
    {
        const string query = "SELECT * FROM Partidos ORDER BY Id;";
        return EjecutarConsulta(query);
    }

    public void ActualizarResultados(int id, int golesEquipo1, int golesEquipo2, string estado)
    {
        const string query = @"
            UPDATE Partidos
            SET GolesEquipo1 = @GolesEquipo1,
                GolesEquipo2 = @GolesEquipo2,
                Estado = @Estado
            WHERE Id = @Id;";

        using var command = new SQLiteCommand(query, _connection);
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@GolesEquipo1", golesEquipo1);
        command.Parameters.AddWithValue("@GolesEquipo2", golesEquipo2);
        command.Parameters.AddWithValue("@Estado", estado);
        command.ExecuteNonQuery();
    }

    public List<Partido> FiltrarPorGrupo(string grupo)
    {
        const string query = "SELECT * FROM Partidos WHERE Grupo = @Grupo ORDER BY Id;";
        return EjecutarConsulta(query, ("@Grupo", grupo));
    }

    public List<Partido> FiltrarPorEstado(string estado)
    {
        const string query = "SELECT * FROM Partidos WHERE Estado = @Estado ORDER BY Id;";
        return EjecutarConsulta(query, ("@Estado", estado));
    }

    private List<Partido> EjecutarConsulta(string query, params (string Name, object Value)[] parametros)
    {
        var partidos = new List<Partido>();

        using var command = new SQLiteCommand(query, _connection);
        foreach (var parametro in parametros)
        {
            command.Parameters.AddWithValue(parametro.Name, parametro.Value);
        }

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            partidos.Add(new Partido
            {
                Id = Convert.ToInt32(reader["Id"]),
                Fecha = Convert.ToString(reader["Fecha"]) ?? string.Empty,
                HoraEste = Convert.ToString(reader["HoraEste"]) ?? string.Empty,
                HoraElSalvador = Convert.ToString(reader["HoraElSalvador"]) ?? string.Empty,
                Equipo1 = Convert.ToString(reader["Equipo1"]) ?? string.Empty,
                Equipo2 = Convert.ToString(reader["Equipo2"]) ?? string.Empty,
                Grupo = Convert.ToString(reader["Grupo"]) ?? string.Empty,
                Estadio = Convert.ToString(reader["Estadio"]) ?? string.Empty,
                GolesEquipo1 = Convert.ToInt32(reader["GolesEquipo1"]),
                GolesEquipo2 = Convert.ToInt32(reader["GolesEquipo2"]),
                Estado = Convert.ToString(reader["Estado"]) ?? string.Empty
            });
        }

        return partidos;
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
