
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interces;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Db;

namespace MinimalApi.Dominio.Servicos;

public class VeiculoServico : IVeiculoServico
{
    private readonly DbContexto _contexto;
    public VeiculoServico(DbContexto dbContext)
    {
        _contexto = dbContext;
    }

    public void Apagar(Veiculo veiculo)
    {
        _contexto.Veiculos.Remove(veiculo);
        _contexto.SaveChanges();
    }

    public void Atualizar(Veiculo veiculo)
    {
        _contexto.Veiculos.Update(veiculo);
        _contexto.SaveChanges();
    }

    public Veiculo? BuscarPorId(int id)
    {
        return _contexto.Veiculos.Where(v => v.Id == id).FirstOrDefault();
    }

    public void Incluir(Veiculo veiculo)
    {
        _contexto.Veiculos.Add(veiculo);
        _contexto.SaveChanges();
    }

   public List<Veiculo> Todos(int pagina = 1, string? nome = null, string? marca = null)
{
    // Declare query como IQueryable<Veiculo>
    IQueryable<Veiculo> query = _contexto.Veiculos;

    if (!string.IsNullOrEmpty(nome))
        query = query.Where(v => v.Nome.ToLower().Contains(nome.ToLower()));

    if (!string.IsNullOrEmpty(marca))
        query = query.Where(v => v.Marca.ToLower().Contains(marca.ToLower()));

    // Exemplo de paginação
    int pageSize = 10;
    return query.Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .ToList(); // Executa a consulta no banco de dados e retorna a lista
}

}