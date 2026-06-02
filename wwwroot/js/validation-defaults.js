(function ($) {
    'use strict';

    if (!globalThis.jQuery || !$.validator?.unobtrusive) {
        return;
    }

    const validationSettings = {
        validClass: 'is-valid',
        errorClass: 'is-invalid'
    };

    $.validator.setDefaults(validationSettings);
    $.validator.unobtrusive.options = validationSettings;
})(globalThis.jQuery);
