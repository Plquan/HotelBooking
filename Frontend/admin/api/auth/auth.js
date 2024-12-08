function login(){
    const userName = document.getElementById('userName').value
    const password = document.getElementById('password').value
    const data = {
        userName: userName,
        password: password
    }
    console.log(data)
    axios.post('https://localhost:7197/api/Auth/Login',data,{
        withCredentials: true
    })
    .then(function (response) {
      const accessToken  = response.data.data.accessToken
      localStorage.setItem("accessToken",accessToken)
      window.location.href = '/admin/menu/index.html'
    
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
        window.location.href = '/admin/auth/login.html'
    })
    .catch(function (error) {
        console.error('Lỗi khi đăng xuất:', error);
    });
}