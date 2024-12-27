let paramPayment = window.location.search;
let currentUrl = window.location.href;
// let cleanUrl = currentUrl.split('?')[0];
// window.history.pushState({}, document.title, cleanUrl);

checkPaymentResult()
function checkPaymentResult(){
   if(paramPayment != null && paramPayment != ""){
    axios.get(`https://localhost:7197/api/Booking/PaymentExecute${paramPayment}`)
    .then(function(response){
       if(response.data.statusCode === '00'){
         console.log(response.data.data)
       }
    }).catch(function (error) {
       console.error('Lỗi máy chủ', error);
    })
   }
}