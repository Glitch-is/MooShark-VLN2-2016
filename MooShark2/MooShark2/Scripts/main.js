$(document).ready(function () {
    $(".button-collapse").sideNav({ menuWidth: 200 });
    $('select').material_select();
    $('.modal-trigger').leanModal();
    $('.datepicker').pickadate({ selectMonths: true, selectYears: 15, container: 'body' });
    $('.tooltipped').tooltip({ delay: 50 });

    $("#problemForm").submit(function (event) {
        event.preventDefault();
        console.log("adding problem");
        $.post($("problemForm").attr('action'),
            $("#addProblem").serialize(), function (data) {
                // Problem added
                Materialize.toast('Problem added!', 4000);
                $("#assignmentModal").closeModal();
            });
    });
    $("#assignmentForm").submit(function (event) {
        event.preventDefault();
        var jsonString = $("#name").serialize() + "&" + "deadline=" + $("#deadline").val() + "&" + $("#compose").serialize() + "&" + $("#courseId").serialize();
    
        $.post($("#assignmentForm").attr('action'), jsonString
            , function (data) {
                // Assignment added
                Materialize.toast('Assignment added!', 4000);
                $("#assignmentModal").closeModal();
            });
    });


    //Edit Assignment GET
    //TODO Deadline birtist vitlaust í edit.
    $("body").on("click", ".editAssign", function (event) {
        var assId = $(this).attr("assignId");
        $(this).addClass("editSelected");
        $.ajax({
            type: "GET",
            url: "/Assignments/GetAssignmentAsJSON/" + assId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                $("#name").val(data["name"]);
                $("label[for=name]").attr("class", "active");
                $("#compose").val(data["compose"]);
                $("label[for=compose]").attr("class", "active");
                $("#deadline").val(data["deadline"]);
                $("label[for=deadline]").attr("class", "active");
            }
        });
    });

    //Edit Assignmetn POST
    $("#editAssignmentFormPost").submit(function (event) {
        event.preventDefault();
        var jsonString = "id=" + $(this).attr("modelAssignId") + "&" + $("#name").serialize() + "&" + "deadline=" + $("#deadline").val() + "&" + $("#compose").serialize();
        console.log(jsonString);

        $.post($("#editAssignmentFormPost").attr("action"), //TODO make this work
              jsonString, function (data) {
                  Materialize.toast('Assignment changed!', 4000);
                  $("#editAssignmentModal").closeModal();
              });
    });

    //Delete Assignmnet
    $("body").on("click", ".deleteAsignment", function (event) {
        console.log($(this).attr("delAssignId"));
        var usrId = $(this).attr("delAssignId");
        var jsonData = '{"id": "' + usrId + '"}';

        $.ajax({
            url: "/Assignments/RemoveAssignment",
            type: "POST",
            data: jsonData,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                var divId = "#itemId" + usrId;
                $(divId).remove();
                Materialize.toast('Assignment removed!', 4000);
            }
        });

    });

    $("#courseForm").submit(function (event) {
        event.preventDefault();
        console.log("adding course");
        console.log($("#courseForm").serialize());
        var crsId = $(this).attr("crsId");
        var teachList = [];
        $("#teacherList [teacherid]").each(function (index) {
            teachList.push($(this).attr("teacherid"));
        });
        var jsonString = $("#courseForm").serialize() + "&teachers=" + JSON.stringify(teachList) + "&id=" + crsId;
        console.log(jsonString);
        $.post($("#courseForm").attr('action'),
            jsonString, function (data) {
                // course added
                if (data["status"] == "SuccessAdd") {
                    var div = '<li class="collection-item avatar hover" id="itemId' + data["courseid"] + '"><img src="' + data["teacherImgPath"] + '" alt="" class="circle"><div class="row nobot"><div class="col s3"><div class="title truncate">' + data["courseName"] + '</div></div><div class="col s6"><p class="description truncate">' + data["courseDesc"] + '</p></div><div class="col s2"><p class="due truncate" style="font-style: italic">' + data["teacherName"] + '</p></div><a class="modal-trigger tooltipped small userOfCourse" data-position="left" data-tooltip="Manage users" href="/Admin/CourseUsers/' + data["courseid"] + '" )"><i class="material-icons small">person</i></a><a class="modal-trigger tooltipped small editCourse" data-position="left" data-tooltip="Edit course" href="#" courseid="' + data["courseid"] + '"><i class="material-icons small">edit</i></a><a class="tooltipped small deleteCourse" data-position="left" data-tooltip="Delete coures" couresid="' + data["courseid"] + '"><i class="material-icons small">delete</i></a></div></li>';
                }
                Materialize.toast('Course added!', 4000);
                $("#courseModal").closeModal();
            });
    });
    $(".deleteCourse").click(function (event) {
        var crsId = $(this).attr("courseid");
        var jsonData = "id=" + crsId;
        console.log(jsonData);
        $.post("RemoveCourse", jsonData, function (response) {
            if (response["status"] == "successRemove") {
                $("[courseid="+crsId+"]").parent().parent().remove();
                Materialize.toast("course removed", 3000);
            }
        });


    });
    $("body").on("click",".editCourse", function (event) {
        console.log($(this).attr("courseid"));
        var crsId = $(this).attr("courseid");
        $(this).addClass("editSelected");
        $("#courseForm").attr("crsId", crsId);
        $("#teacherList").empty();
        $("#teacherList").append('<li class="collection-header" crsId="'+crsId+'"><h4 style="font-size: 18px;">Teachers</h4></li>');
        $("#courseForm").attr("action", "/Admin/EditCourse");
        $.ajax({
            type: "GET",
            url: "GetCourseAsJSON/" + crsId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                console.log(data);
                $("#name").val(data["name"]);
                $("label[for=name]").attr("class", "active");
                $("#description").val(data["description"]);
                $("label[for=description]").attr("class", "active");
                var result = $.parseJSON(data["teachers"]);
                $.each(result, function (k, v) {
                    //display the key and value pair
                    $("#teacherList").append('<li class="collection-item"><div>' + v + '<a href="#" class="secondary-content teacherRemove" teacherid="' + k + '"><i class="material-icons">clear</i></a></div></li>');

                });

                var allTeachers = $.parseJSON(data["allTeachers"]);
                var $selectDropDown = $("#teacherDropDown").empty().html(' ');
                $.each(allTeachers, function (k, v) {
                    $selectDropDown.append($('<option></option>').attr("value", k).text(v));
                });
                $selectDropDown.trigger('contentChanged');

            }
        }); 
    });
    $("body").on("click", "#addTeacherButton", function (e) {
        var teacherId = $("#teacherDropDown").val();
        var teacherEmail = $("#teacherDropDown option[value=" + teacherId + "]").text();
        $("#teacherList").append('<li class="collection-item"><div>' + teacherEmail + '<a href="#" class="secondary-content teacherRemove" teacherid="' + teacherId + '"><i class="material-icons">clear</i></a></div></li>');
        $("#teacherDropDown option[value=" + teacherId + "]").remove();
        $("#teacherDropDown").trigger('contentChanged');
    });

    $("body").on("click", ".teacherRemove", function (event) {
        if ($("#teacherList li").length > 2) {
            var teachId = $(this).attr("teacherid");
            var teachName = $(this).parent().text();
            teachName = teachName.substring(0, teachName.length - 5);
            $("[teacherid=" + teachId + "]").parent().parent().remove();
            $("#teacherDropDown").append($('<option></option>').attr("value", teachId).text(teachName));
            $("#teacherDropDown").trigger('contentChanged');
        }
    });

    $("body").on("click", "#addCourse", function () {
        $("#courseForm").attr("action", "/Admin/AddCourse");
        $("#name").val("");
        $("label[for=name]").removeClass("active")

        // TODO: Check if this is correct (Previous conflict)
        $("#teacher").val("");
        $("label[for=teacher]").removeClass("active")
        $("#description").val("");
        $("label[for=description]").removeClass("active")

        $("#description").val("");
        $("label[for=description]").removeClass("active");
        $("#teacherList").empty();
        $("#teacherList").append('<li class="collection-header"><h4 style="font-size: 18px;">Teachers</h4></li>');
        $.ajax({
            type: "POST",
            url: "GetAllTeachersJSON",
            success: function (response) {
                if (response["status"] == "SuccessGet") {
                    var allTeachers = $.parseJSON(response["teachers"]);
                    var $selectDropDown = $("#teacherDropDown").empty().html(' ');
                    console.log(allTeachers);
                    $.each(allTeachers, function (k, v) {
                        $selectDropDown.append($('<option></option>').attr("value", k).text(v));
                    });
                    $selectDropDown.trigger('contentChanged');
                }
            }
        })
    });

    //  Clarification mix
    $("body").on("click", ".questionButton", function (event) {
        console.log($(this).attr("clarId"));
        console.log($("#problemIdQuestion").val());
        var clarId = $(this).attr("clarId");
        console.log(clarId);
        // $(this).addClass("editSelected");
        $("#clarificationAnswersForm").attr("action", "/Clarifications/SubmitAnswer");
        $.ajax({
            type: "GET",
            url: "/Clarifications/GetClarificationIdAsJSON/" + clarId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
               // $("#clarificationId").val(data["clarificationId"]);
                $("#clarificationId").attr("clarId", data["clarificationId"]);
            }
        });
    });
    
    $("#clarificationAnswersForm").submit(function (event) {
        event.preventDefault();
     //   var clarId = $("#clarificationId").val();
        var clarId = $("#clarificationId").attr("clarId");
        console.log("Submit clar answer");
        console.log(clarId);
        var jsonString = $("#newAnswer").serialize() + "&" + $("#problemId").serialize() + "&clarificationId=" + clarId;

        $.post($("#clarificationAnswersForm").attr('action'), jsonString
            , function (data) {
                var pId = $("#problemId").serialize().split('=')[1];
                Materialize.toast('Clarification question added!', 4000);
                $(window.location).attr('href', '/Clarifications/Index/' + pId);
                $("#answersModal").closeModal();
            });
    });

    $("#clarificationQuestionForm").submit(function (event) {
        event.preventDefault();
        var jsonString = $("#newQuestion").serialize() + "&" + $("#problemId").serialize();

        $.post($("#clarificationQuestionForm").attr('action'), jsonString
            , function (data) {
                var pId = $("#problemId").serialize().split('=')[1];
                Materialize.toast('Clarification added!', 4000);
                $(window.location).attr('href', '/Clarifications/Index/' + pId);
                $("#questionModal").closeModal();
            });
    });

    $("#newClarificationQuestionForm").submit(function (event) {
        event.preventDefault();
        var jsonString = $("#newQuestion").serialize() + "&" + $("#problemId").serialize();

        $.post($("#newClarificationQuestionForm").attr('action'), jsonString
            , function (data) {
                var pId = $("#problemId").serialize().split('=')[1];
                Materialize.toast('Clarification added!', 4000);
                $(window.location).attr('href', '/Clarifications/Index/' + pId);
                $("#questionModal").closeModal();
            });
    });
    $("body").on("click", ".deleteQuestion", function (event) {
        console.log("delete question");
        console.log($(this).attr("clarId"));
        var clarifId = $(this).attr("clarId");
        var jsonData = '{"clarifId": "' + clarifId + '"}';
        $.ajax({
            url: "/Clarifications/RemoveQuestion/" + clarifId,
            type: "POST",
            data: jsonData,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                //  var divId = "#itemId" + questionId;
                // $(divId).remove();
                Materialize.toast('Question removed!', 4000);
                var pId = $("#problemId").serialize().split('=')[1];
                $(window.location).attr('href', '/Clarifications/Index/' + pId);
            }
        });


    });

    $("body").on("click", ".deleteAnswer", function (event) {
        console.log("delete Answer");
        console.log($(this).attr("clarAnsId"));
        var clarAnsId = $(this).attr("clarAnsId");
        var jsonData = '{"clarAnsId": "' + clarAnsId + '"}';
        $.ajax({
            url: "/Clarifications/RemoveAnswer/" + clarAnsId,
            type: "POST",
            data: jsonData,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                //  var divId = "#itemId" + questionId;
                // $(divId).remove();
                Materialize.toast('Answer removed!', 4000);
                var pId = $("#problemId").serialize().split('=')[1];
                $(window.location).attr('href', '/Clarifications/Index/' + pId);
            }
        });


    });


    $("#userForm").submit(function (event) {
        event.preventDefault();
        console.log("adding user");
        var usrId = 2;
        if ($(this).attr("action") == "/Admin/AddUser") {
            var formData = $(this).serialize();
        }
        else {
            var formData = 'id=' + $(".editSelected").attr("userid") + '&' + $(this).serialize();
        }
        console.log(formData);
        $.ajax({
            url: $(this).attr("action"),
            type: "POST",
            data: formData,
            success: function (response) {
                console.log(response["status"]);
                if (response["status"] == "SuccessAdd") {
                    $("#userList").append('<li class="collection-item avatar hover"><i class="material-icons circle green lighten-1"></i><div class="row nobot"><div class="col s6 m2"><p class="title truncate">' + $("#email").val() + '</p></div><div class="col hide-on-small-only m7"><p class="description truncate">' + $("#role option:selected").text() + '</p></div><a class="modal-trigger tooltipped small editUser" data-position="left" data-tooltip="Edit user" href="#addModal" userid="' + response["userId"] + '"><i class="material-icons add_fab small">edit</i></a><a class="tooltipped small deleteUser" data-position="left" data-tooltip="Delete user" userid="' + response["userId"] + '"><i class="material-icons add_fab small">delete</i></a></div></li>');
                    Materialize.toast('User added!', 4000);
                }
                else if (response["status"] == "SuccessEdit") {
                    var justId = "itemId" + response["userId"];
                    var idGet = "#itemId" + response["userId"];
                    $(idGet).replaceWith('<li class="collection-item avatar hover" id="' + justId + '"><i class="material-icons circle green lighten-1"></i><div class="row nobot"><div class="col s6 m2"><p class="title truncate">' + $("#email").val() + '</p></div><div class="col hide-on-small-only m7"><p class="description truncate">' + $("#role option:selected").text() + '</p></div><a class="modal-trigger tooltipped small editUser" data-position="left" data-tooltip="Edit user" href="#addModal" userid="' + response["userId"] + '"><i class="material-icons add_fab small">edit</i></a><a class="tooltipped small deleteUser" data-position="left" data-tooltip="Delete user" userid="' + response["userId"] + '"><i class="material-icons add_fab small">delete</i></a></div></li>');
                    $(".editSelected").removeClass("editSelected");
                    Materialize.toast('User edited!', 4000);
                }

                $("#userModal").closeModal();
            }
        });
    });
    $("body").on("click", "#addUser", function () {
        $("#userForm").attr("action", "/Admin/AddUser");
        $("#name").val("");
        $("label[for=name]").removeClass("active")
        $("#email").val("");
        $("label[for=email]").removeClass("active")
        $("#password").val("");
        $("label[for=password]").removeClass("active")
        $("#role").val(1);
        $('#role').material_select();
        $("#imgPath").val("");
        $("label[for=imgpath]").removeClass("active")
    });

    $("body").on("click", ".editUser", function (event) {
        console.log($(this).attr("userid"));
        var usrId = $(this).attr("userid");
        $(this).addClass("editSelected");
        $("#userForm").attr("action", "/Admin/EditUser");
        $.ajax({
            type: "GET",
            url: "GetUserAsJSON/" + usrId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                $("#name").val(data["name"]);
                $("label[for=name]").attr("class", "active");
                $("#email").val(data["email"]);
                $("label[for=email]").attr("class", "active");
                $("#password").val(data["password"]);
                $("label[for=password]").attr("class", "active");
                $("#role").val(data["roleid"]);
                $('#role').material_select();
                $("#imgPath").val(data["imagePath"]);
                $("label[for=imgPath]").attr("class", "active");
            }
        });


    });
    $("body").on("click", ".deleteUser", function (event) {
        console.log($(this).attr("userid"));
        var usrId = $(this).attr("userid");
        var jsonData = '{"id": "' + usrId + '"}';
        $.ajax({
            url: "/Admin/RemoveUser",
            type: "POST",
            data: jsonData,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                var divId = "#itemId" + usrId;
                $(divId).remove();
                Materialize.toast('User removed!', 4000);
            }
        });


    });

    $("body").on("click", ".removeUserFromCourse", function (e) {
        var userId = $(this).attr("userid");
        var crsId = $("#notEnrolledList").attr("courseId");
        var jsonData = "id=" + userId + "&crsId=" + crsId;
        console.log(jsonData);
        $.post("/Admin/RemoveUserFromCourse", jsonData, function (data) {
            var divId = "#itemId" + userId;
            $(divId).remove();
            $("#notEnrolledList").append('<li class="collection-item avatar hover" id="itemId' + userId + '"><i class="material-icons circle green lighten-1"></i><div class="row nobot"><div class="col s6 m2"><p class="title truncate">' + data["email"] + '</p></div><div class="col hide-on-small-only m7"><p class="description truncate">' + data["role"] + '</p></div><a class="tooltipped small addUserToCourse" data-position="left" data-tooltip="Add" userid="' + userId + '"><i class="material-icons small">add</i></a></div></li>');
            Materialize.toast('User removed!', 4000);
        })
    });

    $("body").on("click", ".addUserToCourse", function (e) {
        var userId = $(this).attr("userid");
        var crsId = $("#notEnrolledList").attr("courseId");
        var jsonData = "id=" + userId + "&crsId=" + crsId;
        $.post("/Admin/AddUserToCourse", jsonData, function (data) {
            var divId = "#itemId" + userId;
            $(divId).remove();
            $("#enrolledList").append('<li class="collection-item avatar hover" id="itemId' + userId + '"><i class="material-icons circle green lighten-1"></i><div class="row nobot"><div class="col s6 m2"><p class="title truncate">' + data["email"] + '</p></div><div class="col hide-on-small-only m7"><p class="description truncate">' + data["role"] + '</p></div><a class="tooltipped small removeUserFromCourse" data-position="left" data-tooltip="Remove" userid="' + userId + '"><i class="material-icons small">clear</i></a></div></li>');
            Materialize.toast("User added!", 4000);
        })
    });


    $("#testCaseForm").submit(function (event) {
        event.preventDefault();
        console.log("adding testcase");
        $.post($("testcaseForm").attr('action'),
            $("#addTestCase").serialize(), function (data) {
                // Assignment added
                Materialize.toast('Test case added!', 4000);
                $("#testCaseModal").closeModal();
            });
    });
    $("#teamSearch").keyup(function (e) {
        $("#teamSearchResult").hide('normal');
        $("#teamSpinner").show();

        $.post($(this).attr("action"), { "filter": $(this).val() }, function (data) {
            var source = $("#student-template").html();
            var template = Handlebars.compile(source);

            $("#teamSearchResult").empty();
            $("#teamSearchResult").append(template(data));

            $("#teamSearchResult").show('slow');
            $("#teamSpinner").hide();
        });
    });
    $("body").on("click", ".addTeamMember", function (e) {
        console.log($(this).attr("action"));
        $.post($("#teamSearchResult").attr("action"), { "memberId": $(this).attr("userid") }, function (data) {
            if (data["status"] == "Successful") {
                $("#teamModal").closeModal();
                Materialize.toast("Added team member!", 3000);
            }
            else {

            }
        });
    });
    $("body").on("click", ".removeMember", function (e) {
        var dis = $(this);
        $.post($("#teamMembers").attr("action"), { "memberId": dis.attr("userId") }, function (data) {
            dis.tooltip("hide");
            dis.parent().parent().parent().remove();
            Materialize.toast("Removed team member!", 3000);
        });
    });
    $("#teamSpinner").hide();

    $("#submit-code").click(function (e) {
        var editor = ace.edit("editor");
        var code = encodeURIComponent(editor.getValue());
        var problemId = $(this).attr("problemId");
        console.log($("#lang-select").val());
        var payload = { code: code, lang: $("#lang-select").val(), probId: problemId };
        console.log(JSON.stringify(payload));
        $.post($(this).attr("action"), payload , function (data) {

            Materialize.toast("Code Submitted!", 3000);

            window.location.href = "/Submissions/Submission/" + data.submissionId;
        });
    });

    $("body").on("click", ".testcase-item", function (e) {
        $("#testcaseModal").openModal();
        var testId = $(this).attr("testid");
        $.get("/TestCase/Index/" + testId, function (data) {
            if (data["result"] == 15)
            {
                $("#testcaseOutput").empty();
                $("#testcaseExpected").empty();
                $("#testOut").show();
                $("#testErr").hide();
                $("#testcaseOutput").text(data["output"]);
                $("#testcaseExpected").text(data["expected"]);
            }
            else {
                $("#testcaseError").empty();
                $("#testOut").hide();
                $("#testErr").show();
                $("#testcaseError").text(data["error"]);

            }
        });
    });

    //Change Profile Image
    $("#profileImageForm").submit(function (event) {
        event.preventDefault();
        console.log($("#imagePath").serialize());
        var jsonString = "id=" + $(this).attr("userId") + "&" + $("#imagePath").serialize();
        console.log(jsonString);
        $.post($("#profileImageForm").attr("action"),
              jsonString, function (data) {
                  // Image changed
                  Materialize.toast('New picture added!', 4000);
                  $("#imageModel").closeModal();

              });

    });

    //Change Password
    $("#ProfilePasswordForm").submit(function (event) {
        event.preventDefault();
        console.log($("#password").serialize());
        console.log("Butt");

        var jsonString = "id=" + $(this).attr("userId") + "&" + $("#password").serialize();
        console.log(jsonString);
        $.post($("#ProfilePasswordForm").attr("action"),
              jsonString, function (data) {
                  // Password changed
                  Materialize.toast('Password changed!', 4000);
                  $("#passwordModel").closeModal();

              });
    });

    //Edit Problem GET
    $("body").on("click", ".editProblem", function (event) {
        console.log($(this).attr("problemId"));
        var prbId = $(this).attr("problemId");
        $(this).addClass("editSelected");
        $.ajax({
            type: "GET",
            url: "/Problems/GetProblemAsJSON/" + prbId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                $("#name").val(data["name"]);
                $("label[for=name]").attr("class", "active");
                $("#description").val(data["description"]);
                $("label[for=description]").attr("class", "active");
                $("#weight").val(data["weight"]);
                $("label[for=weight]").attr("class", "active");
                $("#sampleinput").val(data["sampleinput"]);
                $("label[for=sampleinput]").attr("class", "active");
                $("#sampleoutput").val(data["sampleoutput"]);
                $("label[for=sampleoutput]").attr("class", "active");
            }
        });
    });

    //Edit Problem POST
    $("#problemForm2").submit(function (event) {
        event.preventDefault();
        console.log($("#name").serialize());
        var jsonString = "id=" + $(this).attr("problemId") + "&" + $("#name").serialize() + "&" + $("#description").serialize() + "&" + "weight=" + $("#weight").val() + "&" + $("#sampleinput").serialize() + "&" + $("#sampleoutput").serialize();
        console.log(jsonString);

        $.post($("#problemForm2").attr("action"), //TODO make this work
              jsonString, function (data) {
                  Materialize.toast('Problem changed!', 4000);
                  $("#editProb").closeModal();
              });
    });


    //Add problem
    $("#problemForm3").submit(function (event) {
        event.preventDefault();
        var jsonString = $("#name").serialize() + "&" + $("#description").serialize() + "&" + "weight=" + $("#weight").val() + "&" + $("#inputdescription").serialize() + "&" + $("#outputdescription").serialize() + "&" + $("#sampleinput").serialize() + "&" + $("#sampleoutput").serialize() + "&" + "assignId=" + $("#assignmentId").serialize();
        console.log(jsonString);

        $.post($("#problemForm3").attr('action'), jsonString,
             function (data) {
                 // Problem added
                 Materialize.toast('Problem added!', 4000);
                 $("#addModal").closeModal();
                 window.location.href = "/Problems/Problem/" + data.problemId;
             });
    });

    if ($("#lang-select"))
    {
        $.get("/Problems/GetLanguages", function (data) {
            var selectDropdown = $("#lang-select");
            selectDropdown.empty();
            for (var key in data) {
                if (data.hasOwnProperty(key)) {
                    selectDropdown.append($("<option></option>").attr("value", key).text(data[key]));
                }
            }
            selectDropdown.trigger('contentChanged');
        });
    }

    $("#lang-select").change(function () {
        var editor = ace.edit("editor");
        console.log("changing to: " + getMode($("#lang-select").val()));
        editor.getSession().setMode("ace/mode/" + getMode($("#lang-select").val()));
    });

    var getMode = function(id) {
        switch (id)
        {
            case "7":
                return "ada";
                break;
            case "4":
            case "99":
            case "116":
                return "python";
                break;
            case "17":
                return "ruby";
                break;
            case "40":
                return "sql";
                break;
            case "29":
                return "php";
                break;
            case "54":
                return "perl";
                break;
            case "112":
            case "35":
                return "javascript";
                break;
            case "10":
                return "java";
                break;
            case "11":
            case "1":
            case "44":
                return "c_cpp";
                break;
            case "27":
                return "csharp";
                break;
            default:
                return "c_cpp";
                break;

        }
    }

    $(".deleteProblem").click(function (event) {
        console.log($(this).attr("problemid"));
        var prbId = $(this).attr("problemid");
        var jsonData = '{"id": "' + prbId + '"}';
        $.ajax({
            url: "/Problems/DeleteProblem/"+prbId,
            type: "POST",
            data: jsonData,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                Materialize.toast('Problem removed!', 4000);
                $("#areYouSure").closeModal();
                window.location.href = "/Problems/Index/" + response.assId;

            }
        });
    });


    $("#testcaseForm2").submit(function (event) {
        event.preventDefault();
        var jsonString = $("#input").serialize() + "&" + $("#output").serialize();
        console.log(jsonString);

        $.post($("#testcaseForm2").attr('action'), jsonString,
             function (data) {
                 // Problem added
                 Materialize.toast('Testcase added!', 4000);
                 $("#addTest").closeModal();
                 location.reload(true);
             });
    });

    //Edit Testcase GET
    $("body").on("click", ".editTestcase", function (event) {
        console.log($(this).attr("testcaseId"));
        var testId = $(this).attr("testcaseId");
        $("#testcaseForm3").attr("testcaseid", testId);
        $(this).addClass("editSelected");
        $.ajax({
            type: "GET",
            url: "/Problems/GetTestCaseAsJSON/" + testId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                console.log(data["input"], data["output"])
                $("#input2").val(data["input"]);
                $("label[for=input2]").attr("class", "active");
                $("#output2").val(data["output"]);
                $("label[for=output2]").attr("class", "active");
            }
        });
    });

    //Edit Testcase POST
    $("#testcaseForm3").submit(function (event) {
        event.preventDefault();
        console.log($(this).attr("testcaseid"));
        var jsonString = "id=" + $(this).attr("testcaseid") + "&" + $("#input2").serialize() + "&" + $("#output2").serialize();
        console.log(jsonString);

        $.post($("#testcaseForm3").attr("action"),
              jsonString, function (data) {
                  Materialize.toast('Testcase changed!', 4000);
                  $("#editTest").closeModal();
                  location.reload(true);
              });
    });

    $("body").on("click", ".editTestcase", function (event) {
        console.log($(this).attr("testcaseId"));
        var testId = $(this).attr("testcaseId");
        $("#testcaseForm3").attr("testcaseid", testId);
    });


    $(".deleteTestcase").click(function (event) {
        console.log($(this).attr("testcaseId"));
        var tstId = $(this).attr("testcaseId");
        var jsonData = '{"id": "' + tstId + '"}';
        $.ajax({
            url: "/Problems/DeleteTestCase/" + tstId,
            type: "POST",
            data: jsonData,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                Materialize.toast('Testcase removed!', 4000);
                location.reload(true);

            }
        });
    });

    $(".editGrade").click(function () {
        var gradeId = $(this).attr("gradeid");
        var oldGrade = $(".grade[gradeid=" + gradeId + "]").text();
        $("#gradeModal").openModal();
        $("#gradeField").val($.trim(oldGrade));
        $("#submitGrade").attr("gradeid", gradeId);
    });

        $("#submitGrade").click(function () {
            var gradeId = $(this).attr("gradeid");
            var newGrade = $("#gradeField").val();

            $(".grade[gradeid=" + gradeId + "]").text(newGrade);
            $.post($(this).attr("action"), { "gradeId": gradeId, "grade": newGrade }, function (data) {
                $("#gradeModal").closeModal();

            });
        });

    //we gotta go back marty
        $("body").on("click", ".backButton", function (e) {
            window.history.back();
        });

        //handle material selects changing
        $('select').on('contentChanged', function () {
            // re-initialize (update)
            $(this).material_select();
        });

});