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

var ScrollToDiv = function (pti) {
    var test = $(pti);
    if (test != null) {
        try {
            $('html, body').animate({
                scrollTop: $(pti).offset().top
            }, 0);
        }
        catch (err) {
            // Ignore, we're probably at the bottom of the page.
            // Besides, if it fails, it just won't scroll.
        }
    }
};

var ScrollToTable = function (pti) {
    var table = $('table[data-idx=' + "'" + pti + "'" + ']');
    if (table.length > 0) {
        $('html, body').animate({
            scrollTop: $('table[data-idx=' + "'" + pti + "'" + ']').offset().top
        }, 0);
    }
};

var OpenLink = function(link) {
    var hostname = $.url('hostname', link);
    // If the link is not for another SA thread, open it in IE.
    if (hostname != 'forums.somethingawful.com' && hostname != "") {
        return true;
    }
    var file = $.url('file', link);
    switch(file)
    {
        case 'showthread.php':
            ForumCommand('openThread', link);
            break;
        case 'member.php':
            ForumCommand('profile', $.url('?userid', link));
            break;
        case 'forumdisplay.php':
            ForumCommand('openForum', link);
            break;
        case 'search.php':
            if ($.url('?action', link) == 'do_search_posthistory') {
                ForumCommand('post_history', $.url('?userid', link));
            }
            break;
        case 'banlist.php':
            ForumCommand('rap_sheet', $.url('?userid', link));
    }
    return false;
};

var ResizeWebviewFont = function(value) {
    $('body').css('font-size', value + 'px');
    $('a').css('font-size', value + 'px');
    $('div').css('font-size', value + 'px');
    $('tr').css('font-size', value + 'px');
    $('td').css('font-size', value + 'px');
    $('dl').css('font-size', value + 'px');
    $('dt').css('font-size', value + 'px');
};

var RemoveCustomStyle = function() {
    $('body').removeAttr('style');
    $('a').removeAttr('style');
    $('div').removeAttr('style');
    $('tr').removeAttr('style');
    $('td').removeAttr('style');
    $('dl').removeAttr('style');
    $('dt').removeAttr('style');
};