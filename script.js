const yearEl = document.getElementById('year');
if (yearEl) {
  yearEl.textContent = new Date().getFullYear();
}

const filterButtons = document.querySelectorAll('.filter');
const cards = document.querySelectorAll('.card');

filterButtons.forEach((button) => {
  button.addEventListener('click', () => {
    const filter = button.dataset.filter;

    filterButtons.forEach((item) => item.classList.toggle('active', item === button));

    cards.forEach((card) => {
      const category = card.dataset.category;
      const shouldShow = filter === 'all' || category === filter;
      card.classList.toggle('hidden', !shouldShow);
    });
  });
});














































































