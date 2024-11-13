function checkDate() {
    const fromDate = document.getElementById('fromDate').value;
    const toDate = document.getElementById('toDate').value;
    const numberPerson = document.getElementById('numberPerson').value;
    
    localStorage.setItem('fromDate', fromDate);
    localStorage.setItem('toDate', toDate);
    localStorage.setItem('numberPerson',numberPerson);
    window.location.href = 'http://127.0.0.1:5500/user/checkRoom.html';
}

const baseUrl = "https://localhost:7197/images";

document.addEventListener('DOMContentLoaded', function () {




    axios.get('https://localhost:7197/api/RoomType/GetAll')
        .then(function (response) {
            const rooms = response.data; // Lấy dữ liệu từ response
            console.log(rooms); // Kiểm tra dữ liệu trả về từ API

            const container = document.getElementById('rooms'); // Sửa lại getElementById

            // Tạo một biến để chứa các item
            let itemDiv = document.createElement('div');
            itemDiv.classList.add('item');

            // Vòng lặp để thêm các phòng vào itemDiv
            rooms.forEach((room, index) => {
                // Tạo HTML cho mỗi phòng
                const roomHTML = `
                        <div class="col-lg-4 col-md-4 col-sm-6 col-xs-6">
                            <div class="wrap-box">
                                <div class="box-img">
                                    <img src="${baseUrl}/${room.thumb}" class="img-responsive" alt="${room.name}" title="${room.name}">
                                </div>
                                   <a href="https://localhost:7060/Room/RoomDetail">
                                    <div class="rooms-content">
                                        <h4 class="sky-h4">${room.name}</h4>
                                        <p class="price">${room.price}Đ / NGÀY</p>
                                    </div>
                               </a>
                            </div>
                        </div>`;


                itemDiv.innerHTML += roomHTML;

               
                if ((index + 1) % 3 === 0) {
                    container.appendChild(itemDiv);
                    itemDiv = document.createElement('div'); 
                    itemDiv.classList.add('item'); 
                }
            });

            
            if (itemDiv.innerHTML.trim() !== '') {
                container.appendChild(itemDiv);
            }

            // Khởi tạo Owl Carousel
            $('#rooms').owlCarousel({
                loop: true,
                nav: true,
                margin: 0,
                /* autoplay: true,
                autoplayTimeout: 12000,*/
                responsive: {
                    0: {
                        items: 1
                    },
                    600: {
                        items: 1
                    },
                    1000: {
                        items: 1
                    }
                }
            });

            console.log("Owl Carousel initialized with jQuery version: " + $.fn.jquery);
        })
        .catch(function (error) {
            console.error('Error fetching room data:', error);
        });
});