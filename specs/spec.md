# Acervo Pessoal — Gestão de livros e empréstimos

## Problema

Quem tem uma biblioteca pessoal perde o controle de quais livros possui e, principalmente, de para quem emprestou cada um. Livros emprestados não voltam porque ninguém lembra quem está com eles.

## Objetivo

Permitir que o dono do acervo cadastre e mantenha seus livros e registre empréstimos e devoluções, sabendo a qualquer momento se um livro está disponível ou com quem ele está.

## Usuários

- **Dono do acervo**: única pessoa que usa o sistema. Cadastra livros e registra empréstimos e devoluções.
- A pessoa que pega um livro emprestado não usa o sistema. Ela é apenas um nome registrado no empréstimo.

## Histórias

- H-01: Como dono do acervo, quero cadastrar um livro para que ele passe a fazer parte do meu acervo.
- H-02: Como dono do acervo, quero consultar um livro específico para ver os dados e a situação dele.
- H-03: Como dono do acervo, quero listar todos os meus livros para ter uma visão do acervo.
- H-04: Como dono do acervo, quero buscar livros por título ou autor para encontrá-los rapidamente.
- H-05: Como dono do acervo, quero corrigir os dados de um livro para manter o acervo correto.
- H-06: Como dono do acervo, quero remover um livro que não tenho mais, sem perder o histórico dele.
- H-07: Como dono do acervo, quero registrar que emprestei um livro a alguém para saber com quem ele está.
- H-08: Como dono do acervo, quero registrar a devolução de um livro para que ele volte a ficar disponível.

## Requisitos funcionais

- RF-01: O sistema deve permitir cadastrar um livro informando título, autor e ano. Todo livro novo nasce com a situação "disponível".
- RF-02: O sistema deve permitir consultar um livro pelo seu identificador, devolvendo título, autor, ano e situação.
- RF-03: O sistema deve permitir listar todos os livros do acervo, ordenados por título.
- RF-04: O sistema deve permitir buscar livros por um termo, devolvendo os livros cujo título ou autor contenham esse termo, sem diferenciar maiúsculas de minúsculas.
- RF-05: O sistema deve permitir alterar título, autor e ano de um livro.
- RF-06: O sistema deve permitir remover um livro. A remoção é lógica: o livro deixa de aparecer, mas seus dados e seus empréstimos continuam guardados.
- RF-07: O sistema deve permitir registrar o empréstimo de um livro, informando o nome da pessoa. A data do empréstimo é a do momento do registro.
- RF-08: O sistema deve permitir registrar a devolução de um livro emprestado. A data da devolução é a do momento do registro.

## Regras de negócio

- RN-01: O título é obrigatório e não pode ser composto apenas de espaços.
- RN-02: O autor é obrigatório e não pode ser composto apenas de espaços.
- RN-03: O ano é obrigatório e deve estar entre 1960 e 2025, inclusive os dois limites.
- RN-04: Só um livro com situação "disponível" pode ser emprestado.
- RN-05: Só um livro com situação "emprestado" pode ser devolvido.
- RN-06: O nome da pessoa do empréstimo é obrigatório e não pode ser composto apenas de espaços.
- RN-07: Um livro removido não aparece em consulta, listagem nem busca, e não pode ser alterado, emprestado, devolvido ou removido de novo. Para o usuário, ele não existe.
- RN-08: A situação de um livro só muda por empréstimo ou devolução. A alteração de dados (RF-05) não muda a situação.
- RN-09: Um livro tem no máximo um empréstimo em aberto por vez. Ao ser devolvido, o empréstimo em aberto é encerrado.
- RN-10: Espaços no início e no fim de título, autor, nome da pessoa e termo de busca são descartados.
- RN-11: Um livro emprestado não pode ser removido. Ele precisa ser devolvido antes.
- RN-12: Título e autor têm no máximo 200 caracteres cada. O nome da pessoa tem no máximo 100 caracteres. O limite vale depois de descartar os espaços das pontas (RN-10).
- RN-13: Pode haver mais de um livro com o mesmo título e o mesmo autor (exemplares repetidos). Cada um é um livro independente.

## Casos de borda

