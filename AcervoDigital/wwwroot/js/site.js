document.addEventListener('DOMContentLoaded', () => {
    const year = document.getElementById('year');
    if (year) {
        year.textContent = new Date().getFullYear();
    }

    const modal = document.getElementById('imageModal');
    if (!modal) return;

    const modalImage = modal.querySelector('.image-modal-image');
    const modalTitle = modal.querySelector('.image-modal-title');
    const modalDescription = modal.querySelector('.image-modal-description');
    const closeButton = modal.querySelector('.image-modal-close');

    const closeModal = () => {
        modal.classList.remove('is-open');
        modal.setAttribute('aria-hidden', 'true');
        modalImage.src = '';
        modalTitle.textContent = '';
        modalDescription.textContent = '';
    };

    document.querySelectorAll('.image-open').forEach((link) => {
        link.addEventListener('click', (event) => {
            event.preventDefault();

            const image = link.dataset.image || link.getAttribute('href');
            const title = link.dataset.title || 'Imagem';
            const description = link.dataset.description || 'Sem descrição disponível.';

            modalImage.src = image;
            modalTitle.textContent = title;
            modalDescription.textContent = description;

            modal.classList.add('is-open');
            modal.setAttribute('aria-hidden', 'false');
        });
    });

    closeButton?.addEventListener('click', closeModal);

    modal.addEventListener('click', (event) => {
        if (event.target === modal) {
            closeModal();
        }
    });

    document.addEventListener('keydown', (event) => {
        if (event.key === 'Escape' && modal.classList.contains('is-open')) {
            closeModal();
        }
    });

    const buttons = document.querySelectorAll('.filter');
    buttons.forEach(button => {
        button.addEventListener('click', () => {
            const filter = button.dataset.filter;
            const cards = document.querySelectorAll('.card');

            buttons.forEach(btn => btn.classList.remove('active'));
            button.classList.add('active');

            cards.forEach(card => {
                const category = card.dataset.category;
                const shouldShow = filter === 'all' || category === filter;
                card.style.display = shouldShow ? 'block' : 'none';
            });
        });
    });
});
