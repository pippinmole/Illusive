// Received a boolean state (True = liked, False = unliked)
function ToggleLikeButton() {
    const state = $(".like-post-action").attr("src") === "https://illusivecdn.blob.core.windows.net/container-1/like_button_checked.png";
    SetLikeState(!state);
}

function SetLikeText(elem, count) {
    console.log("Setting count to " + count + " " + typeof(count))
    elem.text(count + (count === 1 ? " like" : " likes").toString());
}

// Updates the amount of likes
function SetLikeState(isLiked) {
    const likeText = $("span.like-post-text");
    let likes = parseInt(likeText.text().match(/\d/g));
    likes += isLiked ? 1 : -1;
    
    SetLikeText(likeText, likes);

    const likeImage = $(".like-post-action");
    likeImage.attr("src", isLiked ? "https://illusivecdn.blob.core.windows.net/container-1/like_button_checked.png"
                                : "https://illusivecdn.blob.core.windows.net/container-1/like_button_unchecked.png");
}

function LikePost(id) {
    $.ajax({
        type: "POST",
        url: "/ForumPost?handler=LikePost",
        beforeSend: (xhr) =>  {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            "forumId" : id.toString()
        }),
        success: (response) =>  {
            // console.log(JSON.stringify(response))
        },
        failure: (response) =>  {
            alert(response);
        },
        complete: () => { }
    });

    // Toggle like button client-side - We do this so we don't have to wait for the server's response which causes
    // a delay in the front-end.
    ToggleLikeButton();
}

// Delete post request
function DeletePost(id) {
    if (confirm("Are you sure you would like to delete this post?")) {
        $.ajax({
            type: "POST",
            url: "/ForumPost?handler=DeletePost",
            beforeSend: (xhr) =>  {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({
                "forumId" : id.toString()
            }),
            success: (response) =>  {
                window.location.href = response.redirect;
            },
            failure: (response) =>  {
                alert(response);
            }
        });
    }
}

function LockPost(id) {
    if (confirm("Are you sure you would like to lock this post?")) {
        $.ajax({
            type: "POST",
            url: "/ForumPost?handler=LockPost",
            beforeSend: (xhr) =>  {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({
                "forumId" : id.toString()
            }),
            success: (response) =>  {
                window.location.href = response.redirect;
            },
            failure: (response) =>  {
                alert(response);
            }
        });
    }
}

// Delete reply request
function DeleteReply(forumId, replyId) {
    if (confirm("Are you sure you would like to delete this reply? (" + replyId + ")")) {
        $.ajax({
            type: "POST",
            url: "/ForumPost?handler=DeleteReply",
            beforeSend: (xhr) =>  {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({
                "forumId" : forumId.toString(),
                "replyId" : replyId.toString()
            }),
            success: (response) =>  {
                window.location.href = response.redirect;
            },
            failure: (response) =>  {
                alert(response);
            }
        });
    }
}