document.addEventListener('DOMContentLoaded', function () {
    // Lấy ID từ localStorage
    const roomId = localStorage.getItem('editRoomId');

    console.log('Room ID:', roomId);

    // Gọi API để lấy thông tin phòng bằng ID
    axios.get(`https://localhost:7197/api/Room/GetById/${roomId}`)
        .then(function (response) {
            const roomData = response.data;
            // Hiển thị dữ liệu vào form
            document.getElementById('roomId').value = roomData.id;
            document.getElementById('roomNumber').value = roomData.roomNumber;
            document.getElementById('roomTypeId').value = roomData.roomTypeId;
            document.getElementById('status').value = roomData.status;
            document.getElementById('capacity').value = roomData.capacity;
            document.getElementById('price').value = roomData.price;
            localStorage.removeItem('editRoomId');
            
        })
        .catch(function (error) {
            console.error('Lỗi khi lấy thông tin phòng:', error);
        });
});

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

            window.location.href = 'http://127.0.0.1:5500/admin/room/index.html';
        })
        .catch(function (error) {
            console.error('Lỗi khi cập nhật phòng:', error);
        });
}
function cancel() {
    window.location.href = 'http://127.0.0.1:5500/admin/room/index.html';
}

