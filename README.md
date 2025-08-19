# TwoWheels

Documentação básica e coesa do projeto.

## 1. Visão Geral
TwoWheels foi pensado para ser uma aplicação .NET Functions para implementar endpoints de API's + Events,
por isso adotei um estilo híbrido (síncrono + orientado a eventos) para suportar evolução e escalabilidade.

## 2. Arquitetura (Visão Simplificada)

Principais componentes:
- API .NET (Web) expõe endpoints REST (CRUD).
- Padrões comportamentais: Command + Mediator para desacoplar Controllers da lógica de negócio.
- Singleton (via DI do .NET) para serviços que precisam de uma única instância (ex.: event publisher).
- PostgreSQL armazena dados transacionais (estado atual).
- RabbitMQ transporta eventos (ex.: MotorcycleCreated).
- Azure Function consome a fila e trata o evento.
- MongoDB armazena notificações/eventos derivados (visão orientada a leitura / histórico).
- Decorator + logs para rastreabilidade.

### Diagrama ASCII

```
           +-----------+
           |  Cliente  |
           +-----+-----+
                 |
             HTTP REST
                 v
       +----------------------+
       | API .NET (Web)       |
       | Controllers          |
       |  + Mediator          |
       |  + Commands/Handlers |
       +--+----+----+---------+
          |    |    |
          |    |    +--> Logs (Decorator)
          |    |
          |    +--> PostgreSQL (CRUD)
          |
          +--> Evento "MotorcycleCreated"
                       |
                       v
                 RabbitMQ Queue
                       |
                       v
             +--------------------+
             | Azure Function     |
             | (RabbitMQ Trigger) |
             +---------+----------+
                       |
                   Persiste
                       v
            MongoDB (MotorcycleNotifications)
```

## 3. Benefícios para Escalabilidade
- Desacoplamento: Command + Mediator facilita evolução sem “espalhar” regras nos controllers.
- Event-driven: Produção/consumo assíncrono via RabbitMQ reduz acoplamento temporal e permite múltiplos consumidores futuros.
- Separação de persistências: PostgreSQL (estado consistente) + MongoDB (eventos / notificações) melhora leitura e relatórios sem sobrecarregar o banco transacional.
- Azure Functions: Escala sob demanda no processamento de eventos.
- Logs estruturados: Facilitam observabilidade e troubleshooting.
- Extensibilidade: Fila permite adicionar novos fluxos (ex.: analytics, auditoria) sem mudar a API.

## 4. Padrões de Projeto Usados
- Command: encapsula cada ação (ex.: criar moto).
- Mediator: centraliza o dispatch de comandos/queries.
- Singleton (via DI): reaproveitamento de instâncias necessárias (ex.: contextos configurados / publisher).
- Decorator (para logging): acrescenta rastreabilidade sem alterar lógica central.

## 5. Decisões e Observações
- Endpoints DELETE retornando 200/400 em vez de 204 (padrão). Em produção, preferir:
  - 204 No Content ou
  - Soft delete (flag de desativação) evitando remoção física.
- Mensagens/eventos simplificados propositalmente (escopo de teste).
- Estrutura preparada para enriquecer payloads e padronizar contratos.
- Visando agilidade, apenas a function de entregadores (deliverers) foi feita de forma mais “robusta”. Em uma aplicação real, a mesma abordagem seria aplicada às demais entidades.

## 6. Passo a Passo para Execução (Ambiente Local)

Pré-requisitos:
- Docker & Docker Compose
- .NET SDK instalado
- Ferramentas de acesso: pgAdmin (opcional), Mongo Express (já exposta), interface RabbitMQ (management plugin)

### 6.1 Subir infraestrutura básica
```
docker-compose up -d
```

### 6.2 Criar migração e aplicar no PostgreSQL
(Execute no projeto onde está o DbContext)
```
dotnet ef migrations add InitialCreate
dotnet ef database update --connection "Host=localhost;Port=5432;Database=twowheels;Username=admin;Password=admin123;"
```

### 6.3 Preparar MongoDB
Acessar:
```
http://localhost:8081/
```
Criar:
- Database: TwoWheelsDb
- Collection: MotorcycleNotifications

### 6.4 Preparar RabbitMQ
Acessar painel (geralmente em http://localhost:15672/).  
Criar fila:
- Nome: motorcycle-created-queue
- Configurações padrão (durable, etc.)

### 6.5 Executar a aplicação
Iniciar a API (`dotnet run`) e/ou Azure Functions (caso em projeto separado).

Testar criação de motocicleta via endpoint (ex.: POST /motorcycles).  
Verificar:
- Registro no PostgreSQL
- Evento na fila
- Consumo pela Function
- Documento/Notificação no MongoDB

## 7. Testes (Unitários e Integração)
- Como as entidades são lógicamente iguais (function -> command -> validator -> handlers -> repository) o foco nos testes foi em demonstrar conhecimento
e não para alcançar plenitude na cobertura dos testes, mas poderia ser facilmente implementado no restante da aplicação. Em um projeto real sabemos
que poderia facilmente ultrapassar +300 testes (combinação de casos de validação, erros, endpoints, variações de persistência e eventos).

Foram feitos testes:
- Unitários: Entities, Commands, Handlers, Validators, Events (focam lógica isolada)
- Integração: Integration.Deliverer (exercita cenário cruzando camadas)

## 8. Logs e Observabilidade
- Decorators adicionam logs em pontos críticos (entrada/saída handlers).
- Gostaria de ter utilizado CorrelationId / TraceId para uma melhor rastreabilidade da entidade, porém a aplicação ficaria mais complexa, fugindo do escopo solicitado.

## 9. Status avaliado dos diferenciais solicitados

| Diferencial | Situação | Observação |
|-------------|----------|-----------|
| Testes unitários | Atendido (parcial) | Cobrem entidades, comandos, handlers, validators, eventos principais. |
| Testes de integração | Atendido (parcial) | Fluxo Deliverer; expandir para controllers e Azure Function. |
| EntityFramework e/ou Dapper | Atendido | Uso de EF (migrations). |
| Docker e Docker Compose | Atendido | Orquestra PostgreSQL, RabbitMQ, Mongo, etc. |
| Design Patterns | Atendido | Command, Mediator, Decorator, Singleton (DI). |
| Documentação | Atendido | README estruturado (este arquivo). |
| Tratamento de erros | Parcial | Base existente; formalizar middleware / mapeamento de exceções. |
| Arquitetura e modelagem de dados | Atendido | Separação de responsabilidades + event-driven para notificações. |
| Código em inglês | Atendido | Nomes de classes, namespaces e membros em inglês. |
| Código limpo e organizado | Atendido (com espaço p/ refinamento) | Segue convenções .NET; pode evoluir modularização. |
| Logs bem estruturados | Atendido | Decorator + estruturação. |
| Convenções da comunidade | Majoritariamente atendido | REST quase padrão (ajustar DELETE), DI/mediator conforme práticas. |


## 10. Resumo
A arquitetura favorece escalabilidade horizontal (processamento assíncrono), evolução modular e observabilidade básica. Testes concentram-se em áreas críticas (validação e fluxo deliverer) para demonstrar qualidade sem inflar o escopo de um MVP.

## 11. Em um projeto real o que mais eu implementaria... caso eu fosse contrato rsrs 🤭:
- Padronizar contratos de resposta (envelopes, códigos).
- Implementar soft delete (flag IsActive).
- Health checks + métricas/dashboards.
- Políticas de retry / DLQ para falhas no consumo de eventos.
- Autenticação.
- CI/CD + Sonar.
---
