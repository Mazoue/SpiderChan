window.updateDownloadManagerContainerMaxWidth = () => {
    const sidebar = document.querySelector('.sidebar');
    const downloadManagerContainer = document.getElementById('download-manager-container');

    if (sidebar && downloadManagerContainer) {
        const sidebarWidth = sidebar.offsetWidth;
        downloadManagerContainer.style.maxWidth = `calc(100% - ${sidebarWidth}px)`;
    }
}

window.addEventListener('resize', window.updateDownloadManagerContainerMaxWidth);