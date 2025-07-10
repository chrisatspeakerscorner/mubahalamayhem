mergeInto(LibraryManager.library, {
    RequestFullScreen: function () {
        var canvas = document.getElementById('unity-canvas');
        if (canvas.requestFullscreen) {
            canvas.requestFullscreen();
        } else if (canvas.webkitRequestFullscreen) {
            canvas.webkitRequestFullscreen(); // Safari
        } else if (canvas.msRequestFullscreen) {
            canvas.msRequestFullscreen(); // IE/Edge
        }
    }
});