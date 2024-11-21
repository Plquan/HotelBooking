document.addEventListener('DOMContentLoaded',function(){
    axios.get('https://localhost:7197/api/Menu/GetAll')
    .then(function (response) {
        const menus = response.data.data;  // Dữ liệu loại phòng từ API
        const tableData = document.getElementById('parentId');
        tableData.innerHTML = '';				
        tableData.innerHTML = ' <option value="" selected hidden disabled>Chọn</option>';
        
      
        renderMenus(menus, tableData)
    
    }).catch(function (error) {
        console.error('Lỗi khi lấy data:', error);
    });
})

function renderMenus(menus, tableData, level = 0){
    menus.forEach(menu => {
        const html = `<option value="${menu.id}">${'|--- '.repeat(level)}${level + 1}. ${menu.name}</option>`           
        tableData.insertAdjacentHTML('beforeend', html);
        if (menu.menuChild && menu.menuChild.length > 0) {
         renderMenus(menu.menuChild, tableData, level + 1);
        }
    });
}

function addMenu() {
    const name = document.getElementById('name').value || null
    const link = document.getElementById('link').value || null
    const parentId = document.getElementById('parentId').value || null
    const typeOpen = document.getElementById('typeOpen').value || null
    const status = document.getElementById('status').value || null
   if(name == null){
    toastr.warning('Chưa nhập tên menu')
    return
   }
    const data = {
        name: name,
        link: link,
        parentId: parentId,
        typeOpen: typeOpen,
        status: status
    }
    console.log(data)
    axios.post('https://localhost:7197/api/Menu/Add', data)
    .then(function (response) {
        window.location.href = 'http://127.0.0.1:5500/admin/menu/index.html';
    }).catch(function (error) {
        console.error('Lỗi khi thêm phòng:', error);
    });
}

function cancel(){
    window.location.href = 'http://127.0.0.1:5500/admin/menu/index.html';
}