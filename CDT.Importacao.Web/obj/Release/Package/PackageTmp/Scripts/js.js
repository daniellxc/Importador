function Confirm(msg) {
    if (confirm(msg)) {
        return true;
    } else {
        return false;
    }
}

function BindDDL(trigger, target, controller, action) {

    if ($(trigger).val() != "") {
        $(target).empty();

        $.ajax({
            type: 'POST',
            url: "/importacao/" + controller + "/" + action, // we are calling json method

            dataType: 'json',

            data: { id: $(trigger).val() },
            // here we are get value of selected country and passing same value


            success: function (states) {

                // states contains the JSON formatted list
                // of states passed from the controller

                $.each(states, function (i, state) {

                    $(target).append('<option value="' + state.Value + '">' +
                         state.Text + '</option>');
                    // here we are adding option for States

                });
            },
            error: function (ex) {
                alert('erro ao recuperar itens.' + ex);
            }

        });

       

        return false;

    } else {
        $(target).empty();
        $(target).append('<option value="">---</option>');
        $(target).change();
    }

   

}