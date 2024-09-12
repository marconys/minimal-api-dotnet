using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Servicos;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Db;

#region Builder

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options => {
    options.UseMySql(builder.Configuration.GetConnectionString("mysql"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
});

var app = builder.Build();

#endregion


#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");

#endregion

#region Administrador
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => {
    if (administradorServico.Login(loginDTO) != null)
        return Results.Ok("Login com sucesso");
    else
        return Results.Unauthorized();
}).WithTags("Administradores");

#endregion

#region Veiculos
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => {

    var veiculo = new Veiculo {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };

    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);


}).WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
{
    // Se a página não for informada, retorna todos os registros
    if (!pagina.HasValue)
    {
        var todosVeiculos = veiculoServico.Todos(null); // ou outro método para listar todos
        return Results.Ok(todosVeiculos);
    }

    // Verifique se a página informada é válida
    if (pagina < 1)
    {
        return Results.BadRequest("Não existem viculos serem listados.");
    }

    var veiculosPaginados = veiculoServico.Todos(pagina.Value);

    if (veiculosPaginados == null || !veiculosPaginados.Any())
    {
        return Results.NotFound("Pagina não encontrada.");
    }

    return Results.Ok(veiculosPaginados);
}).WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscarPorId(id);
    
    return veiculo == null ? Results.NotFound("Veículo não encontrado") : Results.Ok(veiculo);

}).WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscarPorId(id);
    
    if(veiculo == null) return Results.NotFound("Veículo não encontrado");

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);


}).WithTags("Veiculos");


#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();

#endregion


