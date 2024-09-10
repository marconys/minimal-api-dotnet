using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;

namespace MinimalApi.Dominio.Interces;

public interface IAdministradorServico
{
    Administrador? Login(LoginDTO loginDTO);
}