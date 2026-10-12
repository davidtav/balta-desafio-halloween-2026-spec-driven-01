# Plano técnico

## Contexto

Implementar o `spec.md` (gestão de livros e empréstimos) como uma API HTTP, respeitando o `constitution.md`.

## Arquitetura

Dois projetos, conforme A-01:

```
global.json
AcervoPessoal.slnx
src/
  AcervoPessoal.Api/
    Dominio/          Livro, Emprestimo, StatusLivro, exceções de domínio
    Aplicacao/        ILivroService, LivroService, requisições e respostas
    Infraestrutura/   AcervoDbContext, mapeamentos, Migrations/
    Endpoints/        LivroEndpoints (só mapeia rota → serviço → resposta)
    Erros/            tratador global que converte exceções em ProblemDetails
    Program.cs
    AcervoPessoal.Api.http
  AcervoPessoal.Tests/
    Unit/             regras das entidades e do serviço
    Integration/      endpoints via aplicação em memória
```

Fluxo de uma requisição: **Endpoint → ILivroService → entidade (regras) → AcervoDbContext**. Quando uma regra é violada, a entidade ou o serviço lança uma exceção de domínio. O tratador global (`Erros/`) traduz essa exceção em ProblemDetails, então o endpoint não tem nenhum `if` de regra (A-03).

| Camada | Pode referenciar | Não pode referenciar |
|---|---|---|
| Endpoints | Aplicacao | Infraestrutura, EF Core (A-02) |
| Aplicacao | Dominio, Infraestrutura | Endpoints, tipos HTTP |
| Dominio | nada | EF Core, ASP.NET |

## Decisões

