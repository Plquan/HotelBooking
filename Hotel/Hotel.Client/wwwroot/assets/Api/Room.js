//Load dữ liệu khi tải trang
document.addEventListener('DOMContentLoaded', function () {
    console.log('Trang đã được tải. Đang gọi API...'); // Thêm dòng này để log khi trang tải

    axios.get('https://localhost:7197/api/Room/GetAll')
        .then(function (response) {
            const rooms = response.data; // Giả sử dữ liệu trả về là một mảng
            console.log(rooms);
            const tableBody = document.getElementById('ShowRoom');
            tableBody.innerHTML = ''; // Xóa nội dung cũ

            let counter = 1;
            rooms.forEach(room => {
                const row = document.createElement('tr'); // Tạo một hàng mới
                console.log(row);

                row.innerHTML = `                          			
										<td>${counter}</td>			
                                        <td>${room.roomNumber}</td>
										<td>${room.typeName}</td>
										<td>${room.capacity}</td>
                                        <td>${room.price}</td>
										<td>
											<div class="actions"> <a href="#" class="btn btn-sm bg-success-light mr-2">Inactive</a> </div>
										</td>
										<td class="text-right">
											<div class="dropdown dropdown-action">
												<a href="#" class="action-icon dropdown-toggle" data-toggle="dropdown" aria-expanded="false"><i class="fas fa-ellipsis-v ellipse_color"></i></a>
												<div class="dropdown-menu dropdown-menu-right"> <a class="dropdown-item" href="edit-room.html"><i class="fas fa-pencil-alt m-r-5"></i> Edit</a> <a class="dropdown-item" href="#" data-toggle="modal" data-target="#delete_asset"><i class="fas fa-trash-alt m-r-5"></i> Delete</a> </div>
											</div>
										</td> `;
                tableBody.appendChild(row);
                counter++;
            });

        })
        .catch(function (error) {
            console.error('Lỗi khi lấy dữ liệu:', error);
        });
});


