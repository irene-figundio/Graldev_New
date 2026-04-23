Dropzone.autoDiscover = false;

const compiler = new MINDAR.IMAGE.Compiler();
var myDropzone = null;

const MAX_WIDTH = 320;
const MAX_HEIGHT = 180;
const MIME_TYPE = "image/jpeg";
const QUALITY = 0.7;

const download = (buffer) => {
    var blob = new Blob([buffer]);
    /*var aLink = window.document.createElement('a');
    aLink.download = 'targets.mind';
    aLink.href = window.URL.createObjectURL(blob);
    aLink.click();
    window.URL.revokeObjectURL(aLink.href);*/
    var reader = new FileReader();
    reader.readAsDataURL(blob);
    reader.onloadend = function () {
        AR.sendCompiledBlob(reader.result);
    };
}

const showData = (data) => {
    console.log("data", data);
    for (let i = 0; i < data.trackingImageList.length; i++) {
        const image = data.trackingImageList[i];
        const points = data.trackingData[i].points.map((p) => {
            return { x: Math.round(p.x), y: Math.round(p.y) };
        });
        showImage(image, points);
    }

    for (let i = 0; i < data.imageList.length; i++) {
        const image = data.imageList[i];
        const kpmPoints = [...data.matchingData[i].maximaPoints, ...data.matchingData[i].minimaPoints];
        const points2 = [];
        for (let j = 0; j < kpmPoints.length; j++) {
            points2.push({ x: Math.round(kpmPoints[j].x), y: Math.round(kpmPoints[j].y) });
        }
        showImage(image, points2);
    }
}


const showImage = (targetImage, points) => {
    const container = document.getElementById("container");
    const canvas = document.createElement('canvas');
    container.appendChild(canvas);
    canvas.width = targetImage.width;
    canvas.height = targetImage.height;
    canvas.style.width = canvas.width;
    const ctx = canvas.getContext('2d');
    const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);
    const data = new Uint32Array(imageData.data.buffer);

    const alpha = (0xff << 24);
    for (let c = 0; c < targetImage.width; c++) {
        for (let r = 0; r < targetImage.height; r++) {
            const pix = targetImage.data[r * targetImage.width + c];
            data[r * canvas.width + c] = alpha | (pix << 16) | (pix << 8) | pix;
        }
    }

    var pix = (0xff << 24) | (0x00 << 16) | (0xff << 8) | 0x00; // green
    for (let i = 0; i < points.length; ++i) {
        const x = points[i].x;
        const y = points[i].y;
        const offset = (x + y * canvas.width);
        data[offset] = pix;
        //for (var size = 1; size <= 3; size++) {
        for (var size = 1; size <= 6; size++) {
            data[offset - size] = pix;
            data[offset + size] = pix;
            data[offset - size * canvas.width] = pix;
            data[offset + size * canvas.width] = pix;
        }
    }
    ctx.putImageData(imageData, 0, 0);
}

const loadImage = async (file) => {
    const image = new Image();

    return new Promise((resolve, reject) => {
        var reader = new FileReader();
        reader.onload = function (readerEvent) {
            var image = new Image();
            image.onload = function (imageEvent) {
                // Resize the image
                var canvas = document.createElement('canvas'),
                    max_size = 600,
                    width = image.width,
                    height = image.height;
                if (width > height) {
                    if (width > max_size) {
                        height *= max_size / width;
                        width = max_size;
                    }
                } else {
                    if (height > max_size) {
                        width *= max_size / height;
                        height = max_size;
                    }
                }
                canvas.width = width;
                canvas.height = height;
                var i = canvas.getContext('2d').drawImage(image, 0, 0, width, height);
                resizedImage = canvas.toDataURL('image/jpeg');
                resolve(canvas);
            };
            image.src = readerEvent.target.result;
        };
        reader.readAsDataURL(file);

        //let img = new Image()
        //img.onload = function (imageEvent) {

        //};
        //img.onerror = reject;
        //console.log(file);
        //img.src = URL.createObjectURL(file);
        ////img.src = src
    })
}

