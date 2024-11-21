document.addEventListener('DOMContentLoaded', function () {
    getMenu()
})


function getMenu() {
    const id = localStorage.getItem('menuId')
    axios.get(`https://localhost:7197/api/Menu/GetById/${id}`)
        .then(function (respone) {
            const itemData = respone.data.data
            document.getElementById('name').value = itemData.name
            document.getElementById('link').value = itemData.link

            const selectTypeOpen = document.getElementById('typeOpen')
            const typeHtml = `<option value="_self">Mở cửa sổ hiện tại</option><option value="_blank" ${itemData.typeOpen === '_blank' ? 'selected' : ''}>Mở cửa sổ mới</option>`
            selectTypeOpen.innerHTML = typeHtml

            const selectStatus = document.getElementById('status')
            const statusHtml = `<option value="inactive">Chưa kích hoạt</option>                                                 
                <option value="active" ${itemData.status === 'active' ? 'selected' : ''}>Kích hoạt</option>`
            selectStatus.innerHTML = statusHtml
            
            axios.get(`https://localhost:7197/api/Menu/GetMenuSelect/${itemData.id}`)
                .then(function (response) {
                    const menus = response.data.data;  // Dữ liệu loại phòng từ API
                    const tableData = document.getElementById('parentId');
                    tableData.innerHTML = '';
                    tableData.innerHTML = ` <option value="${itemData.parentId === null ? '' : itemData.parentId}" selected disable hidden>Chọn</option> <option value="">0. Menu gốc</option>`;
                    renderMenus(menus, tableData, itemData.id)

                }).catch(function (error) {
                    console.error('Lỗi khi lấy dữ liệu:', error);
                });
        }).catch(function (error) {
            console.error('Lỗi khi lấy dữ liệu:', error);
        });
}
function renderMenus(menus, tableData, menuId, level = 0) {
    menus.forEach(menu => {
        const html = `<option ${menuId === menu.id ? 'selected' : ''} value="${menu.id}">${'|--- '.repeat(level)}${level + 1}. ${menu.name}</option>`
        tableData.insertAdjacentHTML('beforeend', html);
        if (menu.menuChild && menu.menuChild.length > 0) {
            renderMenus(menu.menuChild, tableData, menuId, level + 1);
        }
    });
}
function cancel() {
    window.location.href = 'http://127.0.0.1:5500/admin/menu/index.html';
}

function editMenu() {
    const id = localStorage.getItem('menuId')
    const name = document.getElementById('name').value || null
    const link = document.getElementById('link').value || null
    const parentId = document.getElementById('parentId').value || null
    const typeOpen = document.getElementById('typeOpen').value || null
    const status = document.getElementById('status').value || null

    const data = {
        id: id,
        name: name,
        link: link,
        parentId: parentId,
        typeOpen: typeOpen,
        status: status
    }
    console.log(data)
    axios.put('https://localhost:7197/api/Menu/update', data)
        .then(function () {
            window.location.href = 'http://127.0.0.1:5500/admin/menu/index.html';
        }).catch(function (error) {
            console.error('Lỗi khi lấy dữ liệu:', error);
        });
}