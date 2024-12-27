
var calendarEl = document.getElementById('calendar');
var calendar = new FullCalendar.Calendar(calendarEl, {
    locale: 'vi', 
    timezone: 'local',
    headerToolbar: {
        left: 'prev,next today',
        center: 'title',
        right: 'dayGridMonth,timeGridWeek,timeGridDay'
    },
    buttonText: {
        today: 'Hôm nay',
        month: 'Tháng',
        week: 'Tuần',
        day: 'Ngày',
        list: 'Danh sách'
    },
    initialView: 'dayGridMonth', 
    events: [],
    displayEventTime: false,
    editable: false,  
    droppable: false, 
    eventClick: function(info) {
        const extendedProps = info.event.extendedProps;

        document.getElementById('userName').textContent = extendedProps.userName || 'N/A'; 
        document.getElementById('phone').textContent = extendedProps.phone || 'N/A';
        document.getElementById('email').textContent = extendedProps.email || 'N/A';
        document.getElementById('fromDate').textContent = extendedProps.fromDate ? extendedProps.fromDate : 'N/A';
        document.getElementById('toDate').textContent = extendedProps.toDate ? extendedProps.toDate : 'N/A';
        document.getElementById('totalPrice').textContent = extendedProps.totalPrice ? extendedProps.totalPrice.toLocaleString('vi-VN') + " đ" : 'N/A';
        document.getElementById('totalPerson').textContent = extendedProps.totalPerson ? extendedProps.totalPerson + " người" : 'N/A';
        document.getElementById('note').value = extendedProps.note || '';
    
        let roomHtml= ''
        const rooms = extendedProps.rooms
        rooms.forEach(room => {
             roomHtml += `${room.roomNumber} `
        });
        document.getElementById('totalRoom').textContent = roomHtml

        $('#booking_detail').modal('show');
    }
    

});
calendar.render();
document.addEventListener('DOMContentLoaded', function() {
getAll()
});


function getAll(){
    axios.get('https://localhost:7197/api/Booking/GetAll')
    .then(function(response){
       const bookings = response.data.data.data
       bookings.forEach( booking => {
        calendar.addEvent({
            id: booking.id,            
            title: 'Mã đơn: ' + booking.code + ' - ' + booking.userName,        
            start: new Date(booking.fromDate),         
            end: new Date(booking.toDate),   
            backgroundColor: eventColor(booking.status),
            extendedProps:{
                userName:booking.userName,
                email: booking.email,
                phone:booking.phone,
                code:booking.code,
                fromDate:booking.fromDate,
                toDate:booking.toDate,
                totalPerson: booking.totalPerson,
                totalPrice:booking.totalPrice,
                note:booking.note,
                rooms:booking.rooms            
            }       
        });
       });
       calendar.render();
    })
}

function eventColor(status) {
    switch (status) {
        case 'Pending':
            return '#4682B4'; 
        case 'Confirmed':
            return 'orange';     
        case 'CheckIn':
            return 'green';
        default:
            return 'gray'; 
    }
}
