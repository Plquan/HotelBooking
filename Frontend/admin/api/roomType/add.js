


    // tagify
    var input1 = document.querySelector('#tags-input1');
    var tagify1 = new Tagify(input1);



    var input2 = document.querySelector('#tags-input2');
    var tagify2 = new Tagify(input2);


    // previewThumb
    document.getElementById('thumb-input').addEventListener('change', function () {
        const imageContainer = document.getElementById('thumb-container');
        const files = this.files; 

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

    // preview ImageAlt
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

        function addRoomType() {
            const name = document.getElementById('name').value || null;
            const content = editorContent.getData() || null;
            const capacity = document.getElementById('capacity').value || null;
            const bedType = document.getElementById('bedType').value || null ;
            const view = document.getElementById('view').value || null ;
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
            const listService = service ?  service.map(service => service.value) : null;
        
            // Lấy danh sách ảnh
            const imgContainer = document.querySelector('#img_container');
            const images = imgContainer.querySelectorAll('img');
            const listImage = [];
            images.forEach(img => {
                const srcWithoutPrefix = img.getAttribute('value').replace(/^data:image\/[^;]+;base64,/, '');
                listImage.push(srcWithoutPrefix);
            });
        
            if (!name || !content || !capacity || !bedType || !view || !size || !price || !listFacility || !listService || !base64Img || images.length <= 0) {
                toastr.warning("Chưa nhập đầy đủ thông tin");
                return
            }
        
            const Data = {
                Name: name ,
                Content: content ,
                Capacity: capacity,
                Price: price ,
                View: view ,
                BedType: bedType,
                Size: size,
                Thumb: thumb,
                RoomServices: listService,
                RoomFacilitys: listFacility,
                RoomImages: listImage 
            };
        
        
            axios.post('https://localhost:7197/api/RoomType/Add', Data)
                .then(function (response) {
                    console.log('Phòng đã được thêm thành công:', response.data);         
                    localStorage.setItem('toastrMessage', 'Đã thêm thành công');
                    window.location.href = 'http://127.0.0.1:5500/admin/roomType/index.html';
                })
                .catch(function (error) {
                    console.error('Lỗi khi thêm phòng:', error);
                });
        }
              