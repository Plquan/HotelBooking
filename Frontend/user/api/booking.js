document.addEventListener('DOMContentLoaded', function () {
    const fromDate = localStorage.getItem('fromDate') ;
    const toDate = localStorage.getItem('toDate');
    const numberPerson = localStorage.getItem('numberPerson');
    const selectedRooms = JSON.parse(localStorage.getItem('selectedRooms')) || [];
    
    if(!fromDate || !toDate || !numberPerson){
        window.location.href = 'http://127.0.0.1:5500/user/checkRoom.html'
    }
  console.log(numberPerson)
    const fromDateFormatted = new Date(fromDate);
    const toDateFormatted = new Date(toDate);
    const totalNight = (toDateFormatted - fromDateFormatted) / (1000 * 60 * 60 * 24);

    document.getElementById('fromDate').textContent = `${fromDate}`
    document.getElementById('toDate').textContent = `${toDate}`
    document.getElementById('numberPerson').textContent = `${numberPerson} người`
    document.getElementById('totalNight').textContent = `${totalNight + 1} ngày ${totalNight} đêm`

    const container = document.getElementById('chooseRoom')
    container.innerHTML = ''
    let count = 1;
    let totalPrice = 0
    selectedRooms.forEach(room => {
        totalPrice += room.price * room.number
        const itemDiv = document.createElement('div')
        itemDiv.classList.add('reservation-room-seleted_item')
        itemDiv.innerHTML = ` 
                                        <div class="reservation-room-seleted_name has-package">
                                            <h2><a href="#">${room.name}</a></h2>
                                        </div>
                                        <div class="reservation-room-seleted_package">                                          
                                            <ul>
                                                <li>
                                                    <span id="price">Giá phòng</span>
                                                    <span>${room.price}Đ</span>
                                                </li>
                                                <li>
                                                    <span>Số lượng</span>
                                                    <span>${room.number} phòng</span>
                                                </li>
                                            </ul>
                                        </div>
                                        <div class="reservation-room-seleted_total-room">
                                            Tổng tiền
                                            <span class="reservation-amout">${room.price * room.number}đ</span>
                                    </div>`
        container.appendChild(itemDiv)
        count++
    });
    document.getElementById('totalPrice').textContent = `${totalPrice}đ`
    document.getElementById('totalPrice').setAttribute("data-totalPrice",totalPrice)

})
function placeOrder(){
    const fromDate = localStorage.getItem('fromDate') ;
    const toDate = localStorage.getItem('toDate');
    const numberPerson = localStorage.getItem('numberPerson');
    const selectedRooms = JSON.parse(localStorage.getItem('selectedRooms')) || [];
    const userName = document.getElementById('name').value 
    const email = document.getElementById('email').value 
    const phone = document.getElementById('phone').value 
    const note = document.getElementById('note').value 
    const totalPrice = document.getElementById('totalPrice').getAttribute("data-totalPrice")
    const selectedPayment = document.querySelector('input[name="payment"]:checked').value

    const chooseRoom = []
    selectedRooms.forEach(room => {
      chooseRoom.push({
        roomTypeId: room.roomTypeId,
        number: room.number
      })
   })

    const data = {
        userName: userName,
        email:email,
        phone: phone,
        note: note,
        fromDate: fromDate,
        toDate: toDate,
        totalPrice: totalPrice,
        totalPerson: numberPerson,
        chooseRooms: chooseRoom
    }
    console.log(data)
    
    if(selectedPayment === 'COD'){
        paymentCOD(data)
    }
    if(selectedPayment === 'OP'){
        paymentOP(data)
    }
    else{
        toastr.warning('Chưa chọn phương thức')
    }

}

function paymentOP(data){
const paymentdata = {
    orderType: 'other',
    amount: data.totalPrice,
    orderDescription: 'test 1234',
    name: 'quan',
    booking: data
}
  axios.post('https://localhost:7197/api/Payment/CreatePaymentUrlVnpay',paymentdata)
  .then(function(response){
    const respData = response.data
    console.log(respData)
    if(respData.data){
        window.location.href = respData.data
    }
     
  })
  .catch(function (error) {
    console.error('Lỗi:', error);
});
}

function paymentCOD(data){
    axios.post('https://localhost:7197/api/Booking/PlaceOrder',data)
    .then(function(respone){
        console.log(respone.data.data)
        window.location.href = 'http://127.0.0.1:5500/user/checkRoom.html'
    })
}

function getCurrentUser() {
    axios.get('https://localhost:7197/api/Account/MyInfo', {
        headers: {
            'Authorization': `Bearer ${accessToken}`
        }
    })
        .then(function (response) {
            console.log(response.data.data.userName)

            data = response.data.data
            console.log(data)
        })
        .catch(function (error) {
            console.log(error)
        });
}