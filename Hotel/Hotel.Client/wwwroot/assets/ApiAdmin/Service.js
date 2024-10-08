function cancel() {
    window.location.href = 'https://localhost:7060/Admin/Service';
}

function addService() {
    const name = document.getElementById('name').value;
    const description = document.getElementById('description').value;

    const Data = {

        Name: name,
        Description: description
    };
    console.log(Data)

    axios.post('https://localhost:7197/api/Service/Add', Data)
        .then(function (response) {
            console.log('Dịch vụ đã được thêm thành công:', response.data);
            window.location.href = 'https://localhost:7060/Admin/Service';
        })
        .catch(function (error) {
            console.error('Lỗi khi thêm dịch vụ:', error);
        });
}
function deleteService(id) {
    const button = document.getElementById('confirmDelete');
    button.addEventListener('click', function () {
        axios.delete(`https://localhost:7197/api/Service/Delete/${id}`)
            .then(function (response) {
                console.log("Room deleted successfully:", response.data);

                location.reload();
            })
            .catch(function (error) {
                console.error("Error deleting room:", error);
                alert("Lỗi khi xóa phòng: " + error.response.data.message); 
            });
    });

}
function editServiceId(id) {
    localStorage.setItem('editServiceId', id);
    window.location.href = 'https://localhost:7060/Admin/Service/Edit';
}

function editService() {
    const id = document.getElementById('serviceId').value;
    const name = document.getElementById('name').value;
    const description = document.getElementById('description').value;

    const Data = {
        Id: id,
        Name: name,
        Description: description
    };

    console.log(Data);
    axios.put('https://localhost:7197/api/Service/Update', Data)
        .then(function (response) {
            console.log('Dịch vụ đã được cập nhật thành công:', response.data);
            window.location.href = 'https://localhost:7060/Admin/Service';
        })
        .catch(function (error) {
            console.error('Lỗi khi cập nhật:', error);
        });
}