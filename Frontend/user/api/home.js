function checkDate() {
    const fromDate = document.getElementById('fromDate').value;
    const toDate = document.getElementById('toDate').value;
    const numberPerson = document.getElementById('numberPerson').value;

    localStorage.setItem('fromDate', fromDate);
    localStorage.setItem('toDate', toDate);
    localStorage.setItem('numberPerson', numberPerson);
    window.location.href = 'http://127.0.0.1:5500/user/checkRoom.html';
}

const baseUrl = "https://localhost:7197/images";

 getAllRoom()


function getAllRoom(){
    toggleLoading(true)
        axios.get('https://localhost:7197/api/RoomType/GetAll')
        .then(function (response) {
           
            const rooms = response.data;
            const container = document.getElementById('rooms');

            let itemDiv = document.createElement('div');
            itemDiv.classList.add('item');
            if (rooms.length > 0) {
                rooms.forEach((room, index) => {
                    const roomHTML = `
                        <div class="col-lg-4 col-md-4 col-sm-6 col-xs-6">
                            <div class="wrap-box">
                                <div class="box-img">
                                    <img src="${baseUrl}/${room.roomImages[0].url}" class="img-responsive" alt="${room.name}" title="${room.name}">
                                </div>
                                   <a>
                                    <div class="rooms-content">
                                        <h4 class="sky-h4">${room.name}</h4>
                                        <p class="price">${room.price.toLocaleString('vi-VN')}Đ / NGÀY</p>
                                    </div>
                               </a>
                            </div>
                        </div>`
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
            }
            else {
                container.innerHTML = '<p>Hiện không có phòng!</p>'
            }

        })
        .catch(function (error) {         
            console.error('Error fetching room data:', error);
        }).finally(function(){
            toggleLoading(false)     
        })
}

function toggleLoading(show) {
    const overlay = document.getElementById("overlay");

    if (show) {
        overlay.style.display = "flex";
    } else {
        setTimeout(() => {
            overlay.style.display = "none"; 
        }, 400); 
    }
}
