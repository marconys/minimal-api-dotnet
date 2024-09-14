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

app.MapPost("/administradores/inserir", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) => {
    
    var validacao = new ErrosDeValidacao {
        Mensages = new List<string>()
    };

    if(string.IsNullOrEmpty(administradorDTO.Email))
        validacao.Mensages.Add("Email não pode ser vazio");

    if(string.IsNullOrEmpty(administradorDTO.Senha))
        validacao.Mensages.Add("Senha não pode ser vazia"); 

    if(administradorDTO.Perfil == null)
        validacao.Mensages.Add("Perfil não pode ser vazio");       

    
    var administrador = new Administrador {
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil.ToString()!
    };

    administradorServico.Incluir(administrador);

    return Results.Created($"/administrador/{administrador.Id}", new AdministradorModelView {
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });

}).WithTags("Administradores");

app.MapGet("/administradores/todos", ([FromQuery] int? pagina, IAdministradorServico administradorServico) => {
    
    var adms = new List<AdministradorModelView>();

    var administradores = administradorServico.Todos(pagina);

    foreach (var adm in administradores)
    {
        adms.Add(new AdministradorModelView {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }
    return Results.Ok(adms);

}).WithTags("Administradores");

app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.BuscarPorId(id);
    
    return administrador == null ? Results.NotFound("Administrador não encontrado") : Results.Ok(new AdministradorModelView {
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });

}).WithTags("Administradores");

#endregion

#region Veiculos
ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosDeValidacao {
        Mensages = new List<string>()
    };

    if (string.IsNullOrEmpty(veiculoDTO.Nome))
    {
        validacao.Mensages.Add("Informe o Nome do veiculo");
    }

    if (string.IsNullOrEmpty(veiculoDTO.Marca))
    {
        validacao.Mensages.Add("Informe a marca do veiculo");
    }

    if (veiculoDTO.Ano < 1950)
    {
        validacao.Mensages.Add("Não aceitamos carruagem, apenas automóveis!");
    }

    return validacao;
}
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => {
        
    var validacao = validaDTO(veiculoDTO);
    if(validacao.Mensages.Count > 0)
        return Results.BadRequest(validacao);

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

    var validacao = validaDTO(veiculoDTO);
    if(validacao.Mensages.Count > 0)
        return Results.BadRequest(validacao);

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);


}).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscarPorId(id);
    
    if(veiculo == null) return Results.NotFound("Veículo não encontrado");

    veiculoServico.Apagar(veiculo);

    return Results.NoContent();

}).WithTags("Veiculos");

#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();

#endregion


