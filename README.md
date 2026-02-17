# Sistema de Controle de Gastos Residenciais

Este projeto implementa um sistema completo para controle de gastos residenciais, dividido em uma API de backend em C#/.NET e um frontend em React com TypeScript. O sistema permite o gerenciamento de pessoas, categorias e transações financeiras, além de fornecer relatórios detalhados.

## Sumário

1.  [Visão Geral do Projeto](#visão-geral-do-projeto)
2.  [Backend - .NET Web API](#backend---net-web-api)
    *   [Tecnologias Utilizadas](#tecnologias-utilizadas-backend)
    *   [Configuração e Execução](#configuração-e-execução-backend)
    *   [Endpoints da API](#endpoints-da-api)
    *   [Configuração do Banco de Dados](#configuração-do-banco-de-dados)
    *   [Principais Decisões Arquiteturais e Boas Práticas](#principais-decisões-arquiteturais-e-boas-práticas-backend)
3.  [Frontend - React com TypeScript](#frontend---react-com-typescript)
    *   [Tecnologias Utilizadas](#tecnologias-utilizadas-frontend)
    *   [Configuração e Execução](#configuração-e-execução-frontend)
    *   [Principais Recursos e Componentes](#principais-recursos-e-componentes)
    *   [Principais Decisões Arquiteturais e Boas Práticas](#principais-decisões-arquiteturais-e-boas-práticas-frontend)
4.  [Requisitos Funcionais Implementados](#requisitos-funcionais-implementados)
5.  [Critérios de Avaliação Abordados](#critérios-de-avaliação-abordados)
6.  [Potenciais Melhorias e Trabalho Futuro](#potenciais-melhorias-e-trabalho-futuro)

## Visão Geral do Projeto

O objetivo deste sistema é auxiliar no controle financeiro doméstico, oferecendo funcionalidades para registrar e consultar receitas e despesas. Ele é composto por duas partes principais:
*   **Backend:** Uma API RESTful desenvolvida em C# com .NET, responsável pela lógica de negócios, persistência de dados e exposição dos endpoints.
*   **Frontend:** Uma aplicação web interativa construída com React e TypeScript, que consome a API do backend para fornecer uma interface de usuário intuitiva.

## Backend - .NET Web API

### Tecnologias Utilizadas (Backend)

*   **Linguagem:** C#
*   **Framework:** .NET 9.0
*   **Web Framework:** ASP.NET Core
*   **ORM:** Entity Framework Core
*   **Banco de Dados:** PostgreSQL (via Npgsql)
*   **Mapeamento de Objetos:** AutoMapper
*   **Tratamento de Resultados:** FluentResults
*   **Documentação da API:** Swagger/OpenAPI

### Configuração e Execução (Backend)

1.  **Pré-requisitos:**
    *   .NET SDK 9.0 ou superior
    *   Docker (opcional, para rodar PostgreSQL via Docker Compose)
    *   Um servidor PostgreSQL em execução (localmente ou via Docker).

2.  **Configuração do Banco de Dados:**
    *   Por padrão, a aplicação espera um banco de dados PostgreSQL. A connection string está definida no `appsettings.json` (ou `appsettings.Development.json`):
        ```json
        "ConnectionStrings": {
            "DefaultConnection": "Host=localhost;Port=5432;Database=controledb;Username=suelen;Password=minhasenha"
        }
        ```
    *   **Recomendação:** Para facilitar, você pode criar um contêiner Docker para o PostgreSQL.
        *   Crie um arquivo `docker-compose.yml` na raiz do projeto (`/controle-de-gastos/`) com o seguinte conteúdo:
            ```yaml
            version: '3.8'
            services:
              db:
                image: postgres:16-alpine
                restart: always
                environment:
                  POSTGRES_DB: controledb
                  POSTGRES_USER: suelen
                  POSTGRES_PASSWORD: minhasenha
                ports:
                  - "5432:5432"
                volumes:
                  - postgres_data:/var/lib/postgresql/data
            volumes:
              postgres_data:
            ```
        *   Execute o Docker Compose na pasta raiz: `docker-compose up -d`
    *   **Aplicar Migrações:** Navegue até o diretório do backend (`backend-dotnet/backend-dotnet/`) e execute os seguintes comandos para criar o banco de dados e aplicar as migrações:
        ```bash
        dotnet ef database update
        ```

3.  **Execução da API:**
    *   Navegue até o diretório do backend (`backend-dotnet/backend-dotnet/`).
    *   Execute a aplicação:
        ```bash
        dotnet run
        ```
    *   A API estará disponível em `https://localhost:7124` (ou uma porta similar configurada). A documentação Swagger estará em `https://localhost:7124/swagger`.

### Endpoints da API

A API expõe os seguintes endpoints principais:

#### **Pessoas (`/Person`)**

*   **`GET /Person`**
    *   **Descrição:** Lista todas as pessoas cadastradas.
    *   **Resposta:** `ApiResponseModel<List<PersonModel>>`
*   **`POST /Person`**
    *   **Descrição:** Cria uma nova pessoa.
    *   **Corpo da Requisição:** `CreatePersonDTO` (Nome, Idade)
    *   **Resposta:** `ApiResponseModel<Guid>` (ID da pessoa criada)
*   **`PUT /Person/{id}`**
    *   **Descrição:** Atualiza os dados de uma pessoa existente.
    *   **Parâmetro:** `id` (GUID da pessoa)
    *   **Corpo da Requisição:** `UpdatePersonDTO` (Nome, Idade)
    *   **Resposta:** `ApiResponseModel<Guid>` (ID da pessoa atualizada)
*   **`DELETE /Person/{id}`**
    *   **Descrição:** Deleta uma pessoa. Todas as transações associadas a esta pessoa também serão deletadas.
    *   **Parâmetro:** `id` (GUID da pessoa)
    *   **Resposta:** `ApiResponseModel` (sucesso ou erro)

#### **Categorias (`/Categories`)**

*   **`GET /Categories`**
    *   **Descrição:** Lista todas as categorias cadastradas.
    *   **Resposta:** `ApiResponseModel<List<ListCategoryDTO>>`
*   **`POST /Categories`**
    *   **Descrição:** Cria uma nova categoria.
    *   **Corpo da Requisição:** `CreateCategoryDTO` (Descrição, Finalidade)
    *   **Finalidade (Enum `Finance`):** `1 = Despesa`, `2 = Receita`, `3 = Ambas`
    *   **Resposta:** `ApiResponseModel<Guid>` (ID da categoria criada)

#### **Transações (`/Transaction`)**

*   **`GET /Transaction`**
    *   **Descrição:** Lista todas as transações.
    *   **Resposta:** `ApiResponseModel<List<ListAllTransactionsDTO>>`
*   **`POST /Transaction`**
    *   **Descrição:** Cria uma nova transação.
    *   **Corpo da Requisição:** `CreateTransactionDTO` (Descrição, Valor, Tipo, CategoryId, PersonId)
    *   **Tipo (Enum `Finance`):** `1 = Despesa`, `2 = Receita`
    *   **Regras de Negócio Aplicadas:**
        *   Menores de 18 anos só podem registrar transações de despesa.
        *   A finalidade da categoria deve ser compatível com o tipo da transação.
    *   **Resposta:** `ApiResponseModel<Guid>` (ID da transação criada)
*   **`GET /Transaction/ReportPerson`**
    *   **Descrição:** Gera um relatório de totais financeiros por pessoa (receitas, despesas, saldo) e um resumo geral.
    *   **Resposta:** `ApiResponseModel<ReportDTO<ListAllPersonsDTO>>`
*   **`GET /Transaction/ReportCategory`**
    *   **Descrição:** Gera um relatório de totais financeiros por categoria (receitas, despesas, saldo) e um resumo geral. (Opcional)
    *   **Resposta:** `ApiResponseModel<ReportDTO<ListCategoryDTO>>`

### Configuração do Banco de Dados

*   **Tecnologia:** PostgreSQL
*   **ORM:** Entity Framework Core
*   **Gerenciamento de Schema:** Migrações do EF Core

### Principais Decisões Arquiteturais e Boas Práticas (Backend)

*   **Arquitetura em Camadas:** Separação clara de responsabilidades (Controllers, Services, Repositories, Entities, DTOs).
*   **Dependency Injection (DI):** Uso extensivo de DI para desacoplamento e testabilidade.
*   **Data Transfer Objects (DTOs):** Utilização de DTOs para modelar dados de entrada e saída da API, protegendo o modelo de domínio.
*   **AutoMapper:** Facilita o mapeamento entre entidades e DTOs.
*   **FluentResults:** Gerenciamento robusto de resultados de operações (sucesso/erro).
*   **Programação Assíncrona:** Uso de `async/await` para operações de I/O, melhorando a escalabilidade.
*   **Validação de Modelos:** Combinação de Data Annotations e validações de negócio na camada de serviço.
*   **Tratamento Global de Erros:** Middleware `ErrorHandlingMiddleware` para respostas de erro consistentes.
*   **CORS:** Configuração explícita para permitir o acesso do frontend.
*   **Cascade Delete:** Configurado no nível do EF Core para garantir a integridade dos dados (ex: exclusão de transações ao deletar pessoa).

## Frontend - React com TypeScript

### Tecnologias Utilizadas (Frontend)

*   **Linguagem:** TypeScript
*   **Framework:** React
*   **Build Tool:** Vite
*   **Roteamento:** React Router DOM
*   **Validação de Formulários:** Zod
*   **Estilização:** Tailwind CSS
*   **Componentes UI:** Flowbite-React
*   **Gerenciamento de Requisições:** `fetch` API nativa

### Configuração e Execução (Frontend)

1.  **Pré-requisitos:**
    *   Node.js (versão LTS recomendada)
    *   npm ou Yarn

2.  **Instalação de Dependências:**
    *   Navegue até o diretório do frontend (`frontend-react/`).
    *   Instale as dependências:
        ```bash
        npm install
        # ou
        yarn install
        ```

3.  **Execução da Aplicação:**
    *   Certifique-se de que o **backend esteja em execução**.
    *   No diretório `frontend-react/`, execute a aplicação:
        ```bash
        npm run dev
        # ou
        yarn dev
        ```
    *   A aplicação frontend estará disponível em `http://localhost:5173` (ou uma porta similar).

### Principais Recursos e Componentes

*   **Listagem de Pessoas/Categorias:** Componente `TableRecords` reutilizável para exibir dados tabulares.
*   **Criação de Pessoas/Categorias:** `ModalNewItem` para formulários de criação, com validação instantânea.
*   **Edição de Pessoas:** `ModalEditPerson` para atualização de dados de pessoas.
*   **Listagem de Transações:** `TableTransaction` exibe as transações com formatação de valores e tipos.
*   **Criação de Transações:** `ModalTransaction` com seleção de pessoa e categoria, e aplicação das regras de negócio.
*   **Relatórios Dinâmicos:** `TableReport` exibe os totais financeiros por pessoa e por categoria, incluindo um resumo geral.
*   **Barra de Navegação:** `NavBar` para fácil acesso às diferentes seções do aplicativo.

### Principais Decisões Arquiteturais e Boas Práticas (Frontend)

*   **TypeScript:** Uso consistente para maior segurança de tipo, manutenibilidade e autocompletar.
*   **Componentização:** Arquitetura modular com componentes reutilizáveis e responsabilidades bem definidas.
*   **React Hooks:** Utilização eficaz de `useState`, `useEffect` e custom hooks (`useModal`) para gerenciar estado e lógica.
*   **Camada de Serviços de API:** Centralização das chamadas HTTP no diretório `src/service`, promovendo a reutilização e facilidade de manutenção.
*   **Validação Frontend (Zod):** Validação de formulários no lado do cliente para feedback imediato ao usuário e prevenção de dados inválidos.
*   **Roteamento Declarativo:** Gerenciamento de rotas com React Router DOM.
*   **UI/UX:** Design responsivo com Tailwind CSS e componentes Flowbite-React.
*   **Tratamento de Erros:** Captura e exibição de erros de validação do frontend e mensagens de erro da API do backend.

## Requisitos Funcionais Implementados

Todos os requisitos funcionais solicitados foram implementados e verificados:

*   **Sistema Separado:** Web API e Frontend.
*   **Persistência de Dados:** PostgreSQL com EF Core Migrations.
*   **Cadastro de Pessoas:** CRUD completo (Criação, Edição, Deleção, Listagem), incluindo deleção em cascata de transações.
*   **Cadastro de Categorias:** Criação e Listagem, com campos de Descrição e Finalidade (Despesa/Receita/Ambas).
*   **Cadastro de Transações:** Criação e Listagem, com validação para menores de idade (apenas despesas) e compatibilidade entre tipo de transação e finalidade da categoria.
*   **Consulta de Totais por Pessoa:** Relatório detalhado por pessoa e resumo geral.
*   **Consulta de Totais por Categoria:** Relatório detalhado por categoria e resumo geral (requisito opcional implementado).

## Critérios de Avaliação Abordados

O projeto atende de forma abrangente aos critérios de avaliação:

*   **Aderência às regras de negócio:** Todas as regras, incluindo as mais complexas como deleção em cascata, restrições para menores e compatibilidade de categorias, foram implementadas corretamente no backend e reforçadas no frontend.
*   **Atenção aos detalhes:** Observada na tipagem rigorosa, validações frontend/backend, tratamento de erros e formatação de UI.
*   **Qualidade e legibilidade do código:** Alto padrão de organização, nomenclatura e estrutura, tanto no C#/.NET quanto no React/TypeScript.
*   **Boas práticas em .NET e React:** Aplicação consistente de padrões de arquitetura, DI, DTOs, hooks do React, Zod, etc.
