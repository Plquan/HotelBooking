let countdown = 50;
let on = false;
let timerId = null;

const resendBtn = document.getElementById("resend-btn");
const timerSpan = document.getElementById("timer");

function startCountdown() {
    if (on) return;
    on = true;
    countdown = 50;
    timerSpan.textContent = countdown + 's';
    resendBtn.classList.add("disabled");
    resendBtn.classList.add("disabled");
    timerId = setInterval(() => {
        countdown -= 1;
        timerSpan.textContent = countdown + 's';

        if (countdown <= 0) {
            clearInterval(timerId);
            timerSpan.textContent = "";
            on = false;
            resendBtn.classList.remove("disabled");
        }
    }, 1000);
}

resendBtn.addEventListener("click", () => {
    startCountdown();
});

function register() {
    toggleLoading(true)
    const userName = document.getElementById('userName').value
    const email = document.getElementById('email').value
    const phone = document.getElementById('phone').value
    const password = document.getElementById('password').value
    const confirmPassword = document.getElementById('confirmPassword').value

    if (password != confirmPassword) {
        toastr.error('Mật khẩu không trùng khớp')
        return
    }
    if(!userName || !email || !phone || !password || !confirmPassword){
        toastr.warning('Vui lòng nhập đầy đủ thông tin')
        return
    }
    const data = {
        userName: userName,
        email: email,
        phone: phone,
        password: password
    }
    axios.post('https://localhost:7197/api/Auth/Register', data,)
        .then(function (response) {
            toggleLoading(false)
            if (response.data.isSuccess) {
                const verifyform = document.getElementById("verify-form");
                const registerform = document.getElementById("register-form");

                registerform.style.display = "none"
                verifyform.style.display = "block"
                startCountdown()
            }
            else{
                toastr.warning(response.data.message)
            }
        }).catch(function (error) {
            console.error('Lỗi máy chủ', error);
        }).finally(function(){
            toggleLoading(false)     
        })
}

function confirmRegister() {
    toggleLoading(true)
    const email = document.getElementById('email').value
    const code = Array.from(document.querySelectorAll('.code-input'))
        .map(input => input.value)
        .join('');

    if (code.length < 4) {
        toastr.error('Chưa nhập đầy đủ mã')
        return
    } 
   const data ={
    email: email,
    code
   }
    axios.post('https://localhost:7197/api/Auth/ConfirmEmailAsync',data,)
    .then(function (response) {
        toggleLoading(false)
        if (response.data.isSuccess) {
           window.location.href = 'http://127.0.0.1:5500/user/login.html'
        }
    }).catch(function (error) {
        console.error('Lỗi máy chủ', error);
    }).finally(function(){
        toggleLoading(false)     
    })
}

function resendCode() {
    const email = document.getElementById('email').value
    axios.get('https://localhost:7197/api/Auth/ResendCodeConfirmEmailAsync',{
        params: { email: email }
     }).then(function (response) {  
        if (response.data.isSuccess) {
          toastr.info('Đã gửi thành công')
        }
    }).catch(function (error) {
        console.error('Lỗi máy chủ', error);
    }).finally(function(){
        toggleLoading(false)     
    })
}

function toggleLoading(show) {
    const overlay = document.getElementById("overlay");

    if (show) {
        overlay.style.display = "flex";
    } else {
        setTimeout(() => {
            overlay.style.display = "none"; 
        }, 300); 
    }
}
