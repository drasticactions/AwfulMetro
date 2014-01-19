var ForumCommand = function(command, id) {
    var forumCommandObject = {
        "Command": command,
        "Id": id
    };
    window.external.notify(JSON.stringify(forumCommandObject));
};

var ScrollToBottom = function() {
    $('html, body').animate({ scrollTop: $(document).height() }, 0);
};

var ScrollToDiv = function (pti){
    $('html, body').animate({
        scrollTop: $(pti).offset().top
    }, 0);
};