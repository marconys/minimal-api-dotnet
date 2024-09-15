using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

[TestClass]
public class AdministradorServicoTest
{
    private DbContexto CriarContextoDeTeste()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
        .SetBasePath(path ?? Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new DbContexto(configuration);
    }

    [TestMethod]
    public void TestarSalvarAdministrador()
    {

        // Arrange: Preparação das variáveis e instância do objeto 'Administrador' para o teste.        
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");
        var adm = new Administrador();
        adm.Id = 1;
        adm.Email = "administrador@teste.com";
        adm.Senha = "1234";
        adm.Perfil = "Adm";
        var administradorServico = new AdministradorServico(context);


        // Act: Atribuição de valores às propriedades do objeto 'Administrador'.
        administradorServico.Incluir(adm);

        // Assert: Verificação se as propriedades retornam os valores esperados.
        Assert.AreEqual(1, administradorServico.Todos(1).Count());

    }

    [TestMethod]
    public void TestandoCriaAdministradorEBucarPorId()
    {

        // Arrange: Preparação das variáveis e instância do objeto 'Administrador' para o teste.        
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");
        var adm = new Administrador();
        adm.Id = 1;
        adm.Email = "administrador@teste.com";
        adm.Senha = "1234";
        adm.Perfil = "Adm";
        var administradorServico = new AdministradorServico(context);


        // Act: Atribuição de valores às propriedades do objeto 'Administrador'.
        administradorServico.Incluir(adm);
        var _adm = administradorServico.BuscarPorId(adm.Id);

        // Assert: Verificação se as propriedades retornam os valores esperados.
        Assert.AreEqual(1, _adm!.Id);

    }

    
}