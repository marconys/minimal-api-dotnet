
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interces;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Db;

namespace MinimalApi.Dominio.Servicos;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _contexto;
    public AdministradorServico(DbContexto dbContext)
    {
        _contexto = dbContext;
    }

    public Administrador? BuscarPorId(int id)
    {
        return _contexto.Administradores.Where(a => a.Id == id).FirstOrDefault();
    }

    public Administrador? Incluir(Administrador administrador)
    {
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();

        return administrador;
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
        var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        return adm;
    }

   public List<Administrador> Todos(int? pagina)
{
    var query = _contexto.Administradores.AsQueryable();

    int itensPorPagina = 10;
    int paginaAtual = pagina ?? 1; // Se 'pagina' for nulo, define '1' como padrão

    query = query.Skip((paginaAtual - 1) * itensPorPagina).Take(itensPorPagina);

    return query.ToList();
}

}