| ID | Decisão | Justificativa |
|---|---|---|
| D-01 | .NET 10 com Minimal API | Exigido por S-01 e S-02. |
| D-02 | Um projeto de API com pastas por camada, em vez de um projeto por camada | São 7 endpoints e 2 entidades. As fronteiras de A-02 e A-04 são verificáveis por namespace, sem o custo de vários projetos. |
| D-03 | `global.json` na raiz do repositório fixando o SDK 10.0.300 (`rollForward: latestFeature`) | Hoje a versão vem de um `global.json` fora do repositório. Sem esse arquivo, o build no GitHub ou em outra máquina pode usar outro SDK. |
| D-04 | Pacote `Microsoft.EntityFrameworkCore.Sqlite` (10.x) | S-03 exige SQLite via EF Core. |
| D-05 | Pacote `Microsoft.EntityFrameworkCore.Design` (10.x, `PrivateAssets=all`) e ferramenta `dotnet-ef` em manifesto local (`.config/dotnet-tools.json`) | S-04 exige migrações versionadas. O manifesto local garante a mesma versão da ferramenta para quem clonar. |
| D-06 | Regras validadas no construtor e nos métodos da entidade, que lançam `ValidacaoException` (com o campo e a mensagem) ou `RegraNegocioException` | A-04 e A-05: o domínio protege os próprios invariantes e é testável sem banco nem HTTP (Q-02). |
| D-07 | Tratador global (`IExceptionHandler` com `AddProblemDetails`): `ValidacaoException` → 400 `ValidationProblemDetails` com o campo em `errors`; `NaoEncontradoException` → 404; `RegraNegocioException` → 409 | Atende Q-04 e Q-05 sem `if` nos endpoints (A-03). Usamos 409 porque emprestar um livro emprestado, devolver um disponível ou remover um emprestado não é erro de formato da entrada, e sim um conflito com o estado atual do livro. |
| D-08 | Remoção lógica com `Removido` e `RemovidoEm` em `Livro`, mais um filtro global de consulta (`HasQueryFilter(l => !l.Removido)`) | RF-06 e RN-07: com o filtro, nenhuma consulta enxerga o livro removido, então "não encontrado" vale em todas as operações sem repetir a condição. |
| D-09 | `Emprestimo` é uma entidade filha de `Livro`. `Livro.Emprestar(pessoa, agora)` cria o empréstimo e `Livro.Devolver(agora)` encerra o empréstimo em aberto | RN-04, RN-05 e RN-09 ficam numa só entidade e são testáveis sem banco. |
| D-10 | Índice único parcial em `Emprestimos(LivroId) WHERE DataDevolucao IS NULL` | Garante RN-09 no banco mesmo que duas requisições cheguem ao mesmo tempo. |
| D-11 | `TimeProvider` injetado no serviço para obter a data atual | RF-07 e RF-08 usam "o momento do registro". Com `TimeProvider`, os testes controlam a data. Faz parte do .NET, então não exige pacote. |
| D-12 | Coluna `Titulo` com collation `NOCASE` e busca com `EF.Functions.Like`, escapando `%` e `_` do termo | RF-03 (ordenar por título) e RF-04 (sem diferenciar maiúsculas). Escapar evita que um termo como `100%` vire curinga. |
| D-13 | Campo `ano` da requisição como `int?` | RN-03 exige distinguir "não informado" (mensagem "Ano é obrigatório") de "fora da faixa". Com `int`, a ausência viraria 0. |
| D-14 | `Situacao` serializada como texto (`"Disponivel"`, `"Emprestado"`) via `JsonStringEnumConverter` | A resposta fica legível e não depende da ordem do enum. |
| D-15 | Identificadores `int` autoincremento | O dono do acervo usa o identificador manualmente; um número curto é mais prático que um GUID. Não há requisito de identificador não adivinhável. |
| D-16 | String de conexão em `appsettings.json` (`Data Source=acervo.db`), com as migrações aplicadas na inicialização (`Database.Migrate()`) | É uma aplicação local de usuário único: subir a API já deixa o banco pronto, sem passo manual. |
| D-17 | Incluir `*.db`, `*.db-shm` e `*.db-wal` no `.gitignore` | O `.gitignore` do repositório não ignora arquivos SQLite. Sem isso, os dados locais iriam para o repositório. |
| D-18 | Testes de integração com `WebApplicationFactory<Program>` (pacote `Microsoft.AspNetCore.Mvc.Testing`), cada teste com um banco SQLite em memória próprio | Q-03 exige executar contra a aplicação real. Q-06 proíbe usar o banco do usuário, e o banco em memória isola cada teste. |
| D-19 | Pacotes de teste: só os do template `dotnet new xunit` do SDK 10 e o de D-18. Sem biblioteca de asserções extra | S-05 e S-06: as asserções do xUnit bastam, e cada pacote a mais precisaria de justificativa. |
| D-20 | Sem OpenAPI/Swagger. O teste manual usa o arquivo `AcervoPessoal.Api.http` | A spec não pede documentação interativa, e isso evita um pacote (S-06). |
| D-21 | Na máquina de desenvolvimento, build, testes, migrações e execução rodam num contêiner descartável da imagem oficial `mcr.microsoft.com/dotnet/sdk:10.0`, com o repositório montado em `/repo` e os pacotes NuGet num volume (`acervo-nuget`). Não há `Dockerfile` nem `docker-compose`: o contêiner é só ambiente de execução do SDK | O Smart App Control do Windows bloqueia o carregamento das DLLs compiladas localmente, que não têm assinatura (erro `0x800711C7`). Isso impede `dotnet test`, `dotnet ef` e `dotnet run`. O contêiner Linux não é afetado e não exige desligar a proteção. A aplicação não depende do Docker: numa máquina sem esse bloqueio, os comandos `dotnet` rodam direto. |

## Modelo de dados

**Livros**

| Coluna | Tipo | Regras |
|---|---|---|
| Id | INTEGER PK autoincremento | |
| Titulo | TEXT(200) NOT NULL, collation NOCASE | RN-01, RN-12, D-12 |
| Autor | TEXT(200) NOT NULL | RN-02, RN-12 |
| Ano | INTEGER NOT NULL | RN-03 |
| Situacao | INTEGER NOT NULL (0 = Disponivel, 1 = Emprestado) | RN-04, RN-05, RN-08 |
| Removido | INTEGER NOT NULL, padrão 0 | RF-06, D-08 |
| RemovidoEm | TEXT NULL (data e hora UTC) | RF-06 |

**Emprestimos**

| Coluna | Tipo | Regras |
|---|---|---|
| Id | INTEGER PK autoincremento | |
| LivroId | INTEGER NOT NULL, FK → Livros.Id | |
| Pessoa | TEXT(100) NOT NULL | RN-06, RN-12 |
| DataEmprestimo | TEXT NOT NULL (UTC) | RF-07 |
| DataDevolucao | TEXT NULL (UTC) | RF-08 |
| Índice | único em LivroId onde DataDevolucao IS NULL | RN-09, D-10 |

Não há restrição de unicidade em título e autor, porque exemplares repetidos são permitidos (RN-13).

## Contratos

Todas as respostas são JSON. Os erros usam `application/problem+json`.

