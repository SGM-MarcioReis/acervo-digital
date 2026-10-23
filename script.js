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

const presentationTitleInput = document.getElementById('presentation-title');
const presentationPeriodInput = document.getElementById('presentation-period');
const generatePdfButton = document.getElementById('btn-generate-pdf');
const clearButton = document.getElementById('btn-clear');

const defaultPresentationTitle = 'Prestação de Contas';
const defaultPresentationPeriod = '2025 / 2026';

function updatePresentationMeta() {
  const title = (presentationTitleInput?.value || '').trim() || defaultPresentationTitle;
  const period = (presentationPeriodInput?.value || '').trim() || defaultPresentationPeriod;

  const titleNodes = document.querySelectorAll('.slide-title, .slide-badge');
  titleNodes.forEach((node) => {
    if (node.classList.contains('slide-badge')) {
      node.textContent = title;
      return;
    }

    node.textContent = title;
  });

  document.querySelectorAll('.slide-period').forEach((node) => {
    node.textContent = period;
  });
}

if (presentationTitleInput) {
  presentationTitleInput.addEventListener('input', updatePresentationMeta);
}

if (presentationPeriodInput) {
  presentationPeriodInput.addEventListener('input', updatePresentationMeta);
}

if (clearButton) {
  clearButton.addEventListener('click', () => {
    if (presentationTitleInput) presentationTitleInput.value = defaultPresentationTitle;
    if (presentationPeriodInput) presentationPeriodInput.value = defaultPresentationPeriod;
    updatePresentationMeta();
  });
}

async function exportPresentationAsPdf() {
  const preview = document.getElementById('presentation-export');

  if (!preview) {
    alert('Não foi possível localizar a pré-visualização da apresentação.');
    return;
  }

  const slides = Array.from(preview.querySelectorAll('.slide'));

  if (!slides.length) {
    alert('Não há slides para exportar.');
    return;
  }

  if (!window.jspdf || !window.jspdf.jsPDF) {
    alert('A biblioteca de PDF não foi carregada.');
    return;
  }

  const { jsPDF } = window.jspdf;
  const pdf = new jsPDF('p', 'mm', 'a4');

  const safeTitle = (presentationTitleInput?.value || '').trim() || defaultPresentationTitle;
  const filename = safeTitle
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-+|-+$/g, '') || 'prestacao-de-contas';

  for (let index = 0; index < slides.length; index += 1) {
    const slide = slides[index];
    const canvas = await html2canvas(slide, {
      scale: 2,
      backgroundColor: '#ffffff',
      useCORS: true,
    });

    const imgData = canvas.toDataURL('image/png');
    const pageWidth = pdf.internal.pageSize.getWidth();
    const pageHeight = pdf.internal.pageSize.getHeight();

    const ratio = Math.min(pageWidth / canvas.width, pageHeight / canvas.height);
    const imgWidth = canvas.width * ratio;
    const imgHeight = canvas.height * ratio;
    const x = (pageWidth - imgWidth) / 2;
    const y = (pageHeight - imgHeight) / 2;

    if (index > 0) {
      pdf.addPage();
    }

    pdf.addImage(imgData, 'PNG', x, y, imgWidth, imgHeight);
  }

  pdf.save(`${filename}.pdf`);
}

if (generatePdfButton) {
  generatePdfButton.addEventListener('click', exportPresentationAsPdf);
}

updatePresentationMeta();
