# Constituição do projeto — Acervo Pessoal

Regras que valem para toda funcionalidade deste repositório. Cada regra é verificável olhando o código, a build ou os testes. Nenhuma especificação, plano ou tarefa pode contrariá-las.

## Stack

- S-01: Todos os projetos usam `net10.0` com C#.
- S-02: A aplicação é exposta como API HTTP no modelo Minimal API (sem Controllers, sem MVC).
- S-03: A persistência usa SQLite acessado exclusivamente via EF Core. Não há SQL escrito à mão fora de migrações.
- S-04: O esquema do banco é versionado por migrações do EF Core, guardadas no repositório.
- S-05: Testes usam xUnit.
- S-06: Nenhum pacote NuGet entra no projeto sem uma justificativa escrita na seção "Decisões" do `plan.md`.

## Arquitetura

- A-01: A solução segue esta estrutura de pastas:
  - `specs/`: constituição, especificação, plano e tarefas
  - `src/AcervoPessoal.Api`: aplicação
  - `src/AcervoPessoal.Tests`: testes, com as subpastas `Unit/` e `Integration/`
  - Todo o conteúdo de `src/` é gerado pela IA a partir das tarefas.
- A-02: Endpoints não referenciam o `DbContext` nem nenhum tipo do EF Core. Todo acesso a dados passa por um serviço de aplicação.
- A-03: Endpoints só fazem três coisas: receber a requisição, chamar um serviço e traduzir o resultado em resposta HTTP. Não há `if` de regra de negócio em endpoint ou handler HTTP.
- A-04: As regras de negócio vivem nas entidades de domínio e nos serviços de aplicação.
- A-05: Entidades protegem os próprios invariantes: propriedades com `set` privado e alteração de estado só por construtor ou métodos da entidade.
- A-06: Os serviços são registrados por injeção de dependência e consumidos por interface.

## Qualidade

- Q-01: Todos os projetos usam `<Nullable>enable</Nullable>` e `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`. A build não tem erros nem warnings.
- Q-02: Toda regra de negócio (`RN-xx` da spec) tem ao menos um teste unitário, e o nome do teste cita o ID da regra.
- Q-03: Todo endpoint tem ao menos um teste de integração do caminho feliz, executado contra a aplicação real em memória.
- Q-04: Erro de validação sempre retorna `400` com corpo `ProblemDetails` (`application/problem+json`).
- Q-05: Um recurso inexistente retorna `404` com corpo `ProblemDetails`.
- Q-06: Os testes nunca usam o banco real do usuário. Cada teste de integração roda em um banco SQLite isolado.
- Q-07: `dotnet build` e `dotnet test` passam integralmente antes de qualquer commit.

## Convenções

- C-01: Nomes de domínio, classes, métodos e propriedades são em português (ex.: `Livro`, `Emprestar`, `ObterPorIdAsync`).
- C-02: Métodos com I/O são assíncronos e terminam em `Async`.
- C-03: Namespaces são file-scoped.
- C-04: Cada arquivo tem um tipo público, e o arquivo tem o nome do tipo.
- C-05: Rotas HTTP são em minúsculas, no plural e sem verbo (ex.: `/livros`, `/livros/{id}`).
- C-06: Commits seguem Conventional Commits em português e citam a tarefa (ex.: `feat(T-03): valida título do livro`). Cada commit implementa uma única tarefa.

## Governança

- G-01: A ordem de precedência é constituição > `spec.md` > `plan.md` > `tasks.md` > código. Quando o código diverge da spec, a spec vence.
- G-02: Toda mudança de requisito altera o `spec.md` antes do código, no mesmo commit.
- G-03: Só se implementa o que consta no `tasks.md`, e toda tarefa aponta para o requisito, regra ou decisão que a originou.
- G-04: Cada fase (spec, plano, tarefas) só avança depois da aprovação humana do documento anterior.
- G-05: Alterar esta constituição exige registrar a data e o motivo na seção "Histórico".

## Histórico

- 2026-10-12: Versão inicial.
- 2026-10-12: A-01 alterada. Os testes passam de `tests/` para `src/AcervoPessoal.Tests` e a pasta `specs/` entra na estrutura. Motivo: o repositório do desafio define `specs/` para os documentos e `src/` para todo o código gerado pela IA.
