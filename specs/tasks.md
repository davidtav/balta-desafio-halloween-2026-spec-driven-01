# Tarefas

Cada tarefa é implementada isoladamente, com seus testes, e termina com `dotnet build` sem warnings e `dotnet test` passando (Q-07). Cada tarefa vira um commit `tipo(T-xx): descrição` (C-06).

| ID | Tarefa | Origem | Depende de | Concluída quando |
|---|---|---|---|---|
| T-01 | Criar `global.json` (SDK 10.0.300, `rollForward: latestFeature`), a solução `AcervoPessoal.slnx`, o projeto web vazio `src/AcervoPessoal.Api` e o projeto xUnit `src/AcervoPessoal.Tests` (referenciando a Api, com as pastas `Unit/` e `Integration/`). Ativar `Nullable` e `TreatWarningsAsErrors` nos dois. Remover o teste de exemplo do template. Incluir `*.db`, `*.db-shm` e `*.db-wal` no `.gitignore` | S-01, S-02, S-05, A-01, Q-01, D-03, D-17, D-19 | | `dotnet build` compila sem erros nem warnings e `dotnet test` executa |
| T-02 | Criar as exceções de domínio: `ValidacaoException` (com campo e mensagem), `RegraNegocioException` e `NaoEncontradoException` | D-06, D-07 | T-01 | Teste unitário confirma que `ValidacaoException` expõe o campo e a mensagem |
| T-03 | Criar `StatusLivro` e a entidade `Livro` com construtor que descarta espaços das pontas e valida título, autor e ano (obrigatório, 1960 a 2025), além dos tamanhos máximos. Livro novo nasce `Disponivel` | RF-01, RN-01, RN-02, RN-03, RN-10, RN-12, D-13 | T-02 | Testes unitários cobrem: título e autor vazios ou só com espaços; ano nulo; anos 1959, 1960, 2025 e 2026; títulos e autores com 200 e 201 caracteres; descarte de espaços; situação inicial |
| T-04 | Criar `Livro.AlterarDados(titulo, autor, ano)` com as mesmas validações do construtor, sem mudar a situação | RF-05, RN-08 | T-03 | Testes unitários cobrem alteração válida, alteração inválida sem mudar nenhum dado e livro disponível que continua disponível depois de alterado |
| T-05 | Criar a entidade `Emprestimo` e os métodos `Livro.Emprestar(pessoa, agora)` e `Livro.Devolver(agora)` | RF-07, RF-08, RN-04, RN-05, RN-06, RN-08, RN-09, RN-12, D-09 | T-04 | Testes unitários cobrem: empréstimo de livro disponível; segundo empréstimo rejeitado sem alterar o primeiro; devolução de livro emprestado registra a data; devolução de livro disponível rejeitada; pessoa vazia; pessoa com 100 e 101 caracteres; livro emprestado que continua emprestado depois de alterado |
| T-06 | Criar `Livro.Remover(agora)` com remoção lógica (`Removido`, `RemovidoEm`), rejeitando livro emprestado | RF-06, RN-11, D-08 | T-05 | Testes unitários cobrem a remoção de livro disponível e a rejeição de livro emprestado |
| T-07 | Adicionar os pacotes do EF Core (Sqlite e Design) e o manifesto local do `dotnet-ef`. Criar o `AcervoDbContext` com mapeamentos (tamanhos, `NOCASE` em `Titulo`, filtro global de removidos, índice único parcial de empréstimo em aberto) e a migração inicial | S-03, S-04, D-04, D-05, D-08, D-10, D-12, Modelo de dados | T-06 | A migração é aplicada em um SQLite em memória, e um teste confirma que um livro removido não volta em consulta |
| T-08 | Criar `ILivroService` e `LivroService` com `CadastrarAsync`, `ObterPorIdAsync` e `ListarAsync`, e as respostas `LivroResposta` e `EmprestimoAtualResposta` | RF-01, RF-02, RF-03, RN-07, RN-13, A-06, D-11 | T-07 | Testes unitários do serviço (SQLite em memória) cobrem: cadastro de exemplar repetido gera novo id; id inexistente ou removido lança `NaoEncontradoException`; listagem ordenada por título sem removidos |
| T-09 | Criar `LivroService.BuscarAsync(termo)` | RF-04, RN-10, D-12 | T-08 | Testes unitários cobrem: "tolkien" encontra "J. R. R. Tolkien"; busca por título e por autor; termo vazio lança `ValidacaoException`; `%` e `_` tratados como texto; nenhum resultado devolve lista vazia; removidos ficam de fora |
| T-10 | Criar `LivroService.AlterarAsync` e `LivroService.RemoverAsync` | RF-05, RF-06, RN-07, RN-11 | T-08 | Testes unitários cobrem: alteração persistida; remoção persistida e livro removido tratado como não encontrado; remover duas vezes lança `NaoEncontradoException` |
| T-11 | Criar `LivroService.EmprestarAsync` e `LivroService.DevolverAsync`, usando `TimeProvider`. Nos testes, usar uma subclasse própria de `TimeProvider` com data fixa, sem pacote extra | RF-07, RF-08, RN-09, D-11 | T-08 | Testes unitários cobrem: empréstimo grava pessoa e data do `TimeProvider`; devolução grava a data; operações em livro inexistente lançam `NaoEncontradoException` |
| T-12 | Criar o tratador global de exceções (`IExceptionHandler` + `AddProblemDetails`) e configurar o `Program.cs`: `DbContext`, serviços, `TimeProvider.System`, `JsonStringEnumConverter` e `Database.Migrate()` na inicialização | Q-04, Q-05, A-03, D-07, D-14, D-16 | T-08 | Testes unitários do tratador confirmam: validação → 400 com o campo em `errors`; não encontrado → 404; regra de negócio → 409; todos com `application/problem+json` |
| T-13 | Criar a fábrica de testes de integração (`WebApplicationFactory<Program>` com SQLite em memória por teste) e os endpoints `POST /livros` e `GET /livros/{id}` | RF-01, RF-02, Q-03, Q-06, A-02, D-18 | T-12 | Testes de integração: POST válido → 201 com `Location`; GET desse id → 200; POST com título vazio → 400 ProblemDetails; GET de id inexistente → 404 |
| T-14 | Criar os endpoints `GET /livros` e `GET /livros/busca?termo=` | RF-03, RF-04 | T-13 | Testes de integração: listagem → 200 ordenada; busca → 200 com resultado; busca com termo vazio → 400 |
| T-15 | Criar os endpoints `PUT /livros/{id}` e `DELETE /livros/{id}` | RF-05, RF-06 | T-13 | Testes de integração: PUT válido → 200; DELETE → 204, e GET em seguida → 404 |
| T-16 | Criar os endpoints `POST /livros/{id}/emprestimos` e `POST /livros/{id}/devolucoes` | RF-07, RF-08, RN-04, RN-05, RN-11 | T-15 | Testes de integração: empréstimo → 200 com `emprestimoAtual`; segundo empréstimo → 409; DELETE de livro emprestado → 409; devolução → 200 com `emprestimoAtual` nulo |
| T-17 | Criar `AcervoPessoal.Api.http` com uma requisição de exemplo para cada endpoint | D-20 | T-16 | A API sobe com `dotnet run` sem erros, e cada requisição do arquivo retorna o status previsto em Contratos |
| T-18 | Preencher o `readme.md` do desafio (nome, problema, o que foi implementado, aprendizados) | Entrega do desafio | T-17 | O `readme.md` não tem mais nenhum marcador do template (`[seu nome aqui]`, `--`, `[Listar ...]`, `[Incluir ...]`) |

## Cobertura

Checagem cruzada entre a spec e as tarefas: todo requisito e regra tem tarefa, e toda tarefa tem origem.

| Item da spec | Tarefas |
|---|---|
| RF-01 | T-03, T-08, T-13 |
| RF-02 | T-08, T-13 |
| RF-03 | T-08, T-14 |
| RF-04 | T-09, T-14 |
| RF-05 | T-04, T-10, T-15 |
| RF-06 | T-06, T-10, T-15 |
| RF-07 | T-05, T-11, T-16 |
| RF-08 | T-05, T-11, T-16 |
| RN-01, RN-02, RN-03 | T-03, T-04 |
| RN-04, RN-05 | T-05, T-16 |
| RN-06 | T-05 |
| RN-07 | T-07, T-08, T-10 |
| RN-08 | T-04, T-05 |
| RN-09 | T-05, T-07, T-11 |
| RN-10 | T-03, T-09 |
| RN-11 | T-06, T-10, T-16 |
| RN-12 | T-03, T-05 |
| RN-13 | T-08 |
| CA-01 a CA-18 | Verificados pelos testes das tarefas acima |
