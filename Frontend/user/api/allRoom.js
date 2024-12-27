const baseUrl = "https://localhost:7197/images";
document.addEventListener('DOMContentLoaded', function () {
    axios.get('https://localhost:7197/api/RoomType/GetAll')
        .then(function (respone) {
            const data = respone.data;
            var container = document.getElementById('roomTypeContainer');
            container.innerHTML = '';
            console.log(data);
            let itemDiv = document.createElement('div');
            itemDiv.classList.add('row');

            data.forEach(room => {
                const roomHTML =
                    `<div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                        <div class="room-item-1">
                            <h2>${room.name}</h2>
                            <div class="img">
                                <a href="#"><img src="${baseUrl}/${room.roomImages[0].url}" alt="#"></a>
                            </div>
                            <div class="content">
                                ${room.content}
                                <ul>
                                    <li>Tối đa: ${room.capacity} người</li>
                                    <li>Quang cảnh: ${room.view}</li>
                                    <li>Kích thước: ${room.size} cm2</li>
                                    <li>Loại giường: ${room.bedType}</li>
                                </ul>
                            </div>
                            <div class="bottom">
                                <span class="price">Giá tiền <span class="amout">${room.price}Đ</span> / NGÀY</span>
                                <a onclick="roomDetail(${room.id})" class="btn">CHI TIẾT</a>
                            </div>
                        </div>
                    </div>
                     `;
                itemDiv.innerHTML += roomHTML;
              
            });
            container.appendChild(itemDiv);

        })
});

function roomDetail(id) {
    localStorage.setItem('roomTypeId', id);
    window.location.href = 'http://127.0.0.1:5500/user/roomDetail.html';
}

