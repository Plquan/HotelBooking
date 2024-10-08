
function deleteRoomType(id) {
    const button = document.getElementById('confirmDelete');
    button.addEventListener('click', function () {
        axios.delete(`https://localhost:7197/api/RoomType/Delete/${id}`)
            .then(function (response) {
                console.log("Room deleted successfully:", response.data);

                location.reload();
            })
            .catch(function (error) {
                console.error("Error deleting roomtype:", error);
                alert("Lỗi khi xóa : " + error.response.data.message); 
            });
    });

}


function cancel() {
    window.location.href = 'https://localhost:7060/Admin/RoomType/';
}

function addRoomType() {
    const name = document.getElementById('typeName').value;
    const description = document.getElementById('description').value;

    const Data = {

        Name: name,
        Description: description
    };
    console.log(Data)

    axios.post('https://localhost:7197/api/RoomType/Add',Data)
        .then(function (response) {
            console.log('Phòng đã được thêm thành công:', response.data);
            window.location.href = 'https://localhost:7060/Admin/RoomType';
        })
        .catch(function (error) {
            console.error('Lỗi khi thêm phòng:', error);
        });
}

function editRoomTypeId(id) {
    localStorage.setItem('editRoomTypeId', id);
    window.location.href = 'https://localhost:7060/Admin/RoomType/Edit';
}

function editRoomType() {
    const id = document.getElementById('roomTypeId').value;
    const name = document.getElementById('typeName').value;
    const description = document.getElementById('description').value;

    const Data = {
        Id: id,
        Name: name,
        Description: description
    };

    console.log(Data);
    axios.put('https://localhost:7197/api/RoomType/Update', Data)
        .then(function (response) {
            console.log('Phòng đã được cập nhật thành công:', response.data);
            window.location.href = 'https://localhost:7060/Admin/RoomType';
        })
        .catch(function (error) {
            console.error('Lỗi khi cập nhật phòng:', error);
        });
}

// ctrl + shift + r