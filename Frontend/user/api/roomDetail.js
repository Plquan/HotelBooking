const baseUrl = "https://localhost:7197/images/";

document.addEventListener('DOMContentLoaded', async function () {

    const roomId = localStorage.getItem('roomTypeId');
    if (roomId == null) {
        window.location.href = 'http://127.0.0.1:5500/user/allRoom.html';
    }
    try {
        const response = await axios.get(`https://localhost:7197/api/RoomType/GetById/${roomId}`);
        const data = response.data;

        document.getElementById('capacity').textContent = `Tối đa: ${data.capacity} người`
        document.getElementById('size').textContent = `Diện tích: ${data.size}`;
        document.getElementById('view').textContent = `Hướng nhìn: ${data.view}`;
        document.getElementById('bedType').textContent = `Loại giường: ${data.bedType}`
        document.getElementById('price').textContent = `${data.price}`
        document.getElementById('content').innerHTML = data.content
        document.getElementById('roomPrice').value = data.price

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

        const reviewImage = `
         <span class="reviewImg">Hình ảnh chi tiết</span>
        <div class="gallery__controls"></div>`;
        $("#imageContainer").append(reviewImage);

        $(document).ready(function () {
            $('.gallery3').each(function () {
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
    preview()
});

function preview() {
    const fromDate = document.getElementById('fromDate').value
    const toDate = document.getElementById('toDate').value
    if (toDate > fromDate) {
        const roomTypeId = localStorage.getItem('roomTypeId')
        const data = {
            roomTypeId: roomTypeId,
            fromDate: fromDate,
            toDate: toDate
        }
        axios.post('https://localhost:7197/api/Booking/CheckRoomById', data)
            .then(function (response) {
                
                console.log("so phong trong:",response.data.data)
                 const roomCount = response.data.data.rooms.length
                const selectElement = document.getElementById('numberRoom')
                const capacityPerRoom = response.data.data.capacity
                let options = '';
                for (let i = 0; i <= roomCount; i++) {
                    options += `<option value="${i}">${i} x phòng -  \u{1F465} x ${capacityPerRoom * i}</option>`;
                }
               selectElement.innerHTML += options
               $('#numberRoom').selectpicker('refresh');         
            }).catch(function (error) {
                console.error('Lỗi khi lấy dữ liệu:', error);
            })
    }

    const price = document.getElementById('roomPrice').value
    const fromDateFormatted = new Date(fromDate);
    const toDateFormatted = new Date(toDate);
    const night = (toDateFormatted - fromDateFormatted) / (1000 * 60 * 60 * 24);
    console.log(night)
    if (night > 0) {
        document.getElementById('totalDay').textContent = `${night + 1} ngày ${night} đêm`
        document.getElementById('totalPrice').textContent = `${(night * price).toLocaleString('vi-VN')}đ`
    } else {
        document.getElementById('totalDay').textContent = ``
        document.getElementById('totalPrice').textContent = ``
    }
    localStorage.setItem('fromDate', fromDate);
    localStorage.setItem('toDate', toDate);
    localStorage.setItem('numberPerson', numberPerson);
    localStorage.setItem('selectedRooms', JSON.stringify(selectedRooms));
    window.location.href = 'http://127.0.0.1:5500/user/booking.html';

}