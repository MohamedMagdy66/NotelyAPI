const API_BASE_URL = 'https://localhost:44319/api/Notely';
const notesGrid = document.getElementById('notesGrid');
const noteForm = document.getElementById('noteForm');
const modalTitle = document.getElementById('modalTitle');
const categoryDropdown = document.getElementById('noteCategory');
const searchInput = document.getElementById('searchInput');
const emptyStateDiv = document.getElementById('emptyState');
const addNoteBtn = document.getElementById('addNoteBtn');
const filterTabsContainer = document.querySelector('.nav-tabs');
const showCompletedCheck = document.getElementById('showCompletedCheck'); 

const noteModal = new bootstrap.Modal(document.getElementById('noteModal'));

let allNotes = [];

async function apiFetch(endpoint = '', options = {}) {
    try {
        const response = await fetch(`${API_BASE_URL}/${endpoint}`, options);
        if (!response.ok) {
            const errorBody = await response.text();
            throw new Error(`HTTP error! status: ${response.status}, message: ${errorBody}`);
        }
        if (response.status === 204) return null;
        return await response.json();
    } catch (error) {
        console.error('API Fetch Error:', error);
        alert('An error occurred while communicating with the server. Please try again later.');
        throw error;
    }
}
function displayNotes(notes) {
    notesGrid.innerHTML = '';
    emptyStateDiv.classList.toggle('d-none', notes.length > 0);

    notes.forEach(note => {
        const noteCard = document.createElement('div');
        noteCard.className = 'col';
        const formattedDate = new Date(note.createdDate).toLocaleDateString('en-GB');
        noteCard.innerHTML = `
            <div class="card h-100 shadow-sm border-0 ${note.isCompleted ? 'note-completed' : ''}">
                <div class="card-body">
                    <span class="badge rounded-pill category-badge ${getCategoryColorClass(note.category)}">${note.category}</span>
                    <h5 class="card-title mt-2">${note.title}</h5>
                    <p class="card-text text-muted">${note.description || ''}</p>
                </div>
                <div class="card-footer d-flex justify-content-between align-items-center">
                    <small class="text-muted">${formattedDate}</small>
                    <div class="d-flex align-items-center">
                        <div class="form-check me-2">
                            <input class="form-check-input" 
                                   type="checkbox" 
                                   id="complete-${note.noteId}" 
                                   ${note.isCompleted ? 'checked' : ''} 
                                   onclick='handleToggleComplete(${JSON.stringify(note)})'>
                        </div>
                        <button class="btn btn-sm border-0" title="Edit" onclick='handleEdit(${JSON.stringify(note)})'>✏️</button>
                        <button class="btn btn-sm border-0" title="Delete" onclick="handleDelete(${note.noteId})">🗑️</button>
                    </div>
                </div>
            </div>
        `;
        notesGrid.appendChild(noteCard);
    });
}
function applyFilters() {
    const showCompletedOnly = showCompletedCheck.checked;
    const activeCategoryTab = document.querySelector('.nav-tabs .nav-link.active');
    const currentCategory = activeCategoryTab.dataset.category;

    let filteredNotes = [...allNotes];

    if (showCompletedOnly) {
        filteredNotes = filteredNotes.filter(note => note.isCompleted);
    }

    if (currentCategory.toLowerCase() !== 'all') {
        filteredNotes = filteredNotes.filter(note => note.category === currentCategory);
    }

    displayNotes(filteredNotes);
}

async function refreshAllNotes() {
    try {
        allNotes = await apiFetch(); 
        applyFilters(); 
    } catch (error) {
        console.error("فشل تحديث الملاحظات:", error);
    }
}


async function handleToggleComplete(note) {
    const updatedNote = { ...note, isCompleted: !note.isCompleted };
    try {
        await apiFetch(`${note.noteId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(updatedNote),
        });
        await refreshAllNotes();
    } catch (error) {
        console.error("Failed to update note completion status.", error);
    }
}

function getCategoryColorClass(category) {
    const colors = {
        "Business": "badge-business",
        "Home": "badge-home",
        "Personal": "badge-personal"
    };
    return colors[category] || 'bg-secondary';
}

function handleAdd() {
    noteForm.reset();
    document.getElementById('noteId').value = '';
    modalTitle.textContent = 'Add New Note';
    noteModal.show();
}

function handleEdit(note) {
    noteForm.reset();
    modalTitle.textContent = 'Edit Note';
    document.getElementById('noteId').value = note.noteId;
    document.getElementById('noteTitle').value = note.title;
    document.getElementById('noteDescription').value = note.description;
    document.getElementById('noteCategory').value = note.category;
    noteModal.show();
}

async function handleSubmit(event) {
    event.preventDefault();
    const id = document.getElementById('noteId').value;
    const originalNote = id ? allNotes.find(n => n.noteId == id) : {};
    const isCompleted = originalNote ? originalNote.isCompleted : false;

    const noteData = {
        noteId: id ? parseInt(id) : 0,
        title: document.getElementById('noteTitle').value,
        description: document.getElementById('noteDescription').value,
        category: document.getElementById('noteCategory').value,
        isCompleted: isCompleted
    };

    const endpoint = id ? `${id}` : '';
    const method = id ? 'PUT' : 'POST';

    try {
        await apiFetch(endpoint, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(noteData),
        });
        noteModal.hide();
        await refreshAllNotes();
    } catch (error) {}
}

async function handleDelete(id) {
    if (confirm('Are you sure you want to delete this note?')) {
        try {
            await apiFetch(`${id}`, { method: 'DELETE' });
            await refreshAllNotes();
        }catch (error) {}
    }
}

function handleFilter(event) {
    event.preventDefault();
    const target = event.target;
    if (target.tagName !== 'A') return;

    document.querySelectorAll('.nav-tabs .nav-link').forEach(tab => tab.classList.remove('active'));
    target.classList.add('active');
    
    applyFilters();
}
document.addEventListener('DOMContentLoaded', async () => {
    try {
        const categories = await apiFetch('categories');
        if (categories) {
            categoryDropdown.innerHTML = categories.map(cat => `<option value="${cat}">${cat}</option>`).join('');
        }
        await refreshAllNotes();
    } catch (error) {}
});

addNoteBtn.addEventListener('click', handleAdd);
noteForm.addEventListener('submit', handleSubmit);
filterTabsContainer.addEventListener('click', handleFilter);
showCompletedCheck.addEventListener('change', applyFilters);

let searchTimeout;
searchInput.addEventListener('input', () => {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(async () => {
        try {
            const notes = await apiFetch(`search?query=${searchInput.value}`);
            displayNotes(notes);
        }catch (error) {}
    }, 300);
});