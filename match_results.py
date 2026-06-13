from dataclasses import dataclass
from typing import Any, MutableMapping, Optional, Union


@dataclass
class Partido:
    id: Any
    estado: str = "Pendiente"
    goles_local: Optional[int] = None
    goles_visitante: Optional[int] = None


class ResultadoPartidoInvalidoError(ValueError):
    pass


PartidoLike = Union[Partido, MutableMapping[str, Any]]


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


def _guardar_campo(partido: PartidoLike, campo: str, valor: Union[int, str]) -> None:
    if isinstance(partido, MutableMapping):
        partido[campo] = valor
        return

    setattr(partido, campo, valor)


def registrar_resultado(partido: PartidoLike, goles_local: int, goles_visitante: int) -> PartidoLike:
    goles_local_validados = _validar_goles("goles_local", goles_local)
    goles_visitante_validados = _validar_goles("goles_visitante", goles_visitante)

    _guardar_campo(partido, "goles_local", goles_local_validados)
    _guardar_campo(partido, "goles_visitante", goles_visitante_validados)
    _guardar_campo(partido, "estado", "Jugado")
    return partido
