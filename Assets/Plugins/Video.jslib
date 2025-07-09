mergeInto(LibraryManager.library, {
    CreateBlobURLFromBase64: function(base64Ptr, fileNamePtr) {
        var base64 = UTF8ToString(base64Ptr);
        var fileName = UTF8ToString(fileNamePtr);
        var binary = atob(base64);
        var len = binary.length;
        var bytes = new Uint8Array(len);
        for (var i = 0; i < len; i++) {
            bytes[i] = binary.charCodeAt(i);
        }
        var blob = new Blob([bytes], {type: "video/mp4"});
        var url = URL.createObjectURL(blob);
        SendMessage('Canvas', 'SetVideoURL', url);
    }
});
