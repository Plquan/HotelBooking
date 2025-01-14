let paramPayment = window.location.search;
let currentUrl = window.location.href;
// let cleanUrl = currentUrl.split('?')[0];
// window.history.pushState({}, document.title, cleanUrl);

checkPaymentResult()
function checkPaymentResult(){
   if(paramPayment != null && paramPayment != ""){
    axios.get(`https://localhost:7197/api/Booking/PaymentExecute${paramPayment}`)
    .then(function(response){
      const resp = response.data
       if(resp.statusCode === '00'){
         toastr.info(resp.message)
         document.getElementById('code').textContent = resp.data
         document.getElementById('paymentTitle').textContent = 'THANH TOÁN THÀNH CÔNG'
       }
       else{
         document.getElementById('paymentTitle').textContent = 'THANH TOÁN THÁT BẠI'
       }
    }).catch(function (error) {
       console.error('Lỗi máy chủ', error);
    })
   }
}