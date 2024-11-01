

document.addEventListener('DOMContentLoaded', function () {
    const roomId = localStorage.getItem('roomDetailId');
    axios.get(`https://localhost:7197/api/RoomType/GetById/${roomId}`)
        .then(function (respone) {

            const data = respone.data;

            console.log(data)
            document.getElementById('capacity').textContent = `Tối đa: ${data.capacity} người`;
            document.getElementById('size').textContent = `Diện tích: ${data.size}`;
            document.getElementById('view').textContent = `Hướng nhìn: ${data.view}`;
            document.getElementById('bedType').textContent = `Loại giường: ${data.bedType}`;
            document.getElementById('content').innerHTML = data.content;
            const roomServiceList = document.getElementById("roomService");
            const services = data.roomServices;
            services.forEach(service => {
                const li = document.createElement("li");
                li.textContent = service.name;
                roomServiceList.appendChild(li);
            });

            const facilityContainer = document.getElementById('facilityContainer');

            let colDiv = document.createElement("div");
            colDiv.className = "col-xs-6 col-lg-4";
            let ul = document.createElement("ul");
            colDiv.appendChild(ul); // Đảm bảo `ul` được thêm vào `colDiv`

            const facilities = data.roomFacilitys;
            facilities.forEach((facility, index) => {

                const li = document.createElement("li");
                li.textContent = facility.name;
                ul.appendChild(li);

                if ((index + 1) % 3 === 0) { // Sau mỗi 3 mục, tạo `colDiv` mới
                    failityContainer.appendChild(colDiv);

                    colDiv = document.createElement("div");
                    colDiv.className = "col-xs-6 col-lg-4";
                    ul = document.createElement("ul");
                    colDiv.appendChild(ul); // Thêm `ul` vào `colDiv` mới
                }
            });

            // Thêm `colDiv` cuối cùng nếu còn mục lẻ
            if (ul.hasChildNodes()) {
                facilityContainer.appendChild(colDiv);
            }


        })
});

