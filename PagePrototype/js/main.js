$( document ).ready(function() {
    $(".button-collapse").sideNav({menuWidth: 200});
    $('select').material_select();
    $('.modal-trigger').leanModal();
    $('.datepicker').pickadate({
        selectMonths: true,
        selectYears: 15,
        container: 'body'
    });

    $("#addProblem").submit(function(e){
        $.post("/Problems/Add", $("#addProblem").serialize(), function(data){
            // Problem added
        });
    });
});
