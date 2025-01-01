function login(){
    toggleLoading(true)
    const userName = document.getElementById('userName').value
    const password = document.getElementById('password').value
    const data = {
        userName: userName,
        password: password
    }
    axios.post('https://localhost:7197/api/Auth/Login',data,{
        withCredentials: true
    })
    .then(function (response) {
        toggleLoading(false)
      const accessToken  = response.data.data.accessToken
      localStorage.setItem("accessToken",accessToken)
    })
    .catch(function (error) {
        console.error('Lỗi khi lấy dữ liệu:', error);
    });
}

function logout() {
    const accessToken = localStorage.getItem('accessToken');
    axios.post('https://localhost:7197/api/Auth/Logout',{},{
        withCredentials: true,
        headers: {
            Authorization: `Bearer ${accessToken}`
        }
    })
    .then(function (response) {
        console.log('Đăng xuất thành công:', response.data);
        localStorage.removeItem('accessToken'); 
     
        
    })
    .catch(function (error) {
        console.error('Lỗi khi đăng xuất:', error);
    });
}
function toggleLoading(show) {
    const overlay = document.getElementById("overlay");
    overlay.style.display = show ? "flex" : "none";
}
