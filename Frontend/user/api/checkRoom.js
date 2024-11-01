document.addEventListener('DOMContentLoaded', function () {
 const fromDate =    localStorage.getItem('fromDate');
    const toDate = localStorage.getItem('toDate');

    document.getElementById('arrivalDate').value = arrivalDate;
    document.getElementById('endDate').value = endDate;
    if (arrivalDate === endDate) {
        toastr.success("vc");
    }
    if (arrivalDate <= endDate) {
        toastr.success(endDate - arrivalDate);
    }
    if (arrivalDate >= endDate) {
        toastr.success("arival > enddate");
    }
});

function changeSearch() {
    const arrivalDate = document.getElementById('arrivalDate').value;
    const endDate = document.getElementById('endDate').value;
    if (arrivalDate >= endDate) {
        toastr.warning("Ngày rời đi lớn hơn ngày đến !!!");
        return;
    }
}