
const accessToken = localStorage.getItem('accessToken')
 getInfo()

function getInfo() {
    if(!accessToken){
        refreshToken()
    }
   else{
    axios.get('https://localhost:7197/api/Account/MyInfo', {
        headers: {
            'Authorization': `Bearer ${accessToken}`
        }
    })
        .then(function (response) {
            console.log(response.data.data.userName)

            data = response.data.data
            var headerList = document.getElementById('headerList')

            const headerHtml =
                ` <li class="dropdown">
                    <a href="#" class="dropdown-toggle" data-toggle="dropdown">
                        <img src="images/About/about-1.jpg" alt="" class="user-avatar-dropdown">
                        ${data.userName} <b class="caret"></b>
                    </a>
                    <ul class="dropdown-menu">
                        <li class="active"><a href="#">Cài đặt</a></li>
                        <li><a href="#" onclick="logout()">Đăng xuất</a></li>
                    </ul>
                </li>             
                `
            headerList.innerHTML = headerHtml
        })
        .catch(function (error) {
            refreshToken()
        });
   }
}
function refreshToken() {
    axios.post('https://localhost:7197/api/Auth/RefreshToken', {}, {
        withCredentials: true,
    }).then(function (response) {
        const isSuccess = response.data.isSuccess
        if (isSuccess) {
            const newAccessToken = response.data.data.accessToken
            if (newAccessToken) {
                localStorage.setItem("accessToken", newAccessToken)
            }
        }
    })
        .catch(function (error) {
            console.error(error)
        });
}

