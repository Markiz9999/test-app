$('#customerForm').submit(function () {

    if (this['Password'] !== this['ConfirmPassword']) {
        $('.errors').removeClass('hidden');
        $('.errors').append('<div class="text-danger">The password and confirmation password do not match</div>');
        return false;
    }
});

function cancel() {
    window.history.back();
}
