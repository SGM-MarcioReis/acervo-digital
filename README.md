# Acervo Digital

Este repositório agora inclui um projeto ASP.NET Core para gerenciamento de imagens com banco de dados SQLite.

## Estrutura

- `AcervoDigital/` — projeto em ASP.NET Core MVC
- `AcervoDigital/wwwroot/uploads/` — pasta para armazenar as imagens enviadas

## Requisitos

- Visual Studio 2022
- .NET 8 SDK

## Como abrir no Visual Studio

1. Abra o projeto `AcervoDigital/AcervoDigital.csproj` no Visual Studio.
2. Restaure os pacotes NuGet.
3. Pressione F5 para rodar a aplicação.
4. Acesse a URL mostrada no navegador, normalmente `https://localhost:xxxx`.

## Funcionalidades

- Cadastro de imagens com título, categoria e descrição
- Upload de imagem para a pasta `wwwroot/uploads`
- Armazenamento dos dados no banco SQLite
- Exibição da galeria em página inicial
- Exclusão de registros e arquivos

## Banco de dados

O projeto usa SQLite e cria o banco automaticamente na primeira execução.

Arquivo do banco:

- `AcervoDigital/acervo.db`

## Usuário de exemplo

O sistema foi montado para funcionar sem configuração adicional de servidor externo.
