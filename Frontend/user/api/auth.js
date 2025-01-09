function login(){
    toggleLoading(true)
    const userName = document.getElementById('userName').value
    const password = document.getElementById('password').value
    const data = {
        userName: userName,
        password: password
    }
    axios.post('https://localhost:7197/api/Auth/Login',data)
    .then(function (response) {
        
      const accessToken  = response.data.data.accessToken
      localStorage.setItem("accessToken",accessToken)
      window.location.href = 'http://127.0.0.1:5500/user/home.html'

    })
    .catch(function (error) {
        console.error('Lỗi khi lấy dữ liệu:', error);
    }).finally(function(){
        toggleLoading(false)
    })
}

function logout() {
    toggleLoading(true)
    const accessToken = localStorage.getItem('accessToken');
    axios.post('https://localhost:7197/api/Auth/Logout',{},{
        withCredentials: true,
        headers: {
            Authorization: `Bearer ${accessToken}`
        }
    })
    .then(function (response) {
        localStorage.removeItem('accessToken'); 
        window.location.reload()
    })
    .catch(function (error) {
        console.error('Lỗi khi đăng xuất:', error);
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
