const baseUrl = "https://localhost:7197/images";

// tagify
var input1 = document.querySelector('#tags-input1');
var tagify1 = new Tagify(input1);



var input2 = document.querySelector('#tags-input2');
var tagify2 = new Tagify(input2);

// preview Thumb
document.getElementById('thumb-input').addEventListener('change', function () {
    const imageContainer = document.getElementById('thumb-container');
    const files = this.files; // Lấy danh sách các tệp đã chọn

    imageContainer.innerHTML = '';
    if (files.length > 0) {
        const file = files[0];
        if (file.type.startsWith('image/')) {
            const reader = new FileReader();
            reader.onload = function (e) {
                const imgSrc = e.target.result;
                const previewHTML = `
                            <div class="preview">
                                    <img src="${imgSrc}" value="${imgSrc}" id="thumb" alt="Preview-thumb">
                                <button class="remove-btn" onclick="this.parentElement.remove()">X</button>
                            </div>
                        `;
                imageContainer.insertAdjacentHTML('beforeend', previewHTML);
            };
            reader.readAsDataURL(file);
        } else {
            console.log("File không được hỗ trợ: " + file.name);
        }
    } else {
        console.log("Không có file nào được chọn");
    }
});

// preview RoomImage
document.getElementById('addImage').addEventListener('click', function () {
    const imageContainer = document.getElementById('img_container');
    const img = document.getElementById('file-input');
    const files = img.files;

    if (files.length > 0) {
        for (let i = 0; i < files.length; i++) {
            const file = files[i];

            if (file.type.startsWith('image/')) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    const imgSrc = e.target.result;
                    const previewHTML = `
                            <div class="preview">
                                <img src="${imgSrc}" value ="${imgSrc}" alt="Preview">
                                <button class="remove-btn" onclick="this.parentElement.remove()">X</button>
                            </div>
                        `;
                    imageContainer.insertAdjacentHTML('beforeend', previewHTML);
                };
                reader.readAsDataURL(file);
            } else {
                console.log("File không được hỗ trợ: " + file.name);
            }
        }
    } else {
        console.log("Không có file nào được chọn");
    }
});

// loading
document.addEventListener('DOMContentLoaded', function () {
    const roomId = localStorage.getItem('editRoomTypeId');
    console.log('Room ID:', roomId);

    axios.get(`https://localhost:7197/api/RoomType/GetById/${roomId}`)
        .then(function (response) {
            const roomData = response.data;

            console.log(roomData)
            document.getElementById('name').value = roomData.name;
            document.getElementById('capacity').value = roomData.capacity;
            document.getElementById('price').value = roomData.price;
            document.getElementById('size').value = roomData.size;
            document.getElementById('view').value = roomData.view;
            document.getElementById('bedType').value = roomData.bedType;
            editorContent.setData(roomData.content);

            // gán ảnh
            const listImage = roomData.roomImages.map(img => img.url); 
            const imgContainer = document.getElementById('img_container');
            imgContainer.innerHTML = '';
            // Sử dụng forEach để lặp qua từng ảnh
            listImage.forEach(image => {
                const previewHTML = `
                 <div class="preview">
                <img src="${baseUrl}/${image}" value="${image}"  alt="Preview">
            <button class="remove-btn" onclick="this.parentElement.remove()">X</button>
             </div>
                    `;
                imgContainer.insertAdjacentHTML('beforeend', previewHTML); // Thêm ảnh vào container
            });



            // gán ảnh bìa
            const imageContainer = document.getElementById('thumb-container');
            imageContainer.innerHTML = '';
                const previewHTML = `
                        <div class="preview">
                                   <img src="${baseUrl}/${roomData.thumb}" value="" id="thumb" alt="Preview-thumb">
                               <button class="remove-btn" onclick="this.parentElement.remove()">X</button>
                        </div>
                    `;
                imageContainer.insertAdjacentHTML('beforeend', previewHTML);
            

            // gắn tiện ích
            const facilitys = roomData.roomFacilitys.map(facility => ({ value: facility.name }));
            tagify2.addTags(facilitys);

            // gắn tiện ích
            const services = roomData.roomServices.map(service => ({ value: service.name }));
            tagify1.addTags(services);

        })
       
});


let editorContent;
//ckeditor
ClassicEditor
    .create(document.querySelector('#content'))
    .then(editor => {
        editorContent = editor;

        console.log('CKEditor instance created.');
    })
    .catch(error => {
        console.error('Failed to create CKEditor instance:', error);
    });

    function cancel() {
        window.location.href = 'http://127.0.0.1:5500/admin/roomType/index.html';
    }

    function editRoomType() {
        const id = localStorage.getItem('editRoomTypeId');
        const name = document.getElementById('name').value || null;
        const content = editorContent.getData() || null;
        const capacity = document.getElementById('capacity').value || null;
        const bedType = document.getElementById('bedType').value || null;
        const view = document.getElementById('view').value || null;
        const size = document.getElementById('size').value || null;
        const price = document.getElementById('price').value || null;
        // Kiểm tra xem ảnh có được chọn không
        const base64Img = document.getElementById('thumb') ? document.getElementById('thumb').getAttribute('value') : null;
        const thumb = base64Img ? base64Img.replace(/^data:image\/[^;]+;base64,/, '') : null;
    
    
        // Lấy danh sách tiện nghi
        const facility = tagify2.value;
        const listFacility = facility ? facility.map(facility => facility.value) : null;
    
        // Lấy danh sách dịch vụ
        const service = tagify1.value;
        const listService = service ? service.map(service => service.value) : null;
    
        // Lấy danh sách ảnh
        const imgContainer = document.querySelector('#img_container');
        const images = imgContainer.querySelectorAll('img');
        const listImage = [];
        images.forEach(img => {
            const srcWithoutPrefix = img.getAttribute('value').replace(/^data:image\/[^;]+;base64,/, '');
            listImage.push(srcWithoutPrefix);
        });
    
        if (!name || !content || !capacity || !bedType || !view || !size || !price || !listFacility || !listService  || images.length <= 0) {
            toastr.warning("Chưa nhập đầy đủ thông tin");
            return
    
        }
    
        const Data = {
            Id: id,
            Name: name ,
            Content: content,
            Capacity: capacity,
            Price: price,
            View: view,
            BedType: bedType,
            Size: size,
            Thumb: thumb,
            RoomServices: listService,
            RoomFacilitys: listFacility,
            RoomImages: listImage
        };
    
    
        axios.put('https://localhost:7197/api/RoomType/Update', Data)
            .then(function (response) {
                console.log('Phòng đã được cập nhật thành công:', response.data);
                window.location.href = 'http://127.0.0.1:5500/admin/roomType/index.html';
                      localStorage.removeItem('editRoomTypeId');
            })
            .catch(function (error) {
                console.error('Lỗi khi cập nhật phòng:', error);
            });
    }
    