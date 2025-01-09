const baseUrl = "https://localhost:7197/images";
document.addEventListener('DOMContentLoaded', function () {
    const fromDate = localStorage.getItem('fromDate');
    const toDate = localStorage.getItem('toDate');
    const numberPerson = localStorage.getItem('numberPerson');

    document.getElementById('fromDate').value = fromDate
    document.getElementById('toDate').value = toDate
    document.getElementById('numberPerson').value = numberPerson
    localStorage.removeItem('fromDate');
    localStorage.removeItem('toDate');
    localStorage.removeItem('numberPerson');

    const data = {
        fromDate: fromDate,
        toDate: toDate
    }
    checkRoom(data)
});
function checkRoom(data){
    axios.post('https://localhost:7197/api/Booking/CheckRoom', data)
    .then(function (respone) {
        const respData = respone.data
        console.log(respData);

        const container = document.getElementById('roomTypeContainer')
        container.innerHTML = ''
        if (respData.length > 0) {
            respData.forEach(roomType => {
                 if(roomType.rooms.length === 0){                      
                    return
                 }
                const fromDateFormatted = new Date(fromDate);
                const toDateFormatted = new Date(toDate);
                const night = (toDateFormatted - fromDateFormatted) / (1000 * 60 * 60 * 24);

                let options = '';
                for (let i = 1; i <= roomType.rooms.length; i++) {
                    const VNDprice = (i * roomType.price * night).toLocaleString('vi-VN');
                    options += `<option value="${i}">${i} (VND ${VNDprice})</option>`;
                }
                //content
                if (roomType.content.length > 150) {
                    roomType.content = roomType.content.slice(0, 150) + "...";
                }
                const itemDiv = document.createElement('div')
                itemDiv.classList.add('reservation-room_item')
                const roomHTML = ` 
                                        <h2 class="reservation-room_name"><a href="#">${roomType.name}</a></h2>
                                        <div class="reservation-room_img">
                                            <a href="#"><img src="${baseUrl}/${roomType.roomImages[0].url}" alt="Ảnh bìa" class="img-responsive"></a>
                                        </div>
                                        <div class="reservation-room_text">
                                            <div class="reservation-room_desc">
                                                ${roomType.content}
                                                <ul>
                                                    <li>Giá cho ${night} đêm: (VND ${(roomType.price * night).toLocaleString('vi-VN')})</li>
                                                    <li>Số lượng: ${roomType.capacity} khách 1 phòng</li>
                                                    <li>Đánh giá</li>
                                                </ul>
                                            </div>
                                            <a href="#" class="reservation-room_view-more">View More Infomation</a>
                                            <div class="clear"></div>

                                           <div>
                                             <span>Chọn phòng</span>
                                            <select data-roomType-id = "${roomType.id}" data-number="${roomType.capacity}" 
                                            data-room-name = ${roomType.name} data-room-price = ${roomType.price}
                                            class="btn btn-room form-select"> 
                                                 <option value="0" selected>0</option>                                        
                                              ${options}
                                            </select></div>            
                                    </div>
                                  `
                itemDiv.innerHTML += roomHTML;
                container.appendChild(itemDiv)
            });
            if(container.textContent.trim() === ""){
                toastr.info("Không có phòng trống")
            }
        }
        else {
            container.innerHTML = '<h4 style="text-align: center; font-family: Poppins, sans-serif;">Không có phòng trống !</h4>'
        }

    });
}

function checkDate() {
    const fromDate = document.getElementById('fromDate').value || null;
    const toDate = document.getElementById('toDate').value || null;
    if (!fromDate || !toDate) {
        toastr.warning("Vui lòng nhập ngày tháng !!!")
    }
    if (fromDate <= toDate) {

    }
    const data = {
        fromDate: fromDate,
        toDate: toDate
    }
    checkRoom(data)
}

function confirm() {
    const fromDate = document.getElementById('fromDate').value || null;
    const toDate = document.getElementById('toDate').value || null;
    const numberPerson = document.getElementById('numberPerson').value || null;
    if (!fromDate || !toDate) {
        toastr.warning("Chưa nhập ngày tháng !")
        return;
    }
    if (!numberPerson || numberPerson < 1) {
        toastr.warning("Số lượng người phải lớn hơn 1")
        return;
    }
    const container = document.getElementById('roomTypeContainer');
    const selectElements = Array.from(container.querySelectorAll('select'));
    let totalPerson = 0
    const selectedRooms = [];
    if (selectElements.length > 0) {
        selectElements.forEach(select => {
            const roomTypeId = select.getAttribute('data-roomType-id')
            const capacity = select.getAttribute('data-number')
            const roomName = select.getAttribute('data-room-name')
            const roomPrice = select.getAttribute('data-room-price')
            const selectedValue = select.value;          
            if (selectedValue > 0 || selectedValue != null) {
                const roomSelected = capacity * selectedValue
                totalPerson += roomSelected
                selectedRooms.push({
                    roomTypeId: roomTypeId,
                    number: selectedValue,
                    name: roomName,
                    price: roomPrice
                });
            }
        });
    }
    console.log(totalPerson)
   if(totalPerson < numberPerson){
    toastr.info(`không đủ phòng cho ${numberPerson} người !!!`)
    return
   }
    localStorage.setItem('fromDate', fromDate);
    localStorage.setItem('toDate', toDate);
    localStorage.setItem('numberPerson', numberPerson);
    localStorage.setItem('selectedRooms', JSON.stringify(selectedRooms));
    window.location.href = 'http://127.0.0.1:5500/user/booking.html';
}
function toggleLoading(show) {
    const overlay = document.getElementById("overlay");

    if (show) {
        overlay.style.display = "flex";
    } else {
        setTimeout(() => {
            overlay.style.display = "none"; 
        }, 300); 
    }
}
