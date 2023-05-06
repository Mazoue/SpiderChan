function isVisible(element) {
    if (!element) return false;

    const style = window.getComputedStyle(element);
    const isHidden = style.display === 'none' || style.visibility === 'hidden';

    return !isHidden;
}

function isWithinViewport(element) {
    const rect = element.getBoundingClientRect();
    const windowHeight = window.innerHeight || document.documentElement.clientHeight;
    const windowWidth = window.innerWidth || document.documentElement.clientWidth;

    const inViewport = (
        rect.top >= 0 &&
        rect.left >= 0 &&
        rect.bottom <= windowHeight &&
        rect.right <= windowWidth
    );

    return inViewport;
}

function isCovered(element) {
    const rect = element.getBoundingClientRect();
    const centerX = rect.left + (rect.width / 2);
    const centerY = rect.top + (rect.height / 2);

    const topElement = document.elementFromPoint(centerX, centerY);

    return topElement && topElement !== element && !element.contains(topElement);
}

function isElementVisibleOnScreen(element) {
    return isVisible(element) && isWithinViewport(element) && !isCovered(element);
}

window.updateDownloadManagerContainerMaxWidth = () => {
    console.log("Function called: updateDownloadManagerContainerMaxWidth");
    const sidebar = document.querySelector('.sidebar');
    const downloadManagerContainer = document.getElementById('download-manager-container');

    if (!sidebar) {
        console.log("Sidebar not found");
    } else if (!isElementVisibleOnScreen(sidebar)) {
        console.log("Sidebar is not visible on the screen");
    }

    if (!downloadManagerContainer) {
        console.log("DownloadManager container not found");
    } else if (!isElementVisibleOnScreen(downloadManagerContainer)) {
        console.log("DownloadManager container is not visible on the screen");
    }

    if (sidebar && downloadManagerContainer && isElementVisibleOnScreen(sidebar) && isElementVisibleOnScreen(downloadManagerContainer)) {
        const sidebarWidth = sidebar.offsetWidth;
        console.log("Sidebar width:", sidebarWidth);
        downloadManagerContainer.style.left = `${sidebarWidth}px`; // Set the left property
        console.log("DownloadManager container left set to:", downloadManagerContainer.style.left);
    }
}
