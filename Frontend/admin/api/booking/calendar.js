
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
        var name = info.event.title;
      

      

        $('#edit_booking').modal('show');
    }

});
calendar.render();
document.addEventListener('DOMContentLoaded', function() {
getAll()
});


function getAll(){
    axios.get('https://localhost:7197/api/Booking/GetAll')
    .then(function(respone){
       const bookings = respone.data.data
       bookings.forEach( booking => {
        calendar.addEvent({
            id: booking.id,            
            title: 'Mã đơn: ' + booking.code + ' - ' + booking.name,        
            start: new Date(booking.fromDate),         
            end: new Date(booking.toDate),   
            backgroundColor: '#6495ED',
            extendedProps:{
                name:booking.name,
                
            }       
        });
       });
       calendar.render();
    })
}

