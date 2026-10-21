# Acervo Digital

Este projeto foi atualizado para a versão 2 com:

- layout mais profissional e inspirado no modelo visual demonstrado
- painel administrativo com login
- upload, edição e exclusão de imagens
- armazenamento no banco SQLite
- salvar imagens na pasta `wwwroot/uploads`

## Login administrativo

Acesso padrão para teste local:

- usuário: `admin`
- senha: `admin123`

## Como abrir no Visual Studio

1. Abra o projeto `AcervoDigital/AcervoDigital.csproj`
2. Restaure os pacotes NuGet
3. Pressione `F5`
4. Acesse a URL do projeto local, normalmente `https://localhost:xxxx`
5. Acesse `/Admin/Login` para entrar no painel administrativo

## Funcionalidades

- cadastro de imagens
- descrição por categoria
- edição de texto e imagem
- exclusão com remoção do arquivo físico
- visualização em galeria pública

## Banco de dados

O SQLite é criado automaticamente na primeira execução.

Arquivo:

- `AcervoDigital/acervo.db`
