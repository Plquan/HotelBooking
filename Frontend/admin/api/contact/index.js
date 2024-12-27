let DEFAULT_PAGE_INDEX = 1;
let DEFAULT_PAGE_SIZE = 5;
let MAX_PAGE = 3;
let KEYWORD = null
let FILTER_TYPE = null
getListPaging()

function getListPaging(keyWord = KEYWORD, filterType = FILTER_TYPE) {
    const tableBody = document.getElementById('showContact');
            tableBody.innerHTML = '';
    const pagingModel = {
        pageIndex: DEFAULT_PAGE_INDEX,
        pageSize: DEFAULT_PAGE_SIZE,
        keyWord: keyWord,
        filterType: filterType
    }
    axios.post('https://localhost:7197/api/Contact/GetListPaging', pagingModel)
        .then(function (response) {
            console.log(response)

            const respData = response.data.data
            const contacts = respData.items;
            if((!contacts ||contacts.length === 0) && DEFAULT_PAGE_INDEX > 1 ){
                DEFAULT_PAGE_INDEX = 1
                getListPaging()
            }
            let counter = (respData.pageIndex - 1) * respData.pageSize + 1;
            contacts.forEach(contact => {
                const row = document.createElement('tr');
                row.innerHTML = `
                                   <input type="hidden" id="message"  value="${contact.message}">
                                   <input type="hidden" id="toEmail"  value="${contact.message}">
                                    <td>${counter}</td>
                                    <td>${contact.userName}</td>
                                    <td  title="This is additional information">${contact.email}</td>
                                    <td>${contact.phone}</td>
                                    <td><button type="button" data-target="#contentModal" onclick="contentModal('${contact.message}')" data-toggle="modal" class="btn btn-primary"><i class ="fa fa-eye"></i></button></td>
                                    <td>                                   
                                    <p><i class="fa fa-clock"></i> ${new Date(contact.createdDate).toLocaleString()}</p></td>                                 
                                    <td title = "${replyTitle(contact.status)}"><button type="button" class="btn ${replyClass(contact.status)} ">${replyStatus(contact.status)}</button></td>
                                    <td class="text-right">                        
                                    <a onclick="sendReply(${contact.id},'${contact.email}')" type="button" data-toggle="modal" data-target="#replyContact" class="btn btn-icon btn-success">
                                  <i class="fa fa-reply"></i>
                                  </a>
                                  <a onclick="deleteRoomType(${contact.id})" type="button" class="btn btn-icon btn-danger" data-toggle="modal"
                                  data-target="#delete_asset">
                                  <i class="fa fa-trash"></i>
                                  </a>    
                                  </td>`;
                tableBody.appendChild(row);
                counter++;               
            });
            renderPagination(respData)
        })
        .catch(function (error) {
            console.error('Lỗi khi lấy dữ liệu:', error);
        })

}
function replyStatus(status){
    let iclass = ''
    switch(status){
        case "Pending":
            iclass = '<i class="fas fa-circle-notch fa-spin"></i>'
            break
        case "Replied":
            iclass = '<i class="fa fa-check "></i>'
            break
        default:
             iclass = '<i class=""></i>'
            break;    
    }
    return iclass
}
function replyTitle(status){
    let title = ''
    switch(status){
        case "Pending":
            title = 'Chưa phản hồi'
            break
        case "Replied":
            title = 'Đã phản hồi'
            break
        default:
            title = ''
            break;    
    }
    return title
}
function replyClass(status){
    let iclass = ''
    switch(status){
        case "Pending":
            iclass = 'btn-warning'
            break
        case "Replied":
            iclass = 'btn-primary'
            break
        default:
             iclass = 'btn-info'
            break;    
    }
    return iclass
}
function contentModal(message) {
    document.getElementById('showMessage').value = `${message}`
}
function sendReply(cid, email) {
    document.getElementById('email').textContent = email
    document.getElementById('myButton').addEventListener('click', function () {
        $('#replyContact').modal('hide');
        const replyMessage = document.getElementById('replyMessage').value
        const data = {
            id: cid,
            email: email,
            message: replyMessage
        }
        axios.post('https://localhost:7197/api/Contact/ReplyEmail', data)
            .then(function (response) {
                if (response.data.data.isSuccess) {
                    document.getElementById('replyMessage').value = ''
                    toastr.info('Đã gửi phản hồi')
                    getListPaging()
                }
            })
            .catch(function (error) {
                console.error('Lỗi khi lấy dữ liệu:', error);
            });

    });
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
function searchContact() {
    const keyWord = document.getElementById('searchInput').value
    const filterType = 'input'
    KEYWORD = keyWord
    FILTER_TYPE = filterType
    getListPaging()
}
function toggleLoader(show) {
    const loader = document.getElementById('loader');
    loader.style.display = show ? 'inline-block' : 'none';
}