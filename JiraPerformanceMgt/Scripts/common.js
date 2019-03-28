; !function () {
    "use strict";
    function initializeWidgets() {
        $('.datepicker').datepicker({
            onSelect: function (dateText) {
                //Force validation to rerun since sometimes it is missed
                $(this).valid();
            }
        });
    }

    $(function () {
        initializeWidgets();
    });

}();