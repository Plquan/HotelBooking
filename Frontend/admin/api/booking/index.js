document.addEventListener('DOMContentLoaded', function () {

    axios.get('https://localhost:7197/api/Booking/GetAll')
        .then(function (response) {
            const bookings = response.data.data;
            const tableBody = document.getElementById('showBooking');
            tableBody.innerHTML = '';



            let counter = 1;
            bookings.forEach(booking => {
                const row = document.createElement('tr');
                row.setAttribute('data-id', booking.id);
                row.innerHTML = `
                                         <td>${counter}</td>
                                            <td>${booking.code}</td>
                                            <td>${booking.name}</td>
                                              <td>${booking.phone}</td>
 
                                     	<td>
																
								     	</td>
                                                <td class="text-right">
                                    <a onclick="editRoomTypeId(${booking.id})" type="button" data-toggle="modal" data-target="#booking_detail" class="btn btn-icon btn-success">
                                     <i class="fa fa-search"></i>
                                      </a>
                                      <a onclick="deleteRoomType(${booking.id})" type="button" class="btn btn-icon btn-danger" data-toggle="modal"
                                      data-target="#delete_asset">
                                      <i class="fa fa-trash"></i>
                                      </a>
                                                    
                                                </td>`;
                tableBody.appendChild(row);
                counter++;
            });
        })
        .catch(function (error) {
            console.error('Lỗi khi lấy dữ liệu:', error);
        });
});

