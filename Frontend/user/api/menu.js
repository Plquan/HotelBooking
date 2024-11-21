
document.addEventListener('DOMContentLoaded', function () {
  getAllMenu()
})
function getAllMenu() {
  axios.get('https://localhost:7197/api/Menu/GetAll')
    .then(function (response) {
      const menus = response.data.data
      const tableData = document.getElementById('menuContainer')
            tableData.innerHTML = ''
      if (menus.length > 0) {
        let menuHtml = ''
        const activeMenus = menus.filter(m => m.status === "active")
        activeMenus.forEach(menu => {
         menuHtml += `<li class="dropdown">
           <a href="${menu.link}" target="${menu.typeOpen}">${menu.name}${menu.menuChild.length === 0 ? '':'<b class="caret"></b>'}</></a>`
          if (menu.menuChild.length > 0) {
            menuHtml += recursiveMenu(menu.menuChild)
          }
          tableData.innerHTML = menuHtml
        });
      }
    }).catch(function (error) {
      console.error('Lỗi lấy dữu liệu:', error);
    });
}

function recursiveMenu(menus,html = '',ulClass = 'dropdown-menu icon-fa-caret-up submenu-hover') {
  html += `<ul class="${ulClass}">`
  menus.forEach(menu => {
   html += `<li class="submenu-hover1">
    <a href="${menu.link}" target="${menu.typeOpen}" >${menu.name}${menu.menuChild.length === 0 ? '' : '<b class="caret"></b>'}</a>`
    if(menu.menuChild.length > 0){
      html = recursiveMenu(menu.menuChild,html,ulClass = 'dropdown-menu dropdown-margin')
    }
    html += '</li>'
  });
  html += '</ul>'
  return html
}
