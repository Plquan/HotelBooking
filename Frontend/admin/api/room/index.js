const baseUrl = "https://localhost:7197/images";
document.addEventListener('DOMContentLoaded', function () {
    const tableBody = document.getElementById('ShowRoom');
    axios.get('https://localhost:7197/api/Room/GetAll')
        .then(function (response) {
            const rooms = response.data;
            console.log(rooms);
            tableBody.innerHTML = '';

            const totalCount = rooms.length;
            const activeCount = rooms.filter(item => item.status === "active").length;
            const inActiveCount = rooms.filter(item => item.status === "inActive").length;

            document.getElementById('allBtn').textContent = `Tất cả (${totalCount})`;
            document.getElementById('activeBtn').textContent = `Kích hoạt (${activeCount})`;
            document.getElementById('inActiveBtn').textContent = `Chưa kích hoạt (${inActiveCount})`;

            let counter = 1;
            rooms.forEach(room => {
                const row = document.createElement('tr');
                row.setAttribute('data-id', room.id);
                row.innerHTML = `
                                         <td>${counter}</td>
                                            <td>${room.roomNumber}</td>
                                            <td>${room.typeName}</td>
                                              <td><span style="width: 42px; display: inline-block">
    <a href="URL_FOR_UP" type="button" class="btn btn-primary btn-sm" data-toggle="tooltip" title="Up" data-original-title="Up">
        <i class="fa fa-arrow-up"></i>
    </a>
</span>
<span style="width: 42px; display: inline-block">
    <a href="URL_FOR_DOWN" type="button" class="btn btn-primary btn-sm" data-toggle="tooltip" title="Down" data-original-title="Down">
        <i class="fa fa-arrow-down"></i>
    </a>
</span>
</td>
                                     	<td>
											<button type="button" data-id="${room.id}" onclick="changeStatus(${room.id})" class="btn btn-rounded ${room.status === "active" ? "btn-primary" : "btn-info active"}">
													${room.status === "active" ? "Kích hoạt" : "Chưa kích hoạt"}
											   </button>						
									</td>
                                                <td class="text-right">
                                                    <div class="dropdown dropdown-action">
                                                        <a href="#" class="action-icon dropdown-toggle"
                                                           data-toggle="dropdown" aria-expanded="false">
                                                            <i class="fas fa-ellipsis-v ellipse_color"></i>
                                                        </a>
                                                        <div class="dropdown-menu dropdown-menu-right">
                                                                    <a onclick="roomTypeId(${room.id})" class="dropdown-item">
                                                                <i class="fas fa-pencil-alt m-r-5"></i> Edit
                                                            </a>
                                                                <a onclick="deleteRoom(${room.id})" class="dropdown-item" href="#" data-toggle="modal"
                                                               data-target="#delete_asset">
                                                                <i class="fas fa-trash-alt m-r-5"></i> Delete
                                                            </a>
                                                        </div>
                                                    </div>
                                                </td>`;

                tableBody.appendChild(row);
                counter++;
            });

        })
        .catch(function (error) {
            console.error('Lỗi khi lấy dữ liệu:', error);
        });
});


function editRoomId(id) {
    localStorage.setItem('editRoomId', id);
    window.location.href = 'https://localhost:7060/Admin/Room/Edit';
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
    });

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