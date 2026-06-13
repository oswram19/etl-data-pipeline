from dataclasses import dataclass
from typing import Any, Optional


@dataclass
class Partido:
    id: Any
    estado: str = "Pendiente"
    goles_local: Optional[int] = None
    goles_visitante: Optional[int] = None


class ResultadoPartidoInvalidoError(ValueError):
    pass


def _validar_goles(nombre_campo: str, goles: Any) -> int:
    if goles is None:
        raise ResultadoPartidoInvalidoError(f"{nombre_campo} no puede estar vacío")

    if isinstance(goles, str) and goles.strip() == "":
        raise ResultadoPartidoInvalidoError(f"{nombre_campo} no puede estar vacío")

    if isinstance(goles, bool) or not isinstance(goles, int):
        raise ResultadoPartidoInvalidoError(f"{nombre_campo} debe ser un número entero")

    if goles < 0:
        raise ResultadoPartidoInvalidoError(f"{nombre_campo} no puede ser negativo")

    return goles


def registrar_resultado(partido: Any, goles_local: Any, goles_visitante: Any) -> Any:
    goles_local_validados = _validar_goles("goles_local", goles_local)
    goles_visitante_validados = _validar_goles("goles_visitante", goles_visitante)

    if isinstance(partido, dict):
        partido["goles_local"] = goles_local_validados
        partido["goles_visitante"] = goles_visitante_validados
        partido["estado"] = "Jugado"
        return partido

    setattr(partido, "goles_local", goles_local_validados)
    setattr(partido, "goles_visitante", goles_visitante_validados)
    setattr(partido, "estado", "Jugado")
    return partido
