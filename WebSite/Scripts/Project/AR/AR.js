function ARTranslation() {

};


var AR = function () {

    var _translation = null;
    var _interactiveTestId = null;

    var _fadeInLogo = function () {
        var $logoAR = $('#logoAROnTarget');
        //setTimeout(function () {
        //    $logoAR.fadeIn(800);
        //}, 1500);

        setTimeout(function () {
            $logoAR.css({ opacity: '1' });
            $logoAR.css({ transform: 'scale(1.5)' });
        }, 1500);
    };

    var _sendCompiledBlob = function (blob) {
        var data = new FormData();
        data.append('fname', _interactiveTestId + ".mind");
        data.append('data', blob);
        data.append('interactiveTestId', _interactiveTestId);
        $.ajax({
            type: 'POST',
            url: '/Project/SendCompiledBlob',
            data: data,
            processData: false,
            contentType: false
        }).done(function (data) {
            if (data.success) {
                if (!Shared.isMobile()) {
                    $('#graldevARQRCode').attr('src', data.qrCode).show();
                }   
            }
        });
    };

    var _showARInteractiveTitle = function () {
        if (Shared.isMobile()) {
            $('#arInteractiveInstructionMobile').show();
        } else {
            $('#arInteractiveInstructionPC').show();
        }
    };

    return {
        init: function (interactiveTestId) {
            _interactiveTestId = interactiveTestId;
            _fadeInLogo();
            _showARInteractiveTitle();
        },
        sendCompiledBlob: function (blob) {
            _sendCompiledBlob(blob);
        }
    };
}();