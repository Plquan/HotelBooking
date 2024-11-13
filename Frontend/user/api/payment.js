document.addEventListener('DOMContentLoaded', function () {
    const fromDate = localStorage.getItem('fromDate');
    const toDate = localStorage.getItem('toDate');
    const numberPerson = localStorage.getItem('numberPerson');
    const selectedRooms = JSON.parse(localStorage.getItem('selectedRooms')) || [];

   console.log(selectedRooms)
    document.getElementById('fromDate').textContent = fromDate
    document.getElementById('toDate').textContent = toDate

})