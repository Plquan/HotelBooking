const baseUrl = "https://localhost:7197/images/";

document.addEventListener('DOMContentLoaded', async function () {
    const roomId = localStorage.getItem('roomDetailId');
    localStorage.clear();
    if(roomId == null){
     window.location.href = 'http://127.0.0.1:5500/user/allRoom.html';
    }
    
    try {
        const response = await axios.get(`https://localhost:7197/api/RoomType/GetById/${roomId}`);
        const data = response.data;
        console.log(data);

        document.getElementById('capacity').textContent = `Tối đa: ${data.capacity} người`;
        document.getElementById('size').textContent = `Diện tích: ${data.size}`;
        document.getElementById('view').textContent = `Hướng nhìn: ${data.view}`;
        document.getElementById('bedType').textContent = `Loại giường: ${data.bedType}`;
        document.getElementById('content').innerHTML = data.content;

        // Gán danh sách dịch vụ
        const roomServiceList = document.getElementById("roomService");
        const services = data.roomServices;
        services.forEach(service => {
            const li = document.createElement("li");
            li.textContent = service.name;
            roomServiceList.appendChild(li);
        });

        // Gán danh sách tiện nghi
        const facilityContainer = document.getElementById('facilityContainer');
        let colDiv = document.createElement("div");
        colDiv.className = "col-xs-6 col-lg-4";
        let ul = document.createElement("ul");
        colDiv.appendChild(ul); 

        const facilities = data.roomFacilitys;
        facilities.forEach((facility, index) => {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
            const li = document.createElement("li");
            li.textContent = facility.name;
            ul.appendChild(li);

            if ((index + 1) % 3 === 0) { 
                facilityContainer.appendChild(colDiv);
                colDiv = document.createElement("div");
                colDiv.className = "col-xs-6 col-lg-4";
                ul = document.createElement("ul");
                colDiv.appendChild(ul); 
            }
        });

        if (ul.hasChildNodes()) {
            facilityContainer.appendChild(colDiv);
        }

        // Gán danh sách ảnh
        let html = '';
        const imageContainer = document.getElementById('imageContainer');
        imageContainer.innerHTML = '';
        const listImage = data.roomImages;
        listImage.forEach(image => {
            html += `<div class="gallery__img-block">
                        <img src="${baseUrl}${image.url}" alt="${baseUrl}${image.url}" class="">
                     </div>`;
        });
        imageContainer.innerHTML = html;

        const reviewImage = `<div class="gallery__controls"></div>`;
        $("#imageContainer").append(reviewImage);        

        $(document).ready(function() {
        
        $('.gallery3').each(function() {
        $(this).vitGallery({
            debag: true,
            thumbnailMargin: 37,
            fullscreen: true
        })
    })
    
    });
    } catch (error) {
        console.error("Có lỗi xảy ra khi tải dữ liệu:", error);
    }
});