**Corpo de livro (`LivroResposta`)**

```json
{
  "id": 1,
  "titulo": "O Hobbit",
  "autor": "J. R. R. Tolkien",
  "ano": 2019,
  "situacao": "Emprestado",
  "emprestimoAtual": { "pessoa": "Maria", "dataEmprestimo": "2026-10-12T14:30:00Z" }
}
```

`emprestimoAtual` é `null` quando o livro está disponível. Ele atende ao objetivo da spec de saber com quem o livro está.

| Método e rota | Corpo | Sucesso | Erros | Origem |
|---|---|---|---|---|
| `POST /livros` | `{ titulo, autor, ano }` | 201 + `Location: /livros/{id}` + LivroResposta | 400 | RF-01, CA-01/02/03/15/16/17 |
| `GET /livros/{id}` | | 200 LivroResposta | 404 | RF-02, CA-04/05 |
| `GET /livros` | | 200 lista ordenada por título | | RF-03, CA-06 |
| `GET /livros/busca?termo=x` | | 200 lista (pode ser vazia) | 400 se termo vazio | RF-04, CA-07/18 |
| `PUT /livros/{id}` | `{ titulo, autor, ano }` | 200 LivroResposta | 400, 404 | RF-05, RN-08 |
| `DELETE /livros/{id}` | | 204 | 404, 409 se emprestado | RF-06, CA-08/14 |
| `POST /livros/{id}/emprestimos` | `{ pessoa }` | 200 LivroResposta | 400, 404, 409 se já emprestado | RF-07, CA-09/10/13 |
| `POST /livros/{id}/devolucoes` | | 200 LivroResposta | 404, 409 se não emprestado | RF-08, CA-11/12 |

Exemplo de erro 400 (CA-02):

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Um ou mais campos são inválidos.",
  "status": 400,
  "errors": { "titulo": ["Título é obrigatório."] }
}
```

**Rastreabilidade: onde cada regra é aplicada**

| Regra | Onde | Tipo de teste |
|---|---|---|
| RN-01, RN-02, RN-03, RN-10, RN-12 | Construtor e `AlterarDados` de `Livro` | Unitário |
| RN-04, RN-05, RN-09 | `Livro.Emprestar` / `Livro.Devolver` | Unitário |
| RN-06 | `Livro.Emprestar` | Unitário |
| RN-07 | Filtro global (D-08) + `NaoEncontradoException` no serviço | Unitário (serviço) + integração |
| RN-08 | `AlterarDados` não altera a situação | Unitário |
| RN-11 | `Livro.Remover` | Unitário |
| RN-13 | Ausência de unicidade no modelo | Unitário (serviço) |
| Termo de busca vazio | `LivroService.BuscarAsync` | Unitário |

## Riscos

| ID | Risco | Mitigação |
|---|---|---|
| R-01 | Sem diferenciar maiúsculas, o `LIKE` e o `NOCASE` do SQLite só tratam letras ASCII. "érico" não encontra "Érico". | Aceito nesta versão. CA-07 usa só ASCII. Se virar requisito, guardar uma coluna normalizada para a busca. |
| R-02 | O limite de ano 2025 é fixo, então livros de 2026 em diante são rejeitados. | Comportamento pedido pela spec (RN-03). Mudar o limite exige alterar a spec antes (G-02). |
| R-03 | Duas requisições simultâneas emprestando o mesmo livro. | O índice único parcial (D-10) rejeita a segunda. Com um único usuário, a chance é baixa. |
| R-04 | Testes unitários do serviço usam SQLite em memória e não o banco real. | Os testes de integração exercitam o mesmo provedor (SQLite), então o comportamento é o mesmo. |
| R-05 | O domínio do projeto (acervo) é diferente do sugerido no desafio (gerador de senhas). | Decisão do dono do projeto. O método, a stack e as regras de arquitetura e qualidade seguem o desafio. |
| R-06 | `Database.Migrate()` na inicialização falha se o arquivo do banco estiver bloqueado. | Aceitável para uso local. O erro aparece na hora de subir a API. |
| R-07 | Com a D-21, o contêiner (Linux) e as ferramentas do Windows (VS Code, Visual Studio) gravam nas mesmas pastas `bin/` e `obj/`. Um restore de um lado quebra a build do outro (`NETSDK1004`, caminhos de pacotes incompatíveis). | Por enquanto, apagar `bin/` e `obj/` antes de trocar de lado. Solução definitiva pendente de decisão: separar a saída de build do contêiner. |
