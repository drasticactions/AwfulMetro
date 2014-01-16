var ForumCommand = function(command, id) {
    var forumCommandObject = {
        "Command": command,
        "Id": id
    };
    window.external.notify(JSON.stringify(forumCommandObject));
};