const accessToken = localStorage.getItem('accessToken'); 

axios.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`;


let DEFAULT_PAGE_INDEX = 1;
let DEFAULT_PAGE_SIZE = 5;
let MAX_PAGE = 3;
let KEYWORD = null
let FILTER_TYPE = null

getListPaging()

function getListPaging(keyWord,filterType){
    const pagingModel = {
        pageIndex: DEFAULT_PAGE_INDEX,
        pageSize: DEFAULT_PAGE_SIZE,
        keyWord: keyWord,
        filterType: filterType
    }
    axios.post(`https://localhost:7197/api/Booking/GetPaymentHistory`,pagingModel)
    .then(function (response) {
        console.log(response)
        const respData = response.data.data
        const bookings = response.data.data.items;
        const tableBody = document.getElementById('showBooking');
        tableBody.innerHTML = '';
       
        let counter = (respData.pageIndex - 1) * respData.pageSize + 1;
        bookings.forEach(booking => {
            const row = document.createElement('tr');
            row.setAttribute('data-id', booking.id);
            row.innerHTML = `
                                    <td>${counter}</td>
                                    <td>${booking.code}</td>
                                    <td>${booking.paymentMethod}</td>
                                    <td><button type="button" class="btn btn-warning btn-sm">Warning</button></td>
                                    <td> 
                                       <p><i class="fa fa-user"></i> ${booking.userName}</p>                                         
                                    <p><i class="fa fa-clock"></i> ${new Date(booking.createdDate).toLocaleString()}</p></td>
                                      <td>
                               <select class="form-control" onchange="updateStatus(${booking.id},this.value)" >
                                  ${bookingStatus(booking.status)}                                                                      
                               </select>
                                  </td>
                                    <td class="text-right">                        
                                <a onclick="showBookingDetail(${booking.id})" type="button" data-toggle="modal" data-target="#booking_detail" class="btn btn-icon btn-success">
                                 <i class="fa fa-search"></i>
                                  </a>
                                  <a onclick="deleteRoomType(${booking.id})" type="button" class="btn btn-icon btn-danger" data-toggle="modal"
                                  data-target="#delete_asset">
                                  <i class="fa fa-trash"></i>
                                  </a>    
                                  </td>`;
            tableBody.appendChild(row);
            counter++;
            renderPagination(respData)
        });
    })
    .catch(function (error) {
        console.error('Lỗi khi lấy dữ liệu:', error);
    });
}



function showBookingDetail(id){
    axios.get(`https://localhost:7197/api/Booking/GetBookingDetail/${id}`)
    .then(function(response){
        const respData = response.data.data.data
        document.getElementById('userName').textContent = respData.userName
        document.getElementById('phone').textContent = respData.phone
        document.getElementById('email').textContent = respData.email
        document.getElementById('fromDate').textContent = respData.fromDate
        document.getElementById('toDate').textContent = respData.toDate
        document.getElementById('totalPrice').textContent = respData.totalPrice.toLocaleString('vi-VN') +" đ"
        document.getElementById('totalPerson').textContent = respData.totalPerson + " người"
        document.getElementById('note').value = respData.note

        let roomHtml= ''
        const rooms = respData.rooms
        rooms.forEach(room => {
             roomHtml += `${room.roomNumber} `
        });
        document.getElementById('totalRoom').textContent = roomHtml

    }).catch(function (error) {
        console.error('Lỗi khi cập nhật dữ liệu:', error);
    });
}

function bookingStatus(status) {
    let options;
    switch (status) {
        case "Pending":
            options = `
                <option value="" hidden disabled selected>Đang chờ</option>
                <option style="color: #228B22;" value="Confirmed">Xác nhận</option>
                <option style="color: red;" value="Cancelled">Hủy đơn</option>
            `;
            break;

        case "Confirmed":
            options = `
                <option value="" hidden disabled selected>Giữ chỗ</option>
                <option value="CheckIn">Nhận phòng</option>
                <option value="CheckOut">Trả phòng</option>
                <option value="Cancelled">Hủy đơn</option>
            `;
            break;

        case "CheckIn":
            options = `
                <option value="" hidden disabled selected>Đang nhận phòng</option>
                <option value="CheckOut" style="color: red;">Trả phòng</option>
            `;
            break;
        case "Cancelled":
                options = `
                    <option value="" hidden disabled selected>Đã hủy</option>
                `;
                break;
        case "CheckOut":
                options = `
                    <option value="" hidden disabled selected>Đã trả phòng</option>
                `;
                break;
        default:
            options = `
                <option value="" hidden disabled selected>Trạng thái không xác định</option>
            `;
            break;
    }
    return options;
}

function updateStatus(bookingId,status){
    axios.get(`https://localhost:7197/api/Booking/UpdateStatus?bookingId=${bookingId}&status=${status}`)
    .then(function(response){
        if(response.data.data.statusCode === 200){
            toastr.info('Cập nhật trạng thái thành công')
            getListPaging()
        }
        else{
            toastr.warning('Cập nhật không thành công')
            getListPaging()
        }
    }) .catch(function (error) {
        console.error('Lỗi khi cập nhật dữ liệu:', error);
    });
}
function filterBooking(keyWord){
 if(keyWord === "All"){
    getListPaging()
    return
 }
 axios.get(`https://localhost:7197/api/Booking/FilterBooking?keyWord=${keyWord}`)
 .then(function(response){
   console.log(response.data)
 })
}

function renderPagination(data) {
    if(data === null || data.length === 0){
        return
    }
    const pagination = document.getElementById("pagination");
    pagination.innerHTML = ''
    let pageItem = '';

    let startPage = Math.max(1, data.pageIndex - Math.floor(MAX_PAGE / 2));
    let endPage = Math.min(data.totalPages, startPage + MAX_PAGE - 1);

    if (endPage - startPage + 1 < MAX_PAGE) {
        startPage = Math.max(1, endPage - MAX_PAGE + 1);
    }

    for (let i = startPage; i <= endPage; i++) {
        pageItem += `
            <li class="page-item ${data.pageIndex === i ? "active" : ""}">
                <a class="page-link" href="#" onclick="loadPage(${i}, ${data.totalPages})">${i}</a>
            </li>`;
    }
    pagination.innerHTML = `
        <li class="page-item ${data.hasPreviousPage ? "" : "disabled"}">
            <a class="page-link" href="#" onclick="loadPage(${data.pageIndex - 1}, ${data.totalPages})">Trang trước</a>
        </li>
        ${pageItem}
        <li class="page-item ${data.hasNextPage ? "" : "disabled"}">
            <a class="page-link" href="#" onclick="loadPage(${data.pageIndex + 1}, ${data.totalPages})">Trang sau</a>
        </li>`;
}
function loadPage(pageIndex, totalPages) {
    if (pageIndex < 1 || pageIndex > totalPages) {
        return;
    }
    DEFAULT_PAGE_INDEX = pageIndex
    getListPaging();
}
function updateEntries(value) {
    DEFAULT_PAGE_SIZE = value
    getListPaging()
}
function filterContact(value) {
    const filterType = 'select'
    KEYWORD = value
    FILTER_TYPE = filterType
    getListPaging()
}