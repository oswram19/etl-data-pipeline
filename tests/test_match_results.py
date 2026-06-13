import unittest

from match_results import Partido, ResultadoPartidoInvalidoError, registrar_resultado


class RegistrarResultadoTests(unittest.TestCase):
    def test_registrar_resultado_cambia_estado_a_jugado(self):
        partido = Partido(id=1)

        registrar_resultado(partido, 2, 1)

        self.assertEqual(partido.goles_local, 2)
        self.assertEqual(partido.goles_visitante, 1)
        self.assertEqual(partido.estado, "Jugado")

    def test_registrar_resultado_rechaza_goles_vacios(self):
        partido = Partido(id=1)

        with self.assertRaises(ResultadoPartidoInvalidoError):
            registrar_resultado(partido, "", 1)

        with self.assertRaises(ResultadoPartidoInvalidoError):
            registrar_resultado(partido, 1, None)

    def test_registrar_resultado_rechaza_goles_negativos(self):
        partido = Partido(id=1)

        with self.assertRaises(ResultadoPartidoInvalidoError):
            registrar_resultado(partido, -1, 0)

        with self.assertRaises(ResultadoPartidoInvalidoError):
            registrar_resultado(partido, 0, -1)

    def test_registrar_resultado_rechaza_goles_no_enteros(self):
        partido = Partido(id=1)

        with self.assertRaises(ResultadoPartidoInvalidoError):
            registrar_resultado(partido, 1.5, 1)

        with self.assertRaises(ResultadoPartidoInvalidoError):
            registrar_resultado(partido, True, 0)


if __name__ == "__main__":
    unittest.main()
