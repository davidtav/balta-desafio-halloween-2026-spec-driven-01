using AcervoPessoal.Api.Dominio;

namespace AcervoPessoal.Tests.Unit.Dominio;

public class ExcecoesDominioTests
{
    [Fact]
    public void ValidacaoException_ExpoeCampoEMensagem()
    {
        var excecao = new ValidacaoException("titulo", "Título é obrigatório.");

        Assert.Equal("titulo", excecao.Campo);
        Assert.Equal("Título é obrigatório.", excecao.Message);
    }

    [Fact]
    public void RegraNegocioException_ExpoeMensagem()
    {
        var excecao = new RegraNegocioException("O livro já está emprestado.");

        Assert.Equal("O livro já está emprestado.", excecao.Message);
    }

    [Fact]
    public void NaoEncontradoException_ExpoeMensagem()
    {
        var excecao = new NaoEncontradoException("Livro não encontrado.");

        Assert.Equal("Livro não encontrado.", excecao.Message);
    }
}
