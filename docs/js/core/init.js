// 永恒地牢 - 页面初始化脚本
// 页面加载完成后隐藏 loading 提示
window.addEventListener('load', function() {
  var loadingEl = document.getElementById('loading');
  if (loadingEl) {
    loadingEl.style.display = 'none';
  }
});
