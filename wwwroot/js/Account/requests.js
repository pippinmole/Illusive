function DeleteAccount(id) {
    if (confirm("Are you sure you want to delete your account?")) {
        $.ajax({
            type: "POST",
            url: "/Account/" + id + "?handler=DeleteAccount",
            beforeSend: (xhr) => {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({
                "accountId": id.toString()
            }),
            success: (response) => {
                // console.log(JSON.stringify(response))
                window.location = response.redirect;
            },
            failure: (response) => {
                alert(response);
            },
            complete: () => {
            }
        });
    }
}