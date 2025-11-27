1. Visão Geral

O sistema é uma aplicação full-stack, composta por múltiplos serviços, com foco em gestão de produtos e vendas, geração de relatórios assíncronos, cache de informações em Redis e mensageria via RabbitMQ.

A solução foi organizada para permitir manutenção, escalabilidade e segregação de responsabilidades, contemplando as camadas de Domain, Application, Infrastructure, WebAPI, Frontend e Jobs.

Solução principal: desafio.sln

Estrutura do Projeto

Domain: Entidades, regras de negócio e agregados.

ApplicationService: Serviços de aplicação, orquestração de operações e regras de negócio.

Infra: Configurações de infraestrutura, acesso a dados (EF e Dapper), mensageria e cache Redis.

WebAPI: Endpoints REST, documentação Swagger, autorização e autenticação.

Frontend: SPA Angular, login, vitrine de produtos, CRUD e relatórios.

Job: Serviço de processamento assíncrono que consome filas RabbitMQ e gera relatórios.

2. Tecnologias Utilizadas

Backend

C# .NET 8

Entity Framework Core (CRUD)

Dapper (consultas de relatórios)

RabbitMQ (mensageria)

Redis (cache)

Swagger (documentação de API)

Frontend

Angular 17

Infraestrutura

Docker / Docker Compose


6. Boas Práticas Implementadas

Segregação de responsabilidades: Separação clara entre Domain, Application, Infra e Presentation.

Mensageria: Uso de RabbitMQ para processamento assíncrono de relatórios.

Cache: Redis para otimização de acessos frequentes (relatórios e carrinho).

Performance: Dapper para consultas de relatório complexas, EF Core para CRUD.

Segurança: Autenticação e autorização via roles.

Escalabilidade: Arquitetura modular, facilitando adição de novos serviços ou microsserviços.

Mensageria e Jobs

Job em .NET que consome a fila relatorio para processar geração de relatórios
