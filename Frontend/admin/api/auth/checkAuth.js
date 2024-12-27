console.log('jauth')
document.addEventListener('DOMContentLoaded', function () {

    const accessToken = localStorage.getItem('accessToken')
    if (accessToken === null) {
        refreshToken()
    } else {
        axios.get('https://localhost:7197/api/Account/MyInfo', {
            headers: {
                'Authorization': `Bearer ${accessToken}`
            }
        })
            .then(function (response) {
                   console.log(response.data.data.userName)
                
                   data = response.data.data
                  document.getElementById('currentUserName').textContent = `${data.userName}`
            })
            .catch(function (error) {
                refreshToken()
            });
    }
})
function refreshToken() {
    axios.post('https://localhost:7197/api/Auth/RefreshToken', {}, {
        withCredentials: true,
    }).then(function (response) {
        const isSuccess = response.data.isSuccess
        console.log(isSuccess)
        if (isSuccess) {
            const newAccessToken = response.data.data.accessToken
            if (newAccessToken) {
                localStorage.setItem("accessToken", newAccessToken)
            }
        }
        else {
            window.location.href = '/admin/auth/login.html'
        }
    })
        .catch(function (error) {
          if(error.response.status === 401){
            window.location.href = "/admin/auth/login.html"
          }
        });
}