using System;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;

namespace MinimalApi.Infraestrutura.Db;

public class DbContexto : DbContext
{
    private readonly IConfiguration _configuracaoAppSettings;
    public DbContexto (IConfiguration configuracaoAppSettings)
    {
        _configuracaoAppSettings = configuracaoAppSettings;
    }
    public DbSet<Administrador> Administradores { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var strigConexao = _configuracaoAppSettings.GetConnectionString("mysql")?.ToString();
        if (!string.IsNullOrEmpty(strigConexao))
        {
           optionsBuilder.UseMySql(strigConexao, ServerVersion.AutoDetect(strigConexao)); 
        }
        }
        
    }
}