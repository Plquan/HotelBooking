const baseUrl = "https://localhost:7197/images";
document.addEventListener('DOMContentLoaded', function () {

	const message = localStorage.getItem('toastrMessage'); // Lấy thông báo từ localStorage
	if (message) {
		toastr.success(message); // Hiển thị toastr
		localStorage.removeItem('toastrMessage'); // Xóa thông báo sau khi hiển thị
	}

	axios.get('https://localhost:7197/api/RoomType/GetAll')
		.then(function (response) {
			const roomTypes = response.data; // Giả sử dữ liệu trả về là một mảng
			console.log(roomTypes);
			const tableBody = document.getElementById('ShowRoomType');
			tableBody.innerHTML = ''; // Xóa nội dung cũ

			const totalCount = roomTypes.length;
			const activeCount = roomTypes.filter(item => item.status === "active").length;
			const inActiveCount = roomTypes.filter(item => item.status === "inActive").length;

			document.getElementById('allBtn').textContent = `Tất cả (${totalCount})`;
			document.getElementById('activeBtn').textContent = `Kích hoạt (${activeCount})`;
			document.getElementById('inActiveBtn').textContent = `Chưa kích hoạt (${inActiveCount})`;

			let counter = 1;
			roomTypes.forEach(roomType => {
				const row = document.createElement('tr');
				row.setAttribute('data-id', roomType.id);
				console.log(row);

				row.innerHTML = `
					                   <td>${counter}</td>
											 <td>${roomType.name}</td>
				                      <td>
										  <div class="preview">
											<img src="${baseUrl}/${roomType.thumb}" id="thumb" alt="Preview-thumb">
									
							         	</div>
								
									</td>
									<td>${roomType.price}đ</td>
								
									<td>
											<button type="button" data-id="${roomType.id}" onclick="changeStatus(${roomType.id})" class="btn btn-rounded ${roomType.status === "active" ? "btn-primary" : "btn-info active"}">
													${roomType.status === "active" ? "Kích hoạt" : "Chưa kích hoạt"}
											   </button>						
									</td>
											<td class="text-right">
												<div class="dropdown dropdown-action">
													<a href="#" class="action-icon dropdown-toggle"
													   data-toggle="dropdown" aria-expanded="false">
														<i class="fas fa-ellipsis-v ellipse_color"></i>
													</a>
													<div class="dropdown-menu dropdown-menu-right">
																<a onclick="editRoomTypeId(${roomType.id})" class="dropdown-item">
															<i class="fas fa-pencil-alt m-r-5"></i> Edit
														</a>
															<a onclick="deleteRoomType(${roomType.id})" class="dropdown-item" href="#" data-toggle="modal"
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

function editRoomTypeId(id) {
    localStorage.setItem('editRoomTypeId', id);
    window.location.href = 'http://127.0.0.1:5500/admin/roomType/edit.html';
}
function deleteRoomType(id) {
    const button = document.getElementById('confirmDelete');
	
    button.addEventListener('click', function () {
        axios.delete(`https://localhost:7197/api/RoomType/Delete/${id}`)
            .then(function (response) {             
				const container = document.getElementById('ShowRoomType')
                const row = container.querySelector(`tr[data-id="${id}"]`);
				row.remove();
				$('#delete_asset').modal('hide');
				toastr.success("Xóa thành công");
            })
            .catch(function (error) {
                console.error("Error deleting roomtype:", error);
                alert("Lỗi khi xóa : " + error.response.data.message); 
            });
    });
}

function changeStatus(id) {
    axios.post(`https://localhost:7197/api/RoomType/ChangeStatus/${id}`)
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