var _calculateSize = function (img, maxWidth, maxHeight) {
    let width = img.width;
    let height = img.height;

    // calculate the width and height, constraining the proportions
    if (width > height) {
        if (width > maxWidth) {
            height = Math.round((height * maxWidth) / width);
            width = maxWidth;
        }
    } else {
        if (height > maxHeight) {
            width = Math.round((width * maxHeight) / height);
            height = maxHeight;
        }
    }
    return [width, height];
};

var _downscaleImage = function (dataUrl, newWidth, imageType, imageArguments) {
    "use strict";
    var image, oldWidth, oldHeight, newHeight, canvas, ctx, newDataUrl;

    // Provide default values
    imageType = imageType || "image/jpeg";
    imageArguments = imageArguments || 0.7;

    // Create a temporary image so that we can compute the height of the downscaled image.
    image = new Image();
    image.src = dataUrl;
    oldWidth = image.width;
    oldHeight = image.height;
    newHeight = Math.floor(oldHeight / oldWidth * newWidth)

    // Create a temporary canvas to draw the downscaled image on.
    canvas = document.createElement("canvas");
    canvas.width = newWidth;
    canvas.height = newHeight;

    // Draw the downscaled image on the canvas and return the new data URL.
    ctx = canvas.getContext("2d");
    ctx.drawImage(image, 0, 0, newWidth, newHeight);
    newDataUrl = canvas.toDataURL(imageType, imageArguments);
    return newDataUrl;
};

const compileFiles = async (files) => {
    const images = [];
    for (let i = 0; i < files.length; i++) {
        images.push(await loadImage(files[i]));
    }
    let _start = new Date().getTime();
    const dataList = await compiler.compileImageTargets(images, (progress) => {
        document.getElementById("progress").innerHTML = 'progress: ' + progress.toFixed(2) + "%";
    });
    console.log('exec time compile: ', new Date().getTime() - _start);
    for (let i = 0; i < dataList.length; i++) {
        showData(dataList[i]);
    }
    const exportedBuffer = await compiler.exportData();

    const filess = myDropzone.files;
    $('#interactiveTestTargetImage').show();
    document.getElementById('interactiveTestTargetImage').setAttribute('src', filess[filess.length-1].dataURL);

    if (isMobile()) {
        $('#arInteractiveInstructionAfterMobile').show();
    } else {
        $('#arInteractiveInstructionAfterPC').show();
    }
    $('#progress').hide();
    download(exportedBuffer);
}

var isMobile = function () {
    return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
};

const loadMindFile = async (file) => {
    var reader = new FileReader();
    reader.onload = function () {
        const dataList = compiler.importData(this.result);
        for (let i = 0; i < dataList.length; i++) {
            showData(dataList[i]);
        }
    }
    reader.readAsArrayBuffer(file);
}

document.addEventListener('DOMContentLoaded', function (event) {
    myDropzone = new Dropzone("#dropzone", { url: "#", autoProcessQueue: false, addRemoveLinks: false });
    myDropzone.on("addedfile", function (file) {
        const files = myDropzone.files;
        setTimeout(function () {
            $('.dz-hidden-input').attr("accept", ".jpg, .jpeg, .png");
        }, 500);
        if (files.length === 0) return;
        const ext = files[files.length - 1].name.split('.').pop();
        if (ext != 'jpg' && ext != "png" && ext != "jpeg") {
            alert(Shared.getTranslation().invalidFileType);
            $('.dz-hidden-input').attr("accept", ".jpg, .jpeg, .png");
            return;
        }
        //$('.dz-button').hide();
        $('#progress').show();
        if (ext === 'mind') {
            loadMindFile(files[files.length - 1]);
        } else {
            compileFiles([files[files.length - 1]]);
        }
    });
    $('.dz-hidden-input').attr("accept", ".jpg, .jpeg, .png");
});