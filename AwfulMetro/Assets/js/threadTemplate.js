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

var ScrollToDiv = function(pti) {
    $('html, body').animate({
        scrollTop: $(pti).offset().top
    }, 0);
};

var ScrollToTable = function (pti) {
    var table = $('table[data-idx=' + "'" + pti + "'" + ']');
    if (table.length > 0) {
        $('html, body').animate({
            scrollTop: $('table[data-idx=' + "'" + pti + "'" + ']').offset().top
        }, 0);
    }
};