window.onload = function () {
    var codeView = ace.edit("codeView");
    codeView.setTheme("ace/theme/iplastic");
    codeView.setReadOnly(true);
    codeView.getSession().setMode("ace/mode/javascript");
};