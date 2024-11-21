const baseUrl = "https://localhost:7197/images";

let editorContent;
ClassicEditor
    .create(document.querySelector('#editor'))
    .then(editor => {
        editorContent = editor;

        console.log('CKEditor instance created.');
    })
    .catch(error => {
        console.error('Failed to create CKEditor instance:', error);
    });


var input = document.querySelector('#tags-input');
var tagify = new Tagify(input);
const tagsContainer = tagify.DOM.scope;
new Sortable(tagsContainer, {
    animation: 150,
    onEnd: function (evt) {
        const sortedTags = Array.from(tagsContainer.querySelectorAll('.tagify__tag')).map(el => el.title);
        tagify.settings.whitelist = sortedTags;
    }
});
function previewImg() {
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
                   <div>
                     <div class="preview">
                  <img src="${imgSrc}" value ="${imgSrc}" alt="Preview">
                </div>
                <button class="remove-btn" onclick="this.parentElement.remove()">
                    Xóa ảnh
                   </button>
                      </div> `;
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
}
document.addEventListener('DOMContentLoaded', function () {
    getAll()

});

function getAll() {
    const roomId = localStorage.getItem('roomTypeId');
    axios.get(`https://localhost:7197/api/RoomType/GetById/${roomId}`)
        .then(function (response) {
            const roomData = response.data;

            console.log(roomData)
            document.getElementById('name').value = roomData.name
            document.getElementById('capacity').value = roomData.capacity
            document.getElementById('slug').value = roomData.slug
            document.getElementById('price').value = roomData.price
            document.getElementById('size').value = roomData.size
            document.getElementById('view').value = roomData.view
            document.getElementById('bedType').value = roomData.bedType
            editorContent.setData(roomData.content)
            
            const listImage = roomData.roomImages.map(img => img.url);
            const imgContainer = document.getElementById('img_container'); 
            imgContainer.innerHTML = '';
            listImage.forEach(image => {
                const previewHTML = `
                <div>
                     <div class="preview">
                  <img src="${baseUrl}/${image}" value ="${image}" alt="Preview">
                </div>
                <button class="remove-btn" onclick="this.parentElement.remove()">
                    Xóa ảnh
                   </button>
                      </div> 
                    `;
                imgContainer.insertAdjacentHTML('beforeend', previewHTML);
            });
          
            const facilitys = roomData.roomFacilitys.map(facility => ({ value: facility.name }));
            tagify.addTags(facilitys);
        }).catch(function (error) {
            console.error('Lỗi khi cập nhật phòng:', error);
        });
}
function cancel() {
    window.location.href = 'http://127.0.0.1:5500/admin/roomType/index.html';
}
function edit() {
    const id = localStorage.getItem('roomTypeId')
    const name = document.getElementById('name').value || null
    const content = editorContent.getData() || null
    const slug = document.getElementById('slug').value || null
    const capacity = document.getElementById('capacity').value || null
    const bedType = document.getElementById('bedType').value || null
    const view = document.getElementById('view').value || null
    const size = document.getElementById('size').value || null
    const price = document.getElementById('price').value || null

    // Lấy danh sách tiện nghi
    const facility = tagify.value;
    const listFacility = facility ? facility.map(facility => facility.value) : null

    // Lấy danh sách ảnh
    const imgContainer = document.querySelector('#img_container');
    const images = imgContainer.querySelectorAll('img');
    const listImage = []
    images.forEach(img => {
        const srcWithoutPrefix = img.getAttribute('value').replace(/^data:image\/[^;]+;base64,/, '')
        listImage.push(srcWithoutPrefix);
    })

    if (!name || !content || !slug || !capacity || !bedType || !view || !size || !price || !listFacility || images.length <= 0) {
        toastr.warning("Chưa nhập đầy đủ thông tin");
        return

    }

    const Data = {
        Id: id,
        Name: name,
        Content: content,
        slug: slug,
        Capacity: capacity,
        Price: price,
        View: view,
        BedType: bedType,
        Size: size,
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