- Ano igual a 1960 ou a 2025: aceito. Ano igual a 1959 ou a 2026: rejeitado. Um livro publicado em 2026 não pode ser cadastrado.
- Título, autor ou pessoa vazios ou só com espaços: rejeitado.
- Busca com termo vazio ou só com espaços: rejeitada como inválida.
- Busca sem nenhum resultado: lista vazia, não é erro.
- Listagem com o acervo vazio: lista vazia, não é erro.
- Identificador inexistente ou de livro removido: tratado como "não encontrado", em todas as operações.
- Emprestar um livro já emprestado: rejeitado, informando que o livro já está emprestado.
- Devolver um livro disponível: rejeitado, informando que o livro não está emprestado.
- Alterar os dados de um livro emprestado: permitido, e a situação continua "emprestado".
- Ano não informado: rejeitado.
- Remover um livro emprestado: rejeitado, informando que o livro precisa ser devolvido antes.
- Título ou autor com exatamente 200 caracteres: aceito. Com 201: rejeitado.
- Pessoa com exatamente 100 caracteres: aceita. Com 101: rejeitada.
- Cadastrar um livro com o mesmo título e autor de outro já existente: aceito, gerando um novo identificador.

## Fora de escopo

- Reserva de livros (a situação "reservado" não existe nesta versão).
- Restaurar um livro removido.
- Consultar o histórico de empréstimos de um livro.
- Prazo de devolução, atraso e multa.
- Cadastro de pessoas: a pessoa é só um nome no empréstimo.
- Autenticação, múltiplos usuários e múltiplos acervos.
- Paginação da listagem e da busca.

## Critérios de aceite

- CA-01: QUANDO um livro válido for cadastrado, O SISTEMA DEVE gravá-lo com a situação "disponível" e devolver seu identificador. (RF-01)
- CA-02: QUANDO o cadastro ou a alteração tiver título ou autor vazio, O SISTEMA DEVE rejeitar com erro de validação indicando o campo. (RN-01, RN-02)
- CA-03: QUANDO o cadastro ou a alteração tiver ano fora de 1960 a 2025, O SISTEMA DEVE rejeitar com erro de validação. (RN-03)
- CA-04: QUANDO um livro existente for consultado, O SISTEMA DEVE devolver título, autor, ano e situação. (RF-02)
- CA-05: QUANDO um identificador inexistente ou removido for usado em qualquer operação, O SISTEMA DEVE responder "não encontrado". (RN-07)
- CA-06: QUANDO a listagem for pedida, O SISTEMA DEVE devolver todos os livros não removidos ordenados por título. (RF-03)
- CA-07: QUANDO uma busca por "tolkien" for feita e existir o autor "J. R. R. Tolkien", O SISTEMA DEVE incluí-lo no resultado. (RF-04)
- CA-08: QUANDO um livro for removido, O SISTEMA DEVE deixar de exibi-lo em consulta, listagem e busca, mantendo seus dados guardados. (RF-06, RN-07)
- CA-09: QUANDO um livro disponível for emprestado a uma pessoa, O SISTEMA DEVE mudar sua situação para "emprestado" e registrar a pessoa e a data. (RF-07)
- CA-10: QUANDO um livro já emprestado for emprestado de novo, O SISTEMA DEVE rejeitar a operação sem alterar o empréstimo existente. (RN-04)
- CA-11: QUANDO um livro emprestado for devolvido, O SISTEMA DEVE mudar sua situação para "disponível" e registrar a data da devolução no empréstimo. (RF-08, RN-09)
- CA-12: QUANDO um livro disponível for devolvido, O SISTEMA DEVE rejeitar a operação. (RN-05)
- CA-13: QUANDO o empréstimo tiver o nome da pessoa vazio, O SISTEMA DEVE rejeitar com erro de validação. (RN-06)
- CA-14: QUANDO um livro emprestado for removido, O SISTEMA DEVE rejeitar a operação e manter o livro visível e emprestado. (RN-11)
- CA-15: QUANDO o cadastro ou a alteração não informar o ano, O SISTEMA DEVE rejeitar com erro de validação. (RN-03)
- CA-16: QUANDO título ou autor passar de 200 caracteres, ou o nome da pessoa passar de 100, O SISTEMA DEVE rejeitar com erro de validação indicando o campo. (RN-12)
- CA-17: QUANDO for cadastrado um livro com título e autor iguais aos de outro já existente, O SISTEMA DEVE aceitá-lo como um novo livro. (RN-13)
- CA-18: QUANDO a busca for feita com termo vazio, O SISTEMA DEVE rejeitar com erro de validação. (RF-04)

## Clarificações

Registro das ambiguidades resolvidas em 2026-10-12. Não há perguntas em aberto.

- Um livro emprestado pode ser removido? Não, ele precisa ser devolvido antes. → RN-11, CA-14
- O ano é obrigatório? Sim. → RN-03, CA-15
- Pode haver exemplares repetidos (mesmo título e autor)? Sim. → RN-13, CA-17
- Tamanho máximo dos textos? 200 caracteres para título e autor, 100 para a pessoa. → RN-12, CA-16
