
function addRoom() {
    const roomNumber = document.getElementById('roomNumber').value;
    const roomTypeId = document.getElementById('roomTypeId').value;
    const status = document.getElementById('status').value;
    const capacity = document.getElementById('capacity').value;
    const price = document.getElementById('price').value;

    const roomData = {
        RoomNumber: roomNumber,
        RoomTypeId: roomTypeId,
        Capacity: capacity,
        Price: price
    };
    console.log(roomData)

    axios.post('https://localhost:7197/api/Room/Add', roomData)
        .then(function (response) {
            console.log('Phòng đã được thêm thành công:', response.data);
            window.location.href = 'https://localhost:7060/Admin/Room';
        })
        .catch(function (error) {
            console.error('Lỗi khi thêm phòng:', error);
        });
}


function editRoomId(id) {
    localStorage.setItem('editRoomId', id);
    window.location.href = 'https://localhost:7060/Admin/Room/Edit';
}
function editRoom() {
    const id = document.getElementById('roomId').value;
    const roomNumber = document.getElementById('roomNumber').value;
    const roomTypeId = document.getElementById('roomTypeId').value;
    const status = document.getElementById('status').value;
    const capacity = document.getElementById('capacity').value;
    const price = document.getElementById('price').value;
    const roomData = {
        Id:id,
        RoomNumber: roomNumber,
        RoomTypeId: roomTypeId,
        Capacity: capacity,
        Price: price,
        Status: status
    };

    console.log(roomData);
    axios.put('https://localhost:7197/api/Room/Update', roomData)
        .then(function (response) {
            console.log('Phòng đã được cập nhật thành công:', response.data);

            window.location.href = 'https://localhost:7060/Admin/Room';
        })
        .catch(function (error) {
            console.error('Lỗi khi cập nhật phòng:', error);
        });
}


function deleteRoom(id) {
    const button = document.getElementById('deleteRoom');
    button.addEventListener('click', function () {     
        axios.delete(`https://localhost:7197/api/Room/Delete/${id}`)
            .then(function (response) {
                console.log("Room deleted successfully:", response.data);

                location.reload();
            })
            .catch(function (error) {
                console.error("Error deleting room:", error);
                alert("Lỗi khi xóa phòng: " + error.response.data.message); // Hiển thị thông báo lỗi nếu có
            });
    });

}


document.getElementById('price').addEventListener('input', function (e) {
    let value = e.target.value.replace(/[^0-9]/g, ''); // Loại bỏ ký tự không phải số
    if (value) {
        value = Number(value).toLocaleString('vi-VN'); // Định dạng tiền Việt (3 số chấm một lần)
    }
    e.target.value = value; // Gán giá trị đã định dạng lại vào textbox
});


function Cancel() {
    window.location.href = 'https://localhost:7060/Admin/Room/';
}



























