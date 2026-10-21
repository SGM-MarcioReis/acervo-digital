@model IEnumerable<AcervoDigital.Models.Imagem>

@{
    ViewData["Title"] = "Painel administrativo";
}

<div class="dashboard-header">
    <div>
        <span class="eyebrow">Painel administrativo</span>
        <h1>Gerenciamento do acervo</h1>
    </div>
    <div class="header-actions">
        <a asp-action="Create" class="btn btn-primary">Nova imagem</a>
        <a asp-action="Logout" class="btn btn-secondary">Sair</a>
    </div>
</div>

@if (!Model.Any())
{
    <div class="empty-state admin-empty">
        <h2>Nenhuma imagem cadastrada</h2>
        <p>Cadastre sua primeira imagem para começar a organização do acervo.</p>
    </div>
}
else
{
    <div class="admin-grid">
        @foreach (var imagem in Model)
        {
            <article class="admin-card">
                <a href="@imagem.CaminhoArquivo"
                   class="image-open"
                   data-image="@imagem.CaminhoArquivo"
                   data-title="@imagem.Titulo"
                   data-description="@(!string.IsNullOrWhiteSpace(imagem.Descricao) ? imagem.Descricao : "Sem descrição disponível.")">
                    <img src="@imagem.CaminhoArquivo" alt="@imagem.Titulo" />
                </a>
                <div class="admin-body">
                    <span class="tag">@imagem.Categoria</span>
                    <h3>@imagem.Titulo</h3>
                    @if (!string.IsNullOrWhiteSpace(imagem.Descricao))
                    {
                        <p>@imagem.Descricao</p>
                    }
                    <small>@imagem.DataCadastro.ToString("dd/MM/yyyy")</small>
                    <div class="admin-actions">
                        <a asp-action="Edit" asp-route-id="@imagem.Id" class="btn btn-secondary">Editar</a>
                        <form asp-action="Delete" method="post">
                            @Html.AntiForgeryToken()
                            <input type="hidden" name="id" value="@imagem.Id" />
                            <button type="submit" class="btn btn-danger">Excluir</button>
                        </form>
                    </div>
                </div>
            </article>
        }
    </div>
}

<div class="image-modal" id="imageModal" aria-hidden="true">
    <div class="image-modal-content">
        <button type="button" class="image-modal-close" aria-label="Fechar">×</button>
        <img class="image-modal-image" src="" alt="Imagem ampliada" />
        <div class="image-modal-body">
            <h3 class="image-modal-title"></h3>
            <p class="image-modal-description"></p>
        </div>
    </div>
</div>
