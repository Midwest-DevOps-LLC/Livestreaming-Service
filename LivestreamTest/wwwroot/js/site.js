
function SubmitForm(formID, url, type, alertID, clearFields, successFunctionName) {
    var frmData = $(formID).serialize();

    $.ajax({
        url: url,
        type: type,
        data: frmData,
        success: function (result) {
            $(formID).find($("[id$='_error']")).hide();

            $(formID).find($(":input")).removeClass("is-invalid");

            //$(formID).find($(alertID)).find($('.alert')).html(result.fullMessage);

            switch (result.validationStatus) {
                case 0: //Success
                    if (alertID != null) {
                        $(formID).find($(alertID)).find($('.alert')).html(result.successFullMessage);

                        if (IsNullOrEmpty(result.successFullMessage) == false)
                            $(alertID).show();
                        else
                            $(alertID).hide();
                    }

                    if (clearFields == true) {
                        $(formID).find($(":input")).val("");
                    }

                    if (successFunctionName != "NULL") {
                        window[successFunctionName]();
                    }

                    $(formID).find($(alertID)).find($('.alert')).addClass("alert-success");
                    $(formID).find($(alertID)).find($('.alert')).removeClass("alert-danger");

                    ShowGreenStatus(result.successMessage);
                    break;
                case 1: //Error
                    if (alertID != null) {
                        $(formID).find($(alertID)).find($('.alert')).html(result.errorFullMessage);

                        if (IsNullOrEmpty(result.errorFullMessage) == false)
                            $(alertID).show();
                        else
                            $(alertID).hide();
                    }

                    result.validationMessages.forEach(function (i) {
                        DisplayError(formID, i);
                    });

                    $(formID).find($(alertID)).find($('.alert')).addClass("alert-danger");
                    $(formID).find($(alertID)).find($('.alert')).removeClass("alert-success");

                    ShowRedStatus(result.errorMessage);
                    break;
            }
        },
        error: function (result) {

        }
    });
}

function IsNullOrEmpty(value) {
    return !value;
}

function DisplayError(frm, validationMessage, validationAlert) {
    $(frm).find($("#" + validationMessage.htmlid)).addClass('is-invalid');
    $(frm).find($('#' + validationMessage.htmlid + "_error")).css('display', 'block');
    $(frm).find($('#' + validationMessage.htmlid + "_error")).html(validationMessage.message);
}

function ShowGreenStatus(message) {
    $("#success-message").text(message).show().delay(3000).slideUp(1000).fadeOut(10000);
}

function ShowRedStatus(message) {
    $("#error-message").text(message).show().delay(3000).slideUp(1000).fadeOut(10000);
}