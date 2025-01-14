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
    toggleLoading(true)
    axios.post(`https://localhost:7197/api/Booking/GetListPaging`,pagingModel)
    .then(function (response) {
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
                                    <td><button type="button" class="btn btn-primary" style="background-color: #4682B4;">${booking.paymentMethod === 'COD' ?
                                    '<i class ="fa fa-hand-holding-usd">' :'<i class ="fa fa-credit-card"> </i>'}
                                    </button></td>
                                    <td><button type="button" onclick="showTransaction('${booking.paymentMethod}',${booking.id})" class="btn ${paymentClass(booking.paymentStatus)} ">${paymentStatus(booking.paymentStatus)}</button></td>
                                    <td>  
                                    <p><i class="fa fa-user"></i> ${booking.userName}</p>                                 
                                    <p><i class="fa fa-clock"></i> ${new Date(booking.createdDate).toLocaleString()}</p>
                                    </td>
                                    <td>  
                                    <p><i class="fa fa-user"></i> ${booking.confirmBy ?? '...'}</p>                                 
                                    <p><i class="fa fa-clock"></i> ${booking.confirmDate ? new Date(booking.confirmDate).toLocaleString() : '...'}</p>
                                    </td>
                                    <td>                                
                                    <select class="form-control" onchange="updateStatus(${booking.id},this.value)" >
                                    ${bookingStatus(booking.status)}                                                                      
                                    </select>
                                    </td>
                                    <td class="text-right">                        
                                    <a onclick="showBookingDetail(${booking.id})" type="button"  class="btn btn-icon btn-success" data-toggle="modal" data-target="#booking_detail">
                                    <i class="fa fa-search"></i>
                                    </a>
                                    <button onclick="refund(${booking.id})" style="background-color: #4682B4;" ${booking.paymentMethod === 'OP' && booking.paymentStatus === 'Paid' ? '' : 'disabled'} type="button" data-toggle="modal" data-target="#refund_detail" class="btn btn-icon btn-success">
                                    <i class="fa fa-exchange-alt"></i>
                                    </button> 
                                    <a onclick="deleteBooking(${booking.id})" type="button" class="btn btn-icon btn-danger" data-toggle="modal"
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
    }).finally(function(){
        toggleLoading(false)     
    })
}
function refund(id){
    toggleLoading(true)     
    axios.get(`https://localhost:7197/api/Booking/GetBookingDetail/${id}`)
    .then(function(response){
        const respData = response.data.data.data
        document.getElementById('code').textContent = respData.code
        document.getElementById('bookingPrice').textContent = respData.totalPrice.toLocaleString('vi-VN') +" đ"
        document.getElementById('bookingId').value = respData.id
    }).catch(function (error) {
        console.error('Lỗi:', error);
    }).finally(function(){
        toggleLoading(false)     
    })

}

