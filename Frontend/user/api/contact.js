function sendContact() {
    const userName = document.getElementById('userName').value
    const email = document.getElementById('email').value
    const phone = document.getElementById('phone').value
    const message = document.getElementById('message').value
    const data = {
        userName: userName,
        email: email,
        phone: phone,
        message: message
    }
    axios.post('https://localhost:7197/api/Contact/CreateContact', data)
        .then(function (response) {
            console.log()
            if(response.data.data.statusCode){
            document.getElementById('userName').value = ''
            document.getElementById('email').value = ''
            document.getElementById('phone').value = ''
            document.getElementById('message').value = ''
            toastr.success('Gửi thành công')
            }
            else{
                toastr.error('Gửi thất bại')
            }
        })

}