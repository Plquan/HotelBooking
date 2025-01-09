const baseUrl = "https://localhost:7197/images";
document.addEventListener('DOMContentLoaded', function () {
    const tableBody = document.getElementById('ShowRoom');
    axios.get('https://localhost:7197/api/Room/GetAll')
        .then(function (response) {
            const rooms = response.data;
            console.log(rooms);
            tableBody.innerHTML = '';

            const totalCount = rooms.length;
            
            document.getElementById('allBtn').textContent = `${totalCount}`;
            
            let counter = 1;
            rooms.forEach(room => {
                const row = document.createElement('tr');
                row.setAttribute('data-id', room.id);
                row.innerHTML = `
                                         <td>${counter}</td>
                                            <td>${room.roomNumber}</td>
                                            <td>${room.typeName}</td>
 
                                     	<td>
											<button type="button" data-id="${room.id}" onclick="changeStatus(${room.id})" class="btn btn-rounded ${room.status === "active" ? "btn-primary" : "btn-info active"}">
													${room.status === "active" ? "Kích hoạt" : "Chưa kích hoạt"}
											   </button>						
									</td>
                                    <td class="text-right">
                                      <a onclick="redirectEditPage(${room.id})" type="button" class="btn btn-icon btn-success">
                                      <i class="fa fa-pencil-alt"></i>
                                      </a>
                                      <a onclick="deleteRoom(${room.id})" type="button" class="btn btn-icon btn-danger" data-toggle="modal"
                                      data-target="#delete_asset">
                                      <i class="fa fa-trash"></i>
                                      </a>
									</td>`;
                tableBody.appendChild(row);
                counter++;
            });
        })
        .catch(function (error) {
            console.error('Lỗi khi lấy dữ liệu:', error);
        });
});

function redirectEditPage(id) {
    localStorage.setItem('editRoomId', id);
    window.location.href = 'http://127.0.0.1:5500/admin/room/edit.html';
}

function deleteRoom(id) {
    const button = document.getElementById('confirmDelete');
    button.addEventListener('click', function () {
        axios.delete(`https://localhost:7197/api/Room/Delete/${id}`)
            .then(function (response) {
                console.log("Room deleted successfully:", response.data);

                const container = document.getElementById('ShowRoom')
                const row = container.querySelector(`tr[data-id="${id}"]`);
				row.remove();
				$('#delete_asset').modal('hide');
				toastr.success("Xóa thành công");

            })
            .catch(function (error) {
                console.error("Error deleting room:", error);
                alert("Lỗi khi xóa phòng: " + error.response.data.message);
            });
    }, { once: true });
}

function changeStatus(id) {
    axios.post(`https://localhost:7197/api/Room/ChangeStatus/${id}`)
        .then(function (respone) {
            console.log(respone.data);

            const button = document.querySelector(`button[data-id="${id}"]`);
            if (respone.data === "active") {
                button.textContent = `Kích hoạt`;
                button.className = 'btn btn-rounded btn-primary';
            }
            else {
                button.textContent = `Chưa kích hoạt`;
                button.className = 'btn btn-rounded btn-info active';
            }
        })
        .catch(function (error) {
            console.error('Lỗi khi cập nhật trạng thái:', error);
        });

}