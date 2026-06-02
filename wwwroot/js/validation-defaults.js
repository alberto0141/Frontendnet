(function ($) {
    'use strict';

    if (!window.jQuery || !$.validator || !$.validator.unobtrusive) {
        return;
    }

    const validationSettings = {
        validClass: 'is-valid',
        errorClass: 'is-invalid'
    };

    $.validator.setDefaults(validationSettings);
    $.validator.unobtrusive.options = validationSettings;
})(window.jQuery);