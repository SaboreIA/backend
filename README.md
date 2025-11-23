# 🍽️ SaboreIA API

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791)](https://www.postgresql.org/)
[![Supabase](https://img.shields.io/badge/Supabase-Database-3ECF8E)](https://supabase.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)
[![C#](https://img.shields.io/badge/C%23-12.0-239120)](https://docs.microsoft.com/en-us/dotnet/csharp/)

## 📖 Sobre o Projeto

**SaboreIA** é uma API REST inteligente para recomendação de restaurantes, desenvolvida em .NET 8. A aplicação utiliza inteligência artificial (integração com Perplexity AI) para processar preferências dos usuários e fornecer recomendações personalizadas de restaurantes, produtos e categorias.

### ✨ Principais Funcionalidades

- 🤖 **Recomendações Inteligentes**: Sistema de IA que interpreta preferências e sugere restaurantes
- 👥 **Gerenciamento de Usuários**: Cadastro com diferentes perfis (USER, OWNER, ADMIN)
- 🏪 **Gestão de Restaurantes**: CRUD completo com imagens, horários e cardápios
- ⭐ **Sistema de Avaliações**: Reviews e ratings de restaurantes
- ❤️ **Favoritos**: Marque seus restaurantes preferidos
- 🏷️ **Tags e Categorias**: Organização por tipos de culinária e produtos
- 🔐 **Autenticação JWT**: Sistema seguro com autenticação e autorização
- 📸 **Upload de Imagens**: Integração com Cloudinary para armazenamento

## ⚙️ Tecnologias Utilizadas

### Backend
- **.NET 8** - Framework principal
- **Entity Framework Core 8** - ORM para acesso a dados
- **PostgreSQL 16** - Banco de dados relacional
- **Supabase** - Backend-as-a-Service (BaaS) para PostgreSQL
- **JWT (JSON Web Tokens)** - Autenticação e autorização
- **BCrypt.Net** - Hash seguro de senhas
- **Swagger/OpenAPI** - Documentação da API

### Integrações
- **Perplexity AI** - Processamento de linguagem natural para recomendações
- **Cloudinary** - Armazenamento e gerenciamento de imagens

### Principais Pacotes NuGet
```
BCrypt.Net-Next (4.0.3)
CloudinaryDotNet (1.27.8)
Microsoft.AspNetCore.Authentication.JwtBearer (8.0.0)
Npgsql.EntityFrameworkCore.PostgreSQL (8.0.8)
System.IdentityModel.Tokens.Jwt (8.14.0)
Swashbuckle.AspNetCore (9.0.6)
```

## 🏗️ Arquitetura

O projeto segue uma arquitetura em camadas bem definida:

```
SaboreIA/
├── Controllers/          # Endpoints da API
├── Models/              # Entidades do domínio
├── DTOs/                # Data Transfer Objects
├── Services/            # Lógica de negócio
│   └── IA/             # Serviços de inteligência artificial
├── Repositories/        # Acesso a dados
├── Interfaces/          # Contratos de serviços e repositórios
│   ├── Service/
│   └── Repository/
├── Database/            # Contexto do EF Core
├── Authorization/       # Políticas de autorização customizadas
├── Config/             # Configurações da aplicação
├── Integrations/       # Clientes de APIs externas
└── Migrations/         # Migrações do banco de dados
```

## 🚀 Como Executar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Conta [Supabase](https://supabase.com/) (para banco de dados PostgreSQL hospedado)
- Conta [Cloudinary](https://cloudinary.com/) (para upload de imagens)
- API Key da [Perplexity AI](https://www.perplexity.ai/)

### Configuração

1. **Clone o repositório**
```bash
git clone https://github.com/SaboreIA/backend.git
cd backend
```

2. **Configure o appsettings.json**

Crie um arquivo `appsettings.json` na raiz do projeto:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.your-project.supabase.co;Database=postgres;Username=postgres;Password=sua_senha_supabase"
  },
  "JwtSettings": {
    "SecretKey": "sua_chave_secreta_jwt_aqui_com_pelo_menos_32_caracteres",
    "Issuer": "SaboreIA",
    "Audience": "SaboreIA-Users",
    "ExpirationInMinutes": 60
  },
  "Cloudinary": {
    "CloudName": "seu_cloud_name",
    "ApiKey": "sua_api_key",
    "ApiSecret": "seu_api_secret"
  },
  "IAServiceConfig": {
    "ApiKey": "sua_perplexity_api_key",
    "ApiUrl": "https://api.perplexity.ai/chat/completions"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

> **Nota**: Para obter sua connection string do Supabase:
> 1. Acesse seu projeto no [Supabase Dashboard](https://app.supabase.com/)
> 2. Vá em Settings > Database
> 3. Copie a **Connection String** e substitua `[YOUR-PASSWORD]` pela sua senha

3. **Restaure as dependências**
```bash
dotnet restore
```

4. **Execute as migrações do banco de dados**
```bash
dotnet ef database update
```

5. **Execute a aplicação**
```bash
dotnet run
```

A API estará disponível em:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger**: `http://localhost:5000/swagger`

## 📚 Documentação da API

Após executar o projeto, acesse a documentação interativa do Swagger em:

```
http://localhost:5000/swagger
```

### Principais Endpoints

#### Autenticação
- `POST /api/auth/register` - Registrar novo usuário
- `POST /api/auth/login` - Login e obtenção de token JWT

#### Usuários
- `GET /api/user` - Listar usuários
- `GET /api/user/{id}` - Obter usuário por ID
- `PUT /api/user/{id}` - Atualizar usuário
- `DELETE /api/user/{id}` - Remover usuário

#### Restaurantes
- `GET /api/restaurant` - Listar restaurantes (com paginação)
- `GET /api/restaurant/{id}` - Detalhes de um restaurante
- `POST /api/restaurant` - Criar restaurante (apenas OWNER/ADMIN)
- `PUT /api/restaurant/{id}` - Atualizar restaurante
- `DELETE /api/restaurant/{id}` - Remover restaurante

#### Avaliações
- `GET /api/review/restaurant/{restaurantId}` - Reviews de um restaurante
- `POST /api/review` - Criar avaliação
- `PUT /api/review/{id}` - Atualizar avaliação
- `DELETE /api/review/{id}` - Remover avaliação

#### Favoritos
- `GET /api/favorite` - Listar favoritos do usuário
- `POST /api/favorite` - Adicionar favorito
- `DELETE /api/favorite/{id}` - Remover favorito

#### Tags e Produtos
- `GET /api/tag` - Listar categorias/tags
- `GET /api/products` - Listar produtos

## 🔑 Autenticação

A API utiliza **JWT (JSON Web Tokens)** para autenticação. 

### Como usar:

1. Faça login através do endpoint `/api/auth/login`
2. Copie o token JWT retornado
3. Adicione o token no header das requisições:
```
Authorization: Bearer {seu_token_aqui}
```

### Roles disponíveis:
- **USER**: Usuário comum (pode fazer reviews, favoritar restaurantes)
- **OWNER**: Proprietário de restaurante (pode gerenciar seus restaurantes)
- **ADMIN**: Administrador (acesso total ao sistema)

## 🧠 Sistema de IA

O SaboreIA utiliza integração com Perplexity AI para:

1. **Interpretar preferências**: Converte entrada do usuário em categorias estruturadas
2. **Identificar produtos**: Reconhece menções a pratos e alimentos
3. **Categorizar automaticamente**: Associa produtos a tags adequadas
4. **Aprendizado contínuo**: Expande base de produtos e categorias

### Fluxo de Recomendação

```
Entrada do Usuário → Perplexity AI → Análise de Produto → 
Tag/Categoria → Busca de Restaurantes → Recomendação
```

## 🌐 CORS

A API está configurada para aceitar requisições de:
- `http://localhost:5173` (Vite/React)
- `http://localhost:8080` (Vue.js)
- `http://localhost:3000` (Next.js/React)

Para adicionar outras origens, edite a configuração CORS em `Program.cs`.

## 💾 Estrutura do Banco de Dados

### Principais Entidades

- **User**: Usuários do sistema
- **Restaurant**: Restaurantes cadastrados
- **Address**: Endereços (usuários e restaurantes)
- **Tag**: Categorias/tipos de culinária
- **Product**: Produtos/pratos
- **Review**: Avaliações de restaurantes
- **Favorite**: Restaurantes favoritos dos usuários

## 🧪 Testes

```bash
# Executar testes (quando implementados)
dotnet test
```

## 🔄 Migrações

```bash
# Criar nova migração
dotnet ef migrations add NomeDaMigracao

# Atualizar banco de dados
dotnet ef database update

# Reverter migração
dotnet ef database update MigracaoAnterior
```

## 🤝 Contribuindo

Contribuições são bem-vindas! Sinta-se à vontade para:

1. Fazer um Fork do projeto
2. Criar uma branch para sua feature (`git checkout -b feature/NovaFuncionalidade`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova funcionalidade'`)
4. Push para a branch (`git push origin feature/NovaFuncionalidade`)
5. Abrir um Pull Request

## 👨‍💻 Autor

**Estevão Alves**
- Email: ealves1710@hotmail.com
- GitHub: [@SaboreIA](https://github.com/SaboreIA)

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 🗺️ Roadmap

- [ ] Implementar testes unitários e de integração
- [ ] Sistema de notificações em tempo real
- [ ] Filtros avançados de busca com geolocalização
- [ ] Sistema de recompensas/gamificação
- [ ] Integração com redes sociais
- [ ] App mobile (React Native/Flutter)
- [ ] Dashboard analytics para proprietários

## 💬 Suporte

Para reportar bugs ou sugerir melhorias, abra uma [issue](https://github.com/SaboreIA/backend/issues) no GitHub.

---

⭐ Se este projeto foi útil para você, considere dar uma estrela no GitHub!
