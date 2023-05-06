window.updateDownloadManagerContainerMaxWidth = () => {
    console.log("Function called: updateDownloadManagerContainerMaxWidth");
    const sidebar = document.querySelector('.sidebar');
    const downloadManagerContainer = document.getElementById('download-manager-container');

    if (sidebar && downloadManagerContainer) {
        const sidebarWidth = sidebar.offsetWidth;
        console.log("Sidebar width:", sidebarWidth);
        downloadManagerContainer.style.left = `${sidebarWidth}px`; // Set the left property
        console.log("DownloadManager container left set to:", downloadManagerContainer.style.left);
    } else {
        console.log("Sidebar or DownloadManager container not found");
    }
}
