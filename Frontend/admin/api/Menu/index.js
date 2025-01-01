const accessToken = localStorage.getItem('accessToken'); 

axios.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`;


getAll()

function getAll() {
    axios.get('https://localhost:7197/api/Menu/GetAll')
        .then(function (response) {
            const menus = response.data.data;

            const totalCount = menus.length;
            const activeCount = menus.filter(item => item.status === "active").length;
            const inActiveCount = menus.filter(item => item.status === "inActive").length;

            document.getElementById('allBtn').textContent = `${totalCount}`;
            document.getElementById('activeBtn').textContent = `${activeCount}`;
            document.getElementById('inActiveBtn').textContent = `${inActiveCount}`;

            const tableData = document.getElementById('showMenu');
            tableData.innerHTML = ''         
            renderMenus(menus, tableData);
        }).catch(function (error) {
            console.log('Lỗi khi lấy dữ liệu từ API:', error.response.data);
        });
}

function renderMenus(menus, tableData, counter = 1, level = 0) {
    menus.forEach((menu, index) => {
        const row = document.createElement('tr');
        const isLast = (index === 0 && index != menus.length - 1 || index != menus.length - 1)
        const isFirst = (index === menus.length - 1 && index != 0 || index != 0)
        row.innerHTML = `
            <td>${counter}</td>
            <td>${'|------ '.repeat(level)}<span class="badge bg-danger text-white">${level + 1}</span> <b>${menu.name}</b></td>
            <td>
           <span style="width: 42px; display: inline-block"> 
            ${isFirst ? ` 
                <a onclick='saveItem("ordering","up",${menu.id})' style="background-color: #4682B4;" type="button" class="btn btn-primary btn-sm" title="Up">
                    <i class="fa fa-arrow-up"></i>
                </a>
            ` : ''} </span>
           <span style="width: 42px; display: inline-block">
            ${isLast ? `  
                <a onclick='saveItem("ordering","down",${menu.id})' style="background-color: #4682B4;" type="button" class="btn btn-primary btn-sm" title="Down">
                    <i class="fa fa-arrow-down"></i>
                </a>
            ` : ''}</span>
          
        </td>
        <td>
            <button type="button" onclick='saveItem("status","${menu.status}",${menu.id})'
                    class="btn btn-rounded ${menu.status === "active" ? "btn-primary" : "btn-info active"}">
                ${menu.status === "active" ? "Kích hoạt" : "Chưa kích hoạt"}
            </button>						
        </td>
        <td>
            <select class="form-control" onchange='saveItem("typeOpen", this.value, ${menu.id})' >
            <option value="_self">Mở cửa sổ hiện tại</option>
            <option value="_blank" ${menu.typeOpen === "_blank" ? 'selected' : ''}>Mở cửa sổ mới</option>                                                                             
            </select>
        </td>
        <td class="text-right">            
            <a onclick="editMenu(${menu.id})" type="button" class="btn btn-icon btn-success">
              <i class="fa fa-pencil-alt"></i>
               </a>
            <a onclick="deleteMenu(${menu.id})" type="button" class="btn btn-icon btn-danger" data-toggle="modal"
            data-target="#delete_asset">
            <i class="fa fa-trash"></i>
            </a>
        </td>
    `;
        tableData.appendChild(row);
        counter++;

        if (menu.menuChild && menu.menuChild.length > 0) {
            counter = renderMenus(menu.menuChild, tableData, counter, level + 1);
        }
    });

    return counter;
}
function saveItem(task, button, id) {
    const request = {
        task: task,
        button: button,
        id: id
    }
    // console.log(request)
    axios.put('https://localhost:7197/api/Menu/SaveItem', request)
        .then(function () {
            getAll();
        })
        .catch(function (error) {
            console.error('Lỗi khi lấy dữ liệu từ API:', error);
        });
}
function deleteMenu(id) {
    const button = document.getElementById('confirmDelete');
    button.addEventListener('click', function () {
        axios.delete(`https://localhost:7197/api/Menu/Delete/${id}`)
            .then(function () {
                getAll()
                $('#delete_asset').modal('hide');
                toastr.success("Xóa thành công");
            })
            .catch(function (error) {
                console.error("Error deleting roomtype:", error);
                alert("Lỗi khi xóa : " + error.response.data.message);
            });
    }, { once: true });
}
function editMenu(id) {
    localStorage.setItem('menuId', id)
    window.location.href = 'http://127.0.0.1:5500/admin/Menu/edit.html'
}