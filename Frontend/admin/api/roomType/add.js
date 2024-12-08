
//ckeditor
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


function previewImg(){
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

function cancel() {
    window.location.href = 'http://127.0.0.1:5500/admin/roomType/index.html';
}

function addRoomType() {
    const name = document.getElementById('name').value || null;
    const content = editorContent.getData() || null;
    const slug = document.getElementById('slug').value || null;
    const capacity = document.getElementById('capacity').value || null;
    const bedType = document.getElementById('bedType').value || null;
    const view = document.getElementById('view').value || null;
    const size = document.getElementById('size').value || null;
    const price = document.getElementById('price').value || null;

    // Lấy danh sách tiện nghi
    const facility = tagify.value;
    const listFacility = facility ? facility.map(facility => facility.value) : null;

    // Lấy danh sách ảnh
    const imgContainer = document.querySelector('#img_container');
    const images = imgContainer.querySelectorAll('img');
    const listImage = [];
    images.forEach(img => {
        const srcWithoutPrefix = img.getAttribute('value').replace(/^data:image\/[^;]+;base64,/, '');
        listImage.push(srcWithoutPrefix);
    });

    if (!name || !content || !capacity || !bedType || !view || !size || !price || !listFacility || images.length <= 0) {
        toastr.warning("Chưa nhập đầy đủ thông tin");
        return
    }

    const data = {
        Name: name,
        Content: content,
        Slug: slug,
        Capacity: capacity,
        Price: price,
        View: view,
        BedType: bedType,
        Size: size,
        RoomFacilitys: listFacility,
        RoomImages: listImage,
    };
    axios.post('https://localhost:7197/api/RoomType/Add', data)
        .then(function () {
            localStorage.setItem('toastrMessage', 'Đã thêm thành công');
            window.location.href = 'http://127.0.0.1:5500/admin/roomType/index.html';
        })
        .catch(function (error) {
            toastr.error("Không kết nối được máy chủ!")
            console.error('Lỗi khi thêm phòng:', error);

        });
}
