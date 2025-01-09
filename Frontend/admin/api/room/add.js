document.addEventListener('DOMContentLoaded', function () {
        axios.get('https://localhost:7197/api/RoomType/GetAll')
            .then(function (response) {
                const roomTypes = response.data;  // Dữ liệu loại phòng từ API
                const tableData = document.getElementById('roomTypeId');
                tableData.innerHTML = '';				
                tableData.innerHTML = '<option value="">Chọn</option>';
            
                roomTypes.forEach(roomType => {
                    const option = document.createElement('option');
                    option.value = roomType.id;    
                    option.text = roomType.name;   
                    tableData.appendChild(option); 
                });
            })
            .catch(function (error) {
                console.error('Lỗi khi lấy loại phòng:', error);
            });
    
});
function addRoom() {
    toggleLoading(true)
    const roomNumber = document.getElementById('roomNumber').value;
    const roomTypeId = document.getElementById('roomTypeId').value;
    if(!roomNumber || roomTypeId){
        toastr.warning('Vui lòng nhập đầy đủ dữ liệu !')
        toggleLoading(false);
        return
    }
    const roomData = {
        RoomNumber: roomNumber,
        RoomTypeId: roomTypeId,

    };

    axios.post('https://localhost:7197/api/Room/Add', roomData)
    .then(function (response) {
        console.log('Phòng đã được thêm thành công:', response.data);
        window.location.href = 'http://127.0.0.1:5500/admin/room/index.html';
    })
    .catch(function (error) {
        console.error('Lỗi khi thêm phòng:', error);
    })
    .finally(function () {
        toggleLoading(false);
    });

}
function cancel() {
    window.location.href = 'http://127.0.0.1:5500/admin/room/index.html';
}

function toggleLoading(show) {
    const overlay = document.getElementById("overlay");
    overlay.style.display = show ? "flex" : "none";
}