function showTransaction(method,id){
switch (method){
    case "OP":
        toggleLoading(true)
        axios.get(`https://localhost:7197/api/Booking/GetTransactionDetail?BookingId=${id}`)
        .then(function(response){
            const resp = response.data.data
            if(resp.isSuccess){        
                document.getElementById('transactionId').textContent = resp.data.transactionId
                document.getElementById('totalAmount').textContent = resp.data.amount.toLocaleString('vi-VN') +" đ"
                document.getElementById('refundAmount').textContent = resp.data.refundAmount.toLocaleString('vi-VN') +" đ"
                document.getElementById('createdDate').textContent = new Date(resp.data.createdDate).toLocaleString()
                $('#transaction_detail').modal('show')
            }
            else{
                toastr.warning(resp.data.message)
            }
          
        }).catch(function (error) {
             console.error('Lỗi khi lấy dữ liệu:', error);
         }).finally(function(){
             toggleLoading(false)     
         })
       break
    case "COD":
        toastr.info("Đơn đặt phòng này thanh toán trực tiếp")
        break
    default:
        toastr.info("Ko xác định")
        break
}
 
}
function confirmRefund(){
    const typeRefund = document.querySelector('input[name="radio"]:checked').value
    const amount = document.getElementById('amount').value || 0
    const refundNote = document.getElementById('refundNote').value
    const bookingId = document.getElementById('bookingId').value

    if(!typeRefund){
        toastr.warning('Chưa chọn kiểu hoàn tiền')
        return
    }
    const data = {
        refundAmount:amount,
        refundReason:refundNote,
        bookingId:bookingId,
        transactionType:typeRefund
     }
     toggleLoading(true)   
    axios.post('https://localhost:7197/api/Payment/RefundAsync',data)
    .then(function(response){
        const resp = response.data.data
        console.log(response.data)
        if(resp.isSuccess){
            getListPaging()
        }
        else{
            toastr.error(resp.message)
        }
    }).catch(function (error) {
        console.error('Lỗi:', error);
    }).finally(function(){
        $('#refund_detail').modal('hide')
        toggleLoading(false)     
    })
}
function paymentClass(status){
    let iclass = ''
    switch(status){
        case "Unpaid":
            iclass = 'btn-warning'
            break
        case "Paid":
            iclass = 'btn-primary'
            break
        default:
             iclass = 'btn-info'
            break;    
    }
    return iclass
}
function paymentStatus(status){
    let iclass = ''
    switch(status){
        case "Unpaid":
            iclass = '<i class="fas fa-circle-notch fa-spin"></i>'
            break
        case "Paid":
            iclass = '<i class="fa fa-check "></i>'
            break
        case "Refund":
            iclass = '<i class=" fa-check-circle"></i>'    
        default:
             iclass = '<i class="fa fa-undo-alt"></i>'
            break;    
    }
    return iclass
}
function deleteBooking(id) {
    const button = document.getElementById('confirmDelete');
    button.addEventListener('click', function () {
        axios.delete(`https://localhost:7197/api/Booking/deleteBooking?id=${id}`)
            .then(function () {
                $('#delete_asset').modal('hide');
                getListPaging()
                toastr.success("Xóa thành công");
            })
            .catch(function (error) {
                console.error("Error deleting roomtype:", error);
                alert("Lỗi khi xóa : " + error.response.data.message);
            });
    }, { once: true });
}
document.getElementById('searchInput').addEventListener('input', function () {
    const searchTerm = this.value.toLowerCase();  
    const tableRows = document.querySelectorAll('#showBooking tr'); 

    tableRows.forEach(function (row) {
        const columns = row.querySelectorAll('td'); 
        let isMatch = false; 
      
        columns.forEach(function (column) {
            if (column.textContent.toLowerCase().includes(searchTerm)) {
                isMatch = true;
            }
        });     
        row.style.display = isMatch ? '' : 'none';
    });
})
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
    const data = {
        bookingId:bookingId,
        status:status
    }
    toggleLoading(true)     
    axios.post(`https://localhost:7197/api/Booking/UpdateStatus`,data)
    .then(function(response){
        const resp = response.data.data
        console.log(response)
        if(response.data.data.statusCode === 200){
            toastr.info(resp.message)
          getListPaging()
        }
        else{
            toastr.warning(resp.message)
            getListPaging()
        }
    }) .catch(function (error) {
        console.error('Lỗi khi cập nhật dữ liệu:', error);
    }).finally(function(){
        toggleLoading(false)     
    })
}
function filterBooking(keyWord){
    toggleLoading(true)
 if(keyWord === "All"){
    getListPaging()
    return
 }
 axios.get(`https://localhost:7197/api/Booking/FilterBooking?keyWord=${keyWord}`)
 .then(function(response){
   console.log(response.data)
 }).finally(function(){
    toggleLoading(false)     
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
function showInputPrice(value){
const amount = document.getElementById('amount')
if(value === true){
    amount.style.display = 'block'
}
else if(value === false){
    amount.style.display = 'none'
}
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
