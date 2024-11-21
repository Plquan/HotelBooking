document.addEventListener('DOMContentLoaded', function() {

});

var calendarEl = document.getElementById('calendar');
var calendar = new FullCalendar.Calendar(calendarEl, {
    locale: 'vi', 
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
    events: [

 

    ],
    dateClick: function(info) {
        var eventDateInput = document.querySelector('#add_new_event');
        eventDateInput.value = info.dateStr; // info.dateStr là ngày được nhấn (YYYY-MM-DD)
        
        $('#add_new_event').modal('show');
    },
    // eventClick: function(info) {
    //     // Lấy thông tin sự kiện đã click
    //     var eventTitle = info.event.title;
    //     var eventStart = info.event.start.toLocaleString();
    //     var eventEnd = info.event.end ? info.event.end.toLocaleString() : 'Không có ngày kết thúc';
    //     var eventDescription = info.event.extendedProps.description;

    //     // Hiển thị thông tin trong modal
    //     document.getElementById('eventTitle').innerText = eventTitle;
    //     document.getElementById('eventStart').innerText = 'Ngày bắt đầu: ' + eventStart;
    //     document.getElementById('eventEnd').innerText = 'Ngày kết thúc: ' + eventEnd;
    //     document.getElementById('eventDescription').innerText = 'Mô tả: ' + eventDescription;

    //     // Mở modal
    //     $('#eventModal').modal('show');
    // }
    
});

calendar.render();

function getAll(){
